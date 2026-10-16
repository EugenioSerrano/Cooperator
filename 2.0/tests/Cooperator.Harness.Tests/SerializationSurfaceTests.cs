using System.Reflection;
using System.Text.Json;
using Cooperator.Harness.Tests.Support;

namespace Cooperator.Harness.Tests;

/// <summary>
/// SPEC-261016-0013 §6/E y §8.1 (AC-1/AC-2): el gate de superficie de serialización, <b>sobre los ensamblados
/// construidos</b> de <c>Core</c>, <c>Data</c>, <c>Library</c> y <c>Modeling</c>, con
/// <c>MetadataLoadContext</c> (se inspeccionan metadatos: sin cargar ni ejecutar el código).
/// <list type="bullet">
///   <item><b>Cero superficie:</b> 0 tipos con el atributo de serialización, 0 que <b>declaren</b>
///         <c>GetObjectData</c>, 0 constructores de serialización y 0 implementaciones de la interfaz de
///         serialización que <b>no</b> hereden de su base (una excepción la implementa porque la BCL la
///         implementa: eso no es superficie del framework).</item>
///   <item><b>Lo que debe seguir ahí:</b> las bases abstractas, la jerarquía de las excepciones y los
///         constructores públicos canónicos de cada tipo en alcance —lo que hace que un
///         <c>catch (CoreException)</c> siga siendo posible exactamente igual—.</item>
///   <item><b>Y el inventario:</b> cada tipo en alcance existe en los ensamblados construidos; el inventario es
///         la lista de trabajo y su comparación la prueba de que no se perdió ningún miembro fuera de lo
///         declarado (lo que el gate de compatibilidad de API reporta como <c>CP0002</c> lo cubre la constancia
///         del Bolt).</item>
/// </list>
/// <para>
/// <b>Un solo contexto para todo el alcance</b>: <c>Data</c> referencia a <c>Library</c>, de modo que el
/// resolutor de metadatos necesita los <b>cuatro</b> ensamblados juntos; resolver cada uno con su propio contexto
/// deja las referencias cruzadas sin resolver (fallo medido en el V-Bounce N°3, `MEM-261016-0237` §8.2).
/// </para>
/// <para>
/// La obsolescencia <c>SYSLIB0051</c> la emite el compilador al construir <b>contra un runtime moderno</b>, no al
/// apuntar a <c>netstandard2.1</c>: por eso el enforcement es <b>la superficie del ensamblado</b> —observable en
/// cualquier TFM— y no la mera ausencia de una advertencia.
/// </para>
/// <para>
/// Los nombres de la superficie de serialización se <b>ensamblan desde fragmentos</b>: este archivo es código
/// propio del árbol y no puede ser, él mismo, una ocurrencia de lo que persigue.
/// </para>
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class SerializationSurfaceTests
{
    private static readonly string[] Projects =
    [
        "Cooperator.Framework.Core",
        "Cooperator.Framework.Data",
        "Cooperator.Framework.Library",
        "Cooperator.Framework.Modeling",
    ];

    private static readonly string SerializableAttributeName = "System." + "SerializableAttribute";
    private static readonly string GetObjectDataName = "GetObject" + "Data";
    private static readonly string SerializableInterfaceName = "System.Runtime.Serialization." + "ISerializable";
    private static readonly string SerializationInfoName = "System.Runtime.Serialization." + "SerializationInfo";
    private static readonly string StreamingContextName = "System.Runtime.Serialization." + "StreamingContext";

    private static readonly Lazy<MetadataLoadContext> Context = new(() => NewContext());
    private static readonly Dictionary<string, Assembly> Assemblies = new(StringComparer.Ordinal);

    [Fact]
    public void No_type_of_the_built_assemblies_carries_serialization_surface()
    {
        var offenders = new List<string>();

        foreach (var project in Projects)
        {
            foreach (var type in Load(project).GetTypes())
            {
                if (type.GetCustomAttributesData().Any(attribute => attribute.AttributeType.FullName == SerializableAttributeName))
                {
                    offenders.Add($"{project}: {type.FullName} declara el atributo de serialización");
                }

                if (type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Any(method => method.Name == GetObjectDataName))
                {
                    offenders.Add($"{project}: {type.FullName} declara {GetObjectDataName}");
                }

                if (type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Any(constructor => constructor.GetParameters() is [{ ParameterType: var first }, { ParameterType: var second }]
                        && first.FullName == SerializationInfoName
                        && second.FullName == StreamingContextName))
                {
                    offenders.Add($"{project}: {type.FullName} declara el constructor de serialización");
                }

                if (type.GetInterfaces().Any(candidate => candidate.FullName == SerializableInterfaceName)
                    && type.BaseType is not null
                    && !type.BaseType.GetInterfaces().Any(candidate => candidate.FullName == SerializableInterfaceName))
                {
                    offenders.Add($"{project}: {type.FullName} declara la interfaz de serialización");
                }
            }
        }

        Assert.Empty(offenders);
    }

    [Fact]
    public void The_hierarchy_and_the_public_constructors_of_the_scope_are_intact()
    {
        var core = Load("Cooperator.Framework.Core");
        var data = Load("Cooperator.Framework.Data");
        var library = Load("Cooperator.Framework.Library");

        // Las bases abstractas siguen siendo bases abstractas y la jerarquía de `catch` no cambia.
        Assert.True(core.GetType("Cooperator.Framework.Core.Exceptions.CoreException", throwOnError: true)!.IsAbstract);
        Assert.True(library.GetType("Cooperator.Framework.Library.Exceptions.BaseException", throwOnError: true)!.IsAbstract);
        Assert.True(data.GetType("Cooperator.Framework.Data.Exceptions.DataException", throwOnError: true)!.IsAbstract);

        Assert.Equal(
            "Cooperator.Framework.Core.Exceptions.CoreException",
            core.GetType("Cooperator.Framework.Core.Exceptions.InvalidDataSetException", throwOnError: true)!.BaseType!.FullName);
        Assert.Equal(
            "Cooperator.Framework.Library.Exceptions.BaseException",
            data.GetType("Cooperator.Framework.Data.Exceptions.DataException", throwOnError: true)!.BaseType!.FullName);

        foreach (var entry in InScopeTypes())
        {
            var type = entry.Type;

            if (type.IsAbstract)
            {
                // La base abstracta del alcance expone su constructor sin parámetros como `protected` (o
                // `public`): lo que se verifica es que siga siendo construible por sus derivadas.
                Assert.Contains(
                    type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
                    constructor => constructor.GetParameters().Length == 0);
                continue;
            }

            // Y todo tipo CONCRETO del alcance conserva los tres constructores públicos canónicos (sin
            // argumentos, mensaje y mensaje + inner): el código del consumidor que construye o captura no cambia.
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            Assert.Contains(constructors, constructor => constructor.GetParameters().Length == 0);
            Assert.Contains(constructors, constructor => constructor.GetParameters() is [{ ParameterType: var only }] && only.FullName == "System.String");
            Assert.Contains(constructors, constructor => constructor.GetParameters() is [{ ParameterType: var first }, { ParameterType: var second }]
                && first.FullName == "System.String"
                && second.FullName == "System.Exception");
        }
    }

    [Fact]
    public void Every_type_of_the_inventory_exists_in_the_built_assemblies()
    {
        var entries = InScopeTypes();
        Assert.Equal(32, entries.Count); // 31 excepciones + BaseObject (el alcance del SPEC §4)

        foreach (var entry in entries)
        {
            Assert.NotNull(entry.Type);
        }
    }

    /// <summary>
    /// Los tipos EN ALCANCE del inventario (fila con <c>action = retire</c>), resueltos contra los ensamblados
    /// construidos. El nombre se toma del inventario y, si el barrido no pudo completarlo, se normaliza con la
    /// convención del <b>archivo</b>: la carpeta <c>Exceptions/</c> sólo aplica a los archivos que viven en ella
    /// (fallo medido en el V-Bounce N°3, `MEM-261016-0237` §8.2: `BaseObject` vive en la raíz del ensamblado).
    /// </summary>
    private static IReadOnlyList<(string Project, Type Type)> InScopeTypes()
    {
        var path = Repo.InTree(@"tests\fixtures\serialization-surface\baseline-inventory.json");
        Assert.True(File.Exists(path), $"Falta el inventario del Bolt 007: {path}.");

        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var entries = new List<(string, Type)>();

        foreach (var item in document.RootElement.GetProperty("types").EnumerateArray())
        {
            if ((item.TryGetProperty("action", out var action) ? action.GetString() : null) != "retire")
            {
                continue;
            }

            var assemblyName = item.GetProperty("assembly").GetString() ?? string.Empty;
            var project = assemblyName;
            var ns = assemblyName.Replace("Cooperator.Framework.", string.Empty, StringComparison.Ordinal);
            var file = item.GetProperty("file").GetString() ?? string.Empty;
            var declared = item.GetProperty("type").GetString() ?? string.Empty;

            var assembly = Load(project);
            var type = declared.Length > 0 ? assembly.GetType(declared, throwOnError: false) : null;

            if (type is null)
            {
                var stem = Path.GetFileNameWithoutExtension(file);
                var convention = file.StartsWith("Exceptions/", StringComparison.Ordinal)
                    ? $"Cooperator.Framework.{ns}.Exceptions.{stem}"
                    : $"Cooperator.Framework.{ns}.{stem}";
                type = assembly.GetType(convention, throwOnError: false);
            }

            Assert.True(
                type is not null,
                $"El inventario declara un tipo de '{assemblyName}' (archivo '{file}', nombre '{declared}') que no existe en el ensamblado construido.");

            entries.Add((project, type!));
        }

        return entries;
    }

    private static string BuiltAssembly(string project) =>
        Repo.InTree("src", project, "bin", "Release", "netstandard2.1", project + ".dll");

    /// <summary>Un contexto con el runtime del host y <b>los cuatro</b> ensamblados del alcance.</summary>
    private static MetadataLoadContext NewContext()
    {
        var runtime = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        var paths = Directory.GetFiles(runtime, "*.dll").ToList();
        paths.AddRange(Projects.Select(BuiltAssembly).Where(File.Exists));
        return new MetadataLoadContext(new PathAssemblyResolver(paths));
    }

    private static Assembly Load(string project)
    {
        if (Assemblies.TryGetValue(project, out var cached))
        {
            return cached;
        }

        var dll = BuiltAssembly(project);
        Assert.True(File.Exists(dll), $"Falta el ensamblado construido: {dll}");

        return Assemblies[project] = Context.Value.LoadFromAssemblyPath(dll);
    }
}

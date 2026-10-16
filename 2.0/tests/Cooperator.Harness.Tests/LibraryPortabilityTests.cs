using System.Reflection;
using Cooperator.Framework.Library.Storage;
using Cooperator.Harness.Tests.Support;

namespace Cooperator.Harness.Tests;

/// <summary>
/// SPEC-261016-0037 §6/E y §8.1 (AC-1/AC-6): <b>la portabilidad verificada</b> de
/// <c>Cooperator.Framework.Library</c> — compila en su TFM portable <b>sin</b> referencias ni APIs de
/// plataforma y <b>sin</b> dependencias de terceros.
/// <list type="bullet">
///   <item>0 invocación nativa, 0 <c>Microsoft.Win32</c>, 0 identidad de máquina por WMI, 0 log de eventos,
///         0 <c>System.Web</c> y 0 <c>System.Windows.Forms</c> en las fuentes de la librería.</item>
///   <item>0 dependencias de terceros: el presupuesto de ADR-001 §3.4.</item>
///   <item>Las <b>capacidades eliminadas</b> no están: ni sus archivos, ni su rastro.</item>
///   <item>El <b>contrato del almacén</b> es portable: sus firmas usan sólo tipos de la BCL.</item>
/// </list>
/// <para>
/// Los tokens se <b>ensamblan desde fragmentos</b>: este archivo es código propio del árbol y no puede ser, él
/// mismo, una ocurrencia de lo que persigue.
/// </para>
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class LibraryPortabilityTests
{
    private const string LibraryProject = "Cooperator.Framework.Library";

    private static readonly string[] PlatformTokens =
    [
        "Dll" + "Import",
        "System." + "Management",
        "Microsoft." + "Win32",
        "System.Diagnostics." + "EventLog",
        "System." + "Web",
        "System.Windows." + "Forms",
    ];

    /// <summary>Las cuatro capacidades que la fase B elimina: sus archivos no pueden existir.</summary>
    private static readonly string[] EliminatedFiles =
    [
        @"IO\CurrentUserInfo.cs",
        @"IO\EventViewer.cs",
        @"IO\Memory.cs",
        @"IO\NativeMethods.cs",
    ];

    [Fact]
    public void No_platform_api_survives_in_the_library_sources()
    {
        var files = LibrarySources();
        Assert.True(files.Length > 10, $"La librería tiene {files.Length} archivos: el escaneo no puede declarar ausencia.");
        Assert.Contains(files, file => file.EndsWith(@"Library\Utility.cs", StringComparison.Ordinal));

        var offenders = files
            .SelectMany(file => File.ReadAllLines(file)
                .Select((line, index) => (Line: line, Number: index + 1))
                .Where(entry => PlatformTokens.Any(token => entry.Line.Contains(token, StringComparison.Ordinal)))
                .Where(entry => !IsComment(entry.Line))
                .Select(entry => $"{Relative(file)}:{entry.Number}"))
            .ToArray();

        Assert.Empty(offenders);
    }

    [Fact]
    public void The_eliminated_capabilities_and_their_trace_are_gone()
    {
        var directory = Repo.InTree("src", LibraryProject);

        foreach (var eliminated in EliminatedFiles)
        {
            Assert.False(File.Exists(Path.Combine(directory, eliminated)), $"La capacidad eliminada '{eliminated}' sigue en el árbol.");
        }

        // Y el rastro de la capacidad de identidad de máquina por WMI tampoco: ni el método ni el tipo del
        // proveedor de administración (se retiró la capacidad, no se comentó).
        var utility = File.ReadAllText(Path.Combine(directory, "Utility.cs"));
        Assert.DoesNotContain("GetDrive" + "SerialNumber", utility, StringComparison.Ordinal);
        Assert.DoesNotContain("win32_" + "logicaldisk", utility, StringComparison.Ordinal);
    }

    [Fact]
    public void The_library_declares_no_third_party_dependency_and_no_platform_reference()
    {
        var projectFile = Repo.InTree("src", LibraryProject, LibraryProject + ".csproj");
        var project = File.ReadAllText(projectFile);

        Assert.DoesNotContain("PackageReference", project, StringComparison.Ordinal);
        Assert.DoesNotContain("<Reference ", project, StringComparison.Ordinal);

        // Y el ensamblado construido no referencia nada fuera de la BCL y del propio framework.
        var referenced = typeof(UserSettingsKey).Assembly
            .GetReferencedAssemblies()
            .Select(name => name.Name ?? string.Empty)
            .ToArray();

        Assert.NotEmpty(referenced);
        Assert.All(
            referenced,
            name => Assert.True(
                name == "netstandard" || name.StartsWith("System.", StringComparison.Ordinal) || name.StartsWith("Cooperator.Framework.", StringComparison.Ordinal),
                $"La librería referencia '{name}', que no es BCL ni del propio framework."));
    }

    [Fact]
    public void The_store_contract_is_portable_and_uses_only_bcl_types()
    {
        var contract = typeof(IUserSettingsStore);

        Assert.True(contract.IsInterface);
        Assert.True(contract.IsPublic);

        var surface = contract.GetMethods()
            .SelectMany(method => method.GetParameters().Select(parameter => parameter.ParameterType).Append(method.ReturnType));

        Assert.All(
            surface,
            type => Assert.True(
                IsBcl(type),
                $"El contrato del almacén expone '{type.FullName}', que no es de la BCL: la abstracción tiene que ser portable sin dependencias."));
    }

    private static bool IsBcl(Type type)
    {
        // Una definición genérica es idempotente bajo GetGenericTypeDefinition(): sin este guardia, un
        // `Nullable<T>` del contrato (decimal?, DateTime?…) produciría recursión infinita.
        if (type.IsGenericType && !type.IsGenericTypeDefinition)
        {
            return IsBcl(type.GetGenericTypeDefinition());
        }

        var name = type.Namespace ?? string.Empty;
        return name.StartsWith("System", StringComparison.Ordinal)
            || type.IsEnum
            || type.IsPrimitive;
    }

    private static string[] LibrarySources() =>
        [.. Directory.EnumerateFiles(Repo.InTree("src", LibraryProject), "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Split(Path.DirectorySeparatorChar).Any(segment => segment is "bin" or "obj"))
            .OrderBy(file => file, StringComparer.Ordinal)];

    private static bool IsComment(string line)
    {
        var trimmed = line.TrimStart();
        return trimmed.StartsWith("//", StringComparison.Ordinal)
            || trimmed.StartsWith("/*", StringComparison.Ordinal)
            || trimmed.StartsWith("*", StringComparison.Ordinal);
    }

    private static string Relative(string path) => Path.GetRelativePath(Repo.InTree(), path);
}

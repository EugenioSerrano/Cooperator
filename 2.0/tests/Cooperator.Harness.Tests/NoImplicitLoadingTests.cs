using System.Runtime.Loader;
using Cooperator.Framework.Core.Configuration;
using Cooperator.Framework.Core.LazyLoad;
using Cooperator.Framework.Core.LazyLoad.Exceptions;
using Cooperator.Harness.Tests.Support;

namespace Cooperator.Harness.Tests;

/// <summary>
/// AC-7 del SPEC-261016-0015 §7 y el NFR <c>no-implicit-assembly-loading</c> de ADR-006 §5:
/// el camino por defecto <b>no</b> carga desde rutas ni sondea directorios, y ninguna clave de
/// configuración puede provocar una carga desde ruta.
/// <para>
/// <b>La medición es dura, no declarativa.</b> El escenario es real: hay un ensamblado de
/// prueba con un provider válido que <b>existe en el despliegue</b> (copiado a una subcarpeta
/// del directorio de salida) pero que <b>no está declarado</b> como dependencia —igual que el
/// patrón "copiar DLLs a una carpeta" que el producto fomentaba—. Si el framework lo
/// resolviera, sondeando o por ruta, este caso <b>fallaría</b>; y el hook del host, que sí
/// conoce su carpeta y usa su propio <c>AssemblyLoadContext</c>, lo obtiene: la carga por
/// carpeta es del host, no una capacidad implícita del framework.
/// </para>
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class NoImplicitLoadingTests : LazyProviderTestBase
{
    private static string PluginFolder => Path.Combine(AppContext.BaseDirectory, UndeclaredFolder);

    private static string PluginAssemblyPath => Path.Combine(PluginFolder, UndeclaredAssembly + ".dll");

    [Fact]
    public void The_default_path_does_not_find_an_assembly_that_is_present_but_not_declared()
    {
        // El escenario es real: el ensamblado EXISTE en el despliegue…
        Assert.True(File.Exists(PluginAssemblyPath), $"Falta el ensamblado no declarado en '{PluginAssemblyPath}'.");

        // …y NO está declarado: no figura entre las dependencias del proyecto de pruebas.
        var deps = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Cooperator.Harness.Tests.deps.json"));
        Assert.DoesNotContain(UndeclaredAssembly, deps, StringComparison.Ordinal);

        var loadedBefore = LoadedAssemblyNames().ToArray();

        var failure = Assert.Throws<ProviderAssemblyNotFoundException>(
            () => ProviderResolver.Resolve(new ProviderName(UndeclaredAssembly, UndeclaredProviderType)));

        // La resolución falla CON el nombre intentado y la causa del runtime: el cargador no
        // lo encuentra —ni por sondeo de directorios ni por ruta— porque no es una dependencia.
        Assert.Equal(UndeclaredAssembly, failure.AssemblyName);
        Assert.True(
            failure.InnerException is FileNotFoundException or FileLoadException,
            $"El diagnóstico debe llevar la causa del runtime; llegó '{failure.InnerException?.GetType().Name}'.");

        // Y esta llamada no cargó el ensamblado (el otro caso de esta clase sí lo hace, con el
        // hook del host: por eso la comparación es contra el conjunto previo, no absoluta).
        Assert.DoesNotContain(LoadedAssemblyNames(), name => name == UndeclaredAssembly && !loadedBefore.Contains(name, StringComparer.Ordinal));
    }

    [Fact]
    public void The_hook_of_the_host_obtains_what_the_default_path_cannot()
    {
        Assert.True(File.Exists(PluginAssemblyPath), $"Falta el ensamblado no declarado en '{PluginAssemblyPath}'.");

        // La política de carga del host: SU AssemblyLoadContext, SU carpeta de plugins.
        ProviderResolution.Register(new PluginFolderHook(PluginFolder));

        var provider = LazyProviderFactory.Get(UndeclaredAssembly, UndeclaredProviderType);

        Assert.Equal(UndeclaredProviderType, provider.GetType().FullName);
        Assert.Contains(LoadedAssemblyNames(), name => name == UndeclaredAssembly);
    }

    [Fact]
    public void The_framework_never_calls_Assembly_LoadFrom_or_LoadFile_and_the_control_is_not_vacuous()
    {
        // Medición sobre el CÓDIGO DEL FRAMEWORK (2.0/src): 0 llamadas a la carga por ruta.
        var frameworkOffenders = SourceFiles(Repo.InTree("src"))
            .Where(file => File.ReadAllText(file).Contains("Assembly.LoadFrom(", StringComparison.Ordinal)
                || File.ReadAllText(file).Contains("Assembly.LoadFile(", StringComparison.Ordinal))
            .ToArray();

        Assert.Empty(frameworkOffenders);

        // Y el control NO es vacuo: el árbol 1.4 congelado SÍ contiene esa carga, que es
        // exactamente la superficie que este Bolt retira. Si el escaneo no la encontrara, el
        // caso estaría midiendo nada.
        var frozenOffenders = SourceFiles(Repo.InRepo("1.4", "Cooperator.Framework.Core"))
            .Where(file => File.ReadAllText(file).Contains("Assembly.LoadFrom(", StringComparison.Ordinal))
            .ToArray();

        Assert.NotEmpty(frozenOffenders);
    }

    [Fact]
    public void No_configuration_key_can_trigger_a_load_from_a_path()
    {
        // Las claves del camino son EXACTAMENTE las dos del contrato (ADR-004 §3.2): no hay
        // una clave de ruta, ni una carpeta, ni un sondeo configurable.
        Assert.Equal(":Assembly", ConfigurationKeys.LazyLoad.AssemblySuffix);
        Assert.Equal(":Class", ConfigurationKeys.LazyLoad.ClassSuffix);
        Assert.DoesNotContain("Path", ConfigurationKeys.LazyLoad.ProvidersPrefix, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Folder", ConfigurationKeys.LazyLoad.ProvidersPrefix, StringComparison.OrdinalIgnoreCase);

        // Y un valor con FORMA DE RUTA no se carga como ruta: no resuelve y no carga nada.
        var loadedBefore = LoadedAssemblyNames().ToArray();

        var pathShaped = Assert.Throws<ProviderAssemblyNotFoundException>(
            () => ProviderResolver.Resolve(new ProviderName(PluginAssemblyPath, UndeclaredProviderType)));

        Assert.Equal(PluginAssemblyPath, pathShaped.AssemblyName);
        Assert.DoesNotContain(LoadedAssemblyNames(), name => name == UndeclaredAssembly && !loadedBefore.Contains(name, StringComparer.Ordinal));
    }

    private static IEnumerable<string> LoadedAssemblyNames() =>
        AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetName().Name ?? string.Empty);

    private static IEnumerable<string> SourceFiles(string root) =>
        Directory
            .EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Split(Path.DirectorySeparatorChar).Any(segment => segment is "bin" or "obj" or "artifacts"));

    /// <summary>
    /// El hook del host: resuelve con su propio <c>AssemblyLoadContext</c> desde su carpeta de
    /// plugins. Es la política de carga del host, fuera del framework (ADR-006 §3.1).
    /// </summary>
    private sealed class PluginFolderHook : IAssemblyProvider
    {
        private readonly string _folder;
        private readonly AssemblyLoadContext _context = new AssemblyLoadContext("harness-host-plugins", isCollectible: false);

        public PluginFolderHook(string folder) => _folder = folder;

        public ILazyProvider CreateProvider(ProviderName name)
        {
            var assembly = _context.LoadFromAssemblyPath(Path.Combine(_folder, name.AssemblyName + ".dll"));
            var type = assembly.GetType(name.TypeName, throwOnError: true)
                ?? throw new InvalidOperationException($"'{name.TypeName}' no existe en '{assembly.FullName}'.");

            return (ILazyProvider)Activator.CreateInstance(type)!;
        }
    }
}

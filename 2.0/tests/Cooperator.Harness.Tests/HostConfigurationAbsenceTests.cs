using System.Runtime.CompilerServices;
using System.Text.Json;
using Cooperator.Framework.Core.Configuration;
using Cooperator.Framework.Library;
using Cooperator.Harness.Tests.Support;

namespace Cooperator.Harness.Tests;

/// <summary>
/// Control de ausencia de la configuración del host y constancia de lo retirado
/// (SPEC-261016-0018 §6/C, §6/E, §8.1; AC-4, AC-5, AC-6, AC-7).
///
/// El control se mide <b>sobre la capa que exista</b>: recorre las fuentes de <c>2.0/src</c>, de modo que al
/// aparecer las fuentes de <c>Data</c> (Bolts 012/013/014) el caso las alcanza solo y exige lo mismo de ellas.
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class HostConfigurationAbsenceTests
{
    private static readonly string[] HostConfigurationApis =
    [
        "ConfigurationManager",
        "OpenMappedExeConfiguration",
        "OpenExeConfiguration",
        "ExeConfigurationFileMap",
        "ConfigurationUserLevel",
        "ConfigurationSaveMode",
        "ConfigurationSection",
        "ConfigurationElement",
        "ConfigurationElementCollection",
        "AppSettings",
    ];

    private static readonly string[] RemovedLazyLoadTypes =
    [
        "ProvidersSection",
        "ProvidersCollection",
        "LazyConfigurationHelper",
        "CreateEmpty",
    ];

    // ------------------------------------------------------------------ AC-4

    [Fact]
    public void No_source_file_of_the_existing_layer_uses_the_host_configuration()
    {
        var offenders = new List<string>();
        foreach (var file in SourceFiles())
        {
            var text = File.ReadAllText(file);
            foreach (var api in HostConfigurationApis)
            {
                if (text.Contains(api, StringComparison.Ordinal))
                {
                    offenders.Add($"{Path.GetRelativePath(Repo.TreeRoot, file)}: {api}");
                }
            }
        }

        Assert.Empty(offenders);
    }

    [Fact]
    public void No_project_declares_the_classic_configuration_package()
    {
        // Se buscan DECLARACIONES (PackageReference / Reference), no la mera mención: el .csproj de Data explica
        // en un comentario que el paquete NO entra, y eso no es una declaración.
        var offenders = new List<string>();
        foreach (var project in ProjectFiles())
        {
            var text = File.ReadAllText(project);
            foreach (var line in text.Split('\n'))
            {
                var trimmed = line.Trim();
                if (!trimmed.StartsWith("<PackageReference", StringComparison.Ordinal) &&
                    !trimmed.StartsWith("<Reference", StringComparison.Ordinal))
                {
                    continue;
                }

                if (trimmed.Contains("System.Configuration", StringComparison.Ordinal))
                {
                    offenders.Add($"{Path.GetRelativePath(Repo.TreeRoot, project)}: {trimmed}");
                }
            }
        }

        Assert.Empty(offenders);
    }

    [Fact]
    public void The_custom_lazyload_section_does_not_exist_in_the_code()
    {
        var offenders = new List<string>();
        foreach (var file in SourceFiles())
        {
            var text = File.ReadAllText(file);
            foreach (var type in RemovedLazyLoadTypes)
            {
                if (text.Contains(type, StringComparison.Ordinal))
                {
                    offenders.Add($"{Path.GetRelativePath(Repo.TreeRoot, file)}: {type}");
                }
            }
        }

        Assert.Empty(offenders);
    }

    [Fact]
    public void The_absence_control_states_which_layer_it_measured()
    {
        // La medición se acota a la capa que existe al cierre de este Bolt, y el caso lo declara en vez de
        // suponerlo: hoy Core y Library tienen fuentes; Data y Modeling todavía no.
        var layer = SourceFiles()
            .Select(file => Path.GetRelativePath(Repo.InTree("src"), file).Split(Path.DirectorySeparatorChar)[0])
            .Distinct(StringComparer.Ordinal)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Contains("Cooperator.Framework.Core", layer);
        Assert.Contains("Cooperator.Framework.Library", layer);
        // SPEC-261016-0013 §6/C.2 (revisión 3): la exigencia de que Data NO figurara entre las capas con
        // fuentes era una INSTANTÁNEA del cierre del Bolt 011 —cuando Data no tenía fuentes— y el aporte de
        // las 16 excepciones de Data llega DESPUÉS POR DISEÑO (§B.2/§B.4 del mismo SPEC): el SPEC vigente y
        // esa línea eran mutuamente excluyentes y cede la instantánea, no la intención. Medido en el
        // V-Bounce N°2 del Bolt 007 (MEM-261016-0220 §8.1). Se conserva lo que el caso existe para probar:
        // que el control DECLARE la capa que midió —y que la declaración no sea vacía— en vez de suponerla.
        Assert.NotEmpty(layer);
    }

    // ------------------------------------------------------------------ AC-5 / AC-7

    [Fact]
    public void The_registered_obligation_of_the_data_layer_is_in_the_fragment_with_its_cross_references()
    {
        using var fragment = JsonDocument.Parse(File.ReadAllText(FragmentPath()));

        var obligations = fragment.RootElement.GetProperty("registered_obligations").EnumerateArray().ToArray();
        Assert.NotEmpty(obligations);

        var obligation = obligations.Single(entry => entry.GetProperty("id").GetString() == "OBLIGATION-DATA-01");
        var statement = obligation.GetProperty("statement").GetString()!;
        Assert.Contains("ConfigurationManager", statement, StringComparison.Ordinal);
        Assert.Contains("ICooperatorConfiguration", statement, StringComparison.Ordinal);

        var crossReferences = obligation.GetProperty("cross_references").EnumerateArray()
            .Select(value => value.GetString()!).ToArray();
        Assert.Contains(crossReferences, reference => reference.Contains("SPEC-261016-0019", StringComparison.Ordinal));
        Assert.Contains(crossReferences, reference => reference.Contains("SPEC-261016-0020", StringComparison.Ordinal));

        var owners = obligation.GetProperty("owner_bolts").EnumerateArray().Select(value => value.GetString()!).ToArray();
        Assert.Contains("US-000.BOLT-013", owners);
        Assert.Contains("US-000.BOLT-014", owners);
    }

    [Fact]
    public void Every_fragment_entry_declares_what_why_and_replacement()
    {
        using var fragment = JsonDocument.Parse(File.ReadAllText(FragmentPath()));
        var entries = fragment.RootElement.GetProperty("entries").EnumerateArray().ToArray();

        Assert.NotEmpty(entries);
        foreach (var entry in entries)
        {
            foreach (var field in new[] { "id", "kind", "what", "why", "replacement", "status" })
            {
                var value = entry.GetProperty(field).GetString();
                Assert.False(string.IsNullOrWhiteSpace(value), $"{entry.GetProperty("id").GetString()} no declara '{field}'.");
            }
        }
    }

    [Fact]
    public void The_breakages_deferred_to_the_data_layer_are_registered_with_their_owner()
    {
        using var fragment = JsonDocument.Parse(File.ReadAllText(FragmentPath()));
        var deferred = fragment.RootElement.GetProperty("entries").EnumerateArray()
            .Where(entry => entry.GetProperty("status").GetString() == "deferred-to-layer")
            .ToArray();

        // Las dos rupturas cuyo retiro materializan los Bolts de la capa Data: fuente de la cadena de conexión
        // (con su heurística y su caché estático) y fuente del timeout. El SPEC §6/E.2 las nombra expresamente.
        Assert.Contains(deferred, entry => entry.GetProperty("id").GetString() == "BOLT011-06");
        Assert.Contains(deferred, entry => entry.GetProperty("id").GetString() == "BOLT011-05");

        foreach (var entry in deferred)
        {
            Assert.NotEmpty(entry.GetProperty("owner_bolts").EnumerateArray());
            Assert.NotEmpty(entry.GetProperty("cross_references").EnumerateArray());
        }
    }

    // ------------------------------------------------------------------ AC-6

    [Fact]
    public void The_smtp_type_loads_without_a_static_initializer_that_explodes_when_the_key_is_absent()
    {
        Environment.SetEnvironmentVariable("Cooperator__Mail__SmtpServer", null);

        // El que explotaba en 1.4 era el *type-initializer* (NullReferenceException al cargar el tipo).
        // Forzarlo acá con la clave ausente: el tipo se carga, no revienta.
        var exception = Record.Exception(() => RuntimeHelpers.RunClassConstructor(typeof(SmtpMail).TypeHandle));

        Assert.Null(exception);
    }

    [Fact]
    public void The_smtp_value_is_read_from_the_catalog_and_its_absence_produces_the_typed_error()
    {
        Environment.SetEnvironmentVariable("Cooperator__Mail__SmtpServer", null);
        CooperatorSettings.Current = new EnvironmentConfiguration();

        var exception = Assert.Throws<CooperatorConfigurationException>(
            () => CooperatorSettings.Current.GetString(ConfigurationKeys.Mail.SmtpServer));

        Assert.Contains(ConfigurationKeys.Mail.SmtpServer, exception.Message, StringComparison.Ordinal);
    }

    // ------------------------------------------------------------------ Auxiliares

    private static string FragmentPath() => Repo.InTree("docs", "api-removals", "US-000.BOLT-011.json");

    private static IEnumerable<string> SourceFiles() => FilesOf("src", "*.cs");

    private static IEnumerable<string> ProjectFiles() => FilesOf("src", "*.csproj");

    private static IEnumerable<string> FilesOf(string folder, string pattern) =>
        Directory.GetFiles(Repo.InTree(folder), pattern, SearchOption.AllDirectories)
            .Where(file => !IsBuildOutput(file));

    private static bool IsBuildOutput(string file)
    {
        var parts = file.Split(Path.DirectorySeparatorChar);
        return parts.Contains("bin", StringComparer.OrdinalIgnoreCase) ||
               parts.Contains("obj", StringComparer.OrdinalIgnoreCase);
    }
}

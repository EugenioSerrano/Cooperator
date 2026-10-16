using System.Text.Json;
using System.Text.RegularExpressions;
using Cooperator.Harness.Tests.Support;

namespace Cooperator.Harness.Tests;

/// <summary>
/// Invariantes del build centralizado (SPEC-261016-0000 §6.A, AC-6) más las
/// restricciones duras AC-1 y AC-8. Todas las aserciones se apoyan en artefactos
/// reales del árbol —los .csproj, los ensamblados construidos, los lock files—
/// y no en expectativas sobre el contenido de los scripts.
/// </summary>
public sealed class BuildInvariantsTests
{
    private static readonly string[] Portables =
    [
        "Cooperator.Framework.Core",
        "Cooperator.Framework.Library",
        "Cooperator.Framework.Data",
        "Cooperator.Framework.Modeling",
    ];

    private static readonly string ReleaseOutput = Path.Combine("bin", "Release", "netstandard2.1");

    // ---------------------------------------------------------------- Toolchain

    [Fact]
    public void GlobalJson_pins_the_sdk_without_roll_forward()
    {
        var globalJson = Repo.InTree("global.json");
        Assert.True(File.Exists(globalJson), "Falta 2.0/global.json: el SDK no está fijado.");

        using var document = JsonDocument.Parse(File.ReadAllText(globalJson));
        var sdk = document.RootElement.GetProperty("sdk");
        Assert.Equal("10.0.402", sdk.GetProperty("version").GetString());
        Assert.Equal("disable", sdk.GetProperty("rollForward").GetString());
    }

    [Fact]
    public void DirectoryBuildProps_declares_the_quality_determinism_and_signing_properties()
    {
        var props = File.ReadAllText(Repo.InTree("Directory.Build.props"));

        Assert.Contains("<Deterministic>true</Deterministic>", props, StringComparison.Ordinal);
        Assert.Contains("<ContinuousIntegrationBuild", props, StringComparison.Ordinal);
        Assert.Contains("<PathMap", props, StringComparison.Ordinal);
        Assert.Contains("<TreatWarningsAsErrors>true</TreatWarningsAsErrors>", props, StringComparison.Ordinal);
        Assert.Contains("<Nullable>enable</Nullable>", props, StringComparison.Ordinal);
        Assert.Contains("<SignAssembly>true</SignAssembly>", props, StringComparison.Ordinal);
        Assert.Contains("<AssemblyOriginatorKeyFile>", props, StringComparison.Ordinal);
        Assert.Contains("<Version>2.0.0</Version>", props, StringComparison.Ordinal);
        Assert.Contains("<AssemblyVersion>2.0.0.0</AssemblyVersion>", props, StringComparison.Ordinal);
        Assert.Contains("<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>", props, StringComparison.Ordinal);
    }

    [Fact]
    public void DirectoryBuildTargets_wires_package_validation_without_a_baseline_version()
    {
        var targets = File.ReadAllText(Repo.InTree("Directory.Build.targets"));

        Assert.Contains("<EnablePackageValidation>true</EnablePackageValidation>", targets, StringComparison.Ordinal);
        // ADR-008 §5 / SPEC §6.D.4: el mecanismo queda cableado y SIN valor de baseline.
        Assert.Contains("<PackageValidationBaselineVersion></PackageValidationBaselineVersion>", targets, StringComparison.Ordinal);
    }

    [Fact]
    public void DirectoryPackagesProps_enables_central_package_management_and_declares_versions()
    {
        var props = File.ReadAllText(Repo.InTree("Directory.Packages.props"));

        Assert.Contains("<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>", props, StringComparison.Ordinal);
        Assert.Contains("Include=\"xunit.v3\"", props, StringComparison.Ordinal);
        Assert.Contains("Include=\"Microsoft.NET.Test.Sdk\"", props, StringComparison.Ordinal);
        Assert.Contains("Include=\"System.Text.Json\"", props, StringComparison.Ordinal);
    }

    // ------------------------------------------------------------------- Layout

    [Fact]
    public void Every_portable_project_targets_netstandard21()
    {
        foreach (var project in Portables)
        {
            var csproj = Repo.InTree("src", project, project + ".csproj");
            Assert.True(File.Exists(csproj), $"Falta el proyecto portable {project}.");
            var text = File.ReadAllText(csproj);
            Assert.Contains("<TargetFramework>netstandard2.1</TargetFramework>", text, StringComparison.Ordinal);
            Assert.Contains("<IsPackable>true</IsPackable>", text, StringComparison.Ordinal);
            Assert.DoesNotContain("<TargetFrameworks>", text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void Portable_assemblies_declare_the_fixed_2_0_0_0_assembly_version()
    {
        foreach (var project in Portables)
        {
            var dll = Repo.InTree("src", project, ReleaseOutput, project + ".dll");
            Assert.True(File.Exists(dll), $"Falta el ensamblado construido {dll}.");
            var name = System.Reflection.AssemblyName.GetAssemblyName(dll);
            Assert.Equal("2.0.0.0", name.Version!.ToString());
        }
    }

    // ------------------------------------------------------------- Signing identity

    [Fact]
    public void Signing_key_is_byte_identical_to_the_frozen_1_4_copy()
    {
        var treeKey = Repo.InTree("CooperatorFramework.snk");
        var frozenKey = Repo.InRepo("1.4", "Cooperator.Framework.Core", "CooperatorFramework.snk");

        Assert.True(File.Exists(treeKey), "Falta la copia del .snk en el árbol 2.0.");
        Assert.True(File.Exists(frozenKey), "Falta el .snk del árbol 1.4 congelado.");

        var treeBytes = File.ReadAllBytes(treeKey);
        var frozenBytes = File.ReadAllBytes(frozenKey);

        Assert.Equal(frozenBytes.Length, treeBytes.Length);
        Assert.Equal(
            Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(frozenBytes)),
            Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(treeBytes)));
    }

    [Fact]
    public void Portable_assemblies_are_signed_with_the_same_key_as_the_1_4_framework()
    {
        var frozenCore = Repo.InRepo("1.4", "Cooperator.Framework.Core", "bin", "Release", "Cooperator.Framework.Core.dll");
        Assert.True(File.Exists(frozenCore), "Falta el ensamblado 1.4 de referencia para comparar la identidad de firma.");

        var expected = Convert.ToHexString(System.Reflection.AssemblyName.GetAssemblyName(frozenCore).GetPublicKeyToken()!);
        Assert.NotEmpty(expected);

        foreach (var project in Portables)
        {
            var dll = Repo.InTree("src", project, ReleaseOutput, project + ".dll");
            var actual = Convert.ToHexString(System.Reflection.AssemblyName.GetAssemblyName(dll).GetPublicKeyToken()!);
            Assert.Equal(expected, actual);
        }
    }

    // ------------------------------------------------------- Dependency policy

    [Fact]
    public void Core_and_Library_declare_zero_third_party_direct_dependencies()
    {
        foreach (var project in new[] { "Cooperator.Framework.Core", "Cooperator.Framework.Library" })
        {
            Assert.Empty(DirectThirdPartyDependencies(project));
        }
    }

    [Fact]
    public void Data_declares_zero_third_party_dependencies()
    {
        // PROCEDENCIA (SPEC-261016-0020 6/A.1): el Bolt 014 fija el provider ADO.NET unico de la capa de datos
        // (Microsoft.Data.SqlClient, ADR-005 3.1 / ADR-001 3.4). La invariante nacio cuando Data NO tenia provider.
        // Adjudicacion del AITL-MEM-Approval del V-Bounce N02 (MEM-261016-0407 8.3) -- alcance exacto: SOLO esta
        // asercion y la de No_lock_file_pulls_configuration_manager.
        Assert.Equal(["Microsoft.Data.SqlClient"], DirectThirdPartyDependencies("Cooperator.Framework.Data"));
    }

    [Fact]
    public void Modeling_declares_exactly_system_text_json()
    {
        var declared = DirectThirdPartyDependencies("Cooperator.Framework.Modeling");
        Assert.Equal(["System.Text.Json"], declared);
    }

    [Fact]
    public void No_lock_file_pulls_configuration_manager()
    {
        foreach (var project in Portables)
        {
            var lockFile = Repo.InTree("src", project, "packages.lock.json");
            Assert.True(File.Exists(lockFile), $"Falta el lock file de {project}.");
            // PROCEDENCIA (SPEC-261016-0020 6/D.2 y 8.3): la excepcion transitoria de
            // System.Configuration.ConfigurationManager que Data declaraba en 1.4 esta VENCIDA (ADR-001 5) y su
            // declaracion DIRECTA sigue prohibida. La aparicion TRANSITIVA en el lock la aporta el provider
            // autorizado (Microsoft.Data.SqlClient, ADR-005 3.1) y NO es una declaracion del arbol: fallar por ella
            // seria culpar a la capa de una dependencia del proveedor. Adjudicacion del AITL-MEM-Approval del
            // V-Bounce N02 (MEM-261016-0407 8.3) -- alcance exacto: SOLO esta asercion y la de
            // Data_declares_zero_third_party_dependencies.
            using var lockDocument = System.Text.Json.JsonDocument.Parse(File.ReadAllText(lockFile));
            foreach (var target in lockDocument.RootElement.GetProperty("dependencies").EnumerateObject())
            {
                foreach (var dependency in target.Value.EnumerateObject())
                {
                    if (!dependency.Name.Contains("System.Configuration.ConfigurationManager", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    bool direct = dependency.Value.TryGetProperty("type", out var kind)
                        && string.Equals(kind.GetString(), "Direct", StringComparison.OrdinalIgnoreCase);
                    Assert.False(direct, $"{project} declara System.Configuration.ConfigurationManager: ADR-001 5 prohibe esa excepcion transitoria.");
                }
            }
        }
    }

    [Fact]
    public void No_project_file_declares_a_package_version()
    {
        var projects = Directory.GetFiles(Repo.InTree("src"), "*.csproj", SearchOption.AllDirectories);
        // Los proyectos de `src` son los portables MÁS las aplicaciones declaradas (ADR-001 §3.1: la utilidad de
        // protección de valores es una aplicación Windows-only que sobrevive, no un paquete portable). La lista es
        // EXPLÍCITA para que un proyecto nuevo tenga que declararse acá y no pase por descuido.
        var applications = new[] { "ConnectionStringEncriptor", "CooperatorModeler" };
        Assert.Equal(Portables.Length + applications.Length, projects.Length);
        Assert.All(
            applications,
            name => Assert.Contains(projects, project => project.EndsWith(name + Path.DirectorySeparatorChar + name + ".csproj", StringComparison.OrdinalIgnoreCase)));

        foreach (var project in projects)
        {
            foreach (var reference in PackageReferences(File.ReadAllText(project)))
            {
                Assert.False(
                    reference.Contains("Version", StringComparison.Ordinal),
                    $"{Path.GetFileName(project)} declara una versión de paquete: {reference}. ADR-001 §3.1 exige versión centralizada.");
            }
        }
    }

    [Fact]
    public void Every_project_of_the_tree_has_a_versioned_lock_file()
    {
        var projects = TreeProjects();

        // Bolt 003 — SPEC-261016-0004 §A.4 (única excepción autorizada sobre un archivo
        // del Bolt 001): el árbol crece por diseño —la suite del paquete portable es su
        // 9.º proyecto— y el conteo EXACTO impedía agregarlo (hallazgo medido en el
        // V-Bounce N°3, MEM-261016-0125 §8.1: Failed: 1, Passed: 48, Total: 49;
        // Expected: 8 / Actual: 9). El conteo pasa a ser un MÍNIMO —una guardia contra
        // una enumeración débil que perdiera proyectos—; la INTENCIÓN no cambia y el
        // foreach de packages.lock.json de abajo queda intacto: sigue exigiendo el lock
        // file versionado en CADA proyecto del árbol, incluido el proyecto nuevo.
        Assert.True(projects.Length >= Portables.Length + 4,
            $"El árbol tiene {projects.Length} proyectos: menos de los {Portables.Length + 4} esperados.");

        foreach (var project in projects)
        {
            var lockFile = Path.Combine(Path.GetDirectoryName(project)!, "packages.lock.json");
            Assert.True(File.Exists(lockFile), $"El proyecto {Path.GetFileName(project)} no tiene packages.lock.json versionado.");
        }
    }

    /// <summary>
    /// Proyectos del árbol 2.0, excluyendo salidas de build (<c>bin/</c>, <c>obj/</c>,
    /// <c>artifacts/</c>): el arnés copia plantillas del 1.4 a su salida y esas copias
    /// no son proyectos del árbol.
    /// </summary>
    private static string[] TreeProjects()
    {
        var excluded = new[] { "bin", "obj", "artifacts", "TestResults" };
        return
        [
            .. Directory.GetFiles(Repo.InTree(), "*.csproj", SearchOption.AllDirectories)
                .Where(file => !file.Split(Path.DirectorySeparatorChar)
                    .Any(segment => excluded.Contains(segment, StringComparer.OrdinalIgnoreCase)))
                .OrderBy(file => file, StringComparer.Ordinal),
        ];
    }

    // ------------------------------------------------------------- Frozen tree

    [Fact]
    public void Frozen_tree_is_intact_relative_to_the_baseline_commit()
    {
        var baseline = BaselineCommit();

        var diff = Shell.Git("diff", "--quiet", baseline, "--", "1.4");
        Assert.True(diff.ExitCode == 0, $"`git diff --quiet {baseline} -- 1.4` no está vacío:{Environment.NewLine}{diff.AllText}");

        var status = Shell.Git("status", "--porcelain", "--", "1.4");
        Assert.True(
            string.IsNullOrWhiteSpace(status.StandardOutput),
            $"Hay archivos sin seguimiento o modificados bajo 1.4/**:{Environment.NewLine}{status.StandardOutput}");
    }

    [Fact]
    public void Baseline_commit_matches_the_one_the_spec_declares()
    {
        Assert.Equal("6aa497f793cd3ab9d83c68db7ef2c1d80d3421d4", BaselineCommit());
    }

    private static string BaselineCommit() =>
        File.ReadAllLines(Repo.InTree("build", "frozen-source.json"))
            .Select(line => Regex.Match(line, "\"frozen_commit\"\\s*:\\s*\"(?<commit>[0-9a-f]{40})\""))
            .Where(match => match.Success)
            .Select(match => match.Groups["commit"].Value)
            .First();

    private static IEnumerable<string> PackageReferences(string csproj) =>
        Regex.Matches(csproj, "<PackageReference\\s+[^>]*/?>")
            .Select(match => match.Value);

    private static string[] DirectThirdPartyDependencies(string project)
    {
        var lockFile = Repo.InTree("src", project, "packages.lock.json");
        Assert.True(File.Exists(lockFile), $"Falta el lock file de {project}.");

        using var document = JsonDocument.Parse(File.ReadAllText(lockFile));
        var result = new List<string>();
        foreach (var framework in document.RootElement.GetProperty("dependencies").EnumerateObject())
        {
            foreach (var dependency in framework.Value.EnumerateObject())
            {
                if (!dependency.Value.TryGetProperty("type", out var type) || type.GetString() != "Direct")
                {
                    continue;
                }

                var id = dependency.Name;
                // System.Text.Json es la ÚNICA dependencia de terceros que ADR-001 §3.4
                // admite (y sólo en Modeling): se cuenta como tal aunque su identificador
                // empiece por "System.".
                if (id.Equals("System.Text.Json", StringComparison.Ordinal))
                {
                    result.Add(id);
                    continue;
                }

                if (id.StartsWith("System.", StringComparison.Ordinal) ||
                    id.StartsWith("Microsoft.NETCore.", StringComparison.Ordinal) ||
                    id.StartsWith("Microsoft.CSharp", StringComparison.Ordinal) ||
                    id.Equals("NETStandard.Library", StringComparison.Ordinal))
                {
                    continue;
                }

                result.Add(id);
            }
        }

        return [.. result.OrderBy(x => x, StringComparer.Ordinal)];
    }
}

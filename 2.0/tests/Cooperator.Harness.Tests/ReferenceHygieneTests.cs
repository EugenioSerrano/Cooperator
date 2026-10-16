using System.Text.RegularExpressions;
using System.Text.Json;
using Cooperator.Harness.Tests.Support;

namespace Cooperator.Harness.Tests;

/// <summary>
/// SPEC-261016-0021 §6/C y §8.1: el <b>control activo</b> que hace que la limpieza de referencias sea
/// «una sola vez». Recorre <b>todos</b> los archivos de proyecto del Árbol 2.0 —incluidos los que porten
/// los Bolts 013–030, presentes o futuros— y <b>falla</b> si reaparece cualquiera de las condiciones de la
/// denylist: una referencia de la lista de declaraciones muertas inventariadas, el bloque de metadata de
/// publicación clásica (ClickOnce) o un ítem <c>&lt;BootstrapperPackage&gt;</c>, un <c>packages.config</c>,
/// <c>HintPath</c>/<c>SpecificVersion</c>, o una <c>Reference</c> a un ensamblado fuera del runtime objetivo.
/// <para>
/// Además: el <b>control negativo del propio control</b> (las declaraciones legítimas del runtime objetivo no
/// fallan), la <b>prueba negativa</b> (una declaración muerta inyectada a propósito se detecta — «el control
/// que nunca falla no controla nada») y el <b>cruce con el inventario</b> del Bolt (ninguna entrada marcada
/// como retirada sin su constancia de usos).
/// </para>
/// <para>
/// Los tokens de la denylist se <b>ensamblan desde fragmentos</b>: este archivo es código propio del árbol y
/// no puede ser, él mismo, una ocurrencia de lo que persigue (misma disciplina que el control de rastro del
/// Bolt 006). El control de higiene es, por diseño, el lugar donde esos nombres se nombran.
/// </para>
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class ReferenceHygieneTests
{
    private const string InventoryRelative = @"docs\reference-inventory\US-000.BOLT-012.json";

    /// <summary>Las declaraciones del inventario del REV-001 que el Árbol 2.0 no puede volver a declarar.</summary>
    private static readonly string[] DeadDeclarations =
    [
        "System." + "Web",
        "System.Data." + "OracleClient",
        "System.Windows." + "Forms",
        "System." + "Deployment",
    ];

    /// <summary>Metadata de publicación clásica (ClickOnce) y prerrequisitos de arranque.</summary>
    private static readonly string[] PublishMetadata =
    [
        "<BootstrapperPackage",
        "<PublishUrl>",
        "<InstallFrom>",
        "<UpdateEnabled>",
        "<MapFileExtensions>",
        "<ApplicationRevision>",
        "<CreateWebPageOnPublish>",
    ];

    /// <summary>Restos del modelo de proyecto clásico.</summary>
    private static readonly string[] ClassicProjectRemains = ["<HintPath>", "<SpecificVersion>", "<Reference Include="];

    [Fact]
    public void The_projects_of_the_tree_carry_no_dead_declaration_nor_publication_metadata()
    {
        var projects = TreeProjects();
        Assert.NotEmpty(projects);
        Assert.Contains(projects, project => project.EndsWith(@"src\Cooperator.Framework.Core\Cooperator.Framework.Core.csproj", StringComparison.Ordinal));

        var violations = projects
            .SelectMany(project => Violations(File.ReadAllText(project)).Select(violation => $"{Relative(project)}: {violation}"))
            .ToArray();

        Assert.Empty(violations);
    }

    [Fact]
    public void Legitimate_declarations_of_the_target_runtime_do_not_trip_the_control()
    {
        // Control negativo del propio control: lo que un proyecto del 2.0 SÍ declara legítimamente
        // (ProjectReference entre capas, paquetes del mapa centralizado) no puede contar como violación.
        var legitimate = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup><TargetFramework>netstandard2.1</TargetFramework></PropertyGroup>
              <ItemGroup>
                <ProjectReference Include="..\Cooperator.Framework.Core\Cooperator.Framework.Core.csproj" />
                <PackageReference Include="System.Text.Json" />
              </ItemGroup>
            </Project>
            """;

        Assert.Empty(Violations(legitimate));

        // Y los proyectos reales del árbol —que declaran eso— tampoco: el control falla por la denylist,
        // no por declarar cosas legítimas.
        Assert.All(TreeProjects(), project => Assert.Empty(Violations(File.ReadAllText(project))));
    }

    [Fact]
    public void The_control_detects_each_dead_declaration_injected_on_purpose()
    {
        foreach (var declaration in DeadDeclarations)
        {
            var planted = $"""
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup><TargetFramework>netstandard2.1</TargetFramework></PropertyGroup>
                  <ItemGroup><Reference Include="{declaration}" /></ItemGroup>
                </Project>
                """;

            Assert.Contains(Violations(planted), violation => violation.Contains(declaration, StringComparison.Ordinal));
        }

        foreach (var planted in new[]
        {
            "<Project><ItemGroup><BootstrapperPackage Include=\"X\" /></ItemGroup></Project>",
            "<Project><PropertyGroup><PublishUrl>http://x</PublishUrl></PropertyGroup></Project>",
            "<Project><ItemGroup><Reference Include=\"X\"><HintPath>..\\x.dll</HintPath></Reference></ItemGroup></Project>",
            "<Project><ItemGroup><Reference Include=\"X\"><SpecificVersion>True</SpecificVersion></Reference></ItemGroup></Project>",
        })
        {
            Assert.NotEmpty(Violations(planted));
        }
    }

    [Fact]
    public void The_inventory_records_no_removal_without_its_use_search_evidence()
    {
        var path = Repo.InTree(InventoryRelative.Split('\\'));
        Assert.True(File.Exists(path), $"Falta el inventario del Bolt: {path}.");

        using var inventory = JsonDocument.Parse(File.ReadAllText(path));
        var root = inventory.RootElement;

        Assert.Equal("US-000.BOLT-012", root.GetProperty("bolt").GetString());
        Assert.Equal("SPEC-261016-0021", root.GetProperty("spec").GetString());

        // La frontera del inventario (§4): qué declaración en uso tiene dueño declarado.
        var frontier = root.GetProperty("watched_declarations").EnumerateArray()
            .Where(item => !string.IsNullOrWhiteSpace(item.GetProperty("owner").GetString()))
            .ToDictionary(
                item => item.GetProperty("name").GetString() ?? string.Empty,
                item => item.GetProperty("owner").GetString() ?? string.Empty,
                StringComparer.Ordinal);

        // El Árbol 2.0 no declara ninguna de las declaraciones muertas inventariadas.
        Assert.Empty(root.GetProperty("after_denylist_hits").EnumerateArray());

        // Y cada declaración de las dos columnas trae su búsqueda de usos: el comando y el número de coincidencias.
        foreach (var column in new[] { "before", "after" })
        {
            foreach (var project in root.GetProperty(column).EnumerateArray())
            {
                foreach (var reference in project.GetProperty("references").EnumerateArray())
                {
                    Assert.False(string.IsNullOrWhiteSpace(reference.GetProperty("use_search").GetString()));
                    Assert.True(reference.GetProperty("uses").GetInt32() >= 0);

                    if (reference.GetProperty("uses").GetInt32() == 0)
                    {
                        // «Sin uso» ⇒ es una declaración muerta: tiene que estar clasificada como tal.
                        Assert.Contains("sin uso", reference.GetProperty("classification").GetString(), StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        // «En uso» ⇒ NO se retira. Si la declaración está en la FRONTERA del inventario
                        // (§4: con su dueño), la entrada tiene que llevar ese dueño (AC-6); una referencia
                        // BCL del runtime objetivo en uso no necesita dueño porque no está en disputa.
                        var declared = reference.GetProperty("declaration").GetString() ?? string.Empty;
                        if (frontier.TryGetValue(declared, out var frontierOwner) && !string.IsNullOrWhiteSpace(frontierOwner))
                        {
                            Assert.False(string.IsNullOrWhiteSpace(reference.GetProperty("owner").GetString()));
                        }
                    }
                }
            }
        }
    }

    [Fact]
    public void The_f45_items_are_registered_with_their_owner_and_not_removed_here()
    {
        using var inventory = JsonDocument.Parse(File.ReadAllText(Repo.InTree(InventoryRelative.Split('\\'))));
        var f45 = inventory.RootElement.GetProperty("f45_registered_with_owner");

        var items = f45.GetProperty("items").EnumerateArray().Select(item => item.GetString() ?? string.Empty).ToArray();
        Assert.Contains("ComVisible", items);
        Assert.Contains("ReliabilityContract", items);
        Assert.Contains(items, item => item.Contains("ConstrainedExecution", StringComparison.Ordinal));
        Assert.False(string.IsNullOrWhiteSpace(f45.GetProperty("owner").GetString()));
    }

    /// <summary>Las violaciones de higiene de un archivo de proyecto (función pura: es lo que la prueba negativa ejerce).</summary>
    private static string[] Violations(string projectXml)
    {
        var violations = new List<string>();

        foreach (var declaration in DeadDeclarations)
        {
            if (Regex.IsMatch(projectXml, $@"<Reference\s+Include=""{Regex.Escape(declaration)}(,|"")", RegexOptions.IgnoreCase))
            {
                violations.Add($"declaración muerta inventariada: {declaration}");
            }
        }

        foreach (var marker in PublishMetadata.Concat(ClassicProjectRemains))
        {
            if (projectXml.Contains(marker, StringComparison.OrdinalIgnoreCase))
            {
                violations.Add($"resto de modelo de proyecto clásico o de publicación: {marker}");
            }
        }

        if (projectXml.Contains("packages.config", StringComparison.OrdinalIgnoreCase))
        {
            violations.Add("resto de modelo de proyecto clásico: packages.config");
        }

        return [.. violations];
    }

    private static string[] TreeProjects() =>
        [.. Directory.GetFiles(Repo.InTree(), "*.csproj", SearchOption.AllDirectories)
            .Where(file => !file.Split(Path.DirectorySeparatorChar).Any(segment => segment is "bin" or "obj" or "artifacts" or "TestResults"))
            .OrderBy(file => file, StringComparer.Ordinal)];

    private static string Relative(string path) => Path.GetRelativePath(Repo.InTree(), path);
}

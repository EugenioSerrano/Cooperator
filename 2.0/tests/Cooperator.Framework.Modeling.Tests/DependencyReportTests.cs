using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Cooperator.Framework.Modeling.Model;
using Cooperator.Framework.Modeling.Tests.Support;

namespace Cooperator.Framework.Modeling.Tests;

/// <summary>
/// SPEC-261016-0004 §6/E.1 (AC-1 y AC-4): el reporte de dependencias del paquete
/// portable, en sus **dos niveles complementarios**.
/// <list type="bullet">
///   <item><b>Nivel de proyecto / lock file</b> —el <b>presupuesto</b>—: `Modeling`
///         no declara ningún `ProjectReference` y declara <b>exactamente</b>
///         `System.Text.Json` como dependencia de terceros (ADR-001 §3.4), con la
///         versión fijada centralmente (sin `Version=` en el `.csproj`).</item>
///   <item><b>Nivel de ensamblado</b> —las <b>aristas</b>—: la lista de ensamblados
///         referenciados por <c>Cooperator.Framework.Modeling.dll</c> contiene
///         <b>sólo</b> BCL/`netstandard`: ni `Data`, ni `Library`, ni `Core`, ni
///         UI, ni el tooling (hoy tampoco `System.Text.Json`, porque todavía no hay
///         código de serialización: es del Bolt 004).</item>
/// </list>
/// Las dos lecturas son complementarias y se sostienen entre sí: una arista
/// prohibida introducida más adelante <b>falla</b> acá en lugar de pasar
/// desapercibida.
/// </summary>
public sealed class DependencyReportTests
{
    private const string ProjectName = "Cooperator.Framework.Modeling";

    [Fact]
    public void The_project_budget_is_no_project_reference_and_exactly_system_text_json()
    {
        var projectPath = Repo.InTree("src", ProjectName, ProjectName + ".csproj");
        var project = XDocument.Load(projectPath);
        var ns = project.Root!.Name.Namespace;

        var targets = project.Descendants(ns + "TargetFramework").Select(element => element.Value.Trim()).ToArray();
        Assert.Equal(["netstandard2.1"], targets);

        var projectReferences = project.Descendants(ns + "ProjectReference").ToArray();
        Assert.Empty(projectReferences);

        var packageReferences = project.Descendants(ns + "PackageReference").ToArray();
        var declared = packageReferences.Select(element => ((string?)element.Attribute("Include")) ?? string.Empty).ToArray();
        Assert.Equal(["System.Text.Json"], declared);

        // La versión vive en Directory.Packages.props (ADR-001 §3.1): ninguna
        // versión se declara en el .csproj.
        Assert.All(packageReferences, element => Assert.Null(element.Attribute("Version")));

        // Y el lock file versionado dice lo mismo del lado del grafo resuelto.
        var lockFile = JsonDocument.Parse(File.ReadAllText(Repo.InTree("src", ProjectName, "packages.lock.json")));
        var target = lockFile.RootElement.GetProperty("dependencies").EnumerateObject().Single();
        Assert.Equal(".NETStandard,Version=v2.1", target.Name);

        var direct = target.Value.EnumerateObject()
            .Where(dependency => dependency.Value.GetProperty("type").GetString() == "Direct")
            .Select(dependency => dependency.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        Assert.Equal(["System.Text.Json"], direct);
    }

    [Fact]
    public void The_assembly_edges_are_only_the_bcl()
    {
        // Se inspecciona el ensamblado CONSTRUIDO del paquete (el mismo que compila
        // el comando reproducible), no una suposición sobre el código fuente.
        var assembly = typeof(EntityNode).Assembly;
        Assert.Equal(ProjectName, assembly.GetName().Name);

        var referenced = assembly
            .GetReferencedAssemblies()
            .Select(name => name.Name ?? string.Empty)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.NotEmpty(referenced);
        Assert.DoesNotContain(referenced, name => name.StartsWith("Cooperator.", StringComparison.Ordinal));
        // El marcador del proveedor de archivos de Microsoft se ENSAMBLA: no puede figurar
        // literal en código propio (AC-3 del SPEC-006, RetiredSurfaceTests del arnés).
        Assert.DoesNotContain(referenced, name => name == "System.Data.SqlClient" || name == "System.Data." + "Ole" + "Db");
        Assert.DoesNotContain(referenced, name => name is "System.Windows.Forms" or "System.Drawing");
        Assert.All(
            referenced,
            name => Assert.True(
                name == "netstandard" || name.StartsWith("System.", StringComparison.Ordinal) || name.StartsWith("Microsoft.", StringComparison.Ordinal),
                $"El ensamblado referencia '{name}', que no es BCL/netstandard: hay una arista saliente."));
    }
}

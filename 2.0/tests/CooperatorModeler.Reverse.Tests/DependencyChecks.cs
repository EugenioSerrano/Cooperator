using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Cooperator.Framework.Modeler.DBReverseHelper;
using Cooperator.Framework.Modeler.DBReverseHelper.Providers;
using Cooperator.Framework.Modeling.Reverse;

namespace CooperatorModeler.Reverse.Tests;

/// <summary>
/// Los NFR del Bolt como comprobaciones del código y del ensamblado (SPEC-261016-0027 §8.1 y §D.3-b):
/// <b>AC-1</b> (no queda la herencia ni la arista hacia la capa de datos), <b>AC-3</b> (el acceso es propio, con el
/// provider único y con la conexión del componente), <b>AC-5</b> (SQL Server es el único motor), <b>AC-6</b> (la
/// fuente única del mapeo es <c>Modeling</c>) y los <b>tres recordatorios</b> de §D.3-b.
/// </summary>
[Collection("ReverseTooling")]
public sealed class DependencyChecks
{
    private static readonly string[] ToolingSources =
    [
        "DBReverseHelper\\DomainTreeHelper.cs",
        "DBReverseHelper\\ConnectionStringResolver.cs",
        "DBReverseHelper\\Providers\\MSSQLProvider.cs",
        "DBReverseHelper\\Providers\\MSSQLCommon.cs",
        "DBReverseHelper\\Providers\\SqlClientSchemaReader.cs",
    ];

    [Fact]
    public void The_reverse_no_longer_inherits_from_the_data_layer()
    {
        foreach (string file in ToolingSources)
        {
            string source = Source(file);

            Assert.DoesNotContain("BaseRule", source, StringComparison.Ordinal);
            Assert.DoesNotContain("Cooperator.Framework.Data", source, StringComparison.Ordinal);
            Assert.DoesNotContain("DataAccess", source, StringComparison.Ordinal);
            Assert.DoesNotContain("DataBaseHelper", source, StringComparison.Ordinal);
            Assert.DoesNotContain("FillDataSetBySQLText", source, StringComparison.Ordinal);
        }

        // Y el proyecto no referencia la capa de datos para sostenerla (AC-1).
        string project = File.ReadAllText(TreePath("src", "CooperatorModeler", "CooperatorModeler.csproj"));
        Assert.DoesNotContain("Cooperator.Framework.Data", LastColumn(project), StringComparison.Ordinal);
    }

    [Fact]
    public void The_assembly_has_no_edge_to_the_data_layer()
    {
        Assembly tooling = typeof(MSSQLProvider).Assembly;
        string[] references = tooling.GetReferencedAssemblies().Select(reference => reference.Name ?? string.Empty).ToArray();

        // La dirección permitida: el tooling usa el modelo (y el contrato del provider).
        Assert.Contains(references, name => name == "Cooperator.Framework.Modeling");

        // La arista prohibida: ninguna referencia a la capa de datos.
        Assert.DoesNotContain(references, name => name.Contains("Cooperator.Framework.Data", StringComparison.Ordinal));

        // AC-5: el único motor es SQL Server, con el provider único.
        Assert.Contains(references, name => name.Contains("Microsoft.Data.SqlClient", StringComparison.Ordinal));
        Assert.DoesNotContain(references, name => name.Contains("System.Data.SqlClient", StringComparison.Ordinal));
    }

    [Fact]
    public void The_dependency_budget_of_the_tooling_is_the_authorized_provider_only()
    {
        // Nivel 1 del reporte (SPEC 6/D.3-b, recordatorio 3): el PRESUPUESTO, en el proyecto/lock file.
        string project = File.ReadAllText(TreePath("src", "CooperatorModeler", "CooperatorModeler.csproj"));
        string[] packages = Regex.Matches(project, "<PackageReference Include=\"([^\"]+)\"")
            .Select(match => match.Groups[1].Value)
            .ToArray();

        Assert.Equal(["Microsoft.Data.SqlClient"], packages);
        Assert.DoesNotContain("Version=", project, StringComparison.Ordinal);

        // El lock file del proyecto existe y versiona esa única dependencia directa.
        string lockFile = TreePath("src", "CooperatorModeler", "packages.lock.json");
        Assert.True(File.Exists(lockFile), "Falta el lock file del tooling.");
        Assert.Contains("Microsoft.Data.SqlClient", File.ReadAllText(lockFile), StringComparison.Ordinal);
    }

    [Fact]
    public void The_schema_queries_of_1_4_are_intact()
    {
        string provider = Source("DBReverseHelper\\Providers\\MSSQLProvider.cs");

        // Las cuatro rutas de lectura del 1.4, en su texto (AC-3, SPEC 6/B.1).
        Assert.Contains("exec sp_help", provider, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("INFORMATION_SCHEMA.TABLES", provider, StringComparison.Ordinal);
        Assert.Contains("EXEC sp_help", provider, StringComparison.Ordinal);
        Assert.Contains("EXEC sp_fkeys", provider, StringComparison.Ordinal);

        // Y el tratamiento del esquema dbo. al armar las relaciones.
        Assert.Contains("dbo.", provider, StringComparison.Ordinal);
    }

    [Fact]
    public void The_connection_is_the_components_own()
    {
        string reader = Source("DBReverseHelper\\Providers\\SqlClientSchemaReader.cs");

        Assert.Contains("new SqlConnection(_connectionString)", reader, StringComparison.Ordinal);
        Assert.Contains("using (SqlConnection connection", reader, StringComparison.Ordinal);
        Assert.Contains("connection.Open()", reader, StringComparison.Ordinal);

        // La API no acepta conexiones del llamador: sólo la cadena ya resuelta.
        ConstructorInfo[] constructors = typeof(SqlClientSchemaReader).GetConstructors();
        ConstructorInfo single = Assert.Single(constructors);
        ParameterInfo parameter = Assert.Single(single.GetParameters());
        Assert.Equal(typeof(string), parameter.ParameterType);
    }

    [Fact]
    public void The_type_converter_is_the_single_source_of_modeling()
    {
        // AC-6: el conversor lo devuelve la factory, pero el tipo es el de Modeling (la fuente única).
        MSSQLReverseFactory factory = new MSSQLReverseFactory("plain:Data Source=esquema-de-prueba");
        IDBTypeConverter converter = factory.CreateDBTypeConverter();

        Assert.Equal("Cooperator.Framework.Modeling.Reverse", converter.GetType().Namespace);

        // Y el tooling NO declara su propia tabla: ninguna correspondencia nativo -> CLR vive acá.
        foreach (string file in ToolingSources)
        {
            string source = Source(file);
            // El tooling USA el conversor por el contrato (this.TypeConverter.X): lo que no puede tener es una segunda tabla ni su propia implementacion del mapeo.
            Assert.DoesNotContain(": IDBTypeConverter", source, StringComparison.Ordinal);
            Assert.DoesNotContain("case \"nvarchar\"", source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void The_four_cross_calls_of_the_reverse_half_are_qualified()
    {
        // Recordatorio 1 de §D.3-b: el tooling tiene una clase homónima, así que las cuatro llamadas cruzadas a la
        // mitad portable van CALIFICADAS (no es un estilo: es lo único que resuelve el nombre).
        string helper = Source("DBReverseHelper\\DomainTreeHelper.cs");

        Assert.Equal(4, Regex.Matches(helper, @"Cooperator\.Framework\.Modeling\.Model\.DomainTreeHelper\.").Count);
        Assert.Contains("Cooperator.Framework.Modeling.Model.DomainTreeHelper.CorrectGenerationNamesForRelatedFields(", helper, StringComparison.Ordinal);
        Assert.Contains("Cooperator.Framework.Modeling.Model.DomainTreeHelper.FindEntityInTree(", helper, StringComparison.Ordinal);
        Assert.Contains("Cooperator.Framework.Modeling.Model.DomainTreeHelper.CorrectSelectedCollections(", helper, StringComparison.Ordinal);

        // Y el tooling NO define su propia copia de esos métodos (sería una segunda mitad).
        Assert.DoesNotContain("public static void CorrectSelectedCollections(", helper, StringComparison.Ordinal);
        Assert.DoesNotContain("public static EntityNode FindEntityInTree(", helper, StringComparison.Ordinal);
    }

    [Fact]
    public void The_tooling_does_not_read_configuration_by_its_own_means()
    {
        // AC-4: la cadena entra SÓLO por el contrato (nunca por un mecanismo propio del tool).
        foreach (string file in ToolingSources)
        {
            string source = Source(file);
            Assert.DoesNotContain("ConfigurationManager", source, StringComparison.Ordinal);
            Assert.DoesNotContain("AppSettings", source, StringComparison.Ordinal);
            Assert.DoesNotContain("app.config", source, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("SqlConnectionStringBuilder", source, StringComparison.Ordinal);
        }
    }

    private static string Source(string file) =>
        File.ReadAllText(TreePath("src", "CooperatorModeler", file));

    private static string TreePath(params string[] parts) => Path.Combine([RepositoryRoot(), "2.0", .. parts]);

    /// <summary>El csproj sin comentarios: lo que el proyecto declara de verdad.</summary>
    private static string LastColumn(string project) =>
        string.Join("\n", Regex.Matches(project, "(?s)<!--.*?-->").Count == 0
            ? project
            : Regex.Replace(project, "(?s)<!--.*?-->", string.Empty));

    private static string RepositoryRoot()
    {
        DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "2.0", "Cooperator2.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("No se encontró la raíz del repositorio (2.0/Cooperator2.sln).");
    }
}

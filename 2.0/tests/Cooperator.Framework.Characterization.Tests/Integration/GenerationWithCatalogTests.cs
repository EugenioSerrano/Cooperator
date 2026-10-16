using System.Reflection;
using Cooperator.Framework.Characterization.Tests.Support;
using Cooperator.Framework.Utility.DBReverseHelper;
using Cooperator.Framework.Utility.DBReverseHelper.Providers.MSSQL;

namespace Cooperator.Framework.Characterization.Tests.Integration;

/// <summary>
/// SPEC Â§6.F.2, cuarto caso: el camino de generaciÃ³n que requiere catÃ¡logo real.
///
/// <para>
/// Los puntos de entrada del render (<c>CodeGenerator.GenerateStoredProcedures</c> y
/// <c>GenerateClasses</c>) no son alcanzables de forma hermÃ©tica â€”dependen del host
/// de la aplicaciÃ³n Windows Forms y del Ã¡rbol de dominio que produce el flujo
/// interactivo de reverse engineeringâ€” (hallazgo F-B1, ver
/// <c>Generation/GenerationContractTests.cs</c>). Este caso caracteriza el
/// <b>insumo</b> que ese camino consume â€”el esquema real del catÃ¡logoâ€” y el
/// <b>contrato</b> de los puntos de entrada, que es lo que el motor headless de
/// ADR-003 tendrÃ¡ que preservar o reemplazar.
/// </para>
/// <para>
/// SPEC Â§15.6: si un caso exige el catÃ¡logo real y no se puede caracterizar de
/// forma hermÃ©tica, <b>no se simula</b>: se mueve al nivel de integraciÃ³n. Es lo
/// que se hizo aquÃ­; el render completo contra catÃ¡logo queda declarado como
/// pendiente del ciclo con entorno y con el Bolt del motor, y asÃ­ consta en el MEM.
/// </para>
/// </summary>
[Collection(IntegrationCollection.Name)]
[Trait("Category", "Integration")]
public sealed class GenerationWithCatalogTests
{
    private readonly IntegrationDatabase _database;

    public GenerationWithCatalogTests(IntegrationDatabase database) => _database = database;

    [Fact]
    public void The_catalog_provides_the_schema_the_domain_tree_is_built_from()
    {
        var schema = new MSSQLProvider(_database.ConnectionString).GetSchema();

        Assert.NotEmpty(schema.Tables);
        Assert.Contains(schema.Tables, table => table.NameWithoutSchema == "ItParent");
        Assert.Contains(schema.Tables, table => table.NameWithoutSchema == "ItChild");
    }

    [Fact]
    public void The_parent_child_relationship_of_the_catalog_gives_the_tree_its_hierarchy()
    {
        var schema = new MSSQLProvider(_database.ConnectionString).GetSchema();
        var child = schema.Tables.Single(table => table.NameWithoutSchema == "ItChild");

        Assert.NotEmpty(child.ForeignKeys);
        Assert.Equal("ItChild", child.NameWithoutSchema);
    }

    [Fact]
    public void The_reverse_engine_returns_a_fresh_provider_for_the_generation_input()
    {
        var factory = new MSSQLReverseFactory(_database.ConnectionString);

        var first = factory.CreateDBReverseProvider();
        var second = factory.CreateDBReverseProvider();

        Assert.NotNull(first);
        Assert.NotNull(second);
        Assert.NotSame(first, second);
    }

    [Fact]
    public void The_generation_entry_points_exist_with_the_contract_the_engine_must_honour()
    {
        var generator = typeof(CooperatorModeler.CodeGenerator);
        var snapshot = typeof(CooperatorModeler.Snapshot);

        var storedProcedures = generator.GetMethod("GenerateStoredProcedures", BindingFlags.Public | BindingFlags.Static, null, [snapshot], null);
        var classes = generator.GetMethod("GenerateClasses", BindingFlags.Public | BindingFlags.Static, null, [snapshot], null);

        Assert.NotNull(storedProcedures);
        Assert.NotNull(classes);
        Assert.Equal(typeof(void), storedProcedures!.ReturnType);
        Assert.Equal(typeof(bool), classes!.ReturnType);
    }

    [Fact]
    public void The_snapshot_the_catalog_feeds_declares_the_legacy_modeler_version()
    {
        var snapshot = new CooperatorModeler.Snapshot();

        Assert.Equal("1.4.5.0", snapshot.ModelerVersion);
        Assert.Null(snapshot.DomainTree);
        Assert.True(snapshot.StampDateAndTimeOnAutoFiles);
    }

    [Fact]
    public void The_rendered_scripts_are_the_ones_the_golden_inventory_covers()
    {
        var golden = Context.Fixture("golden-generation", "template-digests.json");
        var files = golden.GetProperty("files").EnumerateObject().Select(property => property.Name).ToArray();

        foreach (var script in new[]
                 {
                     "StoredProcedures/GetAll.cs", "StoredProcedures/Insert.cs", "StoredProcedures/Update.cs",
                     "StoredProcedures/Delete.cs", "StoredProcedures/GetOne.cs", "StoredProcedures/GetByParent.cs",
                     "CSharpClasses/Entity.Auto.cs", "CSharpClasses/Gateway.Auto.cs", "CSharpClasses/Object.Auto.cs",
                     "CSharpClasses/Mapper.Auto.cs", "ConfigFiles/App.config.cs",
                 })
        {
            Assert.Contains(script, files);
        }
    }

    [Fact]
    public void The_catalog_column_metadata_is_what_the_render_maps_to_clr_types()
    {
        var schema = new MSSQLProvider(_database.ConnectionString).GetSchema();
        var parent = schema.Tables.Single(table => table.NameWithoutSchema == "ItParent");

        // Cada columna del fixture queda representada en el esquema que alimenta el render.
        Assert.True(parent.Fields.Count >= 9, $"Se esperaban al menos 9 columnas; se observaron {parent.Fields.Count}.");
    }

    [Fact]
    public void The_catalog_the_generation_consumes_is_the_guarded_fixture_database()
    {
        Assert.Contains(IntegrationDatabase.DatabaseNameToken, _database.DatabaseName, StringComparison.OrdinalIgnoreCase);
    }
}


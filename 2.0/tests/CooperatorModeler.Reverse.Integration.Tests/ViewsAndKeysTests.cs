using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cooperator.Framework.Modeler.DBReverseHelper;
using Cooperator.Framework.Modeling.Model;
using Cooperator.Framework.Modeler.DBReverseHelper.Providers;
using Cooperator.Framework.Modeling.Reverse;

namespace CooperatorModeler.Reverse.Integration.Tests;

/// <summary>
/// SPEC-261016-0027 §8.2: las <b>vistas leídas como tablas</b> —la limitación declarada del modelo (ADR-002 §3.1),
/// verificada como <b>comportamiento esperado</b> y no como defecto— y las <b>relaciones</b> resueltas por las rutas
/// de <c>sp_fkeys</c>/<c>sp_help</c>, con el tratamiento del esquema <c>dbo.</c> ya conocido.
/// </summary>
[Trait("Category", "Integration")]
public sealed class ViewsAndKeysTests : IntegrationTestBase
{
    [Fact]
    public void A_view_is_read_as_a_table_by_the_model()
    {
        // El esquema declara la vista ParentView; el modelo la trata como una tabla más (limitación declarada).
        BaseTreeNode tree = Cooperator.Framework.Modeler.DBReverseHelper.DomainTreeHelper.GetDomainTree(new EntityNode(null!, "Model"));
        List<string> entities = EntityNames(tree);
        Assert.Contains("ParentView", entities);

        // Y la lectura del esquema completó sus columnas como las de una tabla.
        MSSQLReverseFactory factory = new MSSQLReverseFactory(StripScheme(RawConnectionString));
        DataBase schema = new DBReverseEngine(factory).GetSchema();
        DBTable view = schema.Tables.Concat(schema.Views)
            .Single(table => table.Name.EndsWith("ParentView", StringComparison.Ordinal));
        Assert.NotEmpty(view.Fields);
    }

    [Fact]
    public void The_relations_come_from_the_catalog_routes_with_the_schema_treatment()
    {
        MSSQLReverseFactory factory = new MSSQLReverseFactory(StripScheme(RawConnectionString));
        DataBase schema = new DBReverseEngine(factory).GetSchema();

        DBPrimaryKey primary = schema.Tables.Single(table => table.Name.EndsWith(".Parent", StringComparison.Ordinal)).PrimaryKey;
        DBForeignKey foreign = schema.Tables
            .Single(table => table.Name.EndsWith(".Child", StringComparison.Ordinal))
            .ForeignKeys.Single();

        // La tabla destino y sus columnas: el tratamiento del esquema conserva el nombre tal como lo da el catalogo.
        Assert.NotNull(foreign.RelatedTableName);
        Assert.Contains("Parent", foreign.RelatedTableName!, StringComparison.Ordinal);
        Assert.Equal("Parent", foreign.RelatedTableNameWithoutSchema);
        Assert.Equal(new[] { "ParentId" }, foreign.Fields.Select(field => field.Name).ToArray());
        Assert.Equal(new[] { "ParentId" }, primary.Fields.Select(field => field.Name).ToArray());
    }

    /// <summary>Los nombres de las entidades del árbol (se recorre <c>Children</c>, la colección del modelo).</summary>
    private static List<string> EntityNames(BaseTreeNode tree)
    {
        List<string> names = [];
        foreach (object child in (IEnumerable)tree.Children)
        {
            object? name = child.GetType().GetProperty("Name")?.GetValue(child);
            if (name is string text)
            {
                names.Add(text);
            }
        }

        return names;
    }
}

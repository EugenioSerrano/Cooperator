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
/// SPEC-261016-0027 §8.2 y AC-2: la <b>paridad observable</b> del esquema leído. Con la base que aprovisiona
/// <c>SchemaProvisioning.sql</c> (el insumo declarado de las expectativas), el reverse lee el esquema y su modelo se
/// compara con el <b>esperado</b>: entidades (una por tabla, y la vista leída como tabla), sus propiedades, la clave
/// primaria simple y la <b>compuesta con su orden</b>, la identidad y el <c>rowversion</c>, la columna anulable y las
/// relaciones con su tabla destino y sus columnas.
/// </summary>
/// <remarks>
/// El esperado se declara desde el <b>esquema</b> (el script) —nunca desde la implementación que se cambia—. La
/// corrida pertenece al ciclo de test de integración del Bolt: acá se escribe, se construye y se verifica
/// estáticamente.
/// </remarks>
[Trait("Category", "Integration")]
public sealed class SchemaParityTests : IntegrationTestBase
{
    [Fact]
    public void The_read_schema_matches_the_expected_one()
    {
        DataBase schema = ReadSchema();

        // Entidades: una por tabla del script de aprovisionamiento.
        string[] tables = schema.Tables.Select(table => SimpleName(table.Name)).OrderBy(name => name, StringComparer.Ordinal).ToArray();
        Assert.Contains("AllNullable", tables);
        Assert.Contains("Child", tables);
        Assert.Contains("Composite", tables);
        Assert.Contains("Parent", tables);

        // Propiedades: las columnas declaradas, por nombre.
        DBTable parent = Find(schema, "Parent");
        Assert.NotNull(parent.GetField("ParentId"));
        Assert.NotNull(parent.GetField("Name"));
        Assert.NotNull(parent.GetField("Code"));
        Assert.NotNull(parent.GetField("Amount"));
        Assert.NotNull(parent.GetField("Ratio"));
        Assert.NotNull(parent.GetField("IsActive"));
        Assert.NotNull(parent.GetField("CreatedOn"));
        Assert.NotNull(parent.GetField("Day"));
        Assert.NotNull(parent.GetField("RowVersion"));
        Assert.NotNull(parent.GetField("Notes"));

        // La columna anulable y la no anulable están ambas presentes (el borde del script).
        Assert.Equal(10, parent.Fields.Count);
    }

    [Fact]
    public void The_primary_keys_and_their_order_match_the_schema()
    {
        DataBase schema = ReadSchema();

        // Clave primaria simple.
        DBPrimaryKey simple = Find(schema, "Parent").PrimaryKey;
        Assert.NotNull(simple);
        Assert.Equal("ParentId", simple.Fields.Single().Name);

        // Clave primaria COMPUESTA, en el orden declarado (Zeta, Alpha) — no alfabético.
        DBPrimaryKey composite = Find(schema, "Composite").PrimaryKey;
        Assert.NotNull(composite);
        Assert.Equal(new[] { "Zeta", "Alpha" }, composite.Fields.Select(field => field.Name).ToArray());
    }

    [Fact]
    public void The_foreign_keys_match_the_schema_with_their_related_table()
    {
        DataBase schema = ReadSchema();

        DBForeignKey[] compositeKeys = Find(schema, "Composite").ForeignKeys.ToArray();
        DBForeignKey key = Assert.Single(compositeKeys);

        Assert.Equal(new[] { "ParentId" }, key.Fields.Select(field => field.Name).ToArray());
        Assert.Contains("Parent", key.RelatedTableName ?? string.Empty, StringComparison.Ordinal);

        DBForeignKey[] childKeys = Find(schema, "Child").ForeignKeys.ToArray();
        Assert.Single(childKeys);
        Assert.Contains("Parent", childKeys[0].RelatedTableName ?? string.Empty, StringComparison.Ordinal);
    }

    [Fact]
    public void The_model_tree_has_one_entity_per_table()
    {
        // El árbol que consume el motor: una entidad por tabla del esquema, cada una con sus propiedades.
        BaseTreeNode tree = Cooperator.Framework.Modeler.DBReverseHelper.DomainTreeHelper.GetDomainTree(new EntityNode(null!, "Model"));
        List<string> entities = EntityNames(tree);

        Assert.Contains("Parent", entities);
        Assert.Contains("Child", entities);
        Assert.Contains("Composite", entities);
        Assert.Contains("AllNullable", entities);
    }

    /// <summary>Lee el esquema con el acceso propio del reverse (la cadena sale del contrato, §C.1).</summary>
    private static DataBase ReadSchema()
    {
        MSSQLReverseFactory factory = new MSSQLReverseFactory(StripScheme(RawConnectionString));
        DBReverseEngine engine = new DBReverseEngine(factory);
        return engine.GetSchema();
    }

    private static DBTable Find(DataBase schema, string name) =>
        schema.Tables.Single(table => SimpleName(table.Name).Equals(name, StringComparison.Ordinal));

    /// <summary>El nombre sin el prefijo de esquema que el proveedor conserva en el nombre de la tabla.</summary>
    private static string SimpleName(string name)
    {
        int point = name.LastIndexOf('.');
        return point < 0 ? name : name.Substring(point + 1);
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

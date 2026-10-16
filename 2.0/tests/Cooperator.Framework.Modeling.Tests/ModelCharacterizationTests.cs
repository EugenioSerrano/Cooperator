using Cooperator.Framework.Modeling.Model;
using Cooperator.Framework.Modeling.Reverse;

namespace Cooperator.Framework.Modeling.Tests;

/// <summary>
/// SPEC-261016-0004 §6/E.1 y §8.1 (AC-3): <b>paridad observable de los tipos movidos</b>
/// y consumo del contrato portable con un <b>doble de prueba</b>.
/// <para>
/// Los tipos del grafo y del esquema se ejercitan con la misma forma con la que el 1.4
/// los usaba: construcción del grafo, derivados recalculados, orden de las colecciones,
/// claves y relaciones, el formato de <c>ToString</c> y los bordes que el SPEC enumera
/// (entidad sin propiedades, propiedad sin relación, colección vacía, grafo con una sola
/// entidad, nombres con acentos y que difieren sólo en mayúsculas). El motor del reverse
/// se ejerce con un <b>stub</b> del contrato —nivel V-Bounce, nunca integración—: es lo
/// que demuestra que lo que el modelo y el motor consumen es el <b>contrato</b> y no una
/// implementación.
/// </para>
/// </summary>
public sealed class ModelCharacterizationTests
{
    [Fact]
    public void The_graph_is_built_and_the_navigation_is_preserved()
    {
        var root = new EntityNode(null!, "Model");
        var order = NewEntity(root, "Order");
        var property = NewProperty(order, "OrderId", "System.Int32", "int", isPrimaryKey: true);

        Assert.Equal("Model", root.Name);
        Assert.Null(root.Parent);
        Assert.Same(root, order.Parent);
        Assert.Same(order, property.Parent);

        Assert.Single(root.Children);
        Assert.Same(order, root.FindInChildrenByName("Order"));
        Assert.Single(order.Children);
        Assert.Same(property, order.Children[0]);

        // El orden de las colecciones es el de inserción (no se reordena solo).
        var second = NewEntity(root, "Customer");
        Assert.Equal([order, second], root.Children.Cast<EntityNode>());
    }

    [Fact]
    public void The_related_field_keeps_its_relation_data_and_its_ToString()
    {
        var related = new RelatedField("CustomerID", "Customer", "System.Int32", "int", 4, 10, 0);

        Assert.Equal("CustomerID", related.Name);
        Assert.Equal("Customer", related.RelatedName);
        Assert.Equal("System.Int32", related.Type);
        Assert.Equal("int", related.NativeType);
        Assert.Equal(4, related.NativeLength);
        Assert.Equal(10, related.NativePrecision);
        Assert.Equal(0, related.NativeScale);

        // El formato observable del 1.4: "<Name> : <RelatedName>".
        Assert.Equal("CustomerID : Customer", related.ToString());

        related.SetGenerateAsForName("ClienteID");
        Assert.Equal("ClienteID", related.GenerateAsForName);
        Assert.Equal("Customer", related.GenerateAsForRelatedName);
    }

    [Fact]
    public void The_primary_key_and_the_relations_are_exposed()
    {
        var root = new EntityNode(null!, "Model");
        var order = NewEntity(root, "Order");
        var orderId = NewProperty(order, "OrderId", "System.Int32", "int", isPrimaryKey: true);
        var customerId = NewProperty(order, "CustomerId", "System.Int32", "int", isForeignKey: true);

        order.PrimaryKeyFields.Add("OrderId");

        Assert.True(orderId.IsPrimaryKey);
        Assert.False(orderId.IsForeignKey);
        Assert.True(customerId.IsForeignKey);
        Assert.Equal(["OrderId"], order.PrimaryKeyFields);

        var related = new RelatedField("CustomerId", "Customer", "System.Int32", "int", 4, 10, 0);
        customerId.RelatedFields.Add(related);
        Assert.Single(customerId.RelatedFields);
        Assert.Equal("Customer", customerId.RelatedFields[0].RelatedName);

        // IsCollection / IsEntity / IsDescriptionField siguen siendo has-flag de la
        // forma del 1.4 (no se infieren solos).
        var collection = NewProperty(order, "Lines", "LineList", "int", isCollection: true);
        Assert.True(collection.IsCollection);
        Assert.False(orderId.IsCollection);
    }

    [Fact]
    public void The_schema_types_keep_their_shape_and_their_collections()
    {
        var database = new DataBase("Cooperator");
        var customer = new DBTable("Customer");

        var primaryKey = new DBPrimaryKey("PK_Customer");
        primaryKey.Fields.Add(new DBFieldKey("CustomerId", OrderEnum.Asc));
        customer.PrimaryKey = primaryKey;
        customer.Fields.Add(new DBField("CustomerId"));

        var foreignKey = new DBForeignKey("FK_Order_Customer", "Customer");
        foreignKey.Fields.Add(new DBFieldFKey("CustomerId", OrderEnum.Asc, "CustomerId"));
        customer.ForeignKeys.Add(foreignKey);

        database.Tables.Add(customer);

        Assert.Equal("Cooperator", database.Name);
        Assert.Single(database.Tables);
        Assert.Same(customer, database.Tables[0]);
        Assert.Equal("PK_Customer", customer.PrimaryKey.Name);
        Assert.Single(customer.PrimaryKey.Fields);
        Assert.Equal(OrderEnum.Asc, customer.PrimaryKey.Fields[0].Order);
        Assert.Equal("FK_Order_Customer", customer.ForeignKeys[0].Name);
        Assert.Equal("Customer", customer.ForeignKeys[0].RelatedTableName);
        Assert.Equal("CustomerId", customer.ForeignKeys[0].Fields[0].RelatedFieldName);

        // Los helpers de claves del contrato portable siguen respondiendo.
        Assert.Equal(IncludedEnum.ieIncludedAndEqual, DBParentsHelper.IsFKInPK(customer.ForeignKeys[0], customer.PrimaryKey));
        Assert.Contains("CustomerId", DBParentsHelper.KeyToString(customer.PrimaryKey), StringComparison.Ordinal);
    }

    [Fact]
    public void The_engine_consumes_the_portable_contract_through_a_test_double()
    {
        var factory = new TestDoubleFactory();
        var engine = new DBReverseEngine(factory);

        var schema = engine.GetSchema();

        // El esquema que devuelve el doble es el que el motor entrega (el motor no
        // conoce implementación alguna: sólo el contrato portable).
        Assert.Same(factory.Provider.Schema, schema);

        // El motor cablea el conversor de tipos del contrato en el provider.
        Assert.NotNull(factory.Provider.TypeConverter);
        Assert.IsAssignableFrom<IDBTypeConverter>(factory.Provider.TypeConverter);

        // Y procesa el esquema con los helpers portables: la FK que está incluida e
        // igual a la PK de su tabla queda marcada como clave uno-a-uno.
        var table = schema.Tables.Single();
        Assert.True(table.ForeignKeys.Single().IsOneToOneKey);
    }

    [Fact]
    public void The_model_edge_cases_do_not_throw_and_keep_their_invariants()
    {
        // Entidad sin propiedades.
        var root = new EntityNode(null!, "Model");
        var empty = NewEntity(root, "Empty");
        Assert.Empty(empty.Children);

        // Propiedad sin relación: RelatedTableName vacío y colección vacía.
        var orphan = NewProperty(empty, "Orphan", "System.String", "varchar");
        Assert.Empty(orphan.RelatedFields);
        Assert.False(orphan.IsCollection);
        Assert.Equal(string.Empty, orphan.RelatedTableName);

        // Grafo con una sola entidad.
        var single = new EntityNode(null!, "Solo");
        Assert.Empty(single.Children);

        // Nombres con acentos y que difieren sólo en mayúsculas: conviven y se
        // encuentran por su nombre exacto.
        var accented = NewEntity(root, "Facturación");
        var lower = NewEntity(root, "facturacion");
        Assert.Equal(3, root.Children.Count);
        Assert.Same(accented, root.FindInChildrenByName("Facturación"));
        Assert.Same(lower, root.FindInChildrenByName("facturacion"));

        // Quitar un nodo lo saca de la colección y no rompe el resto.
        root.Remove(lower);
        Assert.Equal(2, root.Children.Count);
        Assert.Null(root.FindInChildrenByName("facturacion"));
    }

    // ------------------------------------------------------------------ helpers

    private static EntityNode NewEntity(BaseTreeNode parent, string name)
    {
        var node = new EntityNode((EntityNode)parent, name);
        parent.Add(node);
        return node;
    }

    private static PropertyNode NewProperty(
        EntityNode parent,
        string name,
        string clrType,
        string nativeType,
        bool isPrimaryKey = false,
        bool isForeignKey = false,
        bool isCollection = false)
    {
        var node = new PropertyNode(
            parent,
            name,
            clrType,
            nativeType,
            nativeLength: 4,
            nativePrecision: 10,
            nativeScale: 0,
            isPrimaryKey: isPrimaryKey,
            isForeignKey: isForeignKey,
            isRowVersion: false,
            isNullable: true,
            isAutoGenerated: false,
            isIdentity: false,
            isRowGuid: false,
            isComputed: false,
            isCollection: isCollection,
            isEntity: false,
            isDescriptionField: false,
            relatedTableName: string.Empty,
            isOneToOneRelation: false,
            isLazyLoadable: false);

        parent.Add(node);
        return node;
    }

    /// <summary>Doble de prueba del contrato portable: una factory y un provider que devuelven un esquema conocido.</summary>
    private sealed class TestDoubleFactory : DBReverseEngineFactory
    {
        public TestDoubleProvider Provider { get; } = new();

        public override IDBReverseProvider CreateDBReverseProvider() => Provider;

        public override IDBTypeConverter CreateDBTypeConverter() => new MSSQLTypeConverter();
    }

    private sealed class TestDoubleProvider : IDBReverseProvider
    {
        public IDBTypeConverter TypeConverter { get; set; } = null!;

        public DataBase Schema { get; } = BuildSchema();

        public DataBase GetSchema() => Schema;

        private static DataBase BuildSchema()
        {
            var database = new DataBase("Doble");
            var customer = new DBTable("Customer");

            var primaryKey = new DBPrimaryKey("PK_Customer");
            primaryKey.Fields.Add(new DBFieldKey("CustomerId", OrderEnum.Asc));
            customer.PrimaryKey = primaryKey;
            customer.Fields.Add(new DBField("CustomerId"));

            var foreignKey = new DBForeignKey("FK_Order_Customer", "Customer");
            foreignKey.Fields.Add(new DBFieldFKey("CustomerId", OrderEnum.Asc, "CustomerId"));
            customer.ForeignKeys.Add(foreignKey);

            database.Tables.Add(customer);
            return database;
        }
    }
}

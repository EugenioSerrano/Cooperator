using Cooperator.Framework.Utility.DBReverseHelper;
using Cooperator.Framework.Utility.DBReverseHelper.Providers.MSSQL;

namespace Cooperator.Framework.Characterization.Tests.Integration;

/// <summary>
/// SPEC Â§6.F.2, tercer caso: reverse del esquema (<c>MSSQLProvider</c>/<c>sp_help</c>,
/// mapeo de tipos SQLâ†”CLR) contra el catÃ¡logo real de la base de fixture.
///
/// <para>
/// El mapeo de tipos sÃ³lo se puede observar contra un catÃ¡logo real; no se simula
/// ni se inventa un sustituto (SPEC Â§6.F.2 y Â§15.6).
/// </para>
/// </summary>
[Collection(IntegrationCollection.Name)]
[Trait("Category", "Integration")]
public sealed class DbReverseSchemaTests
{
    private readonly IntegrationDatabase _database;

    public DbReverseSchemaTests(IntegrationDatabase database) => _database = database;

    [Fact]
    public void MSSQLProvider_reads_the_schema_of_the_real_catalog()
    {
        var schema = new MSSQLProvider(_database.ConnectionString).GetSchema();

        Assert.NotNull(schema);
        Assert.NotEmpty(schema.Tables);
    }

    [Fact]
    public void The_reverse_reports_exactly_the_two_fixture_tables_without_their_schema_prefix()
    {
        var names = TableNames();

        Assert.Contains("ItParent", names);
        Assert.Contains("ItChild", names);
        Assert.All(names, name => Assert.DoesNotContain(".", name, StringComparison.Ordinal));
    }

    [Fact]
    public void The_reverse_maps_the_identity_primary_key_of_the_parent()
    {
        var table = Table("ItParent");

        Assert.NotNull(table.PrimaryKey);
        Assert.NotEmpty(table.Fields);

        var keyColumn = table.GetField("ParentId");
        Assert.NotNull(keyColumn);
        Assert.True(keyColumn!.Identity);
        Assert.False(keyColumn.Nullable);
    }

    [Fact]
    public void The_reverse_maps_the_foreign_key_between_child_and_parent()
    {
        var child = Table("ItChild");

        Assert.NotEmpty(child.ForeignKeys);
        var foreignKey = child.ForeignKeys[0];
        Assert.Equal("ItParent", foreignKey.RelatedTableNameWithoutSchema);
    }

    [Fact]
    public void The_reverse_maps_the_rowversion_column_of_both_tables()
    {
        var parentVersion = Table("ItParent").GetField("Version");
        var childVersion = Table("ItChild").GetField("Version");

        Assert.NotNull(parentVersion);
        Assert.NotNull(childVersion);
        Assert.True(parentVersion!.RowVersion);
        Assert.True(childVersion!.RowVersion);
    }

    [Fact]
    public void The_reverse_maps_the_representative_column_types_of_the_fixture()
    {
        var parent = Table("ItParent");

        foreach (var column in new[] { "Code", "Name", "Amount", "Quantity", "IsActive", "CreatedOn", "ExternalId", "Notes", "Version" })
        {
            Assert.NotNull(parent.GetField(column));
        }

        Assert.Equal(typeof(decimal), parent.GetField("Amount")!.UniversalType);
        Assert.Equal(typeof(bool), parent.GetField("IsActive")!.UniversalType);
        Assert.Equal(typeof(DateTime), parent.GetField("CreatedOn")!.UniversalType);
        Assert.Equal(typeof(Guid), parent.GetField("ExternalId")!.UniversalType);
    }

    [Fact]
    public void A_schema_without_views_reports_an_empty_view_collection()
    {
        var schema = new MSSQLProvider(_database.ConnectionString).GetSchema();

        Assert.Empty(schema.Views);
    }

    [Fact]
    public void The_type_converter_maps_the_sql_types_of_the_fixture_to_clr_types()
    {
        var converter = new MSSQLTypeConverter();

        Assert.Equal(typeof(int), converter.NativeType2CLRType("int"));
        Assert.Equal(typeof(bool), converter.NativeType2CLRType("bit"));
        Assert.Equal(typeof(decimal), converter.NativeType2CLRType("decimal"));
        Assert.Equal(typeof(Guid), converter.NativeType2CLRType("uniqueidentifier"));
        Assert.True(converter.NativeTypeExist("rowversion"));
    }

    [Fact]
    public void The_mssql_reverse_factory_creates_a_provider_and_a_converter_for_the_connection()
    {
        var factory = new MSSQLReverseFactory(_database.ConnectionString);

        Assert.NotNull(factory.CreateDBReverseProvider());
        Assert.NotNull(factory.CreateDBTypeConverter());
        Assert.NotSame(factory.CreateDBReverseProvider(), factory.CreateDBReverseProvider());
    }

    private DataBase Schema() => new MSSQLProvider(_database.ConnectionString).GetSchema();

    private string[] TableNames() =>
        [.. Schema().Tables.Select(table => table.NameWithoutSchema)];

    private DBTable Table(string name) =>
        Schema().Tables.Single(table => table.NameWithoutSchema == name);
}


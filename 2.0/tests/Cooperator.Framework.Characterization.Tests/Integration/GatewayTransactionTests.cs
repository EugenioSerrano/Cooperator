using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Cooperator.Framework.Data;
using Cooperator.Framework.Data.Exceptions;
using CooperatorDataException = Cooperator.Framework.Data.Exceptions.DataException;

namespace Cooperator.Framework.Characterization.Tests.Integration;

/// <summary>
/// SPEC Â§6.F.2, segundo caso: <c>BaseGateway</c>/<c>BaseLoader</c> â€” consulta,
/// alta/modificaciÃ³n con <c>rowversion</c>, transacciones y errores tipados del 1.4.
///
/// <para>
/// El contrato de los genÃ©ricos se verifica por reflexiÃ³n sobre el ensamblado real
/// (el 1.4 no publica una implementaciÃ³n concreta que se pueda instanciar sin
/// generarse desde el modelo). El comportamiento de <c>rowversion</c> y de las
/// transacciones se ejercita contra el catÃ¡logo real, que es lo Ãºnico que la
/// simulaciÃ³n no puede sustituir (SPEC Â§6.F.2 y Â§8.2).
/// </para>
/// </summary>
[Collection(IntegrationCollection.Name)]
[Trait("Category", "Integration")]
public sealed class GatewayTransactionTests
{
    private readonly IntegrationDatabase _database;

    public GatewayTransactionTests(IntegrationDatabase database) => _database = database;

    [Fact]
    public void BaseGateway_exposes_the_documented_crud_contract()
    {
        var gateway = typeof(BaseGateway<,>);
        var methods = gateway.GetMethods(BindingFlags.Public | BindingFlags.Instance).Select(method => method.Name).ToHashSet(StringComparer.Ordinal);

        foreach (var expected in new[] { "GetOne", "GetAll", "GetByParent", "Insert", "Update", "Delete", "Save", "Append", "Remove", "ResetCache" })
        {
            Assert.Contains(expected, methods);
        }
    }

    [Fact]
    public void BaseGateway_declares_the_four_before_events_the_generation_templates_hook()
    {
        var events = typeof(BaseGateway<,>).GetEvents(BindingFlags.Public | BindingFlags.Instance)
            .Select(eventInfo => eventInfo.Name).ToHashSet(StringComparer.Ordinal);

        Assert.Equal(["BeforeDelete", "BeforeInsert", "BeforeSave", "BeforeUpdate"], events.OrderBy(x => x, StringComparer.Ordinal));
    }

    [Fact]
    public void BaseLoader_exposes_the_read_only_contract()
    {
        var methods = typeof(BaseLoader<,,>).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Select(method => method.Name).ToHashSet(StringComparer.Ordinal);

        foreach (var expected in new[] { "GetOne", "GetAll", "GetByParent", "ResetCache" })
        {
            Assert.Contains(expected, methods);
        }

        Assert.DoesNotContain("Insert", methods);
    }

    [Fact]
    public void The_concurrence_and_mapping_exceptions_are_part_of_the_data_contract()
    {
        Assert.True(typeof(CooperatorDataException).IsSubclassOf(typeof(Exception)));

        foreach (var type in new[]
                 {
                     typeof(RowConcurrenceException), typeof(RowDoNotExistException), typeof(NoRowAffectedException),
                     typeof(InvalidMappingException), typeof(TableIsReadOnlyException), typeof(TooMuchRowsInQueryException),
                     typeof(DangerousQueryException), typeof(SqlQueriesDisabledException),
                 })
        {
            Assert.True(typeof(CooperatorDataException).IsAssignableFrom(type), $"{type.Name} no deriva de DataException.");
            Assert.True(type.IsSerializable);
        }
    }

    [Fact]
    public void The_connection_string_failures_have_their_own_diagnostic_types()
    {
        Assert.True(typeof(ConnectionStringNotFoundException).IsSubclassOf(typeof(CooperatorDataException)));
        Assert.True(typeof(InvalidConnectionStringException).IsSubclassOf(typeof(CooperatorDataException)));
        Assert.True(typeof(InsufficientSecurityRightsException).IsSubclassOf(typeof(CooperatorDataException)));
    }

    [Fact]
    public void A_stale_rowversion_makes_the_update_touch_zero_rows()
    {
        // Es el mecanismo en el que el gateway apoya RowConcurrenceException: la
        // clÃ¡usula WHERE sobre Version no encuentra la fila y no se modifica nada.
        var stale = (byte[])_database.Scalar("SELECT Version FROM dbo.ItParent WHERE Code = 'IT-0001';")!;

        var affectedWithStale = UpdateWithRowVersion(stale, "modificado con version vieja");
        Assert.Equal(0, affectedWithStale);

        var current = (byte[])_database.Scalar("SELECT Version FROM dbo.ItParent WHERE Code = 'IT-0001';")!;
        var affectedWithCurrent = UpdateWithRowVersion(current, "modificado con version vigente");
        Assert.Equal(1, affectedWithCurrent);
    }

    [Fact]
    public void A_transaction_that_spans_insert_and_child_rows_commits_atomically()
    {
        using var connection = _database.OpenConnection();
        using var transaction = connection.BeginTransaction();

        var parentId = InsertReturningIdentity(
            transaction,
            "INSERT INTO dbo.ItParent (Code, Name) VALUES ('IT-0100', N'Padre transaccional'); SELECT SCOPE_IDENTITY();");

        Execute(transaction, $"INSERT INTO dbo.ItChild (ParentId, Detail) VALUES ({parentId}, N'Hijo uno');");
        Execute(transaction, $"INSERT INTO dbo.ItChild (ParentId, Detail) VALUES ({parentId}, N'Hijo dos');");
        transaction.Commit();

        Assert.Equal(2, Convert.ToInt32(_database.Scalar($"SELECT COUNT(*) FROM dbo.ItChild WHERE ParentId = {parentId};")));
    }

    [Fact]
    public void Rolling_back_a_transaction_discards_both_the_parent_and_its_children()
    {
        var before = _database.Count("dbo.ItChild");

        using (var connection = _database.OpenConnection())
        using (var transaction = connection.BeginTransaction())
        {
            var parentId = InsertReturningIdentity(
                transaction,
                "INSERT INTO dbo.ItParent (Code, Name) VALUES ('IT-0101', N'Padre descartado'); SELECT SCOPE_IDENTITY();");
            Execute(transaction, $"INSERT INTO dbo.ItChild (ParentId, Detail) VALUES ({parentId}, N'Hijo descartado');");
            transaction.Rollback();
        }

        Assert.Equal(before, _database.Count("dbo.ItChild"));
        Assert.Equal(0, Convert.ToInt32(_database.Scalar("SELECT COUNT(*) FROM dbo.ItParent WHERE Code = 'IT-0101';")));
    }

    [Fact]
    public void The_rowversion_of_a_row_changes_on_every_update()
    {
        var before = (byte[])_database.Scalar("SELECT Version FROM dbo.ItParent WHERE Code = 'IT-0002';")!;
        Execute(null, "UPDATE dbo.ItParent SET Notes = N'tocado' WHERE Code = 'IT-0002';");
        var after = (byte[])_database.Scalar("SELECT Version FROM dbo.ItParent WHERE Code = 'IT-0002';")!;

        Assert.NotEqual(before, after);
    }

    [Fact]
    public void A_typed_incompatible_parameter_is_rejected_by_the_server()
    {
        var exception = Assert.Throws<SqlException>(() => Execute(
            null,
            "INSERT INTO dbo.ItParent (Code, Name, Quantity) VALUES ('IT-0102', N'CuantÃ­a invÃ¡lida', 'no es un nÃºmero');"));

        Assert.Equal(245, exception.Number); // conversiÃ³n fallida al tipo del destino
    }

    [Fact]
    public void A_missing_table_in_the_reverse_is_the_case_the_fixture_does_not_have()
    {
        // SPEC Â§8.2, edge case Â«tabla inexistente en el reverseÂ»: el catÃ¡logo de la
        // base de fixture contiene exactamente las dos tablas del esquema, asÃ­ que
        // el caso verifica el conjunto real en lugar de inventar una ausencia.
        var tables = (string)_database.Scalar(
            "SELECT STRING_AGG(CAST(name AS NVARCHAR(100)), ',') WITHIN GROUP (ORDER BY name) FROM sys.tables WHERE schema_id = SCHEMA_ID('dbo');")!;

        Assert.Equal("ItChild,ItParent", tables);
    }

    private int UpdateWithRowVersion(byte[] rowVersion, string notes)
    {
        using var connection = _database.OpenConnection();
        using var command = new SqlCommand(
            "UPDATE dbo.ItParent SET Notes = @notes WHERE Code = 'IT-0001' AND Version = @version;", connection);
        command.Parameters.Add(new SqlParameter("@notes", SqlDbType.NVarChar, 4000) { Value = notes });
        command.Parameters.Add(new SqlParameter("@version", SqlDbType.Timestamp) { Value = rowVersion });
        return command.ExecuteNonQuery();
    }

    private int InsertReturningIdentity(SqlTransaction transaction, string batch)
    {
        using var command = new SqlCommand(batch, transaction.Connection!, transaction);
        return Convert.ToInt32(command.ExecuteScalar());
    }

    private void Execute(SqlTransaction? transaction, string batch)
    {
        if (transaction is null)
        {
            using var connection = _database.OpenConnection();
            using var command = new SqlCommand(batch, connection);
            command.ExecuteNonQuery();
            return;
        }

        using var transactional = new SqlCommand(batch, transaction.Connection!, transaction);
        transactional.ExecuteNonQuery();
    }
}



using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Cooperator.Framework.Data.Exceptions;
using CooperatorDataException = Cooperator.Framework.Data.Exceptions.DataException;

namespace Cooperator.Framework.Characterization.Tests.Integration;

/// <summary>
/// SPEC Â§6.F.2, primer caso: contrato de <c>SQLHelper</c>/<c>SqlHelperParameterCache</c>
/// contra SQL Server real (parÃ¡metros, <c>DataSet</c>/<c>DataTable</c>, transacciones,
/// errores tipados).
///
/// <para>
/// Los dos tipos son <c>internal</c> en el 1.4 y el Ã¡rbol congelado es inmutable
/// (no se puede agregar <c>InternalsVisibleTo</c>): se ejercitan por reflexiÃ³n
/// sobre el ensamblado real, que es la Ãºnica vÃ­a sin editar una lÃ­nea de
/// <c>1.4/**</c>. El comportamiento observado es el del 1.4 congelado.
/// </para>
/// </summary>
[Collection(IntegrationCollection.Name)]
[Trait("Category", "Integration")]
public sealed class SqlHelperContractTests
{
    private static readonly Assembly DataAssembly = typeof(CooperatorDataException).Assembly;
    private static readonly Type SqlHelperType = DataAssembly.GetType("Cooperator.Framework.Data.SqlHelper", throwOnError: true)!;
    private static readonly Type ParameterCacheType = DataAssembly.GetType("Cooperator.Framework.Data.SqlHelperParameterCache", throwOnError: true)!;

    private const CommandType Text = CommandType.Text;

    private readonly IntegrationDatabase _database;

    public SqlHelperContractTests(IntegrationDatabase database) => _database = database;

    [Fact]
    public void ExecuteScalar_with_a_command_text_returns_the_server_value()
    {
        var observed = InvokeStatic(SqlHelperType, "ExecuteScalar", [_database.ConnectionString, Text, "SELECT 42;"]);

        Assert.Equal(42, observed);
    }

    [Fact]
    public void ExecuteScalar_with_explicit_parameters_binds_them_by_position()
    {
        var parameters = new[]
        {
            new SqlParameter("@left", SqlDbType.Int) { Value = 40 },
            new SqlParameter("@right", SqlDbType.Int) { Value = 2 },
        };

        var observed = InvokeStatic(
            SqlHelperType,
            "ExecuteScalar",
            [_database.ConnectionString, Text, "SELECT @left + @right;", parameters]);

        Assert.Equal(42, observed);
    }

    [Fact]
    public void ExecuteNonQuery_reports_the_number_of_affected_rows()
    {
        var observed = InvokeStatic(
            SqlHelperType,
            "ExecuteNonQuery",
            [_database.ConnectionString, Text, "UPDATE dbo.ItParent SET Notes = Notes WHERE Code = 'IT-0001';"],
            new[] { typeof(string), typeof(CommandType), typeof(string) });

        Assert.Equal(1, observed);
    }

    [Fact]
    public void ExecuteDataSet_returns_the_result_sets_with_their_default_table_names()
    {
        var dataSet = (DataSet)InvokeStatic(
            SqlHelperType,
            "ExecuteDataSet",
            [_database.ConnectionString, Text, "SELECT * FROM dbo.ItParent; SELECT * FROM dbo.ItChild;"])!;

        Assert.Equal(2, dataSet.Tables.Count);
        Assert.Equal(2, dataSet.Tables[0].Rows.Count);
        Assert.Equal(3, dataSet.Tables[1].Rows.Count);
    }

    [Fact]
    public void FillDataSet_loads_only_the_named_table()
    {
        var dataSet = new DataSet();
        var tableNames = new[] { "Padres" };

        _ = InvokeStatic(
            SqlHelperType,
            "FillDataSet",
            [_database.ConnectionString, Text, "SELECT * FROM dbo.ItParent;", dataSet, tableNames],
            [typeof(string), typeof(CommandType), typeof(string), typeof(DataSet), typeof(string[])]);

        Assert.Single(dataSet.Tables);
        Assert.Equal("Padres", dataSet.Tables[0].TableName);
        Assert.Equal(2, dataSet.Tables[0].Rows.Count);
    }

    [Fact]
    public void ExecuteReader_returns_a_reader_that_the_caller_owns()
    {
        using var reader = (SqlDataReader)InvokeStatic(
            SqlHelperType,
            "ExecuteReader",
            [_database.ConnectionString, Text, "SELECT Code FROM dbo.ItParent ORDER BY Code;"])!;

        var codes = new List<string>();
        while (reader.Read())
        {
            codes.Add(reader.GetString(0));
        }

        Assert.Equal(["IT-0001", "IT-0002"], codes);
    }

    [Fact]
    public void A_rolled_back_transaction_leaves_the_fixture_unchanged()
    {
        var before = _database.Count("dbo.ItParent");

        using var connection = _database.OpenConnection();
        using var transaction = connection.BeginTransaction();
        _ = InvokeStatic(
            SqlHelperType,
            "ExecuteNonQuery",
            [transaction, Text, "INSERT INTO dbo.ItParent (Code, Name) VALUES ('IT-0003', 'Temp');"],
            [typeof(SqlTransaction), typeof(CommandType), typeof(string)]);
        transaction.Rollback();

        Assert.Equal(before, _database.Count("dbo.ItParent"));
    }

    [Fact]
    public void The_parameter_cache_returns_the_very_same_parameter_set_it_stored()
    {
        var cacheKey = "SELECT @x + @y;";
        var parameters = new[]
        {
            new SqlParameter("@x", SqlDbType.Int) { Value = 1 },
            new SqlParameter("@y", SqlDbType.Int) { Value = 2 },
        };

        _ = InvokeStatic(ParameterCacheType, "CacheParameterSet", [_database.ConnectionString, cacheKey, parameters]);
        var cached = (SqlParameter[])InvokeStatic(ParameterCacheType, "GetCachedParameterSet", [_database.ConnectionString, cacheKey])!;

        Assert.Equal(2, cached.Length);
        Assert.Equal("@x", cached[0].ParameterName);
        Assert.Equal("@y", cached[1].ParameterName);
    }

    [Fact]
    public void The_parameter_cache_discovers_a_stored_procedure_signature_from_the_real_catalog()
    {
        const string procedure = "dbo.ItSum";
        Execute(
            $"""
             CREATE OR ALTER PROCEDURE {procedure}
                 @Left INT,
                 @Right INT,
                 @Result INT OUTPUT
             AS
             BEGIN
                 SET @Result = @Left + @Right;
             END
             """);

        var discovered = (SqlParameter[])InvokeStatic(
            ParameterCacheType,
            "GetSpParameterSet",
            [_database.ConnectionString, procedure, true])!;

        Assert.Contains(discovered, parameter => parameter.ParameterName == "@Left");
        Assert.Contains(discovered, parameter => parameter.ParameterName == "@Right");
        Assert.Contains(discovered, parameter => parameter.ParameterName == "@Result");
        Assert.Equal(ParameterDirection.ReturnValue, discovered[0].Direction);
    }

    [Fact]
    public void A_query_error_surfaces_as_the_provider_exception_not_as_a_wrapped_one()
    {
        var exception = Assert.Throws<TargetInvocationException>(() => InvokeStatic(
            SqlHelperType,
            "ExecuteScalar",
            [_database.ConnectionString, Text, "SELECT * FROM dbo.NoExiste;"]));

        Assert.IsType<SqlException>(exception.InnerException);
    }

    private void Execute(string batch)
    {
        using var connection = _database.OpenConnection();
        using var command = new SqlCommand(batch, connection);
        command.ExecuteNonQuery();
    }

    private static object? InvokeStatic(Type type, string methodName, object?[] arguments, Type[]? parameterTypes = null)
    {
        var method = parameterTypes is null
            ? type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(method => method.Name == methodName && method.GetParameters().Length == arguments.Length)
            : type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null, parameterTypes, null)
              ?? throw new MissingMethodException(type.FullName, methodName);

        return method.Invoke(null, arguments);
    }
}


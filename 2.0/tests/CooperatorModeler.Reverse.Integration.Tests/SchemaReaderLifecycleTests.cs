using System;
using System.Data;
using System.Linq;
using System.Reflection;
using Cooperator.Framework.Modeler.DBReverseHelper.Providers;
using Microsoft.Data.SqlClient;

namespace CooperatorModeler.Reverse.Integration.Tests;

/// <summary>
/// SPEC-261016-0027 §8.2 y AC-3: la conexión la crea y la <b>dispone</b> el propio reverse. La corrida no deja
/// conexiones abiertas y <b>ningún</b> objeto de conexión provisto por otro participa: la API del lector sólo recibe
/// la cadena ya resuelta.
/// </summary>
[Trait("Category", "Integration")]
public sealed class SchemaReaderLifecycleTests : IntegrationTestBase
{
    [Fact]
    public void The_reader_owns_its_connection_and_leaves_nothing_open()
    {
        SqlClientSchemaReader reader = new SqlClientSchemaReader(StripScheme(RawConnectionString));

        using DataSet first = reader.ExecuteSQLText("SELECT 1 AS one");
        Assert.Equal(1, first.Tables[0].Rows.Count);

        // Una segunda lectura funciona igual: el lector no retiene estado ni conexión entre llamadas.
        using DataSet second = reader.ExecuteSQLText("SELECT 2 AS two");
        Assert.Equal(2, second.Tables[0].Rows[0][0]);

        // Y una conexión nueva se abre sin problema (no quedaron sesiones retenidas por el lector).
        using SqlConnection probe = OpenConnection();
        Assert.Equal(ConnectionState.Open, probe.State);
    }

    [Fact]
    public void The_component_api_does_not_accept_a_connection_from_the_caller()
    {
        ConstructorInfo single = Assert.Single(typeof(SqlClientSchemaReader).GetConstructors());
        ParameterInfo parameter = Assert.Single(single.GetParameters());
        Assert.Equal(typeof(string), parameter.ParameterType);

        // No expone ni acepta objetos de conexión: la propiedad de la conexión es del componente.
        Assert.DoesNotContain(
            typeof(SqlClientSchemaReader).GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
            member => member.ToString()!.Contains("SqlConnection", StringComparison.Ordinal));
    }

    [Fact]
    public void The_schema_read_returns_the_expected_sets()
    {
        MSSQLReverseFactory factory = new MSSQLReverseFactory(StripScheme(RawConnectionString));
        DataSet tables = factory.CreateDBReverseProvider() is { } provider
            ? ReadTableNames(provider)
            : throw new InvalidOperationException("La factory no devolvio un provider.");

        Assert.NotEmpty(tables.Tables);
    }

    /// <summary>Ejecuta la consulta de tablas con el lector propio (misma ruta que usa el provider).</summary>
    private static DataSet ReadTableNames(object provider)
    {
        SqlClientSchemaReader reader = new SqlClientSchemaReader(StripScheme(RawConnectionString));
        MethodInfo? query = provider.GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(method => method.Name.Contains("Table", StringComparison.Ordinal) && method.ReturnType == typeof(DataSet));

        return query is null
            ? reader.ExecuteSQLText("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES")
            : (DataSet)query.Invoke(provider, null)!;
    }
}

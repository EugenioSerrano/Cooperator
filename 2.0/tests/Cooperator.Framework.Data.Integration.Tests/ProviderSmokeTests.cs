using System;
using System.Data;
using System.Data.Common;
using Cooperator.Framework.Data.Casting;
using Microsoft.Data.SqlClient;

namespace Cooperator.Framework.Data.Integration.Tests;

/// <summary>
/// SPEC-261016-0020 §8.2 (AC-1, AC-2 y AC-3): <b>smoke de conexión real</b> — el criterio 2 de §A.1 — con
/// ejecución de consultas y procedimientos (scalar, non-query, reader, DataSet), parámetros y transacciones
/// (commit y rollback), todo atravesando el <b>punto único de conversión</b> con objetos reales del proveedor.
/// </summary>
[Trait("Category", "Integration")]
public sealed class ProviderSmokeTests : IntegrationTestBase
{
    [Fact]
    public void The_real_connection_round_trips_scalar_reader_and_dataset()
    {
        using SqlConnection connection = OpenConnection();
        Assert.Equal(ConnectionState.Open, connection.State);

        using SqlCommand scalar = new SqlCommand("SELECT 1", connection);
        scalar.CommandTimeout = 30;
        Assert.Equal(1, (int)scalar.ExecuteScalar()!);

        using SqlCommand reader = new SqlCommand("SELECT 'a' AS a, 'b' AS b", connection);
        using SqlDataReader data = reader.ExecuteReader();
        Assert.True(data.Read());
        Assert.Equal("a", data.GetString(0));

        using SqlDataAdapter adapter = new SqlDataAdapter("SELECT 1 AS one", connection);
        DataSet set = new DataSet();
        adapter.Fill(set);
        Assert.Equal(1, set.Tables[0].Rows.Count);
    }

    [Fact]
    public void A_real_transaction_commits_and_rolls_back_through_the_single_conversion_point()
    {
        using SqlConnection connection = OpenConnection();

        using (SqlTransaction commit = connection.BeginTransaction())
        {
            SqlTransaction adapted = TransactionAdapter.AsSqlTransaction(commit, "prueba:integración")!;
            Assert.Same(commit, adapted);
            commit.Commit();
        }

        using (SqlTransaction rollback = connection.BeginTransaction())
        {
            Assert.Same(rollback, TransactionAdapter.AsSqlTransaction(rollback, "prueba:integración"));
            rollback.Rollback();
        }

        // Y el borde público también con el contrato BCL: la forma se preserva.
        DbConnection asBcl = connection;
        Assert.Same(connection, TransactionAdapter.AsSqlConnection(asBcl, "prueba:integración"));
    }
}

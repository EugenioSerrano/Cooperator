using System;
using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace Cooperator.Framework.Data.Integration.Tests;

/// <summary>
/// SPEC-261016-0020 §8.2 y AC-7 (ADR-005 §3.4): el <b>descubrimiento de parámetros</b> contra un procedimiento
/// real **no abre ni cierra la conexión del llamador**: la conexión sigue abierta y **no** fue dispuesta por el
/// descubrimiento (el <i>hazard</i> verificado de 1.4, corregido en §C.4).
/// <para>
/// La metadata y el descubrimiento son <b>implementación</b> (`internal`, §C.3): se ejercen por <b>reflexión</b>,
/// sin abrir el ensamblado firmado a un `InternalsVisibleTo` ni ensanchar la superficie.
/// </para>
/// </summary>
[Trait("Category", "Integration")]
public sealed class StoredProcedureMetadataTests : IntegrationTestBase
{
    [Fact]
    public void The_discovery_does_not_open_nor_close_the_callers_connection()
    {
        using SqlConnection connection = OpenConnection();
        Assert.Equal(ConnectionState.Open, connection.State);

        MethodInfo discover = ParameterCacheType().GetMethod(
            "DiscoverSpParameterSet", BindingFlags.NonPublic | BindingFlags.Static)!;
        SqlParameter[] parameters = (SqlParameter[])discover.Invoke(null, [connection, StoredProcedureName, true])!;

        Assert.NotEmpty(parameters);

        // La conexión del llamador sigue abierta y sigue siendo la misma instancia: nadie la cerró ni la dispuso.
        Assert.Equal(ConnectionState.Open, connection.State);
        using SqlCommand probe = new SqlCommand("SELECT 1", connection);
        Assert.Equal(1, (int)probe.ExecuteScalar()!);
    }

    [Fact]
    public void The_metadata_is_complete_enough_to_build_a_real_command()
    {
        using SqlConnection connection = OpenConnection();

        MethodInfo internalSet = ParameterCacheType().GetMethod(
            "GetSpParameterSetInternal",
            BindingFlags.NonPublic | BindingFlags.Static,
            binder: null,
            [typeof(SqlConnection), typeof(string), typeof(bool)],
            modifiers: null)!;

        SqlParameter[] parameters = (SqlParameter[])internalSet.Invoke(null, [connection, StoredProcedureName, false])!;

        using SqlCommand command = new SqlCommand(StoredProcedureName, connection) { CommandType = CommandType.StoredProcedure };
        foreach (SqlParameter parameter in parameters)
        {
            command.Parameters.Add(new SqlParameter(parameter.ParameterName, parameter.SqlDbType));
        }

        Assert.Equal(parameters.Length, command.Parameters.Count);
    }

    /// <summary>El tipo del caché de parámetros vive en el ensamblado de la capa y es implementación interna.</summary>
    private static Type ParameterCacheType() =>
        typeof(Cooperator.Framework.Data.BaseRule).Assembly.GetType("Cooperator.Framework.Data.SqlHelperParameterCache", throwOnError: true)!;
}

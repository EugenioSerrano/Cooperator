using System;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Cooperator.Framework.Data.Integration.Tests;

/// <summary>
/// Base de la lane de integración (SPEC-261016-0020 §8.2): el **entorno** aporta la cadena de conexión
/// (nunca el repositorio) y los casos fallan con un mensaje claro si no está — la corrida pertenece al ciclo de
/// test de integración del Bolt, no al V-Bounce.
/// </summary>
public abstract class IntegrationTestBase
{
    /// <summary>Variable de entorno con la cadena de conexión de la base de prueba (jamas versionada).</summary>
    internal const string ConnectionStringVariable = "COOPERATOR_TEST_SQL_CONNECTION";

    /// <summary>Credencial interna del fixture: la cadena provista por el entorno.</summary>
    private protected static string ConnectionString
    {
        get
        {
            string? value = Environment.GetEnvironmentVariable(ConnectionStringVariable);
            Assert.False(
                string.IsNullOrWhiteSpace(value),
                "La lane de integración necesita la cadena de conexión en la variable de entorno " +
                ConnectionStringVariable + " (la aporta el entorno; el repositorio no la lleva).");
            return value!;
        }
    }

    private protected static SqlConnection OpenConnection()
    {
        SqlConnection connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    /// <summary>Nombre del procedimiento de prueba con parámetros, provisto por el entorno del ciclo.</summary>
    private protected static string StoredProcedureName => "cooperator_test_parameters";
}

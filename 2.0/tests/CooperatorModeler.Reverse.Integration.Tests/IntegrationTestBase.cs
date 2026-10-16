using System;
using System.Configuration;
using Cooperator.Framework.Core.Configuration;
using Microsoft.Data.SqlClient;

namespace CooperatorModeler.Reverse.Integration.Tests;

/// <summary>
/// Base de la lane de integración del reverse (SPEC-261016-0027 §8.2): el <b>entorno</b> aporta la cadena de
/// conexión —nunca el repositorio— y los casos fallan con un mensaje explícito si no está. La corrida pertenece al
/// ciclo de test de integración del Bolt.
/// <para>
/// La cadena entra por la <b>configuración portable</b> (el contrato del Bolt 011): el escenario del ciclo la publica
/// como <c>Cooperator:ConnectionStrings:CooperatorConnectionString</c> con su prefijo de esquema, y el reverse la
/// lee por el contrato —no hay cadena embebida ni mecanismo propio (AC-4)—.
/// </para>
/// </summary>
public abstract class IntegrationTestBase
{
    /// <summary>Variable de entorno con la cadena de conexión de la base de prueba (jamás versionada).</summary>
    internal const string ConnectionStringVariable = "COOPERATOR_TEST_SQL_CONNECTION";

    /// <summary>La cadena del entorno, ya con el prefijo de esquema del contrato.</summary>
    private protected static string RawConnectionString
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

    /// <summary>Publica la cadena del entorno en la configuración portable y la devuelve resuelta.</summary>
    private protected static string ResolveThroughConfiguration()
    {
        ICooperatorConfiguration previous = CooperatorSettings.Current;
        CooperatorSettings.Current = new EnvironmentConfiguration();
        try
        {
            Environment.SetEnvironmentVariable(
                "Cooperator__ConnectionStrings__" + ConfigurationKeys.ConnectionStrings.DefaultName,
                RawConnectionString);
            return Cooperator.Framework.Modeler.DBReverseHelper.ConnectionStringResolver.Resolve();
        }
        finally
        {
            CooperatorSettings.Current = previous;
        }
    }

    private protected static SqlConnection OpenConnection()
    {
        SqlConnection connection = new SqlConnection(StripScheme(RawConnectionString));
        connection.Open();
        return connection;
    }

    /// <summary>La cadena sin el prefijo de esquema del contrato (lo que el provider necesita).</summary>
    private protected static string StripScheme(string raw) =>
        raw.StartsWith(ConnectionStringValue.PlainScheme + ConnectionStringValue.SchemeSeparator, StringComparison.Ordinal)
            ? raw.Substring(ConnectionStringValue.PlainScheme.Length + 1)
            : raw;

    /// <summary>Nombre del esquema de prueba: el que aprovisiona <c>SchemaProvisioning.sql</c>.</summary>
    private protected const string TestSchema = "cooperator_reverse_test";
}

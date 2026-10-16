using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Cooperator.Framework.Characterization.Tests.Integration;

/// <summary>
/// Colección de integración: los casos contra SQL Server real no corren en
/// paralelo entre sí —comparten la base de fixture— y su corrida es la del ciclo
/// de integración del Bolt (SPEC §6.F, §8.2), nunca la del lazo hermético.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class IntegrationCollection : ICollectionFixture<IntegrationDatabase>
{
    public const string Name = "integration-sqlserver";
}

/// <summary>
/// Resolución del entorno de integración y guarda dura de la base.
///
/// <para>
/// La conexión llega por <b>variable de entorno</b>, nunca por un archivo
/// versionado (ADR-004 §3.2/§3.4): nada de credenciales en el repositorio.
/// Dos slots: <c>COOPERATOR_IT_LEGACY_CONNECTIONSTRING</c> (superficie 1.4, usado
/// en este Bolt) y <c>COOPERATOR_IT_TARGET_CONNECTIONSTRING</c> (superficie 2.0,
/// <b>reservado y vacío</b>: todavía no hay código 2.0 que verificar).
/// </para>
/// <para>
/// <b>Guarda dura (SPEC §6.F.1 y §10):</b> el arnés sólo opera contra una base
/// cuyo nombre contenga <c>cooperator_it</c> y aborta en cualquier otro caso. Es
/// la diferencia entre un fixture y un accidente: dirigir un caso de integración
/// contra una base real de un cliente se controla por construcción, no por
/// disciplina.
/// </para>
/// </summary>
public sealed class IntegrationDatabase
{
    public const string LegacyConnectionStringVariable = "COOPERATOR_IT_LEGACY_CONNECTIONSTRING";
    public const string TargetConnectionStringVariable = "COOPERATOR_IT_TARGET_CONNECTIONSTRING";
    public const string DatabaseNameToken = "cooperator_it";

    public IntegrationDatabase()
    {
        var raw = Environment.GetEnvironmentVariable(LegacyConnectionStringVariable);
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new InvalidOperationException(
                $"El ciclo de integración no puede correr: falta la variable de entorno {LegacyConnectionStringVariable}. " +
                "El entorno de integración todavía no está provisionado (dependencia externa declarada por el Bolt). " +
                $"El caso debe quedar registrado como bloqueado por entorno, nunca como omitido ni como pasado.");
        }

        var builder = new SqlConnectionStringBuilder(raw);
        if (builder.InitialCatalog.IndexOf(DatabaseNameToken, StringComparison.OrdinalIgnoreCase) < 0)
        {
            throw new InvalidOperationException(
                $"GUARDA DURA: la base '{builder.InitialCatalog}' no contiene «{DatabaseNameToken}». " +
                "El arnés aborta contra cualquier base que no sea la de fixture: no se ejecuta nada contra una base real.");
        }

        ConnectionString = raw;
        DatabaseName = builder.InitialCatalog;

        // El esquema y los datos de fixture se aplican sólo después de pasar la guarda.
        ApplyFixture();
    }

    public string ConnectionString { get; }

    public string DatabaseName { get; }

    /// <summary>Slot de la superficie 2.0: declarado y deliberadamente vacío en este Bolt.</summary>
    public static string? TargetConnectionString => Environment.GetEnvironmentVariable(TargetConnectionStringVariable);

    public SqlConnection OpenConnection()
    {
        var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    private void ApplyFixture()
    {
        foreach (var script in new[] { "schema.sql", "seed.sql" })
        {
            var path = Path.Combine(AppContext.BaseDirectory, "fixtures", "sqlserver", script);
            Assert.True(File.Exists(path), $"Falta el fixture de integración {path}.");

            var batches = File.ReadAllText(path)
                .Split(["\r\nGO\r\n", "\nGO\n"], StringSplitOptions.RemoveEmptyEntries);

            using var connection = OpenConnection();
            foreach (var batch in batches)
            {
                if (string.IsNullOrWhiteSpace(batch))
                {
                    continue;
                }

                using var command = new SqlCommand(batch, connection);
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
        }
    }

    /// <summary>Ejecuta una consulta escalar sobre la base de fixture.</summary>
    public object? Scalar(string sql)
    {
        using var connection = OpenConnection();
        using var command = new SqlCommand(sql, connection);
        var value = command.ExecuteScalar();
        return value is DBNull ? null : value;
    }

    public int Count(string table) =>
        Convert.ToInt32(Scalar($"SELECT COUNT(*) FROM {table};"), CultureInfo.InvariantCulture);
}

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Cooperator.Framework.Data.Integration.Tests;

/// <summary>
/// SPEC-261016-0020 §8.2 y AC-8 (§E.1, ADR-005 §3.1): los <b>cambios de comportamiento del proveedor</b>
/// verificados <b>contra la base real</b> — cifrado obligatorio y validación de certificado del servidor,
/// `DbType.Time` con `TimeSpan` y `DbType.Date` — que son el insumo de las notas de migración (Bolt 028).
/// </summary>
[Trait("Category", "Integration")]
public sealed class ProviderBehaviorTests : IntegrationTestBase
{
    [Fact]
    public void The_connection_encryption_default_is_the_mandatory_one()
    {
        // El cambio de ADR-005 §3.1: Encrypt pasa a Mandatory por defecto y TrustServerCertificate sigue false.
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);
        Assert.Equal(SqlConnectionEncryptOption.Mandatory, builder.Encrypt);
        Assert.False(builder.TrustServerCertificate);

        // Y la conexión real con esa configuración funciona (si el servidor tiene certificado válido) o el
        // entorno declara explícitamente la salida (TrustServerCertificate=true o Encrypt=false).
        using SqlConnection connection = OpenConnection();
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    public void DbType_time_requires_a_timespan_and_date_truncates_the_time_part()
    {
        using SqlConnection connection = OpenConnection();

        using SqlCommand time = new SqlCommand("SELECT CAST(@value AS time)", connection);
        time.Parameters.Add(new SqlParameter("@value", SqlDbType.Time) { Value = TimeSpan.FromMinutes(90) });
        Assert.Equal(TimeSpan.FromMinutes(90), (TimeSpan)time.ExecuteScalar()!);

        using SqlCommand date = new SqlCommand("SELECT CAST(@value AS date)", connection);
        date.Parameters.Add(new SqlParameter("@value", SqlDbType.Date) { Value = new DateTime(2026, 10, 16, 23, 59, 59) });
        Assert.Equal(new DateTime(2026, 10, 16), (DateTime)date.ExecuteScalar()!);
    }
}

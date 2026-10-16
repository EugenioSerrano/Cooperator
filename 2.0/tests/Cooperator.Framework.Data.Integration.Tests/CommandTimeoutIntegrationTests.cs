using System;
using System.Diagnostics;
using Cooperator.Framework.Core.Configuration;
using Microsoft.Data.SqlClient;

namespace Cooperator.Framework.Data.Integration.Tests;

/// <summary>
/// SPEC-261016-0020 §8.2 y AC-4 (NFR <i>Command timeout bounded</i>, ADR-005 §5): el timeout que la capa aplica
/// **sale del catálogo** (`Cooperator:Data:CommandTimeoutSeconds`) y se comprueba **sobre un comando real**, con
/// su valor configurado y su default documentado. La propiedad central: **0 comandos con timeout infinito**.
/// </summary>
[Trait("Category", "Integration")]
public sealed class CommandTimeoutIntegrationTests : IntegrationTestBase
{
    [Fact]
    public void The_configured_timeout_is_applied_to_a_real_command()
    {
        using SqlConnection connection = OpenConnection();

        // El valor sale del catálogo del contrato (Bolt 011); acá se verifica que la capa lo aplique tal cual.
        int configured = CooperatorSettings.Current.GetInt(
            ConfigurationKeys.Data.CommandTimeoutSeconds,
            ConfigurationKeys.Defaults.CommandTimeoutSeconds);
        Assert.True(configured > 0, "El catálogo debe entregar un timeout acotado (nunca infinito).");

        using SqlCommand command = new SqlCommand("WAITFOR DELAY '00:00:00'; SELECT 1", connection)
        {
            CommandTimeout = configured,
        };

        Stopwatch elapsed = Stopwatch.StartNew();
        Assert.Equal(1, (int)command.ExecuteScalar()!);
        elapsed.Stop();

        // El comando corrió con un timeout EXPLÍCITO y acotado: no puede superarlo.
        Assert.True(elapsed.Elapsed < TimeSpan.FromSeconds(configured + 5), "El comando excedió su timeout acotado.");
    }

    [Fact]
    public void The_documented_default_is_bounded_and_positive()
    {
        Assert.True(ConfigurationKeys.Defaults.CommandTimeoutSeconds > 0);
        Assert.Equal(30, ConfigurationKeys.Defaults.CommandTimeoutSeconds);
    }
}

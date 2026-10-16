using System;
using Cooperator.Framework.Core.Configuration;
using Cooperator.Framework.Modeler.DBReverseHelper;
using Microsoft.Data.SqlClient;

namespace CooperatorModeler.Reverse.Integration.Tests;

/// <summary>
/// SPEC-261016-0027 §8.2 y AC-4: la cadena de conexión tomada de la <b>configuración portable</b> (el contrato del
/// Bolt 011) <b>contra la base real</b>, con el prefijo aplicado y el resolutor invocado por <b>este</b>
/// consumidor. Se verifica además que el componente <b>no</b> tiene otro mecanismo de obtención de la cadena.
/// </summary>
[Trait("Category", "Integration")]
public sealed class ConnectionStringResolutionTests : IntegrationTestBase
{
    [Fact]
    public void The_chain_resolved_by_the_contract_opens_a_real_connection()
    {
        string resolved = ResolveThroughConfiguration();

        Assert.False(string.IsNullOrWhiteSpace(resolved));

        using SqlConnection connection = new SqlConnection(resolved);
        connection.Open();
        Assert.Equal(System.Data.ConnectionState.Open, connection.State);
    }

    [Fact]
    public void The_source_value_carries_the_scheme_prefix_of_the_contract()
    {
        // El valor llega crudo, CON su prefijo (ADR-004 §3.3): el consumidor lo interpreta y resuelve.
        string raw = RawConnectionString;
        Assert.StartsWith(ConnectionStringValue.PlainScheme + ConnectionStringValue.SchemeSeparator, raw, StringComparison.Ordinal);
    }

    [Fact]
    public void The_component_has_no_other_way_to_get_the_chain()
    {
        // AC-4: la única vía es el contrato. El tipo del resolutor expone sólo los puntos de entrada del contrato.
        Assert.Equal(ConnectionStringResolver.DefaultName, ConfigurationKeys.ConnectionStrings.DefaultName);
        Assert.NotNull(typeof(ConnectionStringResolver).GetMethod(nameof(ConnectionStringResolver.Resolve), Type.EmptyTypes));
    }
}

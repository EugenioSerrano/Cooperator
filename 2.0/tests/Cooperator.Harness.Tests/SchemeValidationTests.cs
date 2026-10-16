using System;
using Cooperator.Framework.Core.Configuration;
using Cooperator.Framework.Library.Cryptography.ProtectedValues;

namespace Cooperator.Harness.Tests;

/// <summary>
/// AC-7 del SPEC-261016-0035 §7: la validación de esquemas <b>sin degradación</b> —<c>plain:</c> es identidad, un
/// esquema sin resolvedor registrado produce el error tipado propio, y el valor <b>sin</b> prefijo o con un prefijo
/// desconocido produce el error del <b>contrato de configuración</b> (ADR-004 §3.3, Bolt 011)— y el comportamiento
/// del punto de registro de <b>fijación única</b> (SPEC §14).
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class SchemeValidationTests
{
    [Fact]
    public void The_identity_scheme_returns_the_payload_unchanged()
    {
        ProtectedValueKeyRegistry keys = ProtectedValueTestSupport.Registry((ProtectedValueTestSupport.KeyOne, ProtectedValueTestSupport.NewKey()));
        ProtectedValueResolverRegistry registry = ProtectedValueTestSupport.FrameworkRegistry(keys);

        const string payload = "Server=.;Database=Ventas;Integrated Security=SSPI";
        Assert.Equal(payload, registry.Resolve("Cooperator:ConnectionStrings:Primary", "plain" + ":" + payload));

        // Y el esquema de identidad del framework no interpreta nada: un valor con prefijo pegado vuelve tal cual.
        Assert.Equal("aesgcm:kid:x", registry.Resolve("Cooperator:ConnectionStrings:Primary", "plain" + ":" + "aesgcm:kid:x"));

        // El esquema conocido 'aesgcm' con la mis ma clave no degrada a texto plano: exige una carga del formato.
        Assert.Throws<ProtectedValueIntegrityException>(
            () => registry.Resolve("Cooperator:ConnectionStrings:Primary", "aesgcm" + ":" + "kidSinCarga" + ":"));
    }

    [Fact]
    public void An_unregistered_scheme_says_exactly_that()
    {
        ProtectedValueKeyRegistry keys = ProtectedValueTestSupport.Registry((ProtectedValueTestSupport.KeyOne, ProtectedValueTestSupport.NewKey()));
        ProtectedValueResolverRegistry registry = ProtectedValueTestSupport.FrameworkRegistry(keys);

        ProtectedValueSchemeException exception = Assert.Throws<ProtectedValueSchemeException>(
            () => registry.Resolve("Cooperator:ConnectionStrings:Primary", "dpapi" + ":" + "una-carga"));

        Assert.Contains("dpapi", exception.Message, StringComparison.Ordinal);
        Assert.Contains("Cooperator:ConnectionStrings:Primary", exception.ConfigurationKey, StringComparison.Ordinal);

        // Un esquema del host NO registrado falla igual: el framework no lo implementa (AC-8).
        Assert.Throws<ProtectedValueSchemeException>(() => registry.Require("kv"));

        // Y los esquemas registrados por el framework son exactamente dos.
        Assert.Equal(new[] { "aesgcm", "plain" }, registry.Schemes);
    }

    [Fact]
    public void A_value_without_prefix_produces_the_contract_error_and_never_plain_text()
    {
        ProtectedValueKeyRegistry keys = ProtectedValueTestSupport.Registry((ProtectedValueTestSupport.KeyOne, ProtectedValueTestSupport.NewKey()));
        ProtectedValueResolverRegistry registry = ProtectedValueTestSupport.FrameworkRegistry(keys);

        // El error es el del CONTRATO de configuración (Bolt 011): un valor sin prefijo no se asume texto plano.
        Assert.Throws<CooperatorConfigurationException>(
            () => registry.Resolve("Cooperator:ConnectionStrings:Primary", "Server=.;Database=Ventas"));

        // Y el contrato, por su lado, rechaza lo mismo con su propio error y nombra la clave y el prefijo ofensores.
        CooperatorConfigurationException missing = Assert.Throws<CooperatorConfigurationException>(
            () => ConnectionStringValue.Parse("Cooperator:ConnectionStrings:Primary", "Server=.;Database=Ventas"));
        Assert.Contains("Cooperator:ConnectionStrings:Primary", missing.Message, StringComparison.Ordinal);

        CooperatorConfigurationException unknown = Assert.Throws<CooperatorConfigurationException>(
            () => ConnectionStringValue.Parse("Cooperator:ConnectionStrings:Primary", "desconocido" + ":" + "carga"));
        Assert.Contains("desconocido", unknown.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void The_registration_point_is_unique_does_not_replace_and_freezes()
    {
        ProtectedValueKeyRegistry keys = ProtectedValueTestSupport.Registry((ProtectedValueTestSupport.KeyOne, ProtectedValueTestSupport.NewKey()));
        ProtectedValueResolverRegistry registry = ProtectedValueTestSupport.FrameworkRegistry(keys);

        // El mismo esquema no se registra dos veces (y el del framework ya está).
        Assert.Throws<ProtectedValueSchemeException>(() => registry.Register(new PlainTextValueResolver()));

        // Un esquema del host se registra una vez y se resuelve por su resolvedor…
        HostSchemeResolver host = new HostSchemeResolver("kv");
        registry.Register(host);
        Assert.Equal("valor del host", registry.Resolve("Cooperator:ConnectionStrings:Primary", "kv" + ":" + "carga"));

        // …y tras la fijación, el registro tardío es error tipado (no se reemplaza en caliente).
        registry.Freeze();
        Assert.True(registry.IsFixed);
        Assert.Throws<ProtectedValueSchemeException>(() => registry.Register(new HostSchemeResolver("otro")));
    }

    /// <summary>Resolvedor de un esquema del host, para el contrato con el consumidor (test double).</summary>
    private sealed class HostSchemeResolver : IProtectedValueResolver
    {
        internal HostSchemeResolver(string scheme)
        {
            Scheme = scheme;
        }

        public string Scheme { get; }

        public string Resolve(string key, string payload) => "valor del host";
    }
}

using System;
using System.Collections.Generic;
using Cooperator.Framework.Core.Configuration;
using Cooperator.Framework.Modeler.DBReverseHelper;

namespace CooperatorModeler.Reverse.Tests;

/// <summary>
/// AC-4 del SPEC-261016-0027 §7: la cadena de conexión sale del <b>contrato de configuración portable</b> (Bolt 011)
/// con su <b>prefijo de esquema</b>, la resuelve <b>este</b> consumidor invocando al resolutor, y <b>no</b> hay
/// cadena embebida, ni duplicada, ni inspección de su contenido: un esquema sin resolutor <b>falla</b> en lugar de
/// leerse a ciegas.
/// </summary>
[Collection("ReverseTooling")]
public sealed class ConnectionResolutionTests
{
    private const string Synthetic = "plain:Data Source=esquema-de-prueba;Initial Catalog=prueba";

    [Fact]
    public void The_chain_comes_from_the_configuration_contract_with_its_scheme_prefix()
    {
        using (Configuration.Set(new Dictionary<string, string> { ["CooperatorConnectionString"] = Synthetic }))
        {
            Assert.Equal("Data Source=esquema-de-prueba;Initial Catalog=prueba", ConnectionStringResolver.Resolve());
        }
    }

    [Fact]
    public void The_default_name_is_the_one_of_1_4()
    {
        // Continuidad declarada por el catalogo (Bolt 011): el nombre por defecto no se renombra.
        Assert.Equal("CooperatorConnectionString", ConnectionStringResolver.DefaultName);
        Assert.Equal("CooperatorConnectionString", ConfigurationKeys.ConnectionStrings.DefaultName);
    }

    [Fact]
    public void A_named_connection_string_uses_its_catalog_key()
    {
        using (Configuration.Set(new Dictionary<string, string> { ["EsquemaDePrueba"] = "plain:Data Source=otro" }))
        {
            Assert.Equal("Data Source=otro", ConnectionStringResolver.Resolve("EsquemaDePrueba"));
        }
    }

    [Fact]
    public void A_missing_key_is_the_typed_error_of_the_contract()
    {
        using (Configuration.Set(new Dictionary<string, string>()))
        {
            Assert.Throws<CooperatorConfigurationException>(() => ConnectionStringResolver.Resolve());
        }
    }

    [Fact]
    public void A_value_without_a_scheme_prefix_is_rejected_and_never_assumed_plain()
    {
        using (Configuration.Set(new Dictionary<string, string> { ["CooperatorConnectionString"] = "Data Source=x" }))
        {
            CooperatorConfigurationException error =
                Assert.Throws<CooperatorConfigurationException>(() => ConnectionStringResolver.Resolve());
            Assert.Contains("prefijo de esquema", error.Message, StringComparison.Ordinal);
            Assert.Contains("Cooperator:ConnectionStrings:CooperatorConnectionString", error.Message, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void A_value_with_a_scheme_without_a_resolver_fails_naming_the_offending_prefix()
    {
        using (Configuration.Set(new Dictionary<string, string> { ["CooperatorConnectionString"] = "esquema-desconocido:carga" }))
        {
            CooperatorConfigurationException error =
                Assert.Throws<CooperatorConfigurationException>(() => ConnectionStringResolver.Resolve());
            Assert.Contains("esquema-desconocido", error.Message, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void This_consumer_is_the_one_that_invokes_the_resolver()
    {
        RecordingResolver resolver = new RecordingResolver();

        using (Configuration.Set(new Dictionary<string, string> { ["CooperatorConnectionString"] = Synthetic }))
        {
            string resolved = ConnectionStringResolver.Resolve(ConnectionStringResolver.DefaultName, resolver);

            Assert.Equal("Data Source=esquema-de-prueba;Initial Catalog=prueba", resolved);
            Assert.Equal(1, resolver.Calls);
            // El contrato invoca al resolutor con el ESQUEMA y la carga (su firma): lo que se verifica es que la invocacion la hace ESTE consumidor.
            Assert.Equal(ConnectionStringValue.PlainScheme, resolver.LastKey);
            Assert.Equal("Data Source=esquema-de-prueba;Initial Catalog=prueba", resolver.LastPayload);
        }
    }

    [Fact]
    public void A_resolver_of_another_scheme_is_rejected()
    {
        using (Configuration.Set(new Dictionary<string, string> { ["CooperatorConnectionString"] = Synthetic }))
        {
            Assert.Throws<CooperatorConfigurationException>(
                () => ConnectionStringResolver.Resolve(ConnectionStringResolver.DefaultName, new RecordingResolver("otro")));
        }
    }

    [Fact]
    public void A_null_or_empty_name_is_rejected()
    {
        Assert.Throws<ArgumentNullException>(() => ConnectionStringResolver.Resolve(string.Empty));
    }

    /// <summary>Resolutor de prueba: registra la invocación (es el consumidor el que resuelve, ADR-004 §3.3).</summary>
    private sealed class RecordingResolver : IProtectedValueResolver
    {
        internal RecordingResolver(string scheme = ConnectionStringValue.PlainScheme) => Scheme = scheme;

        public string Scheme { get; }

        internal int Calls { get; private set; }

        internal string LastKey { get; private set; } = string.Empty;

        internal string LastPayload { get; private set; } = string.Empty;

        public string Resolve(string key, string payload)
        {
            Calls++;
            LastKey = key;
            LastPayload = payload;
            return payload;
        }
    }

    /// <summary>Doble del contrato de configuración (Bolt 011): entrega las cadenas sembradas.</summary>
    private sealed class StubConfiguration : ICooperatorConfiguration
    {
        private readonly Dictionary<string, string> _connectionStrings;

        internal StubConfiguration(Dictionary<string, string> connectionStrings) => _connectionStrings = connectionStrings;

        public string GetString(string key) => throw new NotSupportedException();

        public string GetString(string key, string defaultValue) => defaultValue;

        public int GetInt(string key) => throw CooperatorConfigurationException.MissingKey(key);

        public int GetInt(string key, int defaultValue) => defaultValue;

        public string GetConnectionString(string name) =>
            _connectionStrings.TryGetValue(name, out string? value)
                ? value
                : throw CooperatorConfigurationException.MissingKey(ConfigurationKeys.ConnectionStrings.For(name));
    }

    private static class Configuration
    {
        internal static IDisposable Set(Dictionary<string, string> connectionStrings)
        {
            ICooperatorConfiguration previous = CooperatorSettings.Current;
            CooperatorSettings.Current = new StubConfiguration(connectionStrings);
            return new Restore(previous);
        }

        private sealed class Restore : IDisposable
        {
            private readonly ICooperatorConfiguration _previous;

            internal Restore(ICooperatorConfiguration previous) => _previous = previous;

            public void Dispose() => CooperatorSettings.Current = _previous;
        }
    }
}

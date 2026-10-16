using System.Globalization;
using Cooperator.Framework.Core.Configuration;
using Cooperator.Harness.Tests.Support;

namespace Cooperator.Harness.Tests;

/// <summary>
/// Contrato de configuración, default por entorno y los cuatro casos negativos canónicos
/// (SPEC-261016-0018 §6/A, §6/B, §6/D, §8.1; AC-1, AC-2, AC-3).
///
/// El caso hace de <b>consumidor</b> del contrato: lo ejercita con la implementación ambiental y con un doble de
/// prueba, que es exactamente cómo lo alimentan los hosts y los tests (sin tocar archivos).
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class ConfigurationContractTests
{
    private const string TimeoutVariable = "Cooperator__Data__CommandTimeoutSeconds";
    private const string ConnectionStringVariable = "Cooperator__ConnectionStrings__CooperatorConnectionString";
    private const string SmtpVariable = "Cooperator__Mail__SmtpServer";

    // ---------------------------------------------------------------- El contrato

    [Fact]
    public void The_catalog_publishes_the_keys_as_constants_with_the_documented_shape()
    {
        Assert.Equal("Cooperator:Data:CommandTimeoutSeconds", ConfigurationKeys.Data.CommandTimeoutSeconds);
        Assert.Equal("Cooperator:Cache:DefaultTtlSeconds", ConfigurationKeys.Cache.DefaultTtlSeconds);
        Assert.Equal("Cooperator:Cache:ParameterMetadataTtlSeconds", ConfigurationKeys.Cache.ParameterMetadataTtlSeconds);
        Assert.Equal("Cooperator:Cache:MaxEntries", ConfigurationKeys.Cache.MaxEntries);
        Assert.Equal("Cooperator:Mail:SmtpServer", ConfigurationKeys.Mail.SmtpServer);
        Assert.Equal("Cooperator:ConnectionStrings:CooperatorConnectionString", ConfigurationKeys.ConnectionStrings.For(ConfigurationKeys.ConnectionStrings.DefaultName));
        Assert.Equal("Cooperator:LazyLoad:Providers:Customer:Assembly", ConfigurationKeys.LazyLoad.AssemblyFor("Customer"));
        Assert.Equal("Cooperator:LazyLoad:Providers:Customer:Class", ConfigurationKeys.LazyLoad.ClassFor("Customer"));
    }

    [Fact]
    public void The_documented_defaults_are_the_ones_of_the_adr()
    {
        Assert.Equal(30, ConfigurationKeys.Defaults.CommandTimeoutSeconds);
        Assert.Equal(3600, ConfigurationKeys.Defaults.CacheDefaultTtlSeconds);
        Assert.Equal(86400, ConfigurationKeys.Defaults.CacheParameterMetadataTtlSeconds);
    }

    [Fact]
    public void The_environment_convention_is_the_double_underscore_one()
    {
        Assert.Equal("Cooperator__Data__CommandTimeoutSeconds",
            EnvironmentConfiguration.EnvironmentVariableNameFor(ConfigurationKeys.Data.CommandTimeoutSeconds));
    }

    // ---------------------------------------------------------------- El default (AC-2)

    [Fact]
    public void A_value_present_in_the_environment_is_returned()
    {
        var configuration = WithEnvironment(TimeoutVariable, "45");

        Assert.Equal(45, configuration.GetInt(ConfigurationKeys.Data.CommandTimeoutSeconds));
    }

    [Fact]
    public void A_key_with_a_documented_default_returns_it_when_there_is_no_value()
    {
        var configuration = WithoutEnvironment(TimeoutVariable);

        Assert.Equal(30, configuration.GetInt(ConfigurationKeys.Data.CommandTimeoutSeconds, ConfigurationKeys.Defaults.CommandTimeoutSeconds));
    }

    [Fact]
    public void A_string_key_with_a_default_returns_it_when_there_is_no_value()
    {
        var configuration = WithoutEnvironment(SmtpVariable);

        Assert.Equal("no-hay-servidor", configuration.GetString(ConfigurationKeys.Mail.SmtpServer, "no-hay-servidor"));
    }

    [Fact]
    public void An_empty_environment_value_is_treated_as_absent()
    {
        var configuration = WithEnvironment(TimeoutVariable, string.Empty);

        // No es un valor válido: la variante con default devuelve el default...
        Assert.Equal(30, configuration.GetInt(ConfigurationKeys.Data.CommandTimeoutSeconds, 30));
        // ...y la obligatoria falla con el error tipado, en vez de devolver "" o un null silencioso.
        Assert.Throws<CooperatorConfigurationException>(() => configuration.GetInt(ConfigurationKeys.Data.CommandTimeoutSeconds));
    }

    // ---------------------------------------------------------------- Casos negativos (AC-3)

    [Fact]
    public void A_missing_key_without_a_default_fails_with_the_framework_own_exception()
    {
        var configuration = WithoutEnvironment(ConnectionStringVariable);

        var exception = Assert.Throws<CooperatorConfigurationException>(
            () => configuration.GetConnectionString(ConfigurationKeys.ConnectionStrings.DefaultName));

        Assert.Contains(ConfigurationKeys.ConnectionStrings.For(ConfigurationKeys.ConnectionStrings.DefaultName), exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_connection_string_without_a_prefix_is_rejected()
    {
        var configuration = WithEnvironment(ConnectionStringVariable, "Server=.;Database=Ventas;");

        var exception = Assert.Throws<CooperatorConfigurationException>(
            () => configuration.GetConnectionString(ConfigurationKeys.ConnectionStrings.DefaultName));

        Assert.Contains(ConfigurationKeys.ConnectionStrings.DefaultName, exception.Message, StringComparison.Ordinal);
        Assert.Contains("plain:", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_connection_string_with_an_unknown_prefix_is_rejected_naming_the_offending_prefix()
    {
        var configuration = WithEnvironment(ConnectionStringVariable, "aesgcm:kid-1:AAAA");

        var exception = Assert.Throws<CooperatorConfigurationException>(
            () => configuration.GetConnectionString(ConfigurationKeys.ConnectionStrings.DefaultName));

        Assert.Contains("aesgcm", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_non_numeric_value_for_an_integer_key_fails_with_the_framework_own_exception()
    {
        var configuration = WithEnvironment(TimeoutVariable, "no-es-un-numero");

        var exception = Assert.Throws<CooperatorConfigurationException>(
            () => configuration.GetInt(ConfigurationKeys.Data.CommandTimeoutSeconds, 30));

        Assert.Contains("no-es-un-numero", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void No_invalid_path_returns_a_silent_fallback()
    {
        var configuration = WithEnvironment(ConnectionStringVariable, "sin-prefijo");

        // Ni valor vacío, ni null: excepción tipada, siempre.
        Assert.Throws<CooperatorConfigurationException>(
            () => configuration.GetConnectionString(ConfigurationKeys.ConnectionStrings.DefaultName));
    }

    // ---------------------------------------------------------------- Forma del valor y seam (AC-3)

    [Fact]
    public void A_plain_prefixed_value_is_returned_raw_with_its_prefix()
    {
        var raw = "plain:Server=.;Database=Ventas;";
        var configuration = WithEnvironment(ConnectionStringVariable, raw);

        Assert.Equal(raw, configuration.GetConnectionString(ConfigurationKeys.ConnectionStrings.DefaultName));
    }

    [Fact]
    public void The_plain_scheme_resolves_by_identity_and_an_unknown_one_fails_typed()
    {
        var plain = ConnectionStringValue.Parse("k", "plain:Server=.;Database=Ventas");
        Assert.True(plain.IsPlain);
        Assert.Equal("Server=.;Database=Ventas", plain.Resolve(ConnectionStringValue.IdentityResolver));

        var unknown = ConnectionStringValue.Parse("k", "plain:x");
        Assert.Throws<CooperatorConfigurationException>(() => unknown.Resolve(new ForeignResolver()));
    }

    [Theory]
    [InlineData("plain:", true, "")]
    [InlineData("plain:a", true, "a")]
    [InlineData("sin-prefijo", false, null)]
    public void The_value_form_is_parsed_with_the_rule_of_the_adr(string raw, bool valid, string? payload)
    {
        if (!valid)
        {
            Assert.Throws<CooperatorConfigurationException>(() => ConnectionStringValue.Parse("k", raw));
            return;
        }

        Assert.Equal(payload, ConnectionStringValue.Parse("k", raw).Payload);
    }

    // ------------------------------------------- Consumidor: instancia ambiental y doble de prueba (AC-1)

    [Fact]
    public void The_ambient_instance_can_be_injected_and_is_read_lazily()
    {
        CooperatorSettings.Current = new DoubleConfiguration(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [ConfigurationKeys.Data.CommandTimeoutSeconds] = "99",
        });

        // El consumidor lee del contrato, sin saber qué implementación hay detrás.
        Assert.Equal(99, CooperatorSettings.Current.GetInt(ConfigurationKeys.Data.CommandTimeoutSeconds));
        Assert.Equal("doble", CooperatorSettings.Current.GetString("cualquiera", "doble"));
    }

    [Fact]
    public void Replacing_the_ambient_instance_is_the_documented_hot_change()
    {
        CooperatorSettings.Current = new DoubleConfiguration(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [ConfigurationKeys.Data.CommandTimeoutSeconds] = "10",
        });
        Assert.Equal(10, CooperatorSettings.Current.GetInt(ConfigurationKeys.Data.CommandTimeoutSeconds));

        CooperatorSettings.Current = new DoubleConfiguration(new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [ConfigurationKeys.Data.CommandTimeoutSeconds] = "20",
        });

        // Ya no hay caché estática que impida ver el cambio (ADR-004 §3.3).
        Assert.Equal(20, CooperatorSettings.Current.GetInt(ConfigurationKeys.Data.CommandTimeoutSeconds));
    }

    [Fact]
    public void Setting_a_null_ambient_instance_is_rejected()
    {
        Assert.Throws<ArgumentNullException>(() => CooperatorSettings.Current = null!);
    }

    // ---------------------------------------------------------------- Auxiliares

    private static ICooperatorConfiguration WithEnvironment(string variable, string value)
    {
        Environment.SetEnvironmentVariable(variable, value);
        return new EnvironmentConfiguration();
    }

    private static ICooperatorConfiguration WithoutEnvironment(string variable)
    {
        Environment.SetEnvironmentVariable(variable, null);
        return new EnvironmentConfiguration();
    }

    private sealed class ForeignResolver : IProtectedValueResolver
    {
        public string Scheme => "aesgcm";

        public string Resolve(string key, string payload) => payload;
    }

    /// <summary>
    /// Doble de prueba del contrato: lo único que un host con inyección de dependencias necesita implementar.
    /// </summary>
    private sealed class DoubleConfiguration : ICooperatorConfiguration
    {
        private readonly Dictionary<string, string> _values;

        public DoubleConfiguration(Dictionary<string, string> values) => _values = values;

        public string GetString(string key)
        {
            string value;
            if (!_values.TryGetValue(key, out value!))
            {
                throw CooperatorConfigurationException.MissingKey(key);
            }

            return value;
        }

        public string GetString(string key, string defaultValue)
        {
            string value;
            return _values.TryGetValue(key, out value!) ? value : defaultValue;
        }

        public int GetInt(string key) => int.Parse(GetString(key), CultureInfo.InvariantCulture);

        public int GetInt(string key, int defaultValue)
        {
            string value;
            return _values.TryGetValue(key, out value!) ? int.Parse(value, CultureInfo.InvariantCulture) : defaultValue;
        }

        public string GetConnectionString(string name) =>
            GetString(ConfigurationKeys.ConnectionStrings.For(name));
    }
}

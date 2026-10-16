using Cooperator.Framework.Core.Configuration;
using Cooperator.Framework.Core.LazyLoad;

namespace Cooperator.Harness.Tests;

/// <summary>
/// Base de los casos de la carga de providers (SPEC-261016-0015): aísla el estado
/// <b>ambiental</b> —la configuración vigente y el par hook/caché de <c>LazyLoad</c>— entre
/// casos, y ofrece la configuración de prueba que alimenta el camino por entidad con el
/// catálogo de ADR-004 §3.2.
/// <para>
/// Las clases de prueba se marcan con <c>[Collection(ConfigurationCollection.Name)]</c>:
/// tocan el mismo estado compartido que los casos de configuración del Bolt 011, y la
/// colección los serializa.
/// </para>
/// </summary>
public abstract class LazyProviderTestBase : IDisposable
{
    /// <summary>El ensamblado del caso normal: una dependencia DECLARADA del proyecto de pruebas.</summary>
    protected const string DeclaredAssembly = "Cooperator.LazyFixtures.Declared";

    /// <summary>El provider válido del ensamblado declarado.</summary>
    protected const string SampleProviderType = "Cooperator.LazyFixtures.Declared.SampleLazyProvider";

    /// <summary>Un segundo provider válido del mismo ensamblado (para nombres distintos).</summary>
    protected const string SecondProviderType = "Cooperator.LazyFixtures.Declared.SecondLazyProvider";

    /// <summary>El ensamblado del NFR: compilado pero NO declarado como dependencia.</summary>
    protected const string UndeclaredAssembly = "Cooperator.LazyFixtures.Undeclared";

    /// <summary>El provider válido del ensamblado no declarado.</summary>
    protected const string UndeclaredProviderType = "Cooperator.LazyFixtures.Undeclared.UndeclaredLazyProvider";

    /// <summary>Subcarpeta del directorio de salida donde vive el ensamblado no declarado.</summary>
    protected const string UndeclaredFolder = "plugins-undeclared";

    protected LazyProviderTestBase()
    {
        ProviderResolution.Reset();
        CooperatorSettings.Current = new TestConfiguration(new Dictionary<string, string>(StringComparer.Ordinal));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        ProviderResolution.Reset();
        CooperatorSettings.Current = new EnvironmentConfiguration();
    }

    /// <summary>Publica el catálogo de providers de prueba (entidad → ensamblado + tipo).</summary>
    protected static void Configure(params (string Entity, string Assembly, string Type)[] providers)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (entity, assembly, type) in providers)
        {
            values[ConfigurationKeys.LazyLoad.AssemblyFor(entity)] = assembly;
            values[ConfigurationKeys.LazyLoad.ClassFor(entity)] = type;
        }

        CooperatorSettings.Current = new TestConfiguration(values);
    }

    /// <summary>Publica valores crudos del catálogo (para los bordes de la validación estructural).</summary>
    protected static void ConfigureRaw(params (string Key, string Value)[] keys)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (key, value) in keys)
        {
            values[key] = value;
        }

        CooperatorSettings.Current = new TestConfiguration(values);
    }

    /// <summary>Configuración de prueba en memoria: la clave ausente se reporta como tal.</summary>
    private sealed class TestConfiguration : ICooperatorConfiguration
    {
        private readonly IReadOnlyDictionary<string, string> _values;

        public TestConfiguration(IReadOnlyDictionary<string, string> values) => _values = values;

        public string GetString(string key) =>
            _values.TryGetValue(key, out var value) ? value : throw CooperatorConfigurationException.MissingKey(key);

        public string GetString(string key, string defaultValue) =>
            _values.TryGetValue(key, out var value) ? value : defaultValue;

        public int GetInt(string key) =>
            int.TryParse(GetString(key), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var value)
                ? value
                : throw CooperatorConfigurationException.NotAnInteger(key, GetString(key));

        public int GetInt(string key, int defaultValue) =>
            _values.TryGetValue(key, out var raw)
                && int.TryParse(raw, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var value)
                    ? value
                    : defaultValue;

        public string GetConnectionString(string name) =>
            throw new NotSupportedException("El camino de providers de estas pruebas no lee cadenas de conexión.");
    }
}

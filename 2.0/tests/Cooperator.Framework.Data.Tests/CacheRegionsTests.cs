using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Cooperator.Framework.Core.Configuration;
using Cooperator.Framework.Data.Caching;

namespace Cooperator.Framework.Data.Tests;

/// <summary>
/// AC-4 del SPEC-261016-0019 §7: el TTL de cada región y el cap salen del <b>catálogo</b> de configuración
/// (Bolt 011), con sus defaults <b>publicados</b>; no hay un default escondido en el código ni un camino propio de
/// lectura, y un valor inválido propaga el error tipado del contrato (NFR <i>Config fail-fast</i>).
/// </summary>
[Collection("DataCaching")]
public sealed class CacheRegionsTests
{
    [Fact]
    public void Each_regions_ttl_comes_from_its_catalog_key()
    {
        ICooperatorConfiguration previous = CooperatorSettings.Current;
        try
        {
            CooperatorSettings.Current = new StubConfiguration(
                new System.Collections.Generic.Dictionary<string, int>(StringComparer.Ordinal)
                {
                    [ConfigurationKeys.Cache.DefaultTtlSeconds] = 120,
                    [ConfigurationKeys.Cache.ParameterMetadataTtlSeconds] = 240,
                    [ConfigurationKeys.Cache.MaxEntries] = 7,
                });

            Assert.Equal(TimeSpan.FromSeconds(120), CacheRegions.CollectionsTtl);
            Assert.Equal(TimeSpan.FromSeconds(240), CacheRegions.ParameterMetadataTtl);
            Assert.Equal(7, CacheRegions.MaxEntries);
        }
        finally
        {
            CooperatorSettings.Current = previous;
        }
    }

    [Fact]
    public void An_absent_key_uses_the_published_default_of_the_catalog()
    {
        ICooperatorConfiguration previous = CooperatorSettings.Current;
        try
        {
            CooperatorSettings.Current = new StubConfiguration(new System.Collections.Generic.Dictionary<string, int>(StringComparer.Ordinal));

            Assert.Equal(
                TimeSpan.FromSeconds(ConfigurationKeys.Defaults.CacheDefaultTtlSeconds),
                CacheRegions.CollectionsTtl);
            Assert.Equal(
                TimeSpan.FromSeconds(ConfigurationKeys.Defaults.CacheParameterMetadataTtlSeconds),
                CacheRegions.ParameterMetadataTtl);
            Assert.Equal(ConfigurationKeys.Defaults.CacheMaxEntries, CacheRegions.MaxEntries);
        }
        finally
        {
            CooperatorSettings.Current = previous;
        }
    }

    [Fact]
    public void The_timeout_region_reuses_the_published_generic_ttl()
    {
        ICooperatorConfiguration previous = CooperatorSettings.Current;
        try
        {
            CooperatorSettings.Current = new StubConfiguration(
                new System.Collections.Generic.Dictionary<string, int>(StringComparer.Ordinal)
                {
                    [ConfigurationKeys.Cache.DefaultTtlSeconds] = 60,
                });

            // La región del timeout no estrena un tercer default: reusa el genérico publicado.
            Assert.Equal(CacheRegions.CollectionsTtl, CacheRegions.CommandTimeoutTtl);
            Assert.Equal(TimeSpan.FromSeconds(60), CacheRegions.CommandTimeoutTtl);
        }
        finally
        {
            CooperatorSettings.Current = previous;
        }
    }

    [Fact]
    public void The_published_cap_figure_is_the_one_this_bolt_fixed()
    {
        // La cifra la fija el Bolt 013 (dueño de la caché): el Bolt 011 publicó la clave y delegó el valor
        // (MEM-261016-0117 §8.1). Este caso la clava para que un cambio silencioso se vea.
        Assert.Equal(1024, ConfigurationKeys.Defaults.CacheMaxEntries);
    }

    [Fact]
    public void An_invalid_catalog_value_propagates_the_typed_error_and_has_no_fallback()
    {
        ICooperatorConfiguration previous = CooperatorSettings.Current;
        try
        {
            CooperatorSettings.Current = new FailingConfiguration();

            Assert.Throws<CooperatorConfigurationException>(() => CacheRegions.CollectionsTtl);
            Assert.Throws<CooperatorConfigurationException>(() => CacheRegions.ParameterMetadataTtl);
            Assert.Throws<CooperatorConfigurationException>(() => CacheRegions.MaxEntries);
            Assert.Throws<CooperatorConfigurationException>(() => CacheRegions.CreateDefaultProvider());
        }
        finally
        {
            CooperatorSettings.Current = previous;
        }
    }

    [Fact]
    public void No_catalog_default_lives_hidden_in_the_cache_code()
    {
        string[] sources =
        [
            Source("CacheRegions.cs"), Source("CacheKey.cs"), Source("InMemoryCacheProvider.cs"), Source("ICacheProvider.cs"),
        ];

        string all = string.Join("\n", sources);

        // Los valores del catálogo no aparecen como literales en el código de la caché: se leen de ConfigurationKeys.
        Assert.DoesNotContain("3600", all, StringComparison.Ordinal);
        Assert.DoesNotContain("86400", all, StringComparison.Ordinal);
        Assert.DoesNotContain("1024", all, StringComparison.Ordinal);

        // Y las tres claves del catálogo se referencian por su constante, no por su texto.
        Assert.Contains("ConfigurationKeys.Cache.DefaultTtlSeconds", all, StringComparison.Ordinal);
        Assert.Contains("ConfigurationKeys.Cache.ParameterMetadataTtlSeconds", all, StringComparison.Ordinal);
        Assert.Contains("ConfigurationKeys.Cache.MaxEntries", all, StringComparison.Ordinal);
    }

    [Fact]
    public void The_default_provider_respects_the_cap_of_the_catalog()
    {
        ICooperatorConfiguration previous = CooperatorSettings.Current;
        try
        {
            CooperatorSettings.Current = new StubConfiguration(
                new System.Collections.Generic.Dictionary<string, int>(StringComparer.Ordinal)
                {
                    [ConfigurationKeys.Cache.MaxEntries] = 3,
                });

            ICacheProvider provider = CacheRegions.CreateDefaultProvider();
            for (int i = 0; i < 10; i++)
            {
                provider.Set("coop:data:rule-" + i + ":collection-all", i, TimeSpan.FromMinutes(1));
            }

            InMemoryCacheProvider inMemory = Assert.IsType<InMemoryCacheProvider>(provider);
            Assert.Equal(3, inMemory.MaxEntries);
            Assert.True(inMemory.Count <= 3);
        }
        finally
        {
            CooperatorSettings.Current = previous;
        }
    }

    [Fact]
    public void The_process_provider_is_replaceable()
    {
        ICacheProvider previous = CacheRegions.Provider;
        try
        {
            RecordingProvider doubleProvider = new RecordingProvider();
            CacheRegions.Provider = doubleProvider;

            Assert.Same(doubleProvider, CacheRegions.Provider);
            Assert.Throws<ArgumentNullException>(() => CacheRegions.Provider = null!);
        }
        finally
        {
            CacheRegions.Provider = previous;
        }
    }

    private static string Source(string file) =>
        File.ReadAllText(Path.Combine(RepositoryRoot(), "2.0", "src", "Cooperator.Framework.Data", "Caching", file));

    private static string RepositoryRoot()
    {
        DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "2.0", "Cooperator2.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("No se encontró la raíz del repositorio (2.0/Cooperator2.sln).");
    }

    /// <summary>Doble del contrato de configuración (Bolt 011): entrega valores o el default del catálogo.</summary>
    private sealed class StubConfiguration : ICooperatorConfiguration
    {
        private readonly System.Collections.Generic.Dictionary<string, int> _values;

        internal StubConfiguration(System.Collections.Generic.Dictionary<string, int> values) => _values = values;

        public string GetString(string key) => throw new NotSupportedException();

        public string GetString(string key, string defaultValue) => defaultValue;

        public int GetInt(string key) => throw CooperatorConfigurationException.MissingKey(key);

        public int GetInt(string key, int defaultValue) => _values.TryGetValue(key, out int value) ? value : defaultValue;

        public string GetConnectionString(string name) => throw new NotSupportedException();
    }

    /// <summary>Doble que simula una configuración inválida: toda lectura falla con el error del contrato.</summary>
    private sealed class FailingConfiguration : ICooperatorConfiguration
    {
        public string GetString(string key) => throw CooperatorConfigurationException.NotAnInteger(key, "abc");

        public string GetString(string key, string defaultValue) => throw CooperatorConfigurationException.NotAnInteger(key, "abc");

        public int GetInt(string key) => throw CooperatorConfigurationException.NotAnInteger(key, "abc");

        public int GetInt(string key, int defaultValue) => throw CooperatorConfigurationException.NotAnInteger(key, "abc");

        public string GetConnectionString(string name) => throw CooperatorConfigurationException.NotAnInteger(name, "abc");
    }

    /// <summary>Doble de caché que sólo registra el uso del contrato.</summary>
    private sealed class RecordingProvider : ICacheProvider
    {
        public bool TryGet<T>(string key, [MaybeNullWhen(false)] out T value)
        {
            value = default!;
            return false;
        }

        public void Set<T>(string key, [AllowNull] T value, TimeSpan ttl)
        {
        }

        public T GetOrCreate<T>(string key, Func<T> factory, TimeSpan ttl) => factory();

        public void Remove(string key)
        {
        }
    }
}

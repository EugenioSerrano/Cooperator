using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Cooperator.Framework.Core.Configuration;
using Cooperator.Framework.Data;
using Cooperator.Framework.Data.Caching;
using Microsoft.Data.SqlClient;

namespace Cooperator.Framework.Data.Tests;

/// <summary>
/// AC-1, AC-2, AC-6 y AC-7 del SPEC-261016-0019 §7 sobre los <b>puntos de caché de la capa</b>: los cuatro pasan
/// por el <b>contrato</b> (no por una implementación concreta), con las claves y los TTL publicados; la
/// invalidación es la del consumidor que conoce sus claves; y no queda ningún punto condicionado al host.
/// <para>
/// Los dos puntos estáticos (timeout y metadata de parámetros) se ejercen con un <b>doble de
/// <see cref="ICacheProvider"/></b>: si el punto no usara el contrato, el valor entregado no sería el del doble.
/// Los puntos de colecciones viven en las clases genéricas de la capa (necesitan una conexión real para su
/// productor), así que su uso del contrato —clave, TTL y alcance de la invalidación— se verifica sobre el
/// <b>código</b> de los dos archivos.
/// </para>
/// </summary>
[Collection("DataCaching")]
public sealed class CachePointTests
{
    private static readonly string[] LayerFiles =
    [
        "BaseGateway.cs", "BaseLoader.cs", "SQLHelper.cs", "SqlHelperParameterCache.cs", "BaseRule.cs", "IBaseGateway.cs",
    ];

    private static readonly string[] CacheFiles =
    [
        "ICacheProvider.cs", "CacheKey.cs", "CacheRegions.cs", "InMemoryCacheProvider.cs",
    ];

    private static readonly string HostCacheToken = "Http" + "Context";
    private static readonly string WebPlatformToken = "System." + "Web";

    [Fact]
    public void The_published_key_scheme_is_the_one_the_layer_uses()
    {
        // Esquema de ADR-004 §3.1 y las claves del inventario de §6/B.1, publicadas en el contrato.
        Assert.Equal("coop:data:Ventas:collection-all", CacheKey.CollectionAll("Ventas"));
        Assert.Equal("coop:data:Ventas:collection-dictionary", CacheKey.CollectionDictionary("Ventas"));
        Assert.Equal("coop:data:sp-parameters:Ventas_GetAll:metadata", CacheKey.StoredProcedureParameters("Ventas_GetAll"));
        Assert.Equal("coop:data:command-timeout:value", CacheKey.CommandTimeout);
        Assert.Equal("coop", CacheKey.Scheme);
    }

    [Fact]
    public void The_contract_has_the_minimal_surface_and_no_prefix_invalidation()
    {
        string[] members = typeof(ICacheProvider)
            .GetMethods()
            .Select(method => method.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(new[] { "GetOrCreate", "Remove", "Set", "TryGet" }, members);
        Assert.DoesNotContain(typeof(ICacheProvider).GetMethods(), method => method.Name.Contains("Prefix", StringComparison.OrdinalIgnoreCase));
        Assert.Single(typeof(ICacheProvider).GetMethods(), method => method.Name == "Remove");
    }

    [Fact]
    public void The_command_timeout_point_uses_the_contract_with_its_region_ttl()
    {
        ICooperatorConfiguration previous = CooperatorSettings.Current;
        ICacheProvider previousProvider = CacheRegions.Provider;
        try
        {
            CooperatorSettings.Current = new StubConfiguration(new Dictionary<string, int>(StringComparer.Ordinal));
            RecordingCacheProvider doubleProvider = new RecordingCacheProvider();
            doubleProvider.Seed(CacheKey.CommandTimeout, 77);
            CacheRegions.Provider = doubleProvider;

            Assert.Equal(77, ReadCommandTimeout());

            (string Key, TimeSpan Ttl) call = Assert.Single(doubleProvider.Calls);
            Assert.Equal(CacheKey.CommandTimeout, call.Key);
            Assert.Equal(CacheRegions.CommandTimeoutTtl, call.Ttl);

            // El contrato entregó el valor: la lectura del catálogo (la fábrica) no corrió.
            Assert.Equal(0, doubleProvider.FactoryInvocations);
        }
        finally
        {
            CacheRegions.Provider = previousProvider;
            CooperatorSettings.Current = previous;
        }
    }

    [Fact]
    public void The_stored_procedure_metadata_point_uses_the_contract_with_its_key_and_ttl()
    {
        ICooperatorConfiguration previous = CooperatorSettings.Current;
        ICacheProvider previousProvider = CacheRegions.Provider;
        try
        {
            CooperatorSettings.Current = new StubConfiguration(new Dictionary<string, int>(StringComparer.Ordinal));
            RecordingCacheProvider doubleProvider = new RecordingCacheProvider();
            SqlParameter[] canned = [new SqlParameter("@VentasId", SqlDbType.Int)];
            doubleProvider.Seed(CacheKey.StoredProcedureParameters("Ventas_GetAll", false), canned);
            CacheRegions.Provider = doubleProvider;

            SqlParameter[] returned = Discover("Ventas_GetAll", false);

            Assert.Single(returned);
            Assert.Equal("@VentasId", returned[0].ParameterName);

            // Lo entregado es un clon del valor del contrato: el llamador no puede corromper lo cacheado.
            Assert.NotSame(canned[0], returned[0]);

            (string Key, TimeSpan Ttl) call = Assert.Single(doubleProvider.Calls);
            Assert.Equal(CacheKey.StoredProcedureParameters("Ventas_GetAll"), call.Key);
            Assert.Equal(CacheRegions.ParameterMetadataTtl, call.Ttl);
            Assert.Equal(0, doubleProvider.FactoryInvocations);
        }
        finally
        {
            CacheRegions.Provider = previousProvider;
            CooperatorSettings.Current = previous;
        }
    }

    [Fact]
    public void The_metadata_key_varies_with_the_return_value_flag()
    {
        ICooperatorConfiguration previous = CooperatorSettings.Current;
        ICacheProvider previousProvider = CacheRegions.Provider;
        try
        {
            CooperatorSettings.Current = new StubConfiguration(new Dictionary<string, int>(StringComparer.Ordinal));
            RecordingCacheProvider doubleProvider = new RecordingCacheProvider();
            SqlParameter[] canned = [new SqlParameter("@retorno", SqlDbType.Int)];
            doubleProvider.Seed(CacheKey.StoredProcedureParameters("Ventas_GetAll", true), canned);
            CacheRegions.Provider = doubleProvider;

            // La forma del valor cambia con el flag: no puede compartir clave con el caso común.
            SqlParameter[] returned = Discover("Ventas_GetAll", true);

            Assert.Single(returned);
            (string Key, TimeSpan Ttl) call = Assert.Single(doubleProvider.Calls);
            Assert.Equal(CacheKey.StoredProcedureParameters("Ventas_GetAll", true), call.Key);
            Assert.Equal("coop:data:sp-parameters:Ventas_GetAll:metadata-with-return-value", call.Key);

            // Y la clave del caso común NO es la que se usó.
            Assert.NotEqual(CacheKey.StoredProcedureParameters("Ventas_GetAll"), call.Key);
        }
        finally
        {
            CacheRegions.Provider = previousProvider;
            CooperatorSettings.Current = previous;
        }
    }

    [Fact]
    public void The_collection_points_use_the_contract_with_their_published_key_and_ttl()
    {
        foreach (string file in new[] { "BaseGateway.cs", "BaseLoader.cs" })
        {
            string source = Source(file);

            Assert.Contains("protected virtual ICacheProvider CacheProvider => CacheRegions.Provider;", source, StringComparison.Ordinal);
            Assert.Contains("CacheKey.CollectionAll(TableName)", source, StringComparison.Ordinal);
            Assert.Contains("CacheProvider.GetOrCreate(", source, StringComparison.Ordinal);
            Assert.Contains("CacheRegions.CollectionsTtl", source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void ResetCache_removes_only_its_own_keys()
    {
        foreach (string file in new[] { "BaseGateway.cs", "BaseLoader.cs" })
        {
            string source = Source(file);

            Assert.Contains("CacheProvider.Remove(CacheKey.CollectionAll(TableName));", source, StringComparison.Ordinal);
            Assert.Contains("CacheProvider.Remove(CacheKey.CollectionDictionary(TableName));", source, StringComparison.Ordinal);

            // Exactamente sus dos claves: no hay barrido, ni prefijos, ni claves ajenas.
            Assert.Equal(2, Regex.Matches(source, @"CacheProvider\.Remove\(").Count);
            Assert.DoesNotContain("RemoveByPrefix", source, StringComparison.Ordinal);
            Assert.DoesNotContain("Clear()", source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void No_cache_point_is_conditioned_on_the_host_and_no_web_platform_remains()
    {
        foreach (string file in LayerFiles)
        {
            string source = Source(file);
            Assert.DoesNotContain(HostCacheToken, source, StringComparison.Ordinal);
            Assert.DoesNotContain(WebPlatformToken, source, StringComparison.Ordinal);
            Assert.DoesNotContain("Coop_DAL_", source, StringComparison.Ordinal);
            Assert.DoesNotContain("Cooperator_DAL_", source, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void The_new_cache_code_keeps_the_nullability_active()
    {
        // AC-9: la directiva de §B.2 es SÓLO para archivos aportados del 1.4 con diagnósticos; el código nuevo
        // de la caché conserva la nulabilidad activa, y ningún .csproj la apaga a nivel de proyecto.
        foreach (string file in CacheFiles)
        {
            Assert.DoesNotContain("#nullable disable", CachingSource(file), StringComparison.Ordinal);
        }

        foreach (string project in Directory.GetFiles(TreePath("src"), "*.csproj", SearchOption.AllDirectories))
        {
            Assert.DoesNotContain("<Nullable>disable</Nullable>", File.ReadAllText(project), StringComparison.Ordinal);
        }
    }

    /// <summary>El punto de caché del timeout (privado: es implementación, no superficie).</summary>
    private static int ReadCommandTimeout()
    {
        Type helper = typeof(BaseRule).Assembly.GetType("Cooperator.Framework.Data.SqlHelper", throwOnError: true)!;
        MethodInfo method = helper.GetMethod("GetCommnandTimeOut", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (int)method.Invoke(null, null)!;
    }

    /// <summary>El punto de caché de la metadata de parámetros (interno: es implementación, no superficie).</summary>
    private static SqlParameter[] Discover(string storedProcedure, bool includeReturnValueParameter)
    {
        Type cache = typeof(BaseRule).Assembly.GetType("Cooperator.Framework.Data.SqlHelperParameterCache", throwOnError: true)!;
        MethodInfo method = cache.GetMethod(
            "GetSpParameterSetInternal",
            BindingFlags.NonPublic | BindingFlags.Static,
            binder: null,
            [typeof(SqlConnection), typeof(string), typeof(bool)],
            modifiers: null)!;

        // Una conexión SIN cadena: el doble del contrato entrega el valor, así que la conexión no se usa ni se abre.
        return (SqlParameter[])method.Invoke(null, [new SqlConnection(string.Empty), storedProcedure, includeReturnValueParameter])!;
    }

    private static string Source(string file) => File.ReadAllText(TreePath("src", "Cooperator.Framework.Data", file));

    /// <summary>Un archivo del namespace de cache nuevo de este Bolt (Caching/).</summary>
    private static string CachingSource(string file) => File.ReadAllText(TreePath("src", "Cooperator.Framework.Data", "Caching", file));

    private static string TreePath(params string[] parts) => Path.Combine([RepositoryRoot(), "2.0", .. parts]);

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

    /// <summary>Doble de <see cref="ICacheProvider"/>: registra el uso del contrato y entrega lo sembrado.</summary>
    private sealed class RecordingCacheProvider : ICacheProvider
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>(StringComparer.Ordinal);

        internal List<(string Key, TimeSpan Ttl)> Calls { get; } = [];

        internal int FactoryInvocations { get; private set; }

        internal void Seed(string key, object value) => _values[key] = value;

        public bool TryGet<T>(string key, [MaybeNullWhen(false)] out T value)
        {
            if (_values.TryGetValue(key, out object? stored) && stored is T typed)
            {
                value = typed;
                return true;
            }

            value = default!;
            return false;
        }

        public void Set<T>(string key, [AllowNull] T value, TimeSpan ttl)
        {
        }

        public T GetOrCreate<T>(string key, Func<T> factory, TimeSpan ttl)
        {
            Calls.Add((key, ttl));

            if (_values.TryGetValue(key, out object? stored) && stored is T typed)
            {
                return typed;
            }

            FactoryInvocations++;
            return factory();
        }

        public void Remove(string key) => _values.Remove(key);
    }

    /// <summary>Doble del contrato de configuración (Bolt 011): entrega valores o el default publicado.</summary>
    private sealed class StubConfiguration : ICooperatorConfiguration
    {
        private readonly Dictionary<string, int> _values;

        internal StubConfiguration(Dictionary<string, int> values) => _values = values;

        public string GetString(string key) => throw new NotSupportedException();

        public string GetString(string key, string defaultValue) => defaultValue;

        public int GetInt(string key) => throw CooperatorConfigurationException.MissingKey(key);

        public int GetInt(string key, int defaultValue) => _values.TryGetValue(key, out int value) ? value : defaultValue;

        public string GetConnectionString(string name) => throw new NotSupportedException();
    }
}

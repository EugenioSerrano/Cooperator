using System;
using Cooperator.Framework.Core.Configuration;

namespace Cooperator.Framework.Data.Caching
{
    /// <summary>
    /// Las <b>regiones</b> de caché de la capa de datos (SPEC-261016-0019 §6/A.1 y §6/D.1): su nombre, el TTL que
    /// las gobierna y la clave del <b>catálogo de configuración</b> de la que se lee —ningún default vive acá—.
    /// <para>
    /// Los valores se leen del contrato de configuración (Bolt 011) en cada uso: un valor <b>inválido</b> no habilita
    /// un camino propio de fallback (se propaga el error tipado del contrato — NFR <i>Config fail-fast</i>,
    /// ADR-004 §5) y un valor <b>ausente</b> usa el default <b>publicado</b> del catálogo
    /// (<see cref="ConfigurationKeys.Defaults"/>).
    /// </para>
    /// </summary>
    public static class CacheRegions
    {
        /// <summary>Región de las colecciones (<c>GetAll</c> de reglas y loaders).</summary>
        public const string Collections = "collections";

        /// <summary>Región de la metadata de parámetros de stored procedures.</summary>
        public const string ParameterMetadata = "parameter-metadata";

        /// <summary>Región del valor del timeout de comandos.</summary>
        public const string CommandTimeout = "command-timeout";

        private static readonly object ProviderGate = new object();
        private static ICacheProvider? _provider;

        /// <summary>
        /// El proveedor de caché del proceso: por defecto <see cref="InMemoryCacheProvider"/> construido con el cap
        /// del catálogo (ADR-004 §3.1 — default en memoria para <b>todos</b> los hosts). Un host con contenedor
        /// propio, o una prueba con un doble, lo reemplaza por su instancia.
        /// </summary>
        public static ICacheProvider Provider
        {
            get
            {
                lock (ProviderGate)
                {
                    return _provider ??= new InMemoryCacheProvider(MaxEntries);
                }
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                lock (ProviderGate)
                {
                    _provider = value;
                }
            }
        }

        /// <summary>TTL de la región de colecciones: <c>Cooperator:Cache:DefaultTtlSeconds</c> (1 h en 1.4).</summary>
        public static TimeSpan CollectionsTtl =>
            FromCatalog(ConfigurationKeys.Cache.DefaultTtlSeconds, ConfigurationKeys.Defaults.CacheDefaultTtlSeconds);

        /// <summary>
        /// TTL de la región de metadata de parámetros: <c>Cooperator:Cache:ParameterMetadataTtlSeconds</c>
        /// (1 día en 1.4).
        /// </summary>
        public static TimeSpan ParameterMetadataTtl =>
            FromCatalog(
                ConfigurationKeys.Cache.ParameterMetadataTtlSeconds,
                ConfigurationKeys.Defaults.CacheParameterMetadataTtlSeconds);

        /// <summary>
        /// TTL de la región del timeout. La región <b>no</b> tiene default propio: ADR-004 §3.1 enumera los defaults
        /// por región de colecciones y de metadata, y sólo ésos; en vez de estrenar una tercera clave con un tercer
        /// default, el punto reusa el TTL genérico <b>publicado</b> de la caché.
        /// </summary>
        public static TimeSpan CommandTimeoutTtl => CollectionsTtl;

        /// <summary>El cap de entradas del caché en memoria: <c>Cooperator:Cache:MaxEntries</c>.</summary>
        public static int MaxEntries =>
            CooperatorSettings.Current.GetInt(ConfigurationKeys.Cache.MaxEntries, ConfigurationKeys.Defaults.CacheMaxEntries);

        /// <summary>Construye el proveedor por defecto (el del proceso) con el cap del catálogo.</summary>
        /// <returns>Un proveedor en memoria acotado.</returns>
        public static ICacheProvider CreateDefaultProvider() => new InMemoryCacheProvider(MaxEntries);

        private static TimeSpan FromCatalog(string key, int defaultSeconds) =>
            TimeSpan.FromSeconds(CooperatorSettings.Current.GetInt(key, defaultSeconds));
    }
}

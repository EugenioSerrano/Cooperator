using System;
using System.Diagnostics.CodeAnalysis;

namespace Cooperator.Framework.Data.Caching
{
    /// <summary>
    /// El <b>contrato de caché</b> de la capa de datos (ADR-004 §3.1): superficie <b>mínima</b> —leer, escribir,
    /// crear-si-falta y remover— con <b>claves del llamador</b> y <b>TTL por entrada</b>.
    /// <para>
    /// El contrato <b>no interpreta</b> las claves: las recibe del llamador bajo el esquema documentado
    /// <see cref="CacheKey.Scheme"/> (<c>coop:{scope}:{rule}:{kind}</c>) y no ofrece invalidación por prefijo
    /// (quien invalida conoce sus claves: <see cref="Remove"/> y nada más).
    /// </para>
    /// <para>
    /// La implementación por defecto es <see cref="InMemoryCacheProvider"/>, BCL puro, para <b>todos</b> los hosts.
    /// </para>
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Intenta leer una entrada viva. Una entrada <b>vencida nunca se sirve</b>: se descarta y la lectura falla.
        /// </summary>
        /// <typeparam name="T">Tipo del valor guardado.</typeparam>
        /// <param name="key">Clave de la entrada (esquema de <see cref="CacheKey"/>).</param>
        /// <param name="value">El valor guardado cuando la lectura acierta; <c>default</c> cuando falla.</param>
        /// <returns><c>true</c> si había una entrada viva con esa clave y ese tipo.</returns>
        bool TryGet<T>(string key, [MaybeNullWhen(false)] out T value);

        /// <summary>
        /// Guarda un valor con su vencimiento. Un valor <c>null</c> y un TTL no positivo <b>no se cachean</b>
        /// (el contrato no distingue "ausente" de "null", y una entrada sin vencimiento sería un crecimiento sin cota).
        /// </summary>
        /// <typeparam name="T">Tipo del valor.</typeparam>
        /// <param name="key">Clave de la entrada.</param>
        /// <param name="value">Valor a guardar.</param>
        /// <param name="ttl">Vida de la entrada desde este instante.</param>
        void Set<T>(string key, [AllowNull] T value, TimeSpan ttl);

        /// <summary>
        /// Devuelve el valor vivo de la clave o lo crea con <paramref name="factory"/> y lo guarda.
        /// Si la fábrica <b>falla</b>, la excepción se propaga y <b>no</b> se cachea el fallo.
        /// </summary>
        /// <typeparam name="T">Tipo del valor.</typeparam>
        /// <param name="key">Clave de la entrada.</param>
        /// <param name="factory">Productor del valor cuando no hay entrada viva.</param>
        /// <param name="ttl">Vida de la entrada creada.</param>
        /// <returns>El valor vivo, el creado, o el que la fábrica devuelva (incluso <c>null</c>).</returns>
        T GetOrCreate<T>(string key, Func<T> factory, TimeSpan ttl);

        /// <summary>Remueve la entrada de esa clave (y sólo esa). No existe invalidación por prefijo.</summary>
        /// <param name="key">Clave de la entrada.</param>
        void Remove(string key);
    }
}

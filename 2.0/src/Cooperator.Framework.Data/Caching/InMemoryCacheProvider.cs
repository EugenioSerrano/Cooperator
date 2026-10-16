using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Cooperator.Framework.Data.Caching
{
    /// <summary>
    /// La implementación por defecto del contrato de caché (ADR-004 §3.1): <b>BCL puro</b> —sin dependencias de
    /// terceros—, <b>thread-safe</b>, con <b>TTL por entrada</b> y <b>expiración perezosa</b> (una entrada vencida
    /// se descarta al leerla y nunca se sirve) y un <b>cap de entradas con desalojo</b> que empieza por las vencidas
    /// y, si no alcanza, por la de vencimiento más cercano.
    /// <para>
    /// El cap se recibe por constructor —no vive acá: su valor es del catálogo de configuración
    /// (<see cref="CacheRegions.MaxEntries"/>)— y el <b>reloj es sustituible</b> para poder probar el vencimiento y
    /// el desalojo sin esperas reales.
    /// </para>
    /// <para>
    /// <b>Invariante del límite (NFR <i>Cache memory bound</i>, ADR-004 §5):</b> la escritura hace lugar
    /// <b>antes</b> de insertar, dentro de la misma sección crítica, de modo que el total almacenado <b>no supera
    /// el cap en ningún instante</b>, tampoco durante una escritura concurrente.
    /// </para>
    /// <para>
    /// La escritura de una clave nueva toma un lock corto (para hacer lugar e insertar); la lectura es sin lock
    /// sobre una estructura concurrente, y por eso una lectura concurrente con una escritura ve el estado anterior
    /// o el posterior, nunca un estado intermedio.
    /// </para>
    /// </summary>
    public sealed class InMemoryCacheProvider : ICacheProvider
    {
        private readonly ConcurrentDictionary<string, Entry> _entries = new ConcurrentDictionary<string, Entry>(StringComparer.Ordinal);
        private readonly Func<DateTimeOffset> _clock;
        private readonly object _writeGate = new object();
        private long _evictedExpired;
        private long _evictedLive;
        private long _refusedExpired;

        /// <summary>Crea el caché con su cap y el reloj del sistema.</summary>
        /// <param name="maxEntries">Cap de entradas (al menos 1). Lo entrega el catálogo, no esta clase.</param>
        public InMemoryCacheProvider(int maxEntries)
            : this(maxEntries, null)
        {
        }

        /// <summary>Crea el caché con su cap y un reloj <b>sustituible</b> (el del sistema por defecto).</summary>
        /// <param name="maxEntries">Cap de entradas (al menos 1).</param>
        /// <param name="clock">Reloj de la expiración; <c>null</c> usa el del sistema.</param>
        public InMemoryCacheProvider(int maxEntries, Func<DateTimeOffset>? clock)
        {
            if (maxEntries < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxEntries), maxEntries, "El cap de entradas es al menos 1.");
            }

            MaxEntries = maxEntries;
            _clock = clock ?? (() => DateTimeOffset.UtcNow);
        }

        /// <summary>El cap de entradas de esta instancia.</summary>
        public int MaxEntries { get; }

        /// <summary>El total de entradas almacenadas en este instante (siempre <c>≤ MaxEntries</c>).</summary>
        public int Count => _entries.Count;

        /// <summary>Entradas vencidas desalojadas para hacer lugar.</summary>
        public long EvictedExpiredEntries => Interlocked.Read(ref _evictedExpired);

        /// <summary>Entradas vivas desalojadas (las de vencimiento más cercano) cuando no había vencidas.</summary>
        public long EvictedLiveEntries => Interlocked.Read(ref _evictedLive);

        /// <summary>Lecturas que encontraron una entrada vencida y la <b>rechazaron</b> (servidas: cero).</summary>
        public long RefusedExpiredEntries => Interlocked.Read(ref _refusedExpired);

        /// <inheritdoc />
        public bool TryGet<T>(string key, [MaybeNullWhen(false)] out T value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_entries.TryGetValue(key, out Entry? entry))
            {
                if (entry.ExpiresAt <= _clock())
                {
                    // Expiración perezosa: la vencida NO se sirve y se descarta al leerla.
                    _entries.TryRemove(key, out _);
                    Interlocked.Increment(ref _refusedExpired);
                    value = default!;
                    return false;
                }

                if (entry.Value is T typed)
                {
                    value = typed;
                    return true;
                }

                value = default!;
                return false;
            }

            value = default!;
            return false;
        }

        /// <inheritdoc />
        public void Set<T>(string key, [AllowNull] T value, TimeSpan ttl)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value is null || ttl <= TimeSpan.Zero)
            {
                // "null" y un vencimiento no positivo no se cachean: una entrada sin vencimiento sería crecimiento
                // sin cota, y el contrato no distingue "ausente" de "null".
                return;
            }

            Entry candidate = new Entry(value!, _clock() + ttl);

            lock (_writeGate)
            {
                if (!_entries.ContainsKey(key))
                {
                    // Se hace lugar ANTES de insertar: el total no supera el cap en ningún instante.
                    MakeRoom();
                }

                _entries[key] = candidate;
            }
        }

        /// <inheritdoc />
        public T GetOrCreate<T>(string key, Func<T> factory, TimeSpan ttl)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            // El atributo del contrato dice que el valor puede ser null SÓLO cuando la lectura falla.
            if (TryGet(key, out T? live))
            {
                return live!;
            }

            // Si la fábrica falla, la excepción se propaga y el fallo NO se cachea.
            T created = factory();
            Set(key, created, ttl);
            return created;
        }

        /// <inheritdoc />
        public void Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _entries.TryRemove(key, out _);
        }

        /// <summary>Desaloja hasta que haya lugar, empezando por las vencidas. Se llama con <c>_writeGate</c> tomado.</summary>
        private void MakeRoom()
        {
            while (_entries.Count >= MaxEntries)
            {
                DateTimeOffset now = _clock();
                string? victim = null;
                DateTimeOffset victimExpiry = DateTimeOffset.MaxValue;
                bool victimExpired = false;

                foreach (KeyValuePair<string, Entry> candidate in _entries)
                {
                    bool expired = candidate.Value.ExpiresAt <= now;

                    // Primero las vencidas; entre iguales, la de vencimiento más cercano.
                    if (victim == null
                        || (expired && !victimExpired)
                        || (expired == victimExpired && candidate.Value.ExpiresAt < victimExpiry))
                    {
                        victim = candidate.Key;
                        victimExpiry = candidate.Value.ExpiresAt;
                        victimExpired = expired;
                    }
                }

                if (victim == null || !_entries.TryRemove(victim, out _))
                {
                    return;
                }

                if (victimExpired)
                {
                    Interlocked.Increment(ref _evictedExpired);
                }
                else
                {
                    Interlocked.Increment(ref _evictedLive);
                }
            }
        }

        /// <summary>Una entrada guardada: el valor y su instante de vencimiento.</summary>
        private sealed class Entry
        {
            internal Entry(object value, DateTimeOffset expiresAt)
            {
                Value = value;
                ExpiresAt = expiresAt;
            }

            internal object Value { get; }

            internal DateTimeOffset ExpiresAt { get; }
        }
    }
}

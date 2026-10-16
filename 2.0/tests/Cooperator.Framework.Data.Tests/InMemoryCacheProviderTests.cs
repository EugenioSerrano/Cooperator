using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cooperator.Framework.Data.Caching;

namespace Cooperator.Framework.Data.Tests;

/// <summary>
/// AC-3, AC-5 y AC-6 del SPEC-261016-0019 §7 sobre la implementación por defecto: TTL por entrada con expiración
/// perezosa (una vencida <b>nunca</b> se sirve), cap con desalojo (primero las vencidas) que <b>no se supera en
/// ningún instante</b>, <c>GetOrCreate</c> que no recrea lo vivo ni cachea el fallo, y <c>Remove</c> acotado a su
/// clave. El vencimiento y el desalojo se prueban con un <b>reloj sustituible</b>: sin esperas reales.
/// </summary>
[Collection("DataCaching")]
public sealed class InMemoryCacheProviderTests
{
    private const string Key = "coop:data:Ventas:collection-all";
    private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);

    [Fact]
    public void A_stored_value_is_read_back_while_it_is_alive()
    {
        InMemoryCacheProvider cache = new InMemoryCacheProvider(8);

        cache.Set(Key, new List<int> { 1, 2 }, OneMinute);

        Assert.True(cache.TryGet(Key, out List<int>? value));
        Assert.Equal(new[] { 1, 2 }, value);
        Assert.Equal(1, cache.Count);
    }

    [Fact]
    public void A_missing_key_is_a_failure()
    {
        InMemoryCacheProvider cache = new InMemoryCacheProvider(8);

        Assert.False(cache.TryGet(Key, out string? value));
        Assert.Null(value);
    }

    [Fact]
    public void A_value_of_another_type_is_not_a_hit()
    {
        InMemoryCacheProvider cache = new InMemoryCacheProvider(8);
        cache.Set(Key, "texto", OneMinute);

        Assert.False(cache.TryGet(Key, out int _));
    }

    [Fact]
    public void An_expired_entry_is_never_served_and_is_discarded_on_read()
    {
        TestClock clock = new TestClock();
        InMemoryCacheProvider cache = new InMemoryCacheProvider(8, clock.Read);
        cache.Set(Key, 42, TimeSpan.FromSeconds(30));

        clock.Advance(TimeSpan.FromSeconds(31));

        // La vencida no se sirve…
        Assert.False(cache.TryGet(Key, out int _));
        // …y se descartó al leerla (expiración perezosa).
        Assert.Equal(0, cache.Count);
        Assert.Equal(1, cache.RefusedExpiredEntries);
    }

    [Fact]
    public void The_ttl_is_per_entry_and_not_of_the_region()
    {
        TestClock clock = new TestClock();
        InMemoryCacheProvider cache = new InMemoryCacheProvider(8, clock.Read);
        cache.Set("corta", "vive 10 s", TimeSpan.FromSeconds(10));
        cache.Set("larga", "vive 60 s", TimeSpan.FromSeconds(60));

        clock.Advance(TimeSpan.FromSeconds(20));

        Assert.False(cache.TryGet("corta", out string? _));
        Assert.True(cache.TryGet("larga", out string? alive));
        Assert.Equal("vive 60 s", alive);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void A_ttl_that_is_not_positive_does_not_cache(int seconds)
    {
        InMemoryCacheProvider cache = new InMemoryCacheProvider(8);

        cache.Set(Key, "no se guarda", TimeSpan.FromSeconds(seconds));

        Assert.Equal(0, cache.Count);
        Assert.False(cache.TryGet(Key, out string? _));
    }

    [Fact]
    public void A_null_value_is_not_cached()
    {
        InMemoryCacheProvider cache = new InMemoryCacheProvider(8);

        cache.Set<string?>(Key, null, OneMinute);

        Assert.Equal(0, cache.Count);
        Assert.False(cache.TryGet(Key, out string? _));
    }

    [Fact]
    public void The_cap_is_never_exceeded_and_eviction_starts_with_the_expired()
    {
        TestClock clock = new TestClock();
        InMemoryCacheProvider cache = new InMemoryCacheProvider(3, clock.Read);
        cache.Set("a", 1, TimeSpan.FromSeconds(10));
        cache.Set("b", 2, TimeSpan.FromMinutes(30));
        cache.Set("c", 3, TimeSpan.FromMinutes(31));

        clock.Advance(TimeSpan.FromSeconds(20));     // "a" venció

        cache.Set("d", 4, TimeSpan.FromMinutes(32));

        Assert.True(cache.Count <= 3);
        Assert.Equal(1, cache.EvictedExpiredEntries);
        Assert.Equal(0, cache.EvictedLiveEntries);
        Assert.False(cache.TryGet("a", out int _));  // la vencida fue la primera en caer
        Assert.True(cache.TryGet("b", out int _));
        Assert.True(cache.TryGet("c", out int _));
        Assert.True(cache.TryGet("d", out int four));
        Assert.Equal(4, four);
    }

    [Fact]
    public void With_no_expired_entry_the_nearest_to_expire_is_evicted()
    {
        TestClock clock = new TestClock();
        InMemoryCacheProvider cache = new InMemoryCacheProvider(2, clock.Read);
        cache.Set("cercana", 1, TimeSpan.FromMinutes(10));
        cache.Set("lejana", 2, TimeSpan.FromMinutes(20));

        cache.Set("nueva", 3, TimeSpan.FromMinutes(30));

        Assert.Equal(2, cache.Count);
        Assert.Equal(1, cache.EvictedLiveEntries);
        Assert.False(cache.TryGet("cercana", out int _));
        Assert.True(cache.TryGet("lejana", out int _));
        Assert.True(cache.TryGet("nueva", out int _));
    }

    [Fact]
    public void Replacing_an_existing_key_does_not_evict()
    {
        InMemoryCacheProvider cache = new InMemoryCacheProvider(2);
        cache.Set("a", 1, OneMinute);
        cache.Set("b", 2, OneMinute);

        cache.Set("a", 11, TimeSpan.FromMinutes(5));

        Assert.Equal(2, cache.Count);
        Assert.Equal(0, cache.EvictedLiveEntries);
        Assert.True(cache.TryGet("a", out int replaced));
        Assert.Equal(11, replaced);
        Assert.True(cache.TryGet("b", out int _));
    }

    [Fact]
    public void The_cap_of_one_holds_and_a_huge_cap_never_evicts()
    {
        InMemoryCacheProvider small = new InMemoryCacheProvider(1);
        for (int i = 0; i < 5; i++)
        {
            small.Set("clave-" + i, i, OneMinute);
            Assert.True(small.Count <= 1);
        }

        Assert.Equal(1, small.Count);

        InMemoryCacheProvider huge = new InMemoryCacheProvider(100000);
        for (int i = 0; i < 200; i++)
        {
            huge.Set("clave-" + i, i, OneMinute);
        }

        Assert.Equal(200, huge.Count);
        Assert.Equal(0, huge.EvictedLiveEntries);
        Assert.Equal(0, huge.EvictedExpiredEntries);
    }

    [Fact]
    public void The_constructor_rejects_a_cap_below_one()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new InMemoryCacheProvider(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new InMemoryCacheProvider(-1));
    }

    [Fact]
    public void GetOrCreate_creates_once_and_does_not_recreate_a_live_value()
    {
        InMemoryCacheProvider cache = new InMemoryCacheProvider(4);
        int calls = 0;

        int first = cache.GetOrCreate(Key, () => { calls++; return 7; }, OneMinute);
        int second = cache.GetOrCreate(Key, () => { calls++; return 8; }, OneMinute);

        Assert.Equal(7, first);
        Assert.Equal(7, second);
        Assert.Equal(1, calls);
    }

    [Fact]
    public void GetOrCreate_recreates_when_the_value_expired()
    {
        TestClock clock = new TestClock();
        InMemoryCacheProvider cache = new InMemoryCacheProvider(4, clock.Read);
        int calls = 0;

        cache.GetOrCreate(Key, () => { calls++; return calls; }, TimeSpan.FromSeconds(10));
        clock.Advance(TimeSpan.FromSeconds(11));
        int renewed = cache.GetOrCreate(Key, () => { calls++; return calls; }, TimeSpan.FromSeconds(10));

        Assert.Equal(2, calls);
        Assert.Equal(2, renewed);
    }

    [Fact]
    public void GetOrCreate_propagates_the_factory_failure_without_caching_it()
    {
        InMemoryCacheProvider cache = new InMemoryCacheProvider(4);
        int calls = 0;

        InvalidOperationException first = Assert.Throws<InvalidOperationException>(
            () => cache.GetOrCreate<int>(Key, () => { calls++; throw new InvalidOperationException("la fábrica falló"); }, OneMinute));
        Assert.Equal("la fábrica falló", first.Message);
        Assert.Equal(0, cache.Count);
        Assert.Equal(1, calls);

        // La segunda llamada vuelve a intentar: el fallo no quedó cacheado.
        Assert.Throws<InvalidOperationException>(
            () => cache.GetOrCreate<int>(Key, () => { calls++; throw new InvalidOperationException("la fábrica falló"); }, OneMinute));
        Assert.Equal(2, calls);
    }

    [Fact]
    public void Remove_removes_its_key_and_not_the_others()
    {
        InMemoryCacheProvider cache = new InMemoryCacheProvider(8);
        cache.Set(Key, 1, OneMinute);
        cache.Set("coop:data:Otra:collection-all", 2, OneMinute);
        cache.Set("coop:data:sp-parameters:Ventas_GetAll:metadata", 3, OneMinute);

        cache.Remove(Key);

        Assert.False(cache.TryGet(Key, out int _));
        Assert.True(cache.TryGet("coop:data:Otra:collection-all", out int other));
        Assert.Equal(2, other);
        Assert.True(cache.TryGet("coop:data:sp-parameters:Ventas_GetAll:metadata", out int metadata));
        Assert.Equal(3, metadata);
    }

    [Fact]
    public void Concurrent_writers_readers_and_evictions_hold_the_invariants()
    {
        const int Cap = 64;
        const int Writers = 8;
        const int WritesPerWriter = 150;

        // Corrida repetida: descarta un falso verde de una sola pasada afortunada.
        for (int round = 0; round < 5; round++)
        {
            InMemoryCacheProvider cache = new InMemoryCacheProvider(Cap);
            int errors = 0;

            Parallel.For(0, Writers, writer =>
            {
                for (int i = 0; i < WritesPerWriter; i++)
                {
                    string key = "coop:data:rule-" + ((writer * 37) + i) % (Cap * 3) + ":collection-all";
                    cache.Set(key, i, TimeSpan.FromMinutes(30));

                    // El invariante del límite se sostiene EN TODO INSTANTE, también con escritores concurrentes.
                    if (cache.Count > Cap)
                    {
                        Interlocked.Increment(ref errors);
                    }

                    cache.TryGet(key, out int _);
                }
            });

            Assert.Equal(0, errors);
            Assert.True(cache.Count <= Cap);
        }
    }

    /// <summary>Reloj de prueba: el TTL se ejercita avanzándolo, nunca durmiendo el proceso.</summary>
    private sealed class TestClock
    {
        private DateTimeOffset _now = new DateTimeOffset(2026, 10, 16, 0, 0, 0, TimeSpan.Zero);

        internal Func<DateTimeOffset> Read => () => _now;

        internal void Advance(TimeSpan by) => _now = _now.Add(by);
    }
}

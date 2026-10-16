using System.Collections.Concurrent;
using Cooperator.Framework.Core.LazyLoad;
using Cooperator.Framework.Core.LazyLoad.Exceptions;
using Cooperator.LazyFixtures.Declared;

namespace Cooperator.Harness.Tests;

/// <summary>
/// AC-3 del SPEC-261016-0015 §7: la caché es <b>thread-safe</b>, con <b>una instancia por
/// provider y por proceso</b>, con clave por <b>nombre calificado</b> (libre de la colisión
/// de la concatenación con puntos), con <b>reinicio explícito</b> y <b>sin envenenarse</b>
/// ante un fallo (ADR-006 §3.2).
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class ProviderCacheTests : LazyProviderTestBase
{
    private const int Threads = 16;

    private const int Iterations = 8;

    [Fact]
    public void One_instance_per_provider_and_per_process_under_concurrency()
    {
        SampleLazyProvider.ResetInstances();
        var name = new ProviderName(DeclaredAssembly, SampleProviderType);

        var resolved = new ConcurrentBag<object>();
        var failures = new ConcurrentBag<Exception>();

        RunConcurrently(Threads, () =>
        {
            for (var iteration = 0; iteration < Iterations; iteration++)
            {
                try
                {
                    resolved.Add(ProviderCache.Get(name));
                }
                catch (Exception exception)
                {
                    failures.Add(exception);
                }
            }
        });

        // Ni corrupción ni excepciones de concurrencia.
        Assert.Empty(failures);

        // UNA sola instancia construida en el proceso, y todas las lecturas la devuelven.
        Assert.Equal(1, SampleLazyProvider.Instances);
        Assert.All(resolved, instance => Assert.Same(resolved.First(), instance));
        Assert.True(ProviderCache.Contains(name));
        Assert.Equal(1, ProviderCache.Count);
    }

    [Fact]
    public void Concurrent_resolution_of_different_providers_keeps_one_instance_each()
    {
        SampleLazyProvider.ResetInstances();
        var first = new ProviderName(DeclaredAssembly, SampleProviderType);
        var second = new ProviderName(DeclaredAssembly, SecondProviderType);

        var resolved = new ConcurrentDictionary<string, object>(StringComparer.Ordinal);
        var failures = new ConcurrentBag<Exception>();

        RunConcurrently(Threads, () =>
        {
            var name = (Environment.CurrentManagedThreadId & 1) == 0 ? first : second;
            try
            {
                resolved[Thread.CurrentThread.Name ?? Guid.NewGuid().ToString("N")] = ProviderCache.Get(name);
            }
            catch (Exception exception)
            {
                failures.Add(exception);
            }
        });

        Assert.Empty(failures);
        Assert.Equal(1, SampleLazyProvider.Instances);

        // Cada destino tiene su propia instancia: dos claves distintas, dos entradas.
        Assert.True(ProviderCache.Contains(first));
        Assert.True(ProviderCache.Contains(second));
        Assert.Equal(2, ProviderCache.Count);
    }

    [Fact]
    public void A_failure_is_not_cached_and_a_fixed_configuration_resolves_on_the_next_attempt()
    {
        // Configuración que NO resuelve: el ensamblado no existe.
        Configure(("Customer", "No.Such.Cooperator.Assembly", "No.Such.Provider.Type"));
        var failingName = new ProviderName("No.Such.Cooperator.Assembly", "No.Such.Provider.Type");

        Assert.Throws<ProviderAssemblyNotFoundException>(() => LazyProviderFactory.GetForEntity("Customer"));

        // El fallo NO queda cacheado como "ausente": no hay entrada para ese destino.
        Assert.False(ProviderCache.Contains(failingName));
        Assert.Equal(0, ProviderCache.Count);

        // Configuración corregida en caliente: el siguiente intento resuelve (sin reinicio).
        Configure(("Customer", DeclaredAssembly, SampleProviderType));
        Assert.IsType<SampleLazyProvider>(LazyProviderFactory.GetForEntity("Customer"));
    }

    [Fact]
    public void The_explicit_reset_rebuilds_the_instance()
    {
        SampleLazyProvider.ResetInstances();
        Configure(("Customer", DeclaredAssembly, SampleProviderType));
        var name = new ProviderName(DeclaredAssembly, SampleProviderType);

        var first = LazyProviderFactory.GetForEntity("Customer");
        Assert.Equal(1, SampleLazyProvider.Instances);
        Assert.Same(first, ProviderCache.Get(name));

        // Reinicio explícito (pruebas y hosts que recargan providers): se vuelve a construir.
        ProviderCache.Reset();
        Assert.False(ProviderCache.Contains(name));

        var second = ProviderCache.Get(name);
        Assert.Equal(2, SampleLazyProvider.Instances);
        Assert.NotSame(first, second);
    }

    [Fact]
    public void Both_entry_points_share_the_same_instance_of_a_provider()
    {
        SampleLazyProvider.ResetInstances();
        Configure(("Customer", DeclaredAssembly, SampleProviderType));

        // El camino por entidad y el camino por nombre explícito son dos puertas a la MISMA
        // caché: una instancia por provider y por proceso, sin importar por dónde se pida.
        var byEntity = LazyProviderFactory.GetForEntity("Customer");
        var byExplicitName = LazyProviderFactory.Get(DeclaredAssembly, SampleProviderType);

        Assert.Same(byEntity, byExplicitName);
        Assert.Equal(1, SampleLazyProvider.Instances);
    }

    private static void RunConcurrently(int threads, Action body)
    {
        var ready = new Barrier(threads);

        var workers = Enumerable.Range(0, threads)
            .Select(index => new Thread(() =>
            {
                ready.SignalAndWait();
                body();
            })
            {
                IsBackground = true,
                Name = "provider-cache-" + index.ToString(System.Globalization.CultureInfo.InvariantCulture),
            })
            .ToArray();

        foreach (var worker in workers)
        {
            worker.Start();
        }

        foreach (var worker in workers)
        {
            Assert.True(worker.Join(TimeSpan.FromSeconds(30)), "Un hilo de la corrida concurrente no terminó.");
        }
    }
}

using Cooperator.Framework.Core.LazyLoad;
using Cooperator.Framework.Core.LazyLoad.Exceptions;
using Cooperator.LazyFixtures.Declared;

namespace Cooperator.Harness.Tests;

/// <summary>
/// AC-5 del SPEC-261016-0015 §7: el <b>hook del host</b> reemplaza la resolución por defecto
/// —la instancia la aporta el host— y, si el hook falla, el diagnóstico dice que <b>vino del
/// hook</b> y lleva la excepción original (ADR-006 §3.1/§3.3). El punto de fijación es único
/// y su comportamiento tardío es determinista y probado (SPEC §14).
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class ProviderHookTests : LazyProviderTestBase
{
    [Fact]
    public void The_hook_replaces_the_default_resolution_and_provides_the_instance()
    {
        var fromHost = new SampleLazyProvider();
        var hook = new StubHook(_ => fromHost);

        ProviderResolution.Register(hook);

        Assert.True(ProviderResolution.IsHookRegistered);
        Assert.Same(hook, ProviderResolution.AssemblyProvider);

        var provider = LazyProviderFactory.Get(DeclaredAssembly, SampleProviderType);

        Assert.Same(fromHost, provider);
        Assert.True(hook.WasCalled, "El hook registrado debe ser el que resuelve: la vía por defecto no se usa.");
        Assert.Equal("SampleLazyProvider", hook.LastName!.TypeName.Split('.')[^1]);
    }

    [Fact]
    public void A_failing_hook_is_reported_as_a_hook_failure_carrying_the_original_exception()
    {
        var hostFailure = new InvalidOperationException("el host no encontró su contenedor de servicios");
        ProviderResolution.Register(new StubHook(_ => throw hostFailure));

        var failure = Assert.Throws<ProviderResolutionFailedException>(
            () => LazyProviderFactory.Get(DeclaredAssembly, SampleProviderType));

        Assert.True(failure.FromHostHook);
        Assert.Same(hostFailure, failure.HostException);
        Assert.Same(hostFailure, failure.InnerException);
        Assert.Contains("hook", failure.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(DeclaredAssembly, failure.AssemblyName);
        Assert.Equal(SampleProviderType, failure.TypeName);
    }

    [Fact]
    public void A_hook_that_returns_null_is_a_failure_not_a_silent_absence()
    {
        ProviderResolution.Register(new StubHook(_ => null!));

        var failure = Assert.Throws<ProviderResolutionFailedException>(
            () => LazyProviderFactory.Get(DeclaredAssembly, SampleProviderType));

        Assert.True(failure.FromHostHook);
        Assert.IsType<InvalidOperationException>(failure.HostException);
    }

    [Fact]
    public void The_fixing_point_is_single_and_a_late_registration_takes_effect_for_the_next_resolution()
    {
        // Primera resolución por la VÍA POR DEFECTO: queda en la caché del proceso.
        var byDefault = LazyProviderFactory.Get(DeclaredAssembly, SampleProviderType);
        Assert.IsType<SampleLazyProvider>(byDefault);
        Assert.NotSame(byDefault, default);

        // Registro TARDÍO del hook: la estrategia de resolución cambió, así que la caché se
        // reinicia y la resolución siguiente ya la aporta el host (comportamiento elegido y
        // documentado, SPEC §14: no se deja indeterminado).
        var fromHost = new SampleLazyProvider();
        ProviderResolution.Register(new StubHook(_ => fromHost));
        Assert.Same(fromHost, LazyProviderFactory.Get(DeclaredAssembly, SampleProviderType));

        // Fijación ÚNICA: un segundo registro es un error de programa, no una segunda política.
        Assert.Throws<InvalidOperationException>(() => ProviderResolution.Register(new StubHook(_ => fromHost)));

        // Y Reset vuelve a la vía por defecto del host.
        ProviderResolution.Reset();
        Assert.False(ProviderResolution.IsHookRegistered);
        Assert.Null(ProviderResolution.AssemblyProvider);
        Assert.IsType<SampleLazyProvider>(LazyProviderFactory.Get(DeclaredAssembly, SampleProviderType));
    }

    private sealed class StubHook : IAssemblyProvider
    {
        private readonly Func<ProviderName, ILazyProvider> _factory;

        public StubHook(Func<ProviderName, ILazyProvider> factory) => _factory = factory;

        public bool WasCalled { get; private set; }

        public ProviderName? LastName { get; private set; }

        public ILazyProvider CreateProvider(ProviderName name)
        {
            WasCalled = true;
            LastName = name;
            return _factory(name);
        }
    }
}

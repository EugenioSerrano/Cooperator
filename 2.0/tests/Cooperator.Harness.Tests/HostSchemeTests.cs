using System;
using Cooperator.Framework.Core.Configuration;
using Cooperator.Framework.Library.Cryptography.ProtectedValues;

namespace Cooperator.Harness.Tests;

/// <summary>
/// AC-8 del SPEC-261016-0035 §7: los esquemas del <b>host</b> conviven sin que el framework los implemente —el host
/// registra su <b>resolvedor</b> (y su <b>proveedor de claves</b>) por el <b>mismo</b> punto de registro, y un fallo
/// de ese componente se reporta como error tipado <b>del host</b> con la excepción original (ADR-007 §3.1/§3.2)—.
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class HostSchemeTests
{
    [Fact]
    public void A_host_scheme_resolves_through_its_own_resolver_and_the_framework_does_not_implement_it()
    {
        ProtectedValueKeyRegistry keys = ProtectedValueTestSupport.Registry((ProtectedValueTestSupport.KeyOne, ProtectedValueTestSupport.NewKey()));
        ProtectedValueResolverRegistry registry = ProtectedValueTestSupport.FrameworkRegistry(keys);

        RecordingResolver host = new RecordingResolver("dpapi");
        registry.Register(host);

        string resolved = registry.Resolve("Cooperator:ConnectionStrings:Primary", "dpapi" + ":" + "una-carga");

        Assert.Equal("resuelto por el host: una-carga", resolved);
        Assert.Equal("Cooperator:ConnectionStrings:Primary", host.LastKey);
        Assert.Equal("una-carga", host.LastPayload);

        // El framework no implementa el esquema del host: sólo lo despacha al resolvedor que el host registró.
        Assert.Contains("dpapi", registry.Schemes);
        Assert.DoesNotContain("x509", registry.Schemes);
    }

    [Fact]
    public void A_failing_host_resolver_is_reported_as_a_host_error_with_the_original_cause()
    {
        ProtectedValueKeyRegistry keys = ProtectedValueTestSupport.Registry((ProtectedValueTestSupport.KeyOne, ProtectedValueTestSupport.NewKey()));
        ProtectedValueResolverRegistry registry = ProtectedValueTestSupport.FrameworkRegistry(keys);

        InvalidOperationException original = new InvalidOperationException("el almacén del host rechazó la operación");
        registry.Register(new FailingResolver("x509", original));

        ProtectedValueKeyProviderException exception = Assert.Throws<ProtectedValueKeyProviderException>(
            () => registry.Resolve("Cooperator:ConnectionStrings:Primary", "x509" + ":" + "una-carga"));

        Assert.Same(original, exception.InnerException);
        Assert.Contains("x509", exception.Message, StringComparison.Ordinal);
        Assert.Contains("Cooperator:ConnectionStrings:Primary", exception.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("una-carga", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_host_key_provider_hook_is_used_and_the_process_point_freezes_once()
    {
        ProtectedValueTestSupport.TestKeyProvider provider = new ProtectedValueTestSupport.TestKeyProvider(
            (ProtectedValueTestSupport.KeyOne, ProtectedValueTestSupport.NewKey()));
        ProtectedValueKeyRegistry keys = new ProtectedValueKeyRegistry(provider);
        keys.SetActive(ProtectedValueTestSupport.KeyOne);

        AesGcmValueResolver resolver = new AesGcmValueResolver(keys);
        string protectedValue = resolver.Protect("Cooperator:ConnectionStrings:Primary", "un valor");

        // El material lo aporta el hook del host: el framework sólo conoce la referencia.
        Assert.True(keys.HasKey(ProtectedValueTestSupport.KeyOne));
        Assert.Equal("un valor", resolver.Resolve("Cooperator:ConnectionStrings:Primary", protectedValue.Substring("aesgcm".Length + 1)));

        // El punto de registro del proceso se inicializa UNA vez (fijación única al arranque, ADR-007 §3.1).
        Assert.False(ProtectedValueResolvers.IsInitialized);
        ProtectedValueKeyRegistry processKeys = ProtectedValueResolvers.Initialize(provider, ProtectedValueTestSupport.KeyOne);
        Assert.True(ProtectedValueResolvers.IsInitialized);
        Assert.Equal(ProtectedValueTestSupport.KeyOne, processKeys.ActiveKid);

        Assert.Throws<ProtectedValueSchemeException>(
            () => ProtectedValueResolvers.Initialize(provider, ProtectedValueTestSupport.KeyOne));

        ProtectedValueResolvers.Register(new RecordingResolver("kv"));
        ProtectedValueResolvers.Freeze();

        string processProtected = new AesGcmValueResolver(processKeys).Protect("Cooperator:ConnectionStrings:Primary", "valor del proceso");
        Assert.Equal("valor del proceso", ProtectedValueResolvers.Resolve("Cooperator:ConnectionStrings:Primary", processProtected));
        Assert.Equal("resuelto por el host: carga", ProtectedValueResolvers.Resolve("Cooperator:ConnectionStrings:Primary", "kv" + ":" + "carga"));

        Assert.Throws<ProtectedValueSchemeException>(() => ProtectedValueResolvers.Register(new RecordingResolver("otro")));
    }

    private sealed class RecordingResolver : IProtectedValueResolver
    {
        internal RecordingResolver(string scheme)
        {
            Scheme = scheme;
        }

        public string Scheme { get; }

        internal string? LastKey { get; private set; }

        internal string? LastPayload { get; private set; }

        public string Resolve(string key, string payload)
        {
            LastKey = key;
            LastPayload = payload;
            return "resuelto por el host: " + payload;
        }
    }

    private sealed class FailingResolver : IProtectedValueResolver
    {
        private readonly Exception _failure;

        internal FailingResolver(string scheme, Exception failure)
        {
            Scheme = scheme;
            _failure = failure;
        }

        public string Scheme { get; }

        public string Resolve(string key, string payload) => throw _failure;
    }
}

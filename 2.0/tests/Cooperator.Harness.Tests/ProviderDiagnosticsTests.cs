using Cooperator.Framework.Core.Exceptions;
using Cooperator.Framework.Core.LazyLoad;
using Cooperator.Framework.Core.LazyLoad.Exceptions;

namespace Cooperator.Harness.Tests;

/// <summary>
/// AC-4 del SPEC-261016-0015 §7: <b>cada</b> causa del cuadro de ADR-006 §3.3 produce su
/// excepción tipada con el contexto que permite corregirla sin depurar — la clave de
/// configuración, el ensamblado y el tipo intentados, y la causa (subyacente, del hook o el
/// motivo de incompatibilidad)—, sin <c>null</c> y sin degradación silenciosa.
/// <para>
/// Los casos se escriben desde el <b>cuadro normativo</b>, no desde la implementación: lo
/// que se verifica es el contenido obligatorio de cada fila (SPEC §14).
/// </para>
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class ProviderDiagnosticsTests : LazyProviderTestBase
{
    private const string MissingType = "Cooperator.LazyFixtures.Declared.NoSuchProvider";

    private const string NotAProviderType = "Cooperator.LazyFixtures.Declared.NotAProvider";

    private const string AbstractProviderType = "Cooperator.LazyFixtures.Declared.AbstractProvider";

    private const string NoParameterlessCtorType = "Cooperator.LazyFixtures.Declared.NoParameterlessCtorProvider";

    private const string InternalCtorType = "Cooperator.LazyFixtures.Declared.InternalCtorProvider";

    private const string FailingStaticCtorType = "Cooperator.LazyFixtures.Declared.FailingStaticCtorProvider";

    [Fact]
    public void The_five_causes_of_the_diagnostic_table_produce_their_typed_exception()
    {
        // 1 — Falta la clave de configuración: entidad + clave esperada del catálogo.
        ConfigureRaw();
        var notConfigured = Assert.Throws<ProviderNotConfiguredException>(() => LazyProviderFactory.GetForEntity("Customer"));
        Assert.Equal("Customer", notConfigured.Entity);
        Assert.Contains("Cooperator:LazyLoad:Providers:Customer:Assembly", notConfigured.Message, StringComparison.Ordinal);

        // 2 — El ensamblado no se resuelve: nombre intentado + si hay hook + causa subyacente.
        var assemblyMissing = Assert.Throws<ProviderAssemblyNotFoundException>(
            () => ProviderResolver.Resolve(new ProviderName("No.Such.Cooperator.Assembly", "Any.Provider")));
        Assert.Equal("No.Such.Cooperator.Assembly", assemblyMissing.AssemblyName);
        Assert.False(assemblyMissing.HookRegistered);
        Assert.True(
            assemblyMissing.InnerException is FileNotFoundException or FileLoadException,
            $"La causa subyacente del runtime debe viajar en el diagnóstico; llegó '{assemblyMissing.InnerException?.GetType().Name}'.");
        Assert.Contains("No.Such.Cooperator.Assembly", assemblyMissing.Message, StringComparison.Ordinal);

        // 3 — El ensamblado existe pero el tipo no: tipo + ensamblado resuelto + los que sí sirven.
        var typeMissing = Assert.Throws<ProviderTypeNotFoundException>(
            () => ProviderResolver.Resolve(new ProviderName(DeclaredAssembly, MissingType)));
        Assert.Equal(MissingType, typeMissing.TypeName);
        Assert.Contains(DeclaredAssembly, typeMissing.AssemblyName, StringComparison.Ordinal);
        Assert.Contains(SampleProviderType, typeMissing.AvailableProviders);
        Assert.Contains(SampleProviderType, typeMissing.Message, StringComparison.Ordinal);

        // 4 — El tipo existe pero no sirve: los tres motivos del contrato.
        var notImplementing = Assert.Throws<ProviderTypeIncompatibleException>(
            () => ProviderResolver.Resolve(new ProviderName(DeclaredAssembly, NotAProviderType)));
        Assert.Equal(ProviderTypeIncompatibleException.DoesNotImplementContract, notImplementing.Reason);
        Assert.Contains(NotAProviderType, notImplementing.Message, StringComparison.Ordinal);

        var abstractProvider = Assert.Throws<ProviderTypeIncompatibleException>(
            () => ProviderResolver.Resolve(new ProviderName(DeclaredAssembly, AbstractProviderType)));
        Assert.Equal(ProviderTypeIncompatibleException.IsAbstractOrInterface, abstractProvider.Reason);

        var withoutConstructor = Assert.Throws<ProviderTypeIncompatibleException>(
            () => ProviderResolver.Resolve(new ProviderName(DeclaredAssembly, NoParameterlessCtorType)));
        Assert.Equal(ProviderTypeIncompatibleException.HasNoPublicParameterlessConstructor, withoutConstructor.Reason);

        // Un constructor sin parámetros que NO es público es incompatible por el mismo motivo.
        var internalConstructor = Assert.Throws<ProviderTypeIncompatibleException>(
            () => ProviderResolver.Resolve(new ProviderName(DeclaredAssembly, InternalCtorType)));
        Assert.Equal(ProviderTypeIncompatibleException.HasNoPublicParameterlessConstructor, internalConstructor.Reason);

        // 5 — El hook del host falla: cubierto por ProviderHookTests (el fallo viene del host).
    }

    [Fact]
    public void A_failing_static_initializer_is_a_failure_of_the_type_not_a_silent_success()
    {
        // El framework encuentra el tipo, verifica la compatibilidad y la construcción falla
        // ADENTRO del tipo: el fallo es del tipo y viaja como tal (el runtime lo envuelve al
        // invocar el constructor), NO se convierte en un provider nulo ni en una ausencia.
        var failure = Assert.ThrowsAny<Exception>(
            () => ProviderResolver.Resolve(new ProviderName(DeclaredAssembly, FailingStaticCtorType)));

        var chain = new List<Exception>();
        for (Exception? current = failure; current is not null; current = current.InnerException)
        {
            chain.Add(current);
        }

        Assert.Contains(
            chain,
            exception => exception is InvalidOperationException
                && exception.Message.Contains("falla a propósito", StringComparison.Ordinal));

        // El diagnóstico del framework no se disfraza: no es una causa del cuadro de ADR-006 §3.3.
        Assert.DoesNotContain(chain, exception => exception is ProviderNotConfiguredException or ProviderTypeNotFoundException);
    }

    [Fact]
    public void Every_diagnosis_is_a_poco_exception_of_the_base_layer_hierarchy()
    {
        // §C.1: las cinco son excepciones PROPIAS del framework en `Core`, derivadas de la
        // excepción base de la capa base, nacidas POCOs (sin [Serializable]/SerializationInfo).
        Type[] cuadro =
        [
            typeof(ProviderNotConfiguredException),
            typeof(ProviderAssemblyNotFoundException),
            typeof(ProviderTypeNotFoundException),
            typeof(ProviderTypeIncompatibleException),
            typeof(ProviderResolutionFailedException),
            typeof(ProviderValidationException),
        ];

        foreach (var type in cuadro)
        {
            Assert.True(typeof(CoreException).IsAssignableFrom(type), $"{type.Name} debe derivar de CoreException.");
            Assert.DoesNotContain(type.GetCustomAttributes(inherit: false), attribute => attribute is SerializableAttribute);
            Assert.DoesNotContain(
                type.GetConstructors(),
                constructor => constructor.GetParameters().Any(parameter => parameter.ParameterType.Name == "SerializationInfo"));
        }
    }
}

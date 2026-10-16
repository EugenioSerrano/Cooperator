using Cooperator.Framework.Core.Configuration;
using Cooperator.Framework.Core.LazyLoad;
using Cooperator.Framework.Core.LazyLoad.Exceptions;
using Cooperator.LazyFixtures.Declared;

namespace Cooperator.Harness.Tests;

/// <summary>
/// AC-1 y AC-2 del SPEC-261016-0015 §7, más la validación estructural del nombre calificado
/// de §8.1: los nombres son <b>obligatorios y explícitos</b> (ensamblado en forma simple +
/// tipo completo calificado por namespace) y el caso normal se resuelve por la <b>vía
/// natural del host</b>, sin que el framework cargue nada a mano.
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class ProviderResolutionTests : LazyProviderTestBase
{
    [Fact]
    public void The_qualified_name_requires_both_values_and_a_namespace_qualified_type()
    {
        // Falta el ensamblado / falta el tipo: es configuración ausente, no un nombre raro.
        Assert.Throws<ProviderNotConfiguredException>(() => new ProviderName(null, SampleProviderType));
        Assert.Throws<ProviderNotConfiguredException>(() => new ProviderName(DeclaredAssembly, null));
        Assert.Throws<ProviderNotConfiguredException>(() => new ProviderName("   ", SampleProviderType));
        Assert.Throws<ProviderNotConfiguredException>(() => new ProviderName(DeclaredAssembly, string.Empty));

        // Tipo SIN namespace: el framework exige la forma calificada de ADR-006 §3.1 y no
        // busca "algún tipo con ese nombre" (no hay resolución implícita).
        var unqualified = Assert.Throws<ProviderTypeNotFoundException>(() => new ProviderName(DeclaredAssembly, "SampleLazyProvider"));
        Assert.Equal("SampleLazyProvider", unqualified.TypeName);
        Assert.Contains("namespace", unqualified.Message, StringComparison.OrdinalIgnoreCase);

        // Y el caso válido construye el par.
        var name = new ProviderName(DeclaredAssembly, SampleProviderType);
        Assert.Equal(DeclaredAssembly, name.AssemblyName);
        Assert.Equal(SampleProviderType, name.TypeName);
        Assert.True(ProviderName.IsNamespaceQualified(SampleProviderType));

        // Un namespace de un solo segmento también es válido (borde del SPEC §8.1).
        Assert.True(ProviderName.IsNamespaceQualified("Ventas.Provider"));
    }

    [Fact]
    public void Version_culture_and_token_are_accepted_as_documentation_and_do_not_change_the_resolution()
    {
        var withBinding = new ProviderName(
            DeclaredAssembly + ", Version=9.9.9.9, Culture=neutral, PublicKeyToken=0123456789abcdef",
            SampleProviderType);

        // El valor se conserva como documentación y la resolución usa la forma simple.
        Assert.Equal(DeclaredAssembly + ", Version=9.9.9.9, Culture=neutral, PublicKeyToken=0123456789abcdef", withBinding.DeclaredAssemblyName);
        Assert.Equal(DeclaredAssembly, withBinding.AssemblyName);
        Assert.True(withBinding.DeclaredNameCarriesBindingDocumentation);

        // Y resuelve igual que el mismo destino declarado en forma simple.
        Assert.Equal(withBinding, new ProviderName(DeclaredAssembly, SampleProviderType));
    }

    [Fact]
    public void The_cache_key_is_the_qualified_pair_so_the_concatenation_collision_is_gone()
    {
        // El caso que el 1.4 hacía indistinguible: "A.B" + "." + "C" == "A" + "." + "B.C".
        var first = new ProviderName("A.B", "C.D");
        var second = new ProviderName("A", "B.C.D");

        Assert.NotEqual(first, second);
        Assert.NotEqual(first.GetHashCode(), second.GetHashCode());
        Assert.NotEqual(first.Key, second.Key);
        Assert.False(first.Equals(second));

        // Igualdad estructural: el mismo par es la misma clave (y el mismo hash).
        Assert.Equal(first, new ProviderName("A.B", "C.D"));
        Assert.Equal(first.GetHashCode(), new ProviderName("A.B", "C.D").GetHashCode());
    }

    [Fact]
    public void The_entity_path_reports_the_entity_and_the_expected_catalog_key()
    {
        // Entidad sin ninguna de las dos claves: la clave esperada es la del ensamblado.
        ConfigureRaw();
        var missingBoth = Assert.Throws<ProviderNotConfiguredException>(() => LazyProviderFactory.GetForEntity("Customer"));
        Assert.Equal("Customer", missingBoth.Entity);
        Assert.Equal(ConfigurationKeys.LazyLoad.AssemblyFor("Customer"), missingBoth.ExpectedKey);
        Assert.Contains("Cooperator:LazyLoad:Providers:Customer:Assembly", missingBoth.Message, StringComparison.Ordinal);

        // Entidad con UNA sola clave (la del ensamblado): la esperada es la de la clase.
        ConfigureRaw((ConfigurationKeys.LazyLoad.AssemblyFor("Customer"), DeclaredAssembly));
        var missingClass = Assert.Throws<ProviderNotConfiguredException>(() => LazyProviderFactory.GetForEntity("Customer"));
        Assert.Equal(ConfigurationKeys.LazyLoad.ClassFor("Customer"), missingClass.ExpectedKey);

        // Y con un valor presente pero vacío, el diagnóstico es el mismo que si faltara.
        ConfigureRaw(
            (ConfigurationKeys.LazyLoad.AssemblyFor("Customer"), DeclaredAssembly),
            (ConfigurationKeys.LazyLoad.ClassFor("Customer"), "   "));
        var emptyClass = Assert.Throws<ProviderNotConfiguredException>(() => LazyProviderFactory.GetForEntity("Customer"));
        Assert.Equal(ConfigurationKeys.LazyLoad.ClassFor("Customer"), emptyClass.ExpectedKey);
    }

    [Fact]
    public void The_declared_provider_resolves_by_the_default_path_of_the_host()
    {
        // El provider del caso normal es una DEPENDENCIA DECLARADA del proyecto de pruebas:
        // la resolución por defecto del host (Assembly.Load del nombre simple + tipo por
        // nombre completo) lo encuentra sin que el framework cargue nada a mano.
        Configure(("Customer", DeclaredAssembly, SampleProviderType));

        var provider = LazyProviderFactory.GetForEntity("Customer");

        Assert.IsType<SampleLazyProvider>(provider);

        // Se resolvió el ensamblado DECLARADO (el que la aplicación referencia), no una
        // copia cargada por otra vía.
        Assert.Equal(
            typeof(SampleLazyProvider).Assembly.GetName().Name,
            provider.GetType().Assembly.GetName().Name);
    }

    [Fact]
    public void The_explicit_entry_point_keeps_the_contract_that_the_generated_code_uses()
    {
        // `LazyProviderFactory.Get(assemblyName, className)` es la entrada del código
        // generado (plantilla DefaultLazyProvider): misma forma, resuelta por el nombre
        // calificado.
        var provider = LazyProviderFactory.Get(DeclaredAssembly, SampleProviderType);

        Assert.IsType<SampleLazyProvider>(provider);

        // Un valor sin calificar por namespace no se resuelve por el camino explícito.
        Assert.Throws<ProviderTypeNotFoundException>(() => LazyProviderFactory.Get(DeclaredAssembly, "SampleLazyProvider"));
    }
}

using Cooperator.Framework.Core.Configuration;
using Cooperator.Framework.Core.LazyLoad;
using Cooperator.Framework.Core.LazyLoad.Exceptions;

namespace Cooperator.Harness.Tests;

/// <summary>
/// AC-6 del SPEC-261016-0015 §7: la <b>validación de arranque</b> reporta <b>todos</b> los
/// fallos en <b>una sola pasada</b> —no corta en el primero—, cada uno con su excepción
/// tipada y su contexto; es <b>opt-in</b> (una aplicación que no la invoca no la ejecuta) y
/// su entrada es explícita (SPEC §6/D.1, §14).
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class ProviderStartupValidationTests : LazyProviderTestBase
{
    private static readonly string[] Entities = ["SinConfiguracion", "EnsambladoInexistente", "TipoInexistente", "Correcta"];

    [Fact]
    public void The_validation_reports_every_failure_of_the_pass_in_order()
    {
        Configure(
            ("EnsambladoInexistente", "No.Such.Cooperator.Assembly", "Any.Provider"),
            ("TipoInexistente", DeclaredAssembly, "Cooperator.LazyFixtures.Declared.NoSuchProvider"),
            ("Correcta", DeclaredAssembly, SampleProviderType));

        // Se quitan las claves de la primera entidad para que su causa sea "no configurada".
        ConfigureRaw(
            (ConfigurationKeys.LazyLoad.AssemblyFor("EnsambladoInexistente"), "No.Such.Cooperator.Assembly"),
            (ConfigurationKeys.LazyLoad.ClassFor("EnsambladoInexistente"), "Any.Provider"),
            (ConfigurationKeys.LazyLoad.AssemblyFor("TipoInexistente"), DeclaredAssembly),
            (ConfigurationKeys.LazyLoad.ClassFor("TipoInexistente"), "Cooperator.LazyFixtures.Declared.NoSuchProvider"),
            (ConfigurationKeys.LazyLoad.AssemblyFor("Correcta"), DeclaredAssembly),
            (ConfigurationKeys.LazyLoad.ClassFor("Correcta"), SampleProviderType));

        var failures = ProviderStartupValidator.Validate(Entities);

        // TODOS los fallos, en una sola pasada y en el orden pedido (no sólo el primero).
        Assert.Equal(3, failures.Count);
        Assert.Equal(Entities[0], failures[0].Entity);
        Assert.IsType<ProviderNotConfiguredException>(failures[0].Error);
        Assert.Equal(Entities[1], failures[1].Entity);
        Assert.IsType<ProviderAssemblyNotFoundException>(failures[1].Error);
        Assert.Equal(Entities[2], failures[2].Entity);
        Assert.IsType<ProviderTypeNotFoundException>(failures[2].Error);

        // La entidad que sí resolvió no aparece en el listado.
        Assert.DoesNotContain(failures, failure => failure.Entity == "Correcta");
    }

    [Fact]
    public void The_strict_variant_throws_one_aggregate_with_all_the_failures()
    {
        Configure(
            ("EnsambladoInexistente", "No.Such.Cooperator.Assembly", "Any.Provider"),
            ("TipoInexistente", DeclaredAssembly, "Cooperator.LazyFixtures.Declared.NoSuchProvider"));

        var aggregate = Assert.Throws<ProviderValidationException>(() => ProviderStartupValidator.ValidateOrThrow(["SinConfiguracion", "EnsambladoInexistente", "TipoInexistente"]));

        Assert.Equal(3, aggregate.Failures.Count);
        Assert.Contains(aggregate.Failures, failure => failure.Error is ProviderNotConfiguredException);
        Assert.Contains(aggregate.Failures, failure => failure.Error is ProviderAssemblyNotFoundException);
        Assert.Contains(aggregate.Failures, failure => failure.Error is ProviderTypeNotFoundException);

        // El agregado dice cuántas entidades no resolvieron y nombra cada causa.
        Assert.Contains("3", aggregate.Message, StringComparison.Ordinal);
        Assert.Contains("SinConfiguracion", aggregate.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void The_validation_is_opt_in_and_nothing_runs_unless_the_application_invokes_it()
    {
        Configure(("Correcta", DeclaredAssembly, SampleProviderType));

        // Nadie validó nada todavía: la caché está vacía y no hay excepción agregada.
        Assert.Equal(0, ProviderCache.Count);

        // Recién cuando la aplicación la invoca, la pasada ocurre —y deja el provider resuelto.
        var failures = ProviderStartupValidator.Validate(["Correcta"]);
        Assert.Empty(failures);
        Assert.Equal(1, ProviderCache.Count);
    }

    [Fact]
    public void A_clean_pass_returns_no_failures_and_resolves_every_entity()
    {
        Configure(
            ("Primera", DeclaredAssembly, SampleProviderType),
            ("Segunda", DeclaredAssembly, SecondProviderType));

        Assert.Empty(ProviderStartupValidator.Validate(["Primera", "Segunda"]));
        Assert.Equal(2, ProviderCache.Count);
    }
}

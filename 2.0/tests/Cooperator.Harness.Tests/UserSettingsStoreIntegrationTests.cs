using Cooperator.Framework.Library.Storage;

namespace Cooperator.Harness.Tests;

/// <summary>
/// SPEC-261016-0037 §8.2 (nivel de integración): las pruebas que corren contra el <b>almacén real</b> del
/// usuario designado, con la implementación sustituta que aporta el <b>Bolt 010</b> y que el host fija en
/// <see cref="UserSettingsStore.Current"/>.
/// <para>
/// Se <b>escriben y se compilan en el V-Bounce</b> y su <b>corrida es del ciclo de integración</b> (entorno por
/// provisionar): por eso llevan <c>Category=Integration</c> y quedan fuera de la corrida hermética
/// (<c>-Scope Hermetic</c> filtra <c>Category!=Integration</c>).
/// </para>
/// <para>
/// <b>Datos <c>confidential</c> (AC-8):</b> estos casos usan un <b>valor sintético</b> propio, creado y borrado
/// por el caso, y <b>nunca</b> leen ni escriben un valor del cliente: la verificación es del contrato y del
/// camino de ausencia, no del contenido del almacén.
/// </para>
/// </summary>
[Trait("Category", "Integration")]
public sealed class UserSettingsStoreIntegrationTests
{
    private const string Owner = "Cooperator.Bolt009.Integration";

    private const string Parameter = "RoundTrip";

    [Fact]
    public void The_real_store_round_trips_with_the_preserved_key_contract()
    {
        Assert.True(
            UserSettingsStore.IsFixed,
            "El ciclo de integración debe fijar el almacén real (la implementación del Bolt 010) en UserSettingsStore.Current antes de correr estos casos.");

        var store = UserSettingsStore.Current;

        // La clave con la que se escribe es, byte a byte, la del contrato del 1.4: ninguna otra derivación.
        Assert.Equal("cooperator.bolt009.integration\u00A1roundtrip", UserSettingsKey.Compose(Owner, Parameter));

        try
        {
            // El camino de ausencia, contra el almacén REAL: no hay valor por defecto silencioso.
            store.Delete(Owner, Parameter);
            Assert.False(store.Exists(Owner, Parameter));
            Assert.Null(store.ReadText(Owner, Parameter));

            // El ciclo completo con un valor sintético del propio caso.
            store.WriteText(Owner, Parameter, "1");
            Assert.True(store.Exists(Owner, Parameter));
            Assert.Equal("1", store.ReadText(Owner, Parameter));
        }
        finally
        {
            // Limpieza: el caso no deja rastro en el almacén del usuario designado.
            UserSettingsStore.Current.Delete(Owner, Parameter);
        }

        Assert.False(UserSettingsStore.Current.Exists(Owner, Parameter));
    }

    [Fact]
    public void The_destination_of_the_store_is_declared()
    {
        Assert.True(UserSettingsStore.IsFixed, "El ciclo de integración debe fijar el almacén real (Bolt 010).");

        // El destino (usuario o máquina) es parte de la semántica del almacén y el contrato lo declara.
        var destination = UserSettingsStore.Current.Destination;
        Assert.True(
            destination is UserSettingsStoreDestination.CurrentUser or UserSettingsStoreDestination.LocalMachine,
            $"Destino inesperado del almacén: {destination}.");
    }
}

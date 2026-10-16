using Cooperator.Framework.Library.Storage;
using Cooperator.Harness.Tests.Support;

namespace Cooperator.Harness.Tests;

/// <summary>
/// SPEC-261016-0037 §6/C y §6/D y §8.1: <b>el contrato portable del almacén y su contrato de claves</b>.
/// <list type="bullet">
///   <item>Los <b>vectores dorados</b> de la derivación de clave del 1.4, <b>byte a byte</b>: recorte y plegado a
///         minúsculas <b>por parte</b>, el separador <c>U+00A1</c> y su posición; más la evidencia de la fuente
///         congelada (el <c>0xA1</c> es su único byte no ASCII).</item>
///   <item>El <b>camino de ausencia explícito</b>: leer lo que no existe devuelve <c>null</c>, nunca un valor
///         por defecto silencioso, y <c>Exists</c> lo dice.</item>
///   <item>El <b>punto de fijación</b>: el host fija su implementación (el sustituto del Bolt 010); consumirlo
///         sin fijarlo es un error que se nombra.</item>
///   <item>La <b>limitación de cultura declarada</b>, medida como hecho (no se declara corregida).</item>
/// </list>
/// El doble de prueba (<see cref="InMemoryStore"/>) es el consumidor del contrato a nivel V-Bounce: lo que se
/// verifica es el <b>contrato</b>, no una implementación concreta.
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class UserSettingsStoreContractTests
{
    [Fact]
    public void The_key_derivation_matches_the_14_contract_byte_for_byte()
    {
        // El separador del contrato.
        Assert.Equal('\u00A1', UserSettingsKey.Separator);

        // Recorte y plegado POR PARTE (no sobre la composición) — el caso del consumidor que viaja en el árbol.
        Assert.Equal("ventas\u00A1servidor", UserSettingsKey.Compose("Ventas ", " Servidor "));
        Assert.Equal("cooperator.modeler\u00A1currentserver", UserSettingsKey.Compose("Cooperator.Modeler", "CurrentServer"));

        // Un solo carácter separador, en el medio: `owner` + separador + `parameter`.
        var key = UserSettingsKey.Compose("O", "P");
        Assert.Equal(3, key.Length);
        Assert.Equal('\u00A1', key[1]);

        // Los dos pares que la concatenación con puntos del 1.4 habría confundido siguen siendo claves distintas.
        Assert.NotEqual(UserSettingsKey.Compose("A.B", "C"), UserSettingsKey.Compose("A", "B.C"));

        // Y los bordes del contrato: partes vacías o con espacios.
        Assert.Equal("\u00A1", UserSettingsKey.Compose("   ", ""));
        Assert.Equal("a\u00A1b", UserSettingsKey.Compose("A", "B"));
    }

    [Fact]
    public void The_frozen_source_of_the_14_confirms_the_contract_separator()
    {
        var file = Repo.InRepo("1.4", "Cooperator.Framework.Library", "Repository", "RepositoryMapper.cs");
        Assert.True(File.Exists(file), $"Falta la fuente congelada que declara la derivación: {file}");

        var nonAscii = File.ReadAllBytes(file)
            .Select((value, index) => (Byte: value, Index: index))
            .Where(entry => entry.Byte > 127)
            .ToArray();

        // Un ÚNICO byte no ASCII en todo el archivo, y es el separador del contrato.
        Assert.Single(nonAscii);
        Assert.Equal(0xA1, nonAscii[0].Byte);
    }

    [Fact]
    public void The_absence_path_is_explicit_and_never_a_silent_default()
    {
        var store = new InMemoryStore();

        Assert.False(store.Exists("Ventas", "Servidor"));
        Assert.Null(store.ReadText("Ventas", "Servidor"));
        Assert.Null(store.ReadDecimal("Ventas", "Puerto"));
        Assert.Null(store.ReadDate("Ventas", "Arranque"));
        Assert.Null(store.ReadBoolean("Ventas", "Activo"));

        store.WriteText("Ventas", "Servidor", "srv");
        Assert.True(store.Exists("Ventas", "Servidor"));
        Assert.Equal("srv", store.ReadText("Ventas", "Servidor"));

        store.Delete("Ventas", "Servidor");
        Assert.False(store.Exists("Ventas", "Servidor"));
        Assert.Null(store.ReadText("Ventas", "Servidor"));
    }

    [Fact]
    public void The_store_contract_round_trips_every_declared_value_type()
    {
        var store = new InMemoryStore();

        store.WriteDecimal("Ventas", "Puerto", 1433m);
        store.WriteDate("Ventas", "Arranque", new DateTime(2026, 10, 16, 2, 39, 10, DateTimeKind.Utc));
        store.WriteBoolean("Ventas", "Activo", true);

        Assert.Equal(1433m, store.ReadDecimal("Ventas", "Puerto"));
        Assert.Equal(new DateTime(2026, 10, 16, 2, 39, 10, DateTimeKind.Utc), store.ReadDate("Ventas", "Arranque"));
        Assert.True(store.ReadBoolean("Ventas", "Activo"));

        store.DeleteOwner("Ventas");
        Assert.False(store.Exists("Ventas", "Puerto"));
        Assert.False(store.Exists("Ventas", "Arranque"));
        Assert.False(store.Exists("Ventas", "Activo"));
    }

    [Fact]
    public void The_fixing_point_is_explicit_and_throws_when_no_store_is_fixed()
    {
        UserSettingsStore.Reset();
        Assert.False(UserSettingsStore.IsFixed);

        var failure = Assert.Throws<InvalidOperationException>(() => UserSettingsStore.Current);
        Assert.Contains("almacén de configuración de usuario", failure.Message, StringComparison.Ordinal);

        var store = new InMemoryStore();
        UserSettingsStore.Current = store;
        Assert.True(UserSettingsStore.IsFixed);
        Assert.Same(store, UserSettingsStore.Current);

        UserSettingsStore.Reset();
        Assert.False(UserSettingsStore.IsFixed);
    }

    [Fact]
    public void The_declared_culture_limitation_is_measured_as_a_fact()
    {
        // El plegado usa `ToLower()`, sensible a la cultura del proceso, igual que el 1.4: en turco, 'I' no pliega
        // a 'i'. Es la LIMITACIÓN DECLARADA del contrato —se mide como hecho para que nadie la crea corregida—:
        // cambiarla alteraría las claves ya persistidas en culturas con plegado especial.
        var original = System.Globalization.CultureInfo.CurrentCulture;
        try
        {
            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("tr-TR");
            Assert.NotEqual("i", UserSettingsKey.Normalize("I"));
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture = original;
        }

        // Y con la cultura de la máquina, el caso normal es el esperado.
        Assert.Equal("servidor", UserSettingsKey.Normalize("  Servidor  "));
    }

    /// <summary>Doble de prueba del contrato: un almacén en memoria (el consumidor del contrato a nivel V-Bounce).</summary>
    private sealed class InMemoryStore : IUserSettingsStore
    {
        private readonly Dictionary<string, string> _values = new(StringComparer.Ordinal);

        public UserSettingsStoreDestination Destination => UserSettingsStoreDestination.CurrentUser;

        public bool Exists(string owner, string parameter) => _values.ContainsKey(UserSettingsKey.Compose(owner, parameter));

        public string? ReadText(string owner, string parameter) =>
            _values.TryGetValue(UserSettingsKey.Compose(owner, parameter), out var value) ? value : null;

        public decimal? ReadDecimal(string owner, string parameter) =>
            ReadText(owner, parameter) is { } value ? decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture) : null;

        public DateTime? ReadDate(string owner, string parameter) =>
            ReadText(owner, parameter) is { } value ? DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind) : null;

        public bool? ReadBoolean(string owner, string parameter) =>
            ReadText(owner, parameter) is { } value ? bool.Parse(value) : null;

        public void WriteText(string owner, string parameter, string value) =>
            _values[UserSettingsKey.Compose(owner, parameter)] = value ?? throw new ArgumentNullException(nameof(value));

        public void WriteDecimal(string owner, string parameter, decimal value) =>
            WriteText(owner, parameter, value.ToString(System.Globalization.CultureInfo.InvariantCulture));

        public void WriteDate(string owner, string parameter, DateTime value) =>
            WriteText(owner, parameter, value.ToString("O", System.Globalization.CultureInfo.InvariantCulture));

        public void WriteBoolean(string owner, string parameter, bool value) =>
            WriteText(owner, parameter, value.ToString(System.Globalization.CultureInfo.InvariantCulture));

        public void Delete(string owner, string parameter) => _values.Remove(UserSettingsKey.Compose(owner, parameter));

        public void DeleteOwner(string owner) =>
            _values.Keys.Where(key => key.StartsWith(UserSettingsKey.Normalize(owner) + UserSettingsKey.Separator, StringComparison.Ordinal)).ToList()
                .ForEach(key => _values.Remove(key));
    }
}

using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Cooperator.Framework.Characterization.Tests.Support;

namespace Cooperator.Framework.Characterization.Tests.Surface;

/// <summary>
/// Área 2 del SPEC §6.E: inventario del estado *antes* de la superficie de
/// serialización por ensamblado, más el round-trip binario del <c>Snapshot</c>.
///
/// Este inventario es el "antes" contra el que ADR-008 §3.2/§3.4 y su NFR de
/// <i>serialization surface</i> se medirán cuando sus Bolts retiren la superficie:
/// la meta es 0 atributos y 0 <c>SYSLIB0051</c>. Aquí los números se REGISTRAN,
/// no se juzgan.
/// </summary>
public sealed class SerializationSurfaceTests
{
    private const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    private static readonly Type[] SerializationCtorSignature =
        [typeof(SerializationInfo), typeof(StreamingContext)];

    [Theory]
    [InlineData("Cooperator.Framework.Core", 14, 10, 7, 10)]
    [InlineData("Cooperator.Framework.Library", 10, 8, 8, 8)]
    [InlineData("Cooperator.Framework.Data", 17, 16, 16, 16)]
    [InlineData("Cooperator.Framework.Utility", 16, 6, 5, 6)]
    public void The_legacy_serialization_surface_is_measured_per_assembly(
        string assemblyName,
        int expectedSerializable,
        int expectedISerializable,
        int expectedSerializationConstructors,
        int expectedGetObjectData)
    {
        var types = Context.LoadFrozenAssembly(assemblyName).GetTypes();

        // El inventario NO cuenta tipos totales: los tipos generados por el compilador
        // (`<PrivateImplementationDetails>`, display classes) dependen de la emisión de
        // información de depuración —el build reproducible con `DebugType=none` y el
        // build con PDB no coinciden en ese conteo con el MISMO código fuente— y no son
        // superficie ni identidad. Lo que ADR-008 §3.2/§3.4 medirá es la superficie de
        // serialización, y eso es lo que se registra aquí.
        Assert.NotEmpty(types);
        Assert.Equal(expectedSerializable, types.Count(type => Attribute.IsDefined(type, typeof(SerializableAttribute))));
        Assert.Equal(expectedISerializable, types.Count(type => typeof(ISerializable).IsAssignableFrom(type)));
        Assert.Equal(expectedSerializationConstructors, types.Count(type => type.GetConstructor(Instance, null, SerializationCtorSignature, null) is not null));
        Assert.Equal(expectedGetObjectData, types.Count(type => type.GetMethod("GetObjectData", Instance) is not null));

        // La superficie pública del ensamblado no está vacía: es lo que el gate de
        // compatibilidad de API compara miembro por miembro.
        Assert.NotEmpty(Context.LoadFrozenAssembly(assemblyName).GetExportedTypes());
    }

    [Fact]
    public void Every_type_declaring_GetObjectData_also_has_the_serialization_constructor()
    {
        // La regla del formato: quien implementa explícitamente ISerializable declara
        // GetObjectData y necesita el constructor de deserialización. Las excepciones
        // heredan el contrato de Exception sin declararlo, y por eso el inventario por
        // ensamblado muestra más tipos ISerializable que constructores explícitos: es
        // la asimetría observada, no un defecto que este V-Bounce corrija.
        const BindingFlags Declared = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        foreach (var assemblyName in new[]
                 {
                     "Cooperator.Framework.Core", "Cooperator.Framework.Library",
                     "Cooperator.Framework.Data", "Cooperator.Framework.Utility",
                 })
        {
            var declaring = Context.LoadFrozenAssembly(assemblyName).GetTypes()
                .Where(type => typeof(ISerializable).IsAssignableFrom(type) && type.GetMethod("GetObjectData", Declared) is not null);

            Assert.All(
                declaring,
                type => Assert.NotNull(type.GetConstructor(Instance, null, SerializationCtorSignature, null)));
        }
    }

    [Fact]
    public void Every_serializable_type_declares_the_attribute_on_itself_or_inherits_it_from_Exception()
    {
        foreach (var assemblyName in new[]
                 {
                     "Cooperator.Framework.Core", "Cooperator.Framework.Library",
                     "Cooperator.Framework.Data", "Cooperator.Framework.Utility",
                 })
        {
            var types = Context.LoadFrozenAssembly(assemblyName).GetTypes()
                .Where(type => Attribute.IsDefined(type, typeof(SerializableAttribute)));

            Assert.NotEmpty(types);
            foreach (var type in types)
            {
                // Observado: el atributo aparece por tres vías — declarado en el tipo,
                // heredado de Exception, o aportado por la forma del tipo (los delegados
                // y las enumeraciones son serializables por construcción y el atributo
                // lo materializa el compilador).
                var declaredHere = type.GetCustomAttribute<SerializableAttribute>(inherit: false) is not null;
                var inheritsFromException = typeof(Exception).IsAssignableFrom(type);
                var intrinsicallySerializable = type.IsEnum || typeof(Delegate).IsAssignableFrom(type);

                Assert.True(
                    declaredHere || inheritsFromException || intrinsicallySerializable,
                    $"{type.FullName} no declara el atributo y no es una excepción, un delegado ni una enumeración.");
                Assert.True(type.IsSerializable || inheritsFromException, $"{type.FullName} no es serializable.");
            }
        }
    }

    [Fact]
    public void The_snapshot_of_the_legacy_format_is_serializable()
    {
        var snapshotType = SnapshotType();

        Assert.True(snapshotType.IsSerializable);
        // Observado: el Snapshot confía en la serialización automática —declara
        // [Serializable] y NO implementa ISerializable ni el constructor de
        // deserialización. Es parte del "antes" que ADR-008 §3.2 retira.
        Assert.False(typeof(ISerializable).IsAssignableFrom(snapshotType));
        Assert.Null(snapshotType.GetConstructor(Instance, null, SerializationCtorSignature, null));
    }

    [Fact]
    public void The_snapshot_defaults_are_the_ones_the_legacy_generation_assumes()
    {
        var snapshot = NewSnapshot();

        Assert.Equal("1.4.5.0", Property<string>(snapshot, "ModelerVersion"));
        Assert.False(Property<bool>(snapshot, "HaveChanges"));
        Assert.False(Property<bool>(snapshot, "GenerateAutoFileOnly"));
        Assert.False(Property<bool>(snapshot, "HaveChanges"));
        Assert.True(
            Property<bool>(snapshot, "StampDateAndTimeOnAutoFiles"),
            "El 1.4 marca las plantillas auto con la fecha/hora por defecto: por eso la generación determinista tiene que desactivarlo.");
        Assert.True(Property<bool>(snapshot, "GenerateCheckForToken"));
        Assert.Equal(string.Empty, Property<string>(snapshot, "Language"));
        Assert.Equal(string.Empty, Property<string>(snapshot, "SPPrefix"));
        Assert.Null(Property<object>(snapshot, "DomainTree"));
    }

    [Fact]
    public void Setting_any_snapshot_property_to_a_new_value_marks_the_snapshot_as_changed()
    {
        var snapshot = NewSnapshot();

        SetProperty(snapshot, "DataBaseName", "Base");
        Assert.True(Property<bool>(snapshot, "HaveChanges"));
    }

    [Fact]
    public void Re_setting_a_property_to_its_current_value_does_not_mark_the_snapshot_as_changed_again()
    {
        var snapshot = NewSnapshot();

        SetProperty(snapshot, "SPPrefix", "sp_");
        Assert.True(Property<bool>(snapshot, "HaveChanges"));

        // Volver a asignar el MISMO valor no cambia la bandera (que ya estaba en true):
        // el observable es que no se PASA a false.
        SetProperty(snapshot, "SPPrefix", "sp_");
        Assert.True(Property<bool>(snapshot, "HaveChanges"));
    }

    [Fact]
    public void The_snapshot_round_trips_through_the_binary_serializer_the_legacy_format_uses()
    {
        var snapshot = NewSnapshot();
        SetProperty(snapshot, "DataBaseName", "BaseFixture");
        SetProperty(snapshot, "SPPrefix", "sp_");
        SetProperty(snapshot, "StampDateAndTimeOnAutoFiles", false);

        var restored = RoundTrip(snapshot);

        Assert.Equal("BaseFixture", Property<string>(restored, "DataBaseName"));
        Assert.Equal("sp_", Property<string>(restored, "SPPrefix"));
        Assert.False(Property<bool>(restored, "StampDateAndTimeOnAutoFiles"));
        Assert.Equal("1.4.5.0", Property<string>(restored, "ModelerVersion"));
    }

    [Fact]
    public void The_snapshot_round_trip_is_stable_for_the_same_object_graph()
    {
        var snapshot = NewSnapshot();
        SetProperty(snapshot, "DataBaseName", "BaseFixture");

        var first = Serialize(snapshot);
        var second = Serialize(snapshot);

        Assert.Equal(first, second);
    }

    [Fact]
    public void The_vendored_binaries_are_not_the_source_build_and_must_not_be_adopted_silently()
    {
        // SPEC §13: la lista de identidad del migrador no debe adoptar los binarios
        // vendorizados desalineados. El inventario los registra con su hash y su
        // divergencia, y este caso lo hace verificable desde la suite.
        var inventory = Context.Fixture("frozen-1.4", "frozen-1.4-inventory.json");
        var vendored = inventory.GetProperty("vendored_binaries").EnumerateArray().ToArray();

        Assert.NotEmpty(vendored);
        Assert.All(vendored, entry => Assert.False(entry.GetProperty("matches_source_build").GetBoolean()));

        var utility = vendored.Single(entry => entry.GetProperty("file_name").GetString() == "Cooperator.Framework.Utility.dll");
        Assert.Equal("1.3.3.0", utility.GetProperty("assembly_version").GetString());
        Assert.Equal("1.4.5.0", utility.GetProperty("source_build_version").GetString());
    }

    private static Type SnapshotType() => Context.LoadFrozenAssembly("CooperatorModeler").GetType("CooperatorModeler.Snapshot")!;

    private static object NewSnapshot() => Activator.CreateInstance(SnapshotType())!;

    private static T? Property<T>(object instance, string name) =>
        (T?)SnapshotType().GetProperty(name)!.GetValue(instance);

    private static void SetProperty(object instance, string name, object? value) =>
        SnapshotType().GetProperty(name)!.SetValue(instance, value);

    private static byte[] Serialize(object snapshot)
    {
        using var stream = new MemoryStream();
#pragma warning disable SYSLIB0011 // El propio 1.4 usa el serializador binario: aquí se OBSERVA, no se introduce.
        new BinaryFormatter().Serialize(stream, snapshot);
#pragma warning restore SYSLIB0011
        return stream.ToArray();
    }

    private static object RoundTrip(object snapshot)
    {
        var bytes = Serialize(snapshot);
        using var stream = new MemoryStream(bytes);
#pragma warning disable SYSLIB0011
        return new BinaryFormatter().Deserialize(stream);
#pragma warning restore SYSLIB0011
    }
}

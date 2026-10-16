using System.Reflection;
using Cooperator.Framework.Characterization.Tests.Support;
using Cooperator.Framework.Core;
using Cooperator.Framework.Core.Exceptions;

namespace Cooperator.Framework.Characterization.Tests.Contracts;

/// <summary>
/// Área 1 del SPEC §6.E: contratos de datos y estado de <c>Core</c>.
/// Cada caso registra el comportamiento OBSERVADO del 1.4 congelado; ninguno lo
/// juzga ni lo corrige (criterio de "caracterización, no de corrección", §6.E).
/// </summary>
public sealed class CoreContractsTests
{
    [Fact]
    public void ObjectState_declares_the_five_documented_flags_with_their_bit_values()
    {
        Assert.Equal(typeof(short), Enum.GetUnderlyingType(typeof(ObjectState)));
        Assert.NotNull(typeof(ObjectState).GetCustomAttribute<FlagsAttribute>());

        var expected = new Dictionary<string, short>(StringComparer.Ordinal)
        {
            ["New"] = 1,
            ["Modified"] = 2,
            ["Deleted"] = 4,
            ["Restored"] = 8,
            ["Updated"] = 16,
        };

        var observed = Enum.GetNames(typeof(ObjectState))
            .ToDictionary(name => name, name => Convert.ToInt16(Enum.Parse(typeof(ObjectState), name), System.Globalization.CultureInfo.InvariantCulture), StringComparer.Ordinal);

        Assert.Equal(expected.Count, observed.Count);
        Assert.Equal(expected, observed);
    }

    [Fact]
    public void SecurityRights_is_a_plain_enum_with_the_four_crud_rights()
    {
        // Observado: SecurityRights NO es un conjunto de banderas — a diferencia de
        // ObjectState — y sus usos en el 1.4 son de un derecho por vez. Se registra
        // tal como es; el migrador decide si lo cambia.
        Assert.Null(typeof(SecurityRights).GetCustomAttribute<FlagsAttribute>());
        Assert.Equal(typeof(int), Enum.GetUnderlyingType(typeof(SecurityRights)));
        Assert.Equal(
            ["Delete", "Insert", "Read", "Update"],
            Enum.GetNames(typeof(SecurityRights)).OrderBy(x => x, StringComparer.Ordinal));
    }

    [Fact]
    public void StringLike_declares_the_four_comparison_modes()
    {
        Assert.Equal(
            ["Contains", "EndsWith", "Equal", "StartsWith"],
            Enum.GetNames(typeof(StringLike)).OrderBy(x => x, StringComparer.Ordinal));
    }

    [Theory]
    [InlineData(ObjectState.New, true, false, false, false, false)]
    [InlineData(ObjectState.Modified, false, true, false, false, false)]
    [InlineData(ObjectState.Deleted, false, false, true, false, false)]
    [InlineData(ObjectState.Restored, false, false, false, true, false)]
    [InlineData(ObjectState.Updated, false, false, false, false, true)]
    public void ObjectStateHelper_classifies_each_single_state(
        ObjectState state,
        bool isNew,
        bool isModified,
        bool isDeleted,
        bool isRestored,
        bool isUpdated)
    {
        var row = new FakeRow { State = state };

        Assert.Equal(isNew, ObjectStateHelper.IsNew(row));
        Assert.Equal(isModified, ObjectStateHelper.IsModified(row));
        Assert.Equal(isDeleted, ObjectStateHelper.IsDeleted(row));
        Assert.Equal(isRestored, ObjectStateHelper.IsRestored(row));
        Assert.Equal(isUpdated, ObjectStateHelper.IsUpdated(row));
    }

    [Fact]
    public void ObjectStateHelper_treats_the_state_as_a_flag_set_not_as_an_exclusive_value()
    {
        var row = new FakeRow { State = ObjectState.New | ObjectState.Deleted };

        Assert.True(ObjectStateHelper.IsNew(row));
        Assert.True(ObjectStateHelper.IsDeleted(row));
        Assert.True(ObjectStateHelper.IsNewAndDeleted(row));
        Assert.True(ObjectStateHelper.IsNewOrDeletedOrModified(row));
    }

    [Fact]
    public void ObjectStateHelper_reports_a_zero_state_as_not_pending()
    {
        var row = new FakeRow { State = 0 };

        Assert.False(ObjectStateHelper.IsNewOrDeletedOrModified(row));
        Assert.False(ObjectStateHelper.IsNewAndDeleted(row));
        Assert.False(ObjectStateHelper.IsNew(row));
        Assert.False(ObjectStateHelper.IsModified(row));
        Assert.False(ObjectStateHelper.IsDeleted(row));
    }

    [Fact]
    public void SetAsDeleted_adds_the_deleted_flag_without_clearing_the_others()
    {
        var row = new FakeRow { State = ObjectState.Modified };

        ObjectStateHelper.SetAsDeleted(row);

        Assert.Equal(ObjectState.Modified | ObjectState.Deleted, row.State);
        Assert.True(ObjectStateHelper.IsDeleted(row));
        Assert.True(ObjectStateHelper.IsModified(row));
    }

    [Fact]
    public void ObjectStateHelper_does_not_mutate_the_row_when_only_observing()
    {
        var row = new FakeRow { State = ObjectState.Restored };

        _ = ObjectStateHelper.IsNew(row);
        _ = ObjectStateHelper.IsNewOrDeletedOrModified(row);
        _ = ObjectStateHelper.IsNewAndDeleted(row);

        Assert.Equal(ObjectState.Restored, row.State);
    }

    [Fact]
    public void UniqueIdentifierToString_concatenates_the_full_type_name_and_the_key_values()
    {
        var row = new FakeIdentifiable("A", 7);

        var text = UniqueIdentifierHelper.UniqueIdentifierToString(row);

        Assert.Equal(typeof(FakeIdentifiable).FullName + "{A,7}", text);
    }

    [Fact]
    public void UniqueIdentifierToString_renders_null_key_components_as_the_literal_null()
    {
        var row = new NullKeyIdentifiable();

        var text = UniqueIdentifierHelper.UniqueIdentifierToString(row);

        Assert.EndsWith("{null,null}", text, StringComparison.Ordinal);
    }

    [Fact]
    public void UniqueIdentifierHelper_compares_two_rows_by_their_key_only()
    {
        var first = new FakeIdentifiable("A", 1);
        var second = new FakeIdentifiable("A", 1);
        var third = new FakeIdentifiable("A", 2);

        Assert.True(UniqueIdentifierHelper.IsTheSameKey(first, second));
        Assert.False(UniqueIdentifierHelper.IsTheSameKey(first, third));
    }

    [Fact]
    public void IsSameObject_includes_the_runtime_type_in_the_comparison()
    {
        var first = new FakeIdentifiable("A", 1);
        var other = new OtherIdentifiable("A", 1);

        Assert.True(UniqueIdentifierHelper.IsSameObject(first, first));
        // Misma clave, tipo distinto: el identificador único incluye el tipo.
        Assert.False(UniqueIdentifierHelper.IsSameObject(first, other));
    }

    [Fact]
    public void UniqueIdentifierToString_rejects_a_null_row_with_the_documented_parameter_name()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => UniqueIdentifierHelper.UniqueIdentifierToString(null!));

        Assert.Equal("rowOrEntity", exception.ParamName);
        Assert.Contains("This parameter can not be null", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ComposeIdentifier_appends_the_parameters_after_the_identifier()
    {
        var row = new FakeIdentifiable("A");

        var composed = UniqueIdentifierHelper.ComposeIdentifier(row, 1, "B");

        Assert.Equal(new object[] { "A", 1, "B" }, composed);
    }

    [Fact]
    public void ComposeIdentifier_rejects_a_null_identifier_with_the_documented_parameter_name()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => UniqueIdentifierHelper.ComposeIdentifier(null!));

        Assert.Equal("uniqueIdentifier", exception.ParamName);
    }

    [Fact]
    public void ValuesGenerator_advances_its_integral_sequence_by_one_step_per_read()
    {
        var first = ValuesGenerator.GetInt32;
        var second = ValuesGenerator.GetInt32;
        Assert.Equal(first - 1, second);

        var firstLong = ValuesGenerator.GetInt64;
        Assert.Equal(firstLong - 1, ValuesGenerator.GetInt64);
    }

    [Fact]
    public void ValuesGenerator_string_guid_and_byte_array_are_unique_per_read()
    {
        Assert.NotEqual(ValuesGenerator.GetString, ValuesGenerator.GetString);
        Assert.NotEqual(ValuesGenerator.GetGuid, ValuesGenerator.GetGuid);
        Assert.Equal(16, ValuesGenerator.GetByteArray.Length);
        Assert.IsType<Guid>(ValuesGenerator.GetObject);
    }

    [Fact]
    public void ValuesGenerator_datetime_returns_the_same_value_on_every_read()
    {
        // Observado: a diferencia de las secuencias numéricas, GetDateTime NO avanza
        // estado —devuelve siempre MaxValue.AddSeconds(-1)—. Es una asimetría del
        // generador del 1.4 y se registra tal como se comporta.
        var first = ValuesGenerator.GetDateTime;
        var second = ValuesGenerator.GetDateTime;

        Assert.Equal(DateTime.MaxValue.AddSeconds(-1), first);
        Assert.Equal(first, second);
    }

    [Fact]
    public void DescriptionFieldEventArg_exposes_the_object_to_find_and_a_mutable_description()
    {
        var row = new FakeIdentifiable(1);
        var argument = new DescriptionFieldEventArg(row);

        Assert.Same(row, argument.ObjectToFind);
        Assert.Null(argument.DescriptionString);

        argument.DescriptionString = "descripción";
        Assert.Equal("descripción", argument.DescriptionString);
    }

    [Fact]
    public void The_core_exception_hierarchy_derives_from_one_abstract_serializable_base()
    {
        var coreAssembly = typeof(CoreException).Assembly;
        var exceptionTypes = coreAssembly.GetExportedTypes()
            .Where(type => typeof(Exception).IsAssignableFrom(type) && type != typeof(Exception))
            .ToArray();

        Assert.NotEmpty(exceptionTypes);
        Assert.True(typeof(CoreException).IsAbstract);
        Assert.True(typeof(CoreException).IsSubclassOf(typeof(Exception)));

        foreach (var type in exceptionTypes)
        {
            Assert.True(
                type == typeof(CoreException) || typeof(CoreException).IsAssignableFrom(type),
                $"{type.FullName} no deriva de CoreException.");
        }
    }

    [Fact]
    public void The_core_exception_hierarchy_keeps_the_four_constructor_contract()
    {
        var exceptionTypes = typeof(CoreException).Assembly.GetExportedTypes()
            .Where(type => typeof(Exception).IsAssignableFrom(type) && type != typeof(Exception));

        const BindingFlags Instance = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        foreach (var type in exceptionTypes)
        {
            Assert.True(type.IsSerializable, $"{type.FullName} no está marcado [Serializable].");
            Assert.NotNull(type.GetConstructor(Instance, null, Type.EmptyTypes, null));
            Assert.NotNull(type.GetConstructor(Instance, null, [typeof(string)], null));
            Assert.NotNull(type.GetConstructor(Instance, null, [typeof(string), typeof(Exception)], null));
            Assert.NotNull(type.GetConstructor(
                Instance,
                null,
                [typeof(System.Runtime.Serialization.SerializationInfo), typeof(System.Runtime.Serialization.StreamingContext)],
                null));
        }
    }

    [Fact]
    public void The_object_contract_extends_the_component_model_editing_contract()
    {
        // Observado: IObject no es un contrato suelto del framework, hereda el
        // contrato de edición de System.ComponentModel. Es parte de lo que el
        // migrador tiene que preservar o reemplazar conscientemente.
        var interfaces = typeof(IObject).GetInterfaces();

        Assert.Contains(typeof(System.ComponentModel.IEditableObject), interfaces);
        Assert.True(typeof(System.ComponentModel.IEditableObject).IsAssignableFrom(typeof(IObject)));
        Assert.NotNull(typeof(System.ComponentModel.IEditableObject).GetMethod("BeginEdit"));
        Assert.NotNull(typeof(System.ComponentModel.IEditableObject).GetMethod("EndEdit"));
        Assert.NotNull(typeof(System.ComponentModel.IEditableObject).GetMethod("CancelEdit"));
    }

    [Fact]
    public void A_row_implements_the_editing_contract_without_the_helper_interfering()
    {
        var row = new FakeRow();

        row.BeginEdit();
        row.EndEdit();
        row.CancelEdit();

        Assert.True(row.BeginEditCalled);
        Assert.True(row.EndEditCalled);
        Assert.True(row.CancelEditCalled);
    }

    private sealed class OtherIdentifiable : IUniqueIdentifiable
    {
        private readonly object[] _identifier;

        public OtherIdentifiable(params object[] identifier) => _identifier = identifier;

        public object[] Identifier() => _identifier;
    }
}

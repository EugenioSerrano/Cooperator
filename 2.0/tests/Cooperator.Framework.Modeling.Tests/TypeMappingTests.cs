using System.Collections;
using System.Reflection;
using Cooperator.Framework.Modeling.Exceptions;
using Cooperator.Framework.Modeling.Reverse;
using Cooperator.Framework.Modeling.Tests.Support;

namespace Cooperator.Framework.Modeling.Tests;

/// <summary>
/// SPEC-261016-0004 §6/C.2 (AC-5): <b>la</b> fuente única del mapeo SQL↔CLR.
/// <para>
/// La tabla de referencia son los 28 tipos nativos que el `switch` del 1.4 resolvía
/// (<c>Providers/MSSQLCommon.cs</c>: <c>NativeTypeExist</c>, <c>NativeType2CLRType</c> y
/// <c>GetLength</c>), con su tipo CLR y su regla de longitud por defecto. El caso
/// exige que la tabla del paquete sea <b>la misma información</b>, que su orden sea
/// estable y determinista, que resuelva en los dos sentidos del contrato y que
/// <b>no exista una segunda declaración del mapeo</b> en el árbol entregado.
/// </para>
/// </summary>
public sealed class TypeMappingTests
{
    /// <summary>La tabla del 1.4, transcripta como referencia (orden: por nombre nativo, ordinal).</summary>
    private static readonly (string Native, Type Clr, NativeLengthRule Rule)[] ReferenceTable =
    [
        ("bigint", typeof(long), NativeLengthRule.None),
        ("binary", typeof(byte[]), NativeLengthRule.AsIs),
        ("bit", typeof(bool), NativeLengthRule.None),
        ("char", typeof(string), NativeLengthRule.AsIs),
        ("date", typeof(DateTime), NativeLengthRule.None),
        ("datetime", typeof(DateTime), NativeLengthRule.None),
        ("decimal", typeof(decimal), NativeLengthRule.AsIs),
        ("float", typeof(double), NativeLengthRule.None),
        ("image", typeof(byte[]), NativeLengthRule.None),
        ("int", typeof(int), NativeLengthRule.None),
        ("money", typeof(decimal), NativeLengthRule.None),
        ("nchar", typeof(string), NativeLengthRule.Half),
        ("ntext", typeof(string), NativeLengthRule.Half),
        ("numeric", typeof(decimal), NativeLengthRule.AsIs),
        ("nvarchar", typeof(string), NativeLengthRule.Half),
        ("real", typeof(double), NativeLengthRule.None),
        ("smalldatetime", typeof(DateTime), NativeLengthRule.None),
        ("smallint", typeof(short), NativeLengthRule.None),
        ("smallmoney", typeof(decimal), NativeLengthRule.None),
        ("sql_variant", typeof(object), NativeLengthRule.None),
        ("sysname", typeof(string), NativeLengthRule.Half),
        ("text", typeof(string), NativeLengthRule.AsIs),
        ("timestamp", typeof(byte[]), NativeLengthRule.AsIs),
        ("tinyint", typeof(byte), NativeLengthRule.None),
        ("uniqueidentifier", typeof(Guid), NativeLengthRule.None),
        ("varbinary", typeof(byte[]), NativeLengthRule.AsIs),
        ("varchar", typeof(string), NativeLengthRule.AsIs),
        ("xml", typeof(string), NativeLengthRule.AsIs),
    ];

    [Fact]
    public void The_table_matches_the_reference_table_of_the_14()
    {
        var actual = new MSSQLTypeConverter().Table
            .Select(entry => (entry.NativeType, entry.ClrType, entry.LengthRule))
            .ToArray();

        var expected = ReferenceTable
            .OrderBy(entry => entry.Native, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(ReferenceTable.Length, actual.Length);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void The_order_is_stable_and_the_table_is_read_only()
    {
        var first = new MSSQLTypeConverter().Table.Select(entry => entry.NativeType).ToArray();
        var second = new MSSQLTypeConverter().Table.Select(entry => entry.NativeType).ToArray();

        Assert.Equal(first, second);
        Assert.Equal(first.OrderBy(name => name, StringComparer.Ordinal).ToArray(), first);

        // De sólo lectura: no es una lista mutable que un consumidor pueda alterar.
        var table = new MSSQLTypeConverter().Table;
        Assert.IsAssignableFrom<IReadOnlyList<NativeTypeMapping>>(table);
        Assert.Throws<NotSupportedException>(() => ((IList)table).Add(null!));
    }

    [Fact]
    public void Every_entry_resolves_in_both_contract_directions_and_ignores_the_case()
    {
        var converter = new MSSQLTypeConverter();

        foreach (var (native, clr, rule) in ReferenceTable)
        {
            Assert.True(converter.NativeTypeExist(native), $"'{native}' debería existir en el mapeo.");
            Assert.True(converter.NativeTypeExist(native.ToUpperInvariant()), $"'{native}' debería resolverse sin distinguir mayúsculas.");
            Assert.Equal(clr, converter.NativeType2CLRType(native));
            Assert.Equal(clr, converter.NativeType2CLRType(native.ToUpperInvariant()));

            // La longitud por defecto SIEMPRE sale de la regla declarada en la tabla
            // (es dato, no un segundo switch): se contrasta con la regla de la entrada.
            var entry = converter.Table.Single(candidate => candidate.NativeType == native);
            Assert.Equal(rule, entry.LengthRule);
        }
    }

    [Fact]
    public void The_default_length_rule_matches_GetLength_of_the_14()
    {
        var converter = new MSSQLTypeConverter();

        // Sin longitud: la regla del 1.4 devolvía null.
        Assert.Null(converter.GetLength("int", 4));
        Assert.Null(converter.GetLength("bit", 1));

        // Tal cual llega, incluido -1 (= máxima).
        Assert.Equal(100, converter.GetLength("varchar", 100));
        Assert.Equal(-1, converter.GetLength("varchar", -1));

        // La mitad (tipos Unicode), conservando -1.
        Assert.Equal(50, converter.GetLength("nvarchar", 100));
        Assert.Equal(4, converter.GetLength("nchar", 9));
        Assert.Equal(-1, converter.GetLength("nvarchar", -1));

        // Los alias de SQL Server del SPEC §8.1 resuelven por su propia entrada.
        Assert.Equal(typeof(decimal), converter.NativeType2CLRType("numeric"));
        Assert.Equal(typeof(DateTime), converter.NativeType2CLRType("smalldatetime"));
        Assert.Equal(typeof(string), converter.NativeType2CLRType("char"));
        Assert.Equal(typeof(string), converter.NativeType2CLRType("nchar"));
    }

    [Fact]
    public void An_unknown_native_type_raises_the_contract_error_and_never_coerces_silently()
    {
        var converter = new MSSQLTypeConverter();

        Assert.False(converter.NativeTypeExist("rowversion"));
        Assert.False(converter.NativeTypeExist("not_a_sql_type"));

        Assert.Throws<UnknownDBTypeExeception>(() => converter.NativeType2CLRType("not_a_sql_type"));
        Assert.Throws<UnknownDBTypeExeception>(() => converter.NativeType2CLRType(null!));
        Assert.Throws<UnknownDBTypeExeception>(() => converter.GetLength("not_a_sql_type", 10));
        Assert.Throws<UnknownDBTypeExeception>(() => converter.GetLength(null!, 10));

        // La excepción es la del contrato del paquete (no una de BCL).
        Assert.Equal(typeof(Exception), typeof(UnknownDBTypeExeception).BaseType!.BaseType);
    }

    [Fact]
    public void There_is_a_single_declaration_of_the_mapping_in_the_delivered_tree()
    {
        // AC-5: en el árbol entregado hay UNA sola declaración del mapeo —la tabla
        // portable de `Modeling`—: ningún otro proyecto del árbol la redefine (la del
        // reverse relocalizado se verifica cuando la fase D se materialice, §D.3).
        var sources = Directory
            .EnumerateFiles(Repo.InTree("src"), "*.cs", SearchOption.AllDirectories)
            .Where(file => !file.Split(Path.DirectorySeparatorChar).Any(segment => segment is "bin" or "obj"))
            .ToArray();

        Assert.Contains(sources, file => file.EndsWith(@"Modeling\Reverse\TypeMapping.cs", StringComparison.Ordinal));

        var implementations = sources
            .Where(file => File.ReadAllText(file).Contains(": IDBTypeConverter", StringComparison.Ordinal))
            .Select(file => Path.GetRelativePath(Repo.InTree(), file))
            .ToArray();

        Assert.Equal([@"src\Cooperator.Framework.Modeling\Reverse\TypeMapping.cs"], implementations);

        var declarations = sources
            .Where(file => File.ReadAllText(file).Contains("NativeType2CLRType", StringComparison.Ordinal))
            .Select(file => Path.GetRelativePath(Repo.InTree(), file))
            .ToArray();

        Assert.All(declarations, file => Assert.StartsWith(@"src\Cooperator.Framework.Modeling\", file, StringComparison.Ordinal));
    }

    [Fact]
    public void The_converter_is_the_one_the_contract_exposes()
    {
        // El contrato portable del reverse es lo que el motor consume (ADR-003 §3.5):
        // la implementación tiene que responder por la interfaz, no por un tipo propio.
        Assert.IsAssignableFrom<IDBTypeConverter>(new MSSQLTypeConverter());

        var contract = typeof(IDBTypeConverter).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Select(method => method.Name)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(["GetLength", "NativeType2CLRType", "NativeTypeExist"], contract);
    }
}

using Cooperator.Framework.Library.TaxIDVerification;

namespace Cooperator.Framework.Characterization.Tests.TaxId;

/// <summary>
/// Área 4 del SPEC §6.E: validación de CUIT argentina
/// (<c>Library.TaxIDVerification.Argentina.IsValidCuit</c>).
///
/// Todos los números de los casos son <b>sintéticos</b> (SPEC §10). Los casos
/// registran el comportamiento OBSERVADO del 1.4 congelado. Los que documentan
/// una aceptación discutible —el CUIT todo-ceros, la obligatoriedad de los
/// separadores— se marcan aquí como hallazgo para escalar (G04): este V-Bounce
/// NO los corrige ni los aprueba.
/// </summary>
public sealed class ArgentinaCuitTests
{
    // Verificación del dígito verificador con la clave DGI (pesos 2..7 cíclicos):
    // 20-12345678-6 y 27-31123456-6 son válidos; el resto de esta lista no lo es.
    private const string ValidCuit = "20-12345678-6";

    [Theory]
    [InlineData(null, true, true)]
    [InlineData(null, false, false)]
    [InlineData("", true, true)]
    [InlineData("", false, false)]
    [InlineData("   ", true, true)]
    [InlineData("   ", false, false)]
    [InlineData("  -        -", true, true)]
    [InlineData("  -        -", false, false)]
    public void A_null_or_blank_value_returns_the_allow_null_argument(string? cuit, bool allowNull, bool expected)
    {
        Assert.Equal(expected, Argentina.IsValidCuit(cuit!, allowNull));
    }

    [Fact]
    public void A_valid_cuit_with_the_documented_separators_is_accepted()
    {
        Assert.True(Argentina.IsValidCuit(ValidCuit, false));
    }

    [Fact]
    public void A_cuit_whose_check_digit_is_wrong_is_rejected()
    {
        Assert.False(Argentina.IsValidCuit("20-12345678-7", false));
        Assert.False(Argentina.IsValidCuit("20-12345678-0", false));
    }

    [Fact]
    public void The_same_cuit_without_separators_is_rejected_because_the_length_must_be_exactly_thirteen()
    {
        // Comportamiento OBSERVADO: la función no normaliza el formato; exige el
        // patrón 99-99999999-9 con longitud exacta de 13 caracteres.
        Assert.False(Argentina.IsValidCuit("20123456786", false));
        Assert.False(Argentina.IsValidCuit("20123456786", true));
    }

    [Theory]
    [InlineData("20-1234567-6")]   // 12 caracteres
    [InlineData("20-123456789-6")] // 14 caracteres
    [InlineData("2-12345678-6")]   // dígito de tipo incompleto
    [InlineData("")]               // vacío con allowNull en false: ya cubierto arriba
    public void A_cuit_with_a_length_other_than_thirteen_is_rejected(string cuit)
    {
        Assert.False(Argentina.IsValidCuit(cuit, false));
    }

    [Fact]
    public void Any_embedded_space_invalidates_the_cuit()
    {
        Assert.False(Argentina.IsValidCuit("20-1234 678-6", false));
        Assert.False(Argentina.IsValidCuit(" 0-12345678-6", false));
    }

    [Fact]
    public void An_all_zero_cuit_is_accepted_because_the_check_digit_computes_to_zero()
    {
        // HALLAZGO (F-B3, se escala — no se corrige aquí): el algoritmo acepta
        // 00-00000000-0, que no es un CUIT emitible. El caso lo deja registrado
        // para que quien decida la política del migrador lo vea.
        Assert.True(Argentina.IsValidCuit("00-00000000-0", false));
        Assert.False(Argentina.IsValidCuit("00-00000000-1", false));
    }

    [Fact]
    public void A_non_numeric_character_in_the_key_positions_throws_a_format_exception()
    {
        // Observado: la función convierte cada dígito con Convert.ToInt32 y no
        // valida el alfabeto, así que el error sale como FormatException.
        Assert.Throws<FormatException>(() => Argentina.IsValidCuit("AB-12345678-6", false));
    }

    [Fact]
    public void The_check_digit_rule_is_the_one_the_dgi_key_defines()
    {
        // Los pesos son 2,3,4,5,6,7,2,3,4,5 sobre los diez dígitos de la clave
        // (tipo + número), y el verificador es (suma * 10) % 11.
        const string prefix = "23";
        const string body = "45678901";
        var key = prefix + body;

        var sum = 0;
        var weight = 2;
        for (var index = key.Length - 1; index >= 0; index--)
        {
            sum += (key[index] - '0') * weight;
            weight = weight == 7 ? 2 : weight + 1;
        }

        var check = (sum * 10) % 11;

        Assert.True(Argentina.IsValidCuit(prefix + "-" + body + "-" + check, false));
        Assert.False(Argentina.IsValidCuit(prefix + "-" + body + "-" + ((check + 1) % 10), false));
    }

    [Fact]
    public void A_rejected_cuit_is_never_accepted_by_changing_only_the_allow_null_argument()
    {
        Assert.False(Argentina.IsValidCuit("20-12345678-7", true));
        Assert.True(Argentina.IsValidCuit(ValidCuit, true));
    }
}

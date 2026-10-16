using Cooperator.Framework.Characterization.Tests.Support;
using Cooperator.Framework.Library;
using LibraryCrypto = Cooperator.Framework.Library.Cryptography;
using Cooperator.Framework.Library.Exceptions;

namespace Cooperator.Framework.Characterization.Tests.Cryptography;

/// <summary>
/// Ãrea 3 del SPEC Â§6.E: cifrado de <c>Library.Cryptography</c>.
///
/// Los casos se apoyan en los <b>vectores heredados</b> versionados en
/// <c>tests/fixtures/legacy-crypto/crypto-vectors.json</c>, producidos UNA VEZ
/// con el build 1.4 congelado: son el insumo que ADR-007 necesita para migrar
/// valores <b>ya cifrados</b>. El material de clave del 1.4 no se copia a ningÃºn
/// fixture (SPEC Â§10) y este caso lo verifica.
/// </summary>
public sealed class LegacyCryptographyTests
{
    [Fact]
    public void The_fixture_declares_its_provenance_and_carries_no_key_material()
    {
        var root = Context.InTree("tests", "fixtures", "legacy-crypto", "crypto-vectors.json");
        var text = File.ReadAllText(root);

        Assert.Contains("1.4.5.0", text, StringComparison.Ordinal);
        Assert.DoesNotContain("*1Am|9Kh", text, StringComparison.Ordinal);
        Assert.True(Context.Fixture("legacy-crypto", "crypto-vectors.json").GetProperty("plaintexts_are_synthetic").GetBoolean());
    }

    [Fact]
    public void Every_legacy_ciphertext_decrypts_to_its_expected_plaintext()
    {
        foreach (var vector in Vectors().Where(vector => !vector.Plaintext.Contains("?", StringComparison.Ordinal)))
        {
            var observed = LibraryCrypto.Decrypt(vector.Ciphertext);
            Assert.Equal(vector.RoundTrip, observed);
        }
    }

    [Fact]
    public void Encrypt_reproduces_the_legacy_ciphertext_byte_for_byte()
    {
        // La propiedad clave para migrar valores ya cifrados: con la MISMA clave y
        // el mismo IV fijo del 1.4, el texto cifrado es reproducible.
        foreach (var vector in Vectors())
        {
            Assert.Equal(vector.Ciphertext, LibraryCrypto.Encrypt(vector.Plaintext));
        }
    }

    [Fact]
    public void Encrypt_is_deterministic_because_the_1_4_uses_a_fixed_initialization_vector()
    {
        Assert.Equal(LibraryCrypto.Encrypt("Cooperator"), LibraryCrypto.Encrypt("Cooperator"));
        Assert.Equal(
            LibraryCrypto.Encrypt("Cooperator", "clave-de-prueba"),
            LibraryCrypto.Encrypt("Cooperator", "clave-de-prueba"));
    }

    [Fact]
    public void The_round_trip_closes_for_every_ascii_plaintext()
    {
        foreach (var plaintext in new[] { "A", "Cooperator", "Precio unitario: 1.234,56", "0123456789" })
        {
            Assert.Equal(plaintext, LibraryCrypto.Decrypt(LibraryCrypto.Encrypt(plaintext)));
        }
    }

    [Fact]
    public void Non_ascii_plaintexts_lose_their_accents_because_the_1_4_encodes_as_ascii()
    {
        // Comportamiento OBSERVADO, no juzgado: Encoding.ASCII sustituye por '?'.
        var plaintext = "acentuado " + (char)0x00C1 + (char)0x00F1;

        var ciphertext = LibraryCrypto.Encrypt(plaintext);
        var observed = LibraryCrypto.Decrypt(ciphertext);

        Assert.Equal("acentuado ??", observed);
        Assert.Equal(LibraryCrypto.Encrypt("acentuado ??"), ciphertext);
    }

    [Fact]
    public void The_empty_string_encrypts_to_the_block_padded_value_and_round_trips_to_empty()
    {
        var ciphertext = LibraryCrypto.Encrypt(string.Empty);

        Assert.Equal("/lLYac13koU=", ciphertext);
        Assert.Equal(string.Empty, LibraryCrypto.Decrypt(ciphertext));
    }

    [Fact]
    public void Newlines_survive_the_round_trip()
    {
        var plaintext = "linea1" + (char)10 + "linea2";

        Assert.Equal(plaintext, LibraryCrypto.Decrypt(LibraryCrypto.Encrypt(plaintext)));
    }

    [Fact]
    public void The_ciphertext_is_base64_whose_decoded_length_is_a_multiple_of_the_block_size()
    {
        foreach (var plaintext in new[] { "A", "Cooperator", "0123456789" })
        {
            var bytes = Convert.FromBase64String(LibraryCrypto.Encrypt(plaintext));
            Assert.Equal(0, bytes.Length % 8);
        }
    }

    [Fact]
    public void Encrypt_rejects_a_null_plaintext_with_the_exception_the_framework_raises()
    {
        Assert.Throws<ArgumentNullException>(() => LibraryCrypto.Encrypt(null!));
    }

    [Fact]
    public void Decrypt_rejects_a_non_base64_payload_with_the_unwrapped_format_exception()
    {
        // Observado: Decrypt sÃ³lo envuelve CryptographicException; el error de
        // formato de base64 escapa tal cual.
        Assert.Throws<FormatException>(() => LibraryCrypto.Decrypt("esto no es base64"));
    }

    [Fact]
    public void Decrypt_reports_a_wrapped_crypto_error_when_the_block_padding_is_wrong()
    {
        var ciphertext = LibraryCrypto.Encrypt("Cooperator");

        var exception = Assert.Throws<CryptoErrorException>(() => LibraryCrypto.Decrypt(ciphertext, "otra-clave"));

        Assert.NotNull(exception.InnerException);
        Assert.IsAssignableFrom<System.Security.Cryptography.CryptographicException>(exception.InnerException);
    }

    [Fact]
    public void Decrypt_with_a_different_key_does_not_silently_return_the_original_plaintext()
    {
        var ciphertext = LibraryCrypto.Encrypt("Cooperator", "clave-uno");

        string? observed = null;
        try
        {
            observed = LibraryCrypto.Decrypt(ciphertext, "clave-dos");
        }
        catch (CryptoErrorException)
        {
            // Camino observado admitido: el padding no valida.
        }

        Assert.NotEqual("Cooperator", observed);
    }

    [Fact]
    public void A_tampered_ciphertext_does_not_decrypt_to_the_original_plaintext()
    {
        var ciphertext = LibraryCrypto.Encrypt("Cooperator");
        var bytes = Convert.FromBase64String(ciphertext);
        bytes[0] ^= 0xFF;
        var tampered = Convert.ToBase64String(bytes);

        string? observed = null;
        try
        {
            observed = LibraryCrypto.Decrypt(tampered);
        }
        catch (CryptoErrorException)
        {
            // Camino observado admitido.
        }

        Assert.NotEqual("Cooperator", observed);
    }

    [Fact]
    public void The_key_derivation_is_stable_across_calls_for_the_same_key_text()
    {
        Assert.Equal(
            LibraryCrypto.Encrypt("valor-protegido", "clave-estable"),
            LibraryCrypto.Encrypt("valor-protegido", "clave-estable"));
        Assert.NotEqual(
            LibraryCrypto.Encrypt("valor-protegido", "clave-estable"),
            LibraryCrypto.Encrypt("valor-protegido", "clave-distinta"));
    }

    private static IReadOnlyList<Vector> Vectors()
    {
        var root = Context.Fixture("legacy-crypto", "crypto-vectors.json");
        return
        [
            .. root.GetProperty("vectors").EnumerateArray().Select(vector => new Vector(
                vector.GetProperty("plaintext").GetString() ?? string.Empty,
                vector.GetProperty("ciphertext").GetString() ?? string.Empty,
                vector.GetProperty("round_trip").GetString() ?? string.Empty)),
        ];
    }

    private sealed class Vector
    {
        public Vector(string plaintext, string ciphertext, string roundTrip)
        {
            Plaintext = plaintext;
            Ciphertext = ciphertext;
            RoundTrip = roundTrip;
        }

        public string Plaintext { get; }

        public string Ciphertext { get; }

        public string RoundTrip { get; }
    }
}


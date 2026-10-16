using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cooperator.Framework.Library.Cryptography.ProtectedValues;

namespace Cooperator.Harness.Tests;

/// <summary>
/// AC-2, AC-3, AC-6 y AC-10 del SPEC-261016-0035 §7 (NFR <i>Protected value integrity</i> de ADR-007 §5):
/// <b>la integridad precede a cualquier uso</b>, el <b>AAD</b> ata el valor a su clave de configuración, el
/// <b>nonce</b> es aleatorio por operación y <b>ningún</b> diagnóstico filtra el valor ni el material.
/// <para>
/// Las claves se generan <b>en memoria</b> por corrida: el repositorio no lleva material de clave (ADR-007 §5).
/// </para>
/// </summary>
[Collection(ConfigurationCollection.Name)]
public sealed class ProtectedValueIntegrityTests
{
    private const string ConfigurationKey = "Cooperator:ConnectionStrings:Primary";

    [Fact]
    public void A_protected_value_round_trips_and_the_composition_is_the_published_one()
    {
        byte[] key = ProtectedValueTestSupport.NewKey();
        ProtectedValueKeyRegistry keys = ProtectedValueTestSupport.Registry((ProtectedValueTestSupport.KeyOne, key));
        keys.SetActive(ProtectedValueTestSupport.KeyOne);
        AesGcmValueResolver resolver = new AesGcmValueResolver(keys);

        const string value = "Server=.;Database=Ventas;Integrated Security=SSPI";
        string protectedValue = resolver.Protect(ConfigurationKey, value);

        // Composición publicada: aesgcm:<kid>:<base64> con nonce (12 B) | tag (16 B) | ciphertext.
        Assert.StartsWith("aesgcm" + ":", protectedValue, StringComparison.Ordinal);
        ProtectedValuePayload payload = ProtectedValueFormat.ParsePayload(ConfigurationKey, protectedValue.Substring("aesgcm".Length + 1));
        Assert.Equal(ProtectedValueTestSupport.KeyOne, payload.Kid);
        Assert.Equal(12, payload.Nonce.Length);
        Assert.Equal(16, payload.Tag.Length);
        Assert.Equal(Encoding.UTF8.GetByteCount(value), payload.Ciphertext.Length);

        // Y resolver devuelve el mismo valor (idempotente, sin estado).
        Assert.Equal(value, resolver.Resolve(ConfigurationKey, protectedValue.Substring("aesgcm".Length + 1)));
        Assert.Equal(value, resolver.Resolve(ConfigurationKey, protectedValue.Substring("aesgcm".Length + 1)));
    }

    [Fact]
    public void A_flipped_bit_in_the_ciphertext_is_detected_and_the_message_does_not_leak_the_value()
    {
        byte[] key = ProtectedValueTestSupport.NewKey();
        ProtectedValueKeyRegistry keys = ProtectedValueTestSupport.Registry((ProtectedValueTestSupport.KeyOne, key));
        keys.SetActive(ProtectedValueTestSupport.KeyOne);
        AesGcmValueResolver resolver = new AesGcmValueResolver(keys);

        const string value = "top-secret-connection-material";
        string protectedValue = resolver.Protect(ConfigurationKey, value);
        string tampered = ProtectedValueTestSupport.Tamper(ConfigurationKey, protectedValue, byteIndex: 0, inTag: false);

        ProtectedValueIntegrityException exception = Assert.Throws<ProtectedValueIntegrityException>(
            () => resolver.Resolve(ConfigurationKey, tampered.Substring("aesgcm".Length + 1)));

        Assert.Equal(ConfigurationKey, exception.ConfigurationKey);
        Assert.Equal(ProtectedValueTestSupport.KeyOne, exception.Kid);
        Assert.DoesNotContain(value, exception.Message, StringComparison.Ordinal);
        Assert.DoesNotContain(Convert.ToBase64String(key), exception.Message, StringComparison.Ordinal);
        Assert.DoesNotContain(protectedValue, exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_flipped_bit_in_the_tag_and_a_truncated_tag_are_detected()
    {
        byte[] key = ProtectedValueTestSupport.NewKey();
        ProtectedValueKeyRegistry keys = ProtectedValueTestSupport.Registry((ProtectedValueTestSupport.KeyOne, key));
        keys.SetActive(ProtectedValueTestSupport.KeyOne);
        AesGcmValueResolver resolver = new AesGcmValueResolver(keys);

        string protectedValue = resolver.Protect(ConfigurationKey, "un valor");
        string payload = protectedValue.Substring("aesgcm".Length + 1);

        Assert.Throws<ProtectedValueIntegrityException>(
            () => resolver.Resolve(ConfigurationKey, ProtectedValueTestSupport.Tamper(ConfigurationKey, protectedValue, byteIndex: 5, inTag: true).Substring("aesgcm".Length + 1)));

        // Tag truncado: por debajo del mínimo publicado (12 + 16) no hay intento de descifrado.
        Assert.Throws<ProtectedValueIntegrityException>(
            () => resolver.Resolve(ConfigurationKey, ProtectedValueTestSupport.TruncateTo(ConfigurationKey, protectedValue, 20).Substring("aesgcm".Length + 1)));

        // Base64 inválido y carga vacía: la forma publicada no se cumple.
        Assert.Throws<ProtectedValueIntegrityException>(
            () => resolver.Resolve(ConfigurationKey, ProtectedValueTestSupport.CorruptBase64(protectedValue).Substring("aesgcm".Length + 1)));
        Assert.Throws<ProtectedValueIntegrityException>(() => resolver.Resolve(ConfigurationKey, string.Empty));
        Assert.Throws<ProtectedValueKeyNotFoundException>(() => resolver.Resolve(ConfigurationKey, "sinreferencia"));
    }

    [Fact]
    public void The_AAD_binds_the_value_to_its_configuration_key()
    {
        byte[] key = ProtectedValueTestSupport.NewKey();
        ProtectedValueKeyRegistry keys = ProtectedValueTestSupport.Registry((ProtectedValueTestSupport.KeyOne, key));
        keys.SetActive(ProtectedValueTestSupport.KeyOne);
        AesGcmValueResolver resolver = new AesGcmValueResolver(keys);

        string protectedValue = resolver.Protect("Cooperator:ConnectionStrings:Primary", "un valor");
        string payload = protectedValue.Substring("aesgcm".Length + 1);

        // El mismo valor, resuelto como si perteneciera a OTRA clave de configuración: falla de forma explícita en
        // lugar de "funcionar" con la credencial equivocada (ADR-007 §3.1).
        ProtectedValueIntegrityException exception = Assert.Throws<ProtectedValueIntegrityException>(
            () => resolver.Resolve("Cooperator:ConnectionStrings:Other", payload));

        Assert.Equal("Cooperator:ConnectionStrings:Other", exception.ConfigurationKey);
        Assert.Contains("Cooperator:ConnectionStrings:Other", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Thousand_protections_produce_thousand_distinct_nonces()
    {
        byte[] key = ProtectedValueTestSupport.NewKey();
        ProtectedValueKeyRegistry keys = ProtectedValueTestSupport.Registry((ProtectedValueTestSupport.KeyOne, key));
        keys.SetActive(ProtectedValueTestSupport.KeyOne);
        AesGcmValueResolver resolver = new AesGcmValueResolver(keys);

        const string value = "el mismo valor con la misma clave";
        HashSet<string> nonces = new HashSet<string>(StringComparer.Ordinal);
        byte[] zeros = new byte[12];

        for (int index = 0; index < 1000; index++)
        {
            string payload = resolver.Protect(ConfigurationKey, value).Substring("aesgcm".Length + 1);
            ProtectedValuePayload parsed = ProtectedValueFormat.ParsePayload(ConfigurationKey, payload);
            nonces.Add(Convert.ToBase64String(parsed.Nonce));

            // No constante (nunca el bloque de ceros) y no derivado del texto plano.
            Assert.False(parsed.Nonce.SequenceEqual(zeros), "El nonce no puede ser constante.");
        }

        // AC-6: N protecciones ⇒ N nonces distintos. Un payload repetido es un fallo, no una optimización.
        Assert.Equal(1000, nonces.Count);
    }

    [Fact]
    public void Edge_values_round_trip_without_loss()
    {
        byte[] key = ProtectedValueTestSupport.NewKey();
        ProtectedValueKeyRegistry keys = ProtectedValueTestSupport.Registry((ProtectedValueTestSupport.KeyOne, key));
        keys.SetActive(ProtectedValueTestSupport.KeyOne);
        AesGcmValueResolver resolver = new AesGcmValueResolver(keys);

        foreach (string value in new[] { string.Empty, "acentuación y símbolos: ñ, ü, 漢字", new string('x', 8192) })
        {
            string protectedValue = resolver.Protect(ConfigurationKey, value);
            Assert.Equal(value, resolver.Resolve(ConfigurationKey, protectedValue.Substring("aesgcm".Length + 1)));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Cooperator.Framework.Library.Cryptography.ProtectedValues;

namespace Cooperator.Harness.Tests;

/// <summary>
/// Apoyo de las pruebas de valores protegidos (SPEC-261016-0035 §8.1): material de clave <b>generado en memoria</b>
/// por corrida y <b>nunca</b> versionado —ni un vector fijo, ni una clave "de prueba" literal (ADR-007 §5, §14 del
/// SPEC: la regla no distingue "de prueba")—, un <b>proveedor del host</b> de prueba (el hook) y las
/// manipulaciones de carga que ejercitan el cuadro de casos negativos.
/// </summary>
internal static class ProtectedValueTestSupport
{
    internal const string KeyOne = "kidA";
    internal const string KeyTwo = "kidB";

    /// <summary>Clave de 32 bytes generada en memoria (CSPRNG del runtime): nunca un literal.</summary>
    internal static byte[] NewKey()
    {
        byte[] key = new byte[ProtectedValueFormat.KeySize];
        using (RandomNumberGenerator random = RandomNumberGenerator.Create())
        {
            random.GetBytes(key);
        }

        return key;
    }

    /// <summary>Registro de claves con una clave por referencia, provistas en memoria.</summary>
    internal static ProtectedValueKeyRegistry Registry(params (string Kid, byte[] Key)[] keys)
    {
        return new ProtectedValueKeyRegistry(new TestKeyProvider(keys));
    }

    /// <summary>Punto de registro del framework con los dos esquemas, sobre un registro de claves dado.</summary>
    internal static ProtectedValueResolverRegistry FrameworkRegistry(ProtectedValueKeyRegistry keys)
    {
        return ProtectedValueResolverRegistry.CreateForFramework(keys);
    }

    /// <summary>La carga del valor (lo que sigue al prefijo del esquema), acepte el valor completo o la carga.</summary>
    internal static string PayloadOf(string protectedValue)
    {
        string prefix = ProtectedValueFormat.AesGcmScheme + ProtectedValueFormat.SchemeSeparator;
        return protectedValue.StartsWith(prefix, StringComparison.Ordinal)
            ? protectedValue.Substring(prefix.Length)
            : protectedValue;
    }

    /// <summary>Altera un bit de un byte del payload de una carga protegida y recompone el valor.</summary>
    internal static string Tamper(string configurationKey, string protectedValue, int byteIndex, bool inTag)
    {
        ProtectedValuePayload payload = ProtectedValueFormat.ParsePayload(configurationKey, PayloadOf(protectedValue));
        byte[] bytes = new byte[payload.Nonce.Length + payload.Tag.Length + payload.Ciphertext.Length];
        Buffer.BlockCopy(payload.Nonce, 0, bytes, 0, payload.Nonce.Length);
        Buffer.BlockCopy(payload.Tag, 0, bytes, payload.Nonce.Length, payload.Tag.Length);
        Buffer.BlockCopy(payload.Ciphertext, 0, bytes, payload.Nonce.Length + payload.Tag.Length, payload.Ciphertext.Length);

        int target = inTag ? payload.Nonce.Length + byteIndex : payload.Nonce.Length + payload.Tag.Length + byteIndex;
        bytes[target] = (byte)(bytes[target] ^ 0x01);

        string base64 = Convert.ToBase64String(bytes);
        return ProtectedValueFormat.AesGcmScheme + ProtectedValueFormat.SchemeSeparator + payload.Kid +
            ProtectedValueFormat.SchemeSeparator + base64;
    }

    /// <summary>Recorta el tag de la carga: deja el mínimo publicado menos un byte.</summary>
    internal static string TruncateTo(string configurationKey, string protectedValue, int totalBytes)
    {
        ProtectedValuePayload payload = ProtectedValueFormat.ParsePayload(configurationKey, PayloadOf(protectedValue));
        byte[] bytes = new byte[payload.Nonce.Length + payload.Tag.Length + payload.Ciphertext.Length];
        Buffer.BlockCopy(payload.Nonce, 0, bytes, 0, payload.Nonce.Length);
        Buffer.BlockCopy(payload.Tag, 0, bytes, payload.Nonce.Length, payload.Tag.Length);
        Buffer.BlockCopy(payload.Ciphertext, 0, bytes, payload.Nonce.Length + payload.Tag.Length, payload.Ciphertext.Length);

        byte[] shortened = new byte[Math.Min(totalBytes, bytes.Length)];
        Buffer.BlockCopy(bytes, 0, shortened, 0, shortened.Length);

        return ProtectedValueFormat.AesGcmScheme + ProtectedValueFormat.SchemeSeparator + payload.Kid +
            ProtectedValueFormat.SchemeSeparator + Convert.ToBase64String(shortened);
    }

    /// <summary>Conserva el <c>kid</c> y reemplaza la carga por un Base64 que no lo es.</summary>
    internal static string CorruptBase64(string protectedValue)
    {
        string payloadText = PayloadOf(protectedValue);
        int separator = payloadText.IndexOf(ProtectedValueFormat.SchemeSeparator);
        string kid = separator <= 0 ? ProtectedValueTestSupport.KeyOne : payloadText.Substring(0, separator);
        return ProtectedValueFormat.AesGcmScheme + ProtectedValueFormat.SchemeSeparator + kid +
            ProtectedValueFormat.SchemeSeparator + "no-es-base64!!";
    }

    /// <summary>Proveedor del host: claves en memoria, con la posibilidad de fallar a pedido.</summary>
    internal sealed class TestKeyProvider : IProtectedValueKeyProvider
    {
        private readonly Dictionary<string, byte[]> _keys = new Dictionary<string, byte[]>(StringComparer.Ordinal);

        internal TestKeyProvider(params (string Kid, byte[] Key)[] keys)
        {
            foreach ((string kid, byte[] key) in keys)
            {
                _keys[kid] = key;
            }
        }

        /// <summary>Cuando se fija, el proveedor falla como lo haría un almacén del host inaccesible.</summary>
        internal Exception? Failure { get; set; }

        public byte[]? TryGetKey(string kid)
        {
            if (Failure != null)
            {
                throw Failure;
            }

            return _keys.TryGetValue(kid, out byte[]? key) ? key : null;
        }
    }
}

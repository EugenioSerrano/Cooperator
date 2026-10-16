using System;
using System.Text;

namespace Cooperator.Framework.Library.Cryptography.ProtectedValues
{
    /// <summary>
    /// El <b>formato publicado</b> del valor protegido del framework (ADR-007 §3.1, SPEC-261016-0035 §6/A.2).
    /// <para>
    /// <b>Contrato del paquete.</b> El valor protegido es
    /// <c>aesgcm:&lt;kid&gt;:&lt;base64&gt;</c>, donde la carga Base64 es
    /// <c>nonce (12 B) | tag (16 B) | ciphertext</c> <b>en ese orden</b>, el <c>kid</c> identifica la clave y la
    /// clave de entorno se aporta como <b>32 bytes codificados en Base64</b>. Estos tamaños y el orden son
    /// contrato para la herramienta de migración y rotación (Bolt 023) y para quien necesite <b>reproducir</b> el
    /// formato — nunca para reimplementar el mecanismo: la verificación de integridad es de
    /// <see cref="AesGcmValueResolver"/> y se hace <b>antes</b> de usar el valor.
    /// </para>
    /// <para>
    /// Este tipo parsea y compone; <b>no</b> cifra. Un error de forma es
    /// <see cref="ProtectedValueIntegrityException"/> (el valor no tiene la forma publicada y no se resuelve).
    /// </para>
    /// </summary>
    public static class ProtectedValueFormat
    {
        /// <summary>Esquema de identidad (texto plano explícito).</summary>
        public const string PlainScheme = "plain";

        /// <summary>Esquema del mecanismo del framework.</summary>
        public const string AesGcmScheme = "aesgcm";

        /// <summary>Separador entre el esquema y su carga, y entre el <c>kid</c> y la carga protegida.</summary>
        public const char SchemeSeparator = ':';

        /// <summary>Tamaño del nonce, en bytes (aleatorio por operación).</summary>
        public const int NonceSize = 12;

        /// <summary>Tamaño del tag de autenticación, en bytes (verificado al resolver).</summary>
        public const int TagSize = 16;

        /// <summary>Tamaño de la clave, en bytes (32 bytes, aportada en Base64).</summary>
        public const int KeySize = 32;

        /// <summary>Prefijo de la variable de entorno por defecto de una clave: <c>Cooperator__ProtectedValues__Keys__&lt;kid&gt;</c>.</summary>
        public const string EnvironmentKeyPrefix = "Cooperator__ProtectedValues__Keys__";

        /// <summary>El <c>kid</c> del valor es <c>[A-Za-z0-9._-]{1,64}</c>: lo demás se rechaza con error tipado.</summary>
        private const string KidAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._-";

        /// <summary>
        /// Compone el valor protegido publicado a partir de sus partes ya cifradas.
        /// </summary>
        /// <param name="kid">Referencia de la clave usada.</param>
        /// <param name="nonce">Nonce de <see cref="NonceSize"/> bytes.</param>
        /// <param name="tag">Tag de <see cref="TagSize"/> bytes.</param>
        /// <param name="ciphertext">Texto cifrado.</param>
        /// <returns>El valor <c>aesgcm:&lt;kid&gt;:&lt;base64&gt;</c>.</returns>
        public static string Compose(string kid, byte[] nonce, byte[] tag, byte[] ciphertext)
        {
            if (kid == null)
            {
                throw new ArgumentNullException(nameof(kid));
            }

            if (nonce == null)
            {
                throw new ArgumentNullException(nameof(nonce));
            }

            if (tag == null)
            {
                throw new ArgumentNullException(nameof(tag));
            }

            if (ciphertext == null)
            {
                throw new ArgumentNullException(nameof(ciphertext));
            }

            if (!IsValidKid(kid))
            {
                throw new ProtectedValueIntegrityException(
                    "El 'kid' de la referencia de clave no tiene el formato admitido ([A-Za-z0-9._-], 1..64).",
                    string.Empty,
                    kid);
            }

            if (nonce.Length != NonceSize || tag.Length != TagSize)
            {
                throw new ProtectedValueIntegrityException(
                    "La composición publicada exige un nonce de " + NonceSize + " bytes y un tag de " + TagSize +
                    " bytes; no se compone una carga con otras longitudes.", string.Empty, kid);
            }

            byte[] payload = new byte[nonce.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, payload, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, payload, nonce.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, payload, nonce.Length + tag.Length, ciphertext.Length);

            return AesGcmScheme + SchemeSeparator + kid + SchemeSeparator + Convert.ToBase64String(payload);
        }

        /// <summary>
        /// Interpreta la carga del esquema <c>aesgcm:</c> —lo que sigue al prefijo— con la forma publicada
        /// <c>&lt;kid&gt;:&lt;base64&gt;</c>.
        /// </summary>
        /// <param name="configurationKey">Clave de configuración, para el diagnóstico.</param>
        /// <param name="payload">Carga tal como viaja en el valor (sin el prefijo del esquema).</param>
        /// <returns>El <c>kid</c> y las tres partes de la carga.</returns>
        /// <exception cref="ProtectedValueKeyNotFoundException">La carga no declara <c>kid</c>.</exception>
        /// <exception cref="ProtectedValueIntegrityException">La carga no tiene la forma publicada.</exception>
        public static ProtectedValuePayload ParsePayload(string configurationKey, string payload)
        {
            if (configurationKey == null)
            {
                throw new ArgumentNullException(nameof(configurationKey));
            }

            if (string.IsNullOrEmpty(payload))
            {
                throw new ProtectedValueIntegrityException(
                    "La carga del valor protegido está vacía: se espera '<kid>:<base64>'.", configurationKey, null);
            }

            int separator = payload.IndexOf(SchemeSeparator);
            if (separator <= 0)
            {
                throw new ProtectedValueKeyNotFoundException(
                    "La carga del valor protegido no declara el 'kid' de la referencia de clave: se espera" +
                    " '<kid>:<base64>'. Un valor sin referencia de clave no se resuelve con otra clave.", configurationKey, null);
            }

            string kid = payload.Substring(0, separator);
            if (!IsValidKid(kid))
            {
                throw new ProtectedValueKeyNotFoundException(
                    "El 'kid' de la referencia de clave no tiene el formato admitido ([A-Za-z0-9._-], 1..64).",
                    configurationKey, null);
            }

            string base64 = payload.Substring(separator + 1);
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(base64);
            }
            catch (FormatException exception)
            {
                throw new ProtectedValueIntegrityException(
                    "La carga del valor protegido no es Base64 válido: el valor no tiene la forma publicada.",
                    configurationKey, kid, exception);
            }

            if (bytes.Length < NonceSize + TagSize)
            {
                throw new ProtectedValueIntegrityException(
                    "La carga del valor protegido no alcanza el mínimo publicado (nonce de " + NonceSize +
                    " bytes + tag de " + TagSize + " bytes): un tag truncado no se resuelve.", configurationKey, kid);
            }

            byte[] nonce = new byte[NonceSize];
            byte[] tag = new byte[TagSize];
            byte[] ciphertext = new byte[bytes.Length - NonceSize - TagSize];
            Buffer.BlockCopy(bytes, 0, nonce, 0, NonceSize);
            Buffer.BlockCopy(bytes, NonceSize, tag, 0, TagSize);
            Buffer.BlockCopy(bytes, NonceSize + TagSize, ciphertext, 0, ciphertext.Length);

            return new ProtectedValuePayload(kid, nonce, tag, ciphertext);
        }

        /// <summary>
        /// Separa el esquema de su carga. La <b>forma</b> del prefijo es del contrato de configuración (Bolt 011,
        /// ADR-004 §3.3): acá sólo se separa para que el punto de registro despache por esquema.
        /// </summary>
        /// <param name="rawValue">Valor crudo, con su prefijo.</param>
        /// <param name="scheme">Esquema declarado, si lo hay.</param>
        /// <param name="payload">Carga, si hay prefijo.</param>
        /// <returns><c>true</c> cuando el valor declara un prefijo de esquema.</returns>
        public static bool TrySplitScheme(string? rawValue, out string scheme, out string payload)
        {
            scheme = string.Empty;
            payload = string.Empty;

            if (string.IsNullOrEmpty(rawValue))
            {
                return false;
            }

            int separator = rawValue!.IndexOf(SchemeSeparator);
            if (separator <= 0)
            {
                return false;
            }

            scheme = rawValue.Substring(0, separator);
            payload = rawValue.Substring(separator + 1);
            return true;
        }

        /// <summary>
        /// Indica si el <c>kid</c> tiene el formato admitido: <c>[A-Za-z0-9._-]</c>, entre 1 y 64 caracteres (el
        /// separador del valor queda fuera del alfabeto para que el formato no sea ambiguo).
        /// </summary>
        /// <param name="kid">Referencia de clave candidata.</param>
        /// <returns><c>true</c> si el formato es admisible.</returns>
        public static bool IsValidKid(string? kid)
        {
            if (string.IsNullOrEmpty(kid) || kid!.Length > 64)
            {
                return false;
            }

            for (int index = 0; index < kid.Length; index++)
            {
                if (KidAlphabet.IndexOf(kid[index]) < 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Decodifica una clave aportada en Base64 y valida el tamaño publicado.</summary>
        /// <param name="reference">Referencia de la clave (el <c>kid</c> o el nombre de la variable), para el diagnóstico.</param>
        /// <param name="base64Key">La clave en Base64.</param>
        /// <returns>Los 32 bytes de la clave.</returns>
        /// <exception cref="ProtectedValueKeyProviderException">No es Base64 válido o no mide <see cref="KeySize"/> bytes.</exception>
        public static byte[] DecodeKey(string reference, string base64Key)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            if (string.IsNullOrWhiteSpace(base64Key))
            {
                throw new ProtectedValueKeyProviderException(
                    "La clave de la referencia '" + reference + "' está vacía: se espera una clave de " + KeySize +
                    " bytes codificada en Base64.", string.Empty, reference);
            }

            byte[] key;
            try
            {
                key = Convert.FromBase64String(base64Key.Trim());
            }
            catch (FormatException exception)
            {
                throw new ProtectedValueKeyProviderException(
                    "La clave de la referencia '" + reference + "' no es Base64 válido: se espera una clave de " +
                    KeySize + " bytes codificada en Base64.", string.Empty, reference, exception);
            }

            if (key.Length != KeySize)
            {
                throw new ProtectedValueKeyProviderException(
                    "La clave de la referencia '" + reference + "' mide " + key.Length + " bytes y la política exige " +
                    KeySize + ": no se recorta, no se deriva y no se completa con ceros.", string.Empty, reference);
            }

            return key;
        }

        internal static string Describe(byte[] bytes)
        {
            return bytes == null ? string.Empty : Encoding.UTF8.GetString(bytes);
        }
    }
}

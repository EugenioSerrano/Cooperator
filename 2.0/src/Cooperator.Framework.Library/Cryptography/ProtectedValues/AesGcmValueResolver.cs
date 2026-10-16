using System;
using System.Security.Cryptography;
using System.Text;
using Cooperator.Framework.Core.Configuration;

namespace Cooperator.Framework.Library.Cryptography.ProtectedValues
{
    /// <summary>
    /// El mecanismo del framework: <b>AES-GCM de 256 bits</b> con <b>nonce aleatorio por operación</b>,
    /// <b>tag verificado antes de usar el valor</b> y <b>AAD = la clave de configuración</b> que contiene el valor
    /// (ADR-007 §3.1, SPEC-261016-0035 §6/B).
    /// <list type="bullet">
    ///   <item><b>Proteger exige la referencia de clave</b> (<see cref="Protect"/> recibe la clave de configuración y
    ///         usa la clave <b>activa</b> del registro): no existe sobrecarga con clave implícita — es exactamente lo
    ///         que el retiro de la superficie estática elimina (AC-1).</item>
    ///   <item><b>Resolver</b> parsea <c>&lt;kid&gt;:&lt;base64&gt;</c>, toma la clave de <b>lectura</b> de ese
    ///         <c>kid</c> y verifica el tag <b>antes</b> de devolver nada: un bit alterado, un tag truncado o un AAD
    ///         de otra clave de configuración producen <see cref="ProtectedValueIntegrityException"/> y <b>ningún</b>
    ///         valor (AC-2, AC-3).</item>
    ///   <item>Sin comparación de tags propia (la hace la plataforma), sin primitivas prohibidas (MD5/SHA-1/DES/3DES/
    ///         RC2/ECB/`*CryptoServiceProvider` concretos) y sin ningún camino de degradación a texto plano.</item>
    /// </list>
    /// </summary>
    public sealed class AesGcmValueResolver : IProtectedValueResolver
    {
        /// <summary>Nombre del esquema que este resolutor atiende.</summary>
        public const string SchemeName = ProtectedValueFormat.AesGcmScheme;

        private readonly ProtectedValueKeyRegistry _keys;

        /// <summary>Inicializa el resolutor con el registro de claves (activa + lectura).</summary>
        /// <param name="keys">Registro de claves.</param>
        public AesGcmValueResolver(ProtectedValueKeyRegistry keys)
        {
            _keys = keys ?? throw new ArgumentNullException(nameof(keys));
        }

        /// <inheritdoc />
        public string Scheme => SchemeName;

        /// <summary>Registro de claves con el que se protege y se resuelve.</summary>
        public ProtectedValueKeyRegistry Keys => _keys;

        /// <summary>
        /// Protege un valor con la clave <b>activa</b> del registro, para la clave de configuración indicada (que
        /// viaja como <b>AAD</b> y ata el valor a su slot).
        /// </summary>
        /// <param name="configurationKey">Clave de configuración que va a contener el valor (el AAD).</param>
        /// <param name="value">Valor a proteger (se trata como texto).</param>
        /// <returns>El valor protegido <c>aesgcm:&lt;kid&gt;:&lt;base64&gt;</c>.</returns>
        public string Protect(string configurationKey, string value)
        {
            if (configurationKey == null)
            {
                throw new ArgumentNullException(nameof(configurationKey));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            string? kid = _keys.ActiveKid;
            if (kid == null)
            {
                throw new ProtectedValueKeyNotFoundException(
                    "Proteger exige la referencia explícita de clave: no hay clave ACTIVA fijada en el registro.",
                    configurationKey, null);
            }

            byte[] key = _keys.RequireActiveKey();
            byte[] nonce = new byte[ProtectedValueFormat.NonceSize];
            byte[] tag = new byte[ProtectedValueFormat.TagSize];
            byte[] plaintext = Encoding.UTF8.GetBytes(value);
            byte[] ciphertext = new byte[plaintext.Length];
            byte[] associatedData = Encoding.UTF8.GetBytes(configurationKey);

            try
            {
                using (RandomNumberGenerator random = RandomNumberGenerator.Create())
                {
                    // Nonce ALEATORIO por operación: nunca reutilizado, nunca un contador (AC-6).
                    random.GetBytes(nonce);
                }

                using (AesGcm aes = new AesGcm(key))
                {
                    aes.Encrypt(nonce, plaintext, ciphertext, tag, associatedData);
                }

                return ProtectedValueFormat.Compose(kid, nonce, tag, ciphertext);
            }
            finally
            {
                CryptographicMaterial.Clear(key);
                CryptographicMaterial.Clear(plaintext);
            }
        }

        /// <summary>
        /// Resuelve la carga de un valor protegido del esquema <c>aesgcm:</c>, verificando la integridad y el AAD
        /// <b>antes</b> de devolver el valor.
        /// </summary>
        /// <param name="key">Clave de configuración de la que proviene el valor (el AAD con el que se protegió).</param>
        /// <param name="payload">Carga del valor: <c>&lt;kid&gt;:&lt;base64&gt;</c>.</param>
        /// <returns>El valor en claro.</returns>
        /// <exception cref="ProtectedValueIntegrityException">La carga no verifica su integridad o el AAD no corresponde.</exception>
        /// <exception cref="ProtectedValueKeyNotFoundException">La carga no declara <c>kid</c> o su clave no está disponible.</exception>
        public string Resolve(string key, string payload)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            ProtectedValuePayload parsed = ProtectedValueFormat.ParsePayload(key, payload);
            byte[] associatedData = Encoding.UTF8.GetBytes(key);
            byte[] plaintext = new byte[parsed.Ciphertext.Length];
            byte[] keyBytes = _keys.RequireReadKey(parsed.Kid);

            try
            {
                using (AesGcm aes = new AesGcm(keyBytes))
                {
                    aes.Decrypt(parsed.Nonce, parsed.Ciphertext, parsed.Tag, plaintext, associatedData);
                }

                return Encoding.UTF8.GetString(plaintext);
            }
            catch (CryptographicException exception)
            {
                // Un AEAD falla de una sola forma: no se distingue "casi correcto" de "cualquier otra cosa" y el
                // mensaje no lleva el valor ni el material (AC-2, AC-3, AC-10).
                throw new ProtectedValueIntegrityException(
                    "La integridad del valor de la clave de configuración '" + key + "' no verifica (kid '" + parsed.Kid +
                    "'): la carga fue manipulada, su tag no corresponde, o el valor no pertenece a esa clave de" +
                    " configuración (el AAD ata el valor a su slot). El valor NO se resuelve.", key, parsed.Kid, exception);
            }
            finally
            {
                CryptographicMaterial.Clear(keyBytes);
                CryptographicMaterial.Clear(plaintext);
            }
        }
    }
}

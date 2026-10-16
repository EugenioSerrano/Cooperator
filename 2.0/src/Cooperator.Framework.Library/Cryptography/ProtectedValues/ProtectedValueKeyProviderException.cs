using System;

namespace Cooperator.Framework.Library.Cryptography.ProtectedValues
{
    /// <summary>
    /// Un componente de provisión <b>del host</b> falló: su <b>proveedor de claves</b> (el hook hacia su almacén
    /// propio) o su <b>resolvedor de esquema</b>. El error indica que el fallo vino del host y lleva la
    /// <b>excepción original</b> como causa (ADR-007 §3.2, AC-8).
    /// <para>
    /// También es el error de una clave de entorno <b>mal formada</b> (no Base64, o longitud distinta de 32 bytes):
    /// la causa se nombra con la <b>referencia</b> y <b>sin</b> el material (nunca una clave recortada, derivada ni
    /// completada con ceros).
    /// </para>
    /// </summary>
    public sealed class ProtectedValueKeyProviderException : ProtectedValueException
    {
        /// <summary>Inicializa la excepción sin contexto (constructor canónico).</summary>
        public ProtectedValueKeyProviderException()
            : this(string.Empty, string.Empty, null)
        {
        }

        /// <summary>Inicializa la excepción con un mensaje (constructor canónico).</summary>
        /// <param name="message">Mensaje del error.</param>
        public ProtectedValueKeyProviderException(string message)
            : this(message, string.Empty, null)
        {
        }

        /// <summary>Inicializa la excepción con un mensaje y su causa (constructor canónico).</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="innerException">Causa original.</param>
        public ProtectedValueKeyProviderException(string message, Exception innerException)
            : this(message, string.Empty, null, innerException)
        {
        }

        /// <summary>Inicializa la excepción con su contexto permitido.</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="configurationKey">Clave de configuración que contiene el valor.</param>
        /// <param name="kid">Referencia de clave involucrada, si la hay.</param>
        public ProtectedValueKeyProviderException(string message, string configurationKey, string? kid)
            : base(message, configurationKey, kid)
        {
        }

        /// <summary>Inicializa la excepción con su contexto permitido y su causa.</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="configurationKey">Clave de configuración que contiene el valor.</param>
        /// <param name="kid">Referencia de clave involucrada, si la hay.</param>
        /// <param name="innerException">Causa original.</param>
        public ProtectedValueKeyProviderException(string message, string configurationKey, string? kid, Exception innerException)
            : base(message, configurationKey, kid, innerException)
        {
        }
    }
}

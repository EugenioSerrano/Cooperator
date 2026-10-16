using System;

namespace Cooperator.Framework.Library.Cryptography.ProtectedValues
{
    /// <summary>
    /// El valor declara un esquema que <b>no tiene resolvedor registrado</b> en el punto de registro
    /// (ADR-007 §3.1): un esquema del host que nadie registró, o uno ajeno a la política.
    /// <para>
    /// Dice <b>exactamente eso</b> —nunca un intento de descifrado a ciegas— y nombra la clave de configuración y
    /// el prefijo ofensor. Un esquema del host <b>registrado</b> se resuelve por su resolvedor; uno
    /// <b>no</b> registrado produce este error, no una degradación.
    /// </para>
    /// </summary>
    public sealed class ProtectedValueSchemeException : ProtectedValueException
    {
        /// <summary>Inicializa la excepción sin contexto (constructor canónico).</summary>
        public ProtectedValueSchemeException()
            : this(string.Empty, string.Empty, null)
        {
        }

        /// <summary>Inicializa la excepción con un mensaje (constructor canónico).</summary>
        /// <param name="message">Mensaje del error.</param>
        public ProtectedValueSchemeException(string message)
            : this(message, string.Empty, null)
        {
        }

        /// <summary>Inicializa la excepción con un mensaje y su causa (constructor canónico).</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="innerException">Causa original.</param>
        public ProtectedValueSchemeException(string message, Exception innerException)
            : this(message, string.Empty, null, innerException)
        {
        }

        /// <summary>Inicializa la excepción con su contexto permitido.</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="configurationKey">Clave de configuración que contiene el valor.</param>
        /// <param name="kid">Referencia de clave involucrada, si la hay.</param>
        public ProtectedValueSchemeException(string message, string configurationKey, string? kid)
            : base(message, configurationKey, kid)
        {
        }

        /// <summary>Inicializa la excepción con su contexto permitido y su causa.</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="configurationKey">Clave de configuración que contiene el valor.</param>
        /// <param name="kid">Referencia de clave involucrada, si la hay.</param>
        /// <param name="innerException">Causa original.</param>
        public ProtectedValueSchemeException(string message, string configurationKey, string? kid, Exception innerException)
            : base(message, configurationKey, kid, innerException)
        {
        }
    }
}

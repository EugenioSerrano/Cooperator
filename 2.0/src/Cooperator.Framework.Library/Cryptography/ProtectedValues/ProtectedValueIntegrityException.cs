using System;

namespace Cooperator.Framework.Library.Cryptography.ProtectedValues
{
    /// <summary>
    /// La <b>integridad</b> de un valor protegido no verifica: bit alterado en la carga, <b>tag truncado</b>,
    /// AAD que no corresponde a la clave de configuración declarada, o una carga que no tiene la forma publicada
    /// (Base64 inválido, longitud insuficiente).
    /// <para>
    /// <b>La integridad precede a cualquier uso</b> (ADR-007 §5, NFR <i>Protected value integrity</i>): cuando este
    /// error se produce <b>no hay valor</b>, y nunca hay degradación a texto plano ni un intento "casi correcto".
    /// </para>
    /// </summary>
    public sealed class ProtectedValueIntegrityException : ProtectedValueException
    {
        /// <summary>Inicializa la excepción sin contexto (constructor canónico).</summary>
        public ProtectedValueIntegrityException()
            : this(string.Empty, string.Empty, null)
        {
        }

        /// <summary>Inicializa la excepción con un mensaje (constructor canónico).</summary>
        /// <param name="message">Mensaje del error.</param>
        public ProtectedValueIntegrityException(string message)
            : this(message, string.Empty, null)
        {
        }

        /// <summary>Inicializa la excepción con un mensaje y su causa (constructor canónico).</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="innerException">Causa original.</param>
        public ProtectedValueIntegrityException(string message, Exception innerException)
            : this(message, string.Empty, null, innerException)
        {
        }

        /// <summary>Inicializa la excepción con su contexto permitido.</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="configurationKey">Clave de configuración que contiene el valor.</param>
        /// <param name="kid">Referencia de clave involucrada, si la hay.</param>
        public ProtectedValueIntegrityException(string message, string configurationKey, string? kid)
            : base(message, configurationKey, kid)
        {
        }

        /// <summary>Inicializa la excepción con su contexto permitido y su causa.</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="configurationKey">Clave de configuración que contiene el valor.</param>
        /// <param name="kid">Referencia de clave involucrada, si la hay.</param>
        /// <param name="innerException">Causa original.</param>
        public ProtectedValueIntegrityException(string message, string configurationKey, string? kid, Exception innerException)
            : base(message, configurationKey, kid, innerException)
        {
        }
    }
}

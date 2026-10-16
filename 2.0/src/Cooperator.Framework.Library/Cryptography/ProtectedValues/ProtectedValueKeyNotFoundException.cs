using System;

namespace Cooperator.Framework.Library.Cryptography.ProtectedValues
{
    /// <summary>
    /// La referencia de clave del valor (<b>kid</b>) está ausente, es desconocida o no tiene clave disponible.
    /// <para>
    /// <b>Nunca</b> se intenta con otra clave y <b>nunca</b> se degrada a texto plano (ADR-007 §3.2): un valor cuyo
    /// <c>kid</c> no tiene clave <b>falla</b>. La ventana de solapamiento de una rotación agrega claves de
    /// <i>lectura</i>, no adivina.
    /// </para>
    /// </summary>
    public sealed class ProtectedValueKeyNotFoundException : ProtectedValueException
    {
        /// <summary>Inicializa la excepción sin contexto (constructor canónico).</summary>
        public ProtectedValueKeyNotFoundException()
            : this(string.Empty, string.Empty, null)
        {
        }

        /// <summary>Inicializa la excepción con un mensaje (constructor canónico).</summary>
        /// <param name="message">Mensaje del error.</param>
        public ProtectedValueKeyNotFoundException(string message)
            : this(message, string.Empty, null)
        {
        }

        /// <summary>Inicializa la excepción con un mensaje y su causa (constructor canónico).</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="innerException">Causa original.</param>
        public ProtectedValueKeyNotFoundException(string message, Exception innerException)
            : this(message, string.Empty, null, innerException)
        {
        }

        /// <summary>Inicializa la excepción con su contexto permitido.</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="configurationKey">Clave de configuración que contiene el valor.</param>
        /// <param name="kid">Referencia de clave involucrada, si la hay.</param>
        public ProtectedValueKeyNotFoundException(string message, string configurationKey, string? kid)
            : base(message, configurationKey, kid)
        {
        }

        /// <summary>Inicializa la excepción con su contexto permitido y su causa.</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="configurationKey">Clave de configuración que contiene el valor.</param>
        /// <param name="kid">Referencia de clave involucrada, si la hay.</param>
        /// <param name="innerException">Causa original.</param>
        public ProtectedValueKeyNotFoundException(string message, string configurationKey, string? kid, Exception innerException)
            : base(message, configurationKey, kid, innerException)
        {
        }
    }
}

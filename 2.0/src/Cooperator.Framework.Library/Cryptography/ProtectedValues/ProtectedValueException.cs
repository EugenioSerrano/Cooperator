using System;
using Cooperator.Framework.Library.Exceptions;

namespace Cooperator.Framework.Library.Cryptography.ProtectedValues
{
    /// <summary>
    /// Base de la familia de errores de la política de valores protegidos (ADR-007 §3.1/§3.2).
    /// <para>
    /// <b>Sin filtrado (AC-10).</b> El contexto es la <b>clave de configuración</b> que contiene el valor y el
    /// <b>kid</b> de la referencia de clave: <b>nunca</b> el valor, la clave ni material derivado (ADR-007 §3.2:
    /// los reportes identifican valores por clave y <c>kid</c>, nunca por contenido).
    /// </para>
    /// <para>
    /// No deriva de <c>CooperatorConfigurationException</c> (el contrato del Bolt 011, que es <c>sealed</c>): la
    /// política criptográfica tiene su propia familia, y la <b>forma</b> del prefijo sigue produciendo el error del
    /// contrato (ADR-004 §3.3, SPEC-261016-0035 §6/A.1).
    /// </para>
    /// </summary>
    public abstract class ProtectedValueException : BaseException
    {
        /// <summary>Inicializa la excepción sin contexto (constructor canónico).</summary>
        protected ProtectedValueException()
            : this(string.Empty, string.Empty, null)
        {
        }

        /// <summary>Inicializa la excepción con un mensaje (constructor canónico).</summary>
        /// <param name="message">Mensaje del error.</param>
        protected ProtectedValueException(string message)
            : this(message, string.Empty, null)
        {
        }

        /// <summary>Inicializa la excepción con un mensaje y su causa (constructor canónico).</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="innerException">Causa original.</param>
        protected ProtectedValueException(string message, Exception innerException)
            : this(message, string.Empty, null, innerException)
        {
        }

        /// <summary>Inicializa la excepción con su contexto permitido.</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="configurationKey">Clave de configuración que contiene el valor.</param>
        /// <param name="kid">Referencia de clave (<c>kid</c>) involucrada, si la hay.</param>
        protected ProtectedValueException(string message, string configurationKey, string? kid)
            : base(message)
        {
            ConfigurationKey = configurationKey;
            Kid = kid;
        }

        /// <summary>Inicializa la excepción con su contexto permitido y su causa.</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="configurationKey">Clave de configuración que contiene el valor.</param>
        /// <param name="kid">Referencia de clave (<c>kid</c>) involucrada, si la hay.</param>
        /// <param name="innerException">Causa original.</param>
        protected ProtectedValueException(string message, string configurationKey, string? kid, Exception innerException)
            : base(message, innerException)
        {
            ConfigurationKey = configurationKey;
            Kid = kid;
        }

        /// <summary>Clave de configuración que contiene el valor (el contexto permitido del diagnóstico).</summary>
        public string ConfigurationKey { get; }

        /// <summary>Referencia de clave (<c>kid</c>) involucrada, si la hay.</summary>
        public string? Kid { get; }
    }
}

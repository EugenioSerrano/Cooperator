using System;

namespace Cooperator.Framework.Core.Configuration
{
    /// <summary>
    /// Clave de configuración inválida o ausente.
    /// <para>
    /// Es un tipo <b>propio del framework</b> (ADR-004 §3.2/§3.3, §5): <b>nunca</b> un tipo del stack de
    /// configuración clásico ni un <see cref="NullReferenceException"/>. El mensaje nombra la clave y —cuando
    /// corresponde— el prefijo ofensor.
    /// </para>
    /// </summary>
    public sealed class CooperatorConfigurationException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public CooperatorConfigurationException()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">Mensaje accionable, que nombra la clave.</param>
        public CooperatorConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">Mensaje accionable, que nombra la clave.</param>
        /// <param name="innerException">Causa.</param>
        public CooperatorConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Crea la excepción para una clave ausente que no tiene default.
        /// </summary>
        /// <param name="key">Clave ausente.</param>
        /// <returns>La excepción, con la clave en el mensaje.</returns>
        public static CooperatorConfigurationException MissingKey(string key)
        {
            return new CooperatorConfigurationException(
                "No hay valor de configuración para la clave '" + key + "'. Definila en la fuente de configuración" +
                " (por defecto, la variable de entorno correspondiente) o usá la variante con default documentado.");
        }

        /// <summary>
        /// Crea la excepción para un valor que no es un entero válido.
        /// </summary>
        /// <param name="key">Clave del valor.</param>
        /// <param name="value">Valor recibido.</param>
        /// <returns>La excepción, con la clave y el valor en el mensaje.</returns>
        public static CooperatorConfigurationException NotAnInteger(string key, string value)
        {
            return new CooperatorConfigurationException(
                "El valor de configuración de la clave '" + key + "' debe ser un entero y se recibió '" + value + "'.");
        }
    }
}

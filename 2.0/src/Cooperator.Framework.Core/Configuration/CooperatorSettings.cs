using System;

namespace Cooperator.Framework.Core.Configuration
{
    /// <summary>
    /// <b>Instancia ambiental</b> de la configuración del framework (ADR-004 §3.2).
    /// <para>
    /// El framework la lee de forma <b>perezosa</b>: si la app no fijó ninguna, se usa la implementación por
    /// defecto (<see cref="EnvironmentConfiguration"/>, sólo variables de entorno). La app la fija <b>una vez</b>
    /// al arranque; los tests y los hosts con inyección de dependencias pueden <b>reemplazarla</b>, que es la
    /// operación documentada para cambiar la configuración en caliente (ADR-004 §3.3) — ya no hay caché estática
    /// de valores que lo impida.
    /// </para>
    /// <para>
    /// El resto de la superficie del framework recibe la configuración por <b>sobrecargas explícitas</b> que
    /// aceptan un <see cref="ICooperatorConfiguration"/>; esta instancia es el default, no una imposición.
    /// </para>
    /// </summary>
    public static class CooperatorSettings
    {
        private static readonly object Gate = new object();
        private static ICooperatorConfiguration? current;

        /// <summary>
        /// Configuración vigente. Si nadie la fijó, es la implementación por defecto (sólo entorno).
        /// </summary>
        /// <exception cref="ArgumentNullException">Se intentó fijar <c>null</c>.</exception>
        public static ICooperatorConfiguration Current
        {
            get
            {
                lock (Gate)
                {
                    if (current == null)
                    {
                        current = new EnvironmentConfiguration();
                    }

                    return current;
                }
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                lock (Gate)
                {
                    current = value;
                }
            }
        }
    }
}

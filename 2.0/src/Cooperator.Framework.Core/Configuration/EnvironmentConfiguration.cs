using System;
using System.Globalization;

namespace Cooperator.Framework.Core.Configuration
{
    /// <summary>
    /// Implementación por defecto del contrato: <b>sólo variables de entorno</b> (ADR-004 §3.2).
    /// <para>
    /// Es BCL puro y la <b>única</b> fuente propia del framework: <b>no</b> lee archivos, <b>no</b> define
    /// precedencias y <b>no</b> inventa un orden de búsqueda. La jerarquía se expresa con doble guion bajo —
    /// <c>Cooperator:Data:CommandTimeoutSeconds</c> se lee de
    /// <c>Cooperator__Data__CommandTimeoutSeconds</c>— y la precedencia entorno &gt; archivo &gt; default es
    /// propiedad del bootstrap de las apps, no del framework (ADR-004 §3.4).
    /// </para>
    /// <para>
    /// Un valor vacío se trata como <b>ausente</b>: no es un valor válido (SPEC-261016-0018 §8.1).
    /// </para>
    /// </summary>
    public sealed class EnvironmentConfiguration : ICooperatorConfiguration
    {
        /// <inheritdoc />
        public string GetString(string key)
        {
            string? value = ReadValue(key);
            if (value == null)
            {
                throw CooperatorConfigurationException.MissingKey(key);
            }

            return value;
        }

        /// <inheritdoc />
        public string GetString(string key, string defaultValue)
        {
            string? value = ReadValue(key);
            return value ?? defaultValue;
        }

        /// <inheritdoc />
        public int GetInt(string key)
        {
            string? value = ReadValue(key);
            if (value == null)
            {
                throw CooperatorConfigurationException.MissingKey(key);
            }

            return ParseInteger(key, value);
        }

        /// <inheritdoc />
        public int GetInt(string key, int defaultValue)
        {
            string? value = ReadValue(key);
            if (value == null)
            {
                return defaultValue;
            }

            return ParseInteger(key, value);
        }

        /// <inheritdoc />
        public string GetConnectionString(string name)
        {
            string key = ConfigurationKeys.ConnectionStrings.For(name ?? ConfigurationKeys.ConnectionStrings.DefaultName);
            string? rawValue = ReadValue(key);
            if (rawValue == null)
            {
                throw CooperatorConfigurationException.MissingKey(key);
            }

            // La forma se valida acá; el valor se entrega CRUDO, con su prefijo (ADR-004 §3.3).
            ConnectionStringValue.Parse(key, rawValue);
            return rawValue;
        }

        /// <summary>
        /// Nombre de la variable de entorno que corresponde a una clave del catálogo.
        /// </summary>
        /// <param name="key">Clave del catálogo.</param>
        /// <returns>El nombre de la variable de entorno.</returns>
        public static string EnvironmentVariableNameFor(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return key.Replace(ConfigurationKeys.SectionSeparator, ConfigurationKeys.EnvironmentSectionSeparator);
        }

        private static string? ReadValue(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            string raw = Environment.GetEnvironmentVariable(EnvironmentVariableNameFor(key));
            return string.IsNullOrEmpty(raw) ? null : raw;
        }

        private static int ParseInteger(string key, string value)
        {
            int parsed;
            if (!int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed))
            {
                throw CooperatorConfigurationException.NotAnInteger(key, value);
            }

            return parsed;
        }
    }
}

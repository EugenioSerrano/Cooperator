using System;

namespace Cooperator.Framework.Core.Configuration
{
    /// <summary>
    /// Forma del valor de una cadena de conexión (ADR-004 §3.3, ADR-007 §3).
    /// <para>
    /// Reglas únicas, sin heurística y sin adivinanza: el <b>prefijo de esquema es obligatorio</b>; un valor
    /// <b>sin</b> prefijo <b>no</b> se asume texto plano (se rechaza); un <b>prefijo desconocido</b> se rechaza
    /// nombrando la clave <b>y el prefijo ofensor</b>. Nunca hay intento de descifrado a ciegas ni fallback
    /// silencioso.
    /// </para>
    /// <para>
    /// Este tipo valida la <b>forma</b> y expone el esquema y la carga; <b>la capa que consume el valor</b> es
    /// quien invoca al <see cref="IProtectedValueResolver"/> (ADR-004 §3.3). El único esquema registrado en esta
    /// entrega es <see cref="PlainScheme"/> (identidad); el resto son del Bolt 022 (ADR-007).
    /// </para>
    /// </summary>
    public sealed class ConnectionStringValue
    {
        /// <summary>Esquema de texto plano (identidad), explícito y obligatorio.</summary>
        public const string PlainScheme = "plain";

        /// <summary>Separador entre el esquema y la carga.</summary>
        public const char SchemeSeparator = ':';

        private ConnectionStringValue(string raw, string scheme, string payload)
        {
            Raw = raw;
            Scheme = scheme;
            Payload = payload;
        }

        /// <summary>Valor crudo completo, tal como llega de la configuración (con su prefijo).</summary>
        public string Raw { get; }

        /// <summary>Esquema declarado por el valor (por ejemplo <c>plain</c>).</summary>
        public string Scheme { get; }

        /// <summary>Carga del valor: lo que sigue al separador del esquema.</summary>
        public string Payload { get; }

        /// <summary>Indica si el valor declara el esquema de identidad.</summary>
        public bool IsPlain
        {
            get { return string.Equals(Scheme, PlainScheme, StringComparison.Ordinal); }
        }

        /// <summary>
        /// Interpreta y valida la forma de un valor crudo.
        /// </summary>
        /// <param name="key">Clave de la que proviene el valor, para el diagnóstico.</param>
        /// <param name="rawValue">Valor crudo tal como llega de la configuración.</param>
        /// <returns>El valor interpretado.</returns>
        /// <exception cref="CooperatorConfigurationException">
        /// El valor está vacío, no declara prefijo, o declara un prefijo que ningún resolutor registrado atiende.
        /// </exception>
        public static ConnectionStringValue Parse(string key, string rawValue)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (string.IsNullOrEmpty(rawValue))
            {
                throw CooperatorConfigurationException.MissingKey(key);
            }

            int separator = rawValue.IndexOf(SchemeSeparator);
            if (separator <= 0)
            {
                throw new CooperatorConfigurationException(
                    "El valor de configuración de la clave '" + key + "' no declara un prefijo de esquema. Un valor" +
                    " sin prefijo NO se asume texto plano: declaralo explícitamente como '" + PlainScheme +
                    SchemeSeparator + "' (o con el esquema del resolutor que corresponda).");
            }

            string scheme = rawValue.Substring(0, separator);
            string payload = rawValue.Substring(separator + 1);
            if (!string.Equals(scheme, PlainScheme, StringComparison.Ordinal))
            {
                throw new CooperatorConfigurationException(
                    "El valor de configuración de la clave '" + key + "' declara el prefijo de esquema '" + scheme +
                    "', que no tiene resolutor en esta entrega. Prefijo ofensor: '" + scheme + "'.");
            }

            return new ConnectionStringValue(rawValue, scheme, payload);
        }

        /// <summary>
        /// Devuelve el valor resuelto usando el resolutor del esquema declarado.
        /// <para>
        /// Para <see cref="PlainScheme"/> el resolutor por defecto es <see cref="IdentityResolver"/>. Un esquema
        /// conocido sin resolutor registrado <b>falla</b> con error tipado: jamás se resuelve «a ver qué pasa»
        /// (ADR-004 §3.3, SPEC-261016-0018 §D.3).
        /// </para>
        /// </summary>
        /// <param name="resolver">Resolutor a usar; <c>null</c> usa el de identidad si el esquema es <c>plain</c>.</param>
        /// <returns>El valor resuelto.</returns>
        /// <exception cref="CooperatorConfigurationException">El esquema no tiene resolutor.</exception>
        public string Resolve(IProtectedValueResolver resolver)
        {
            if (resolver == null)
            {
                if (!IsPlain)
                {
                    throw new CooperatorConfigurationException(
                        "El esquema '" + Scheme + "' no tiene un resolutor registrado en la capa que consume el valor.");
                }

                resolver = IdentityResolver;
            }

            if (!string.Equals(resolver.Scheme, Scheme, StringComparison.Ordinal))
            {
                throw new CooperatorConfigurationException(
                    "El resolutor declarado atiende el esquema '" + resolver.Scheme + "' y el valor declara '" + Scheme + "'.");
            }

            return resolver.Resolve(Scheme, Payload);
        }

        /// <summary>Resolutor de identidad del esquema <c>plain:</c>.</summary>
        public static IProtectedValueResolver IdentityResolver { get; } = new IdentityValueResolver();

        private sealed class IdentityValueResolver : IProtectedValueResolver
        {
            public string Scheme
            {
                get { return PlainScheme; }
            }

            public string Resolve(string key, string payload)
            {
                return payload;
            }
        }
    }
}

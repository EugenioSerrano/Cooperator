using System;
using Cooperator.Framework.Library.Cryptography.ProtectedValues;

namespace ConnectionStringEncriptor.Protection
{
    /// <summary>
    /// Traduce los errores <b>tipados del motor</b> (Bolt 022) al <b>diagnóstico accionable</b> de la utilidad
    /// (SPEC-261016-0043 §6/B.2): qué falta y qué hacer, con la <b>referencia</b> afectada, <b>sin</b> el valor ni
    /// material de clave.
    /// <para>
    /// La utilidad <b>no</b> captura el error para seguir: si el motor no puede proteger, no hay resultado. Este
    /// tipo sólo produce el texto que el operador lee.
    /// </para>
    /// </summary>
    public static class ProtectionDiagnostics
    {
        /// <summary>Mensaje genérico cuando el fallo no es de la familia conocida del motor.</summary>
        private const string UnknownPrefix = "El motor de protección no pudo proteger el valor: ";

        /// <summary>
        /// Describe un fallo del motor de forma accionable: nombra la <b>referencia</b> (la clave activa o el
        /// <c>kid</c>) y qué hay que hacer, y <b>nunca</b> incluye el valor de entrada ni material de clave.
        /// </summary>
        /// <param name="exception">La excepción del motor (o cualquier otra que impida proteger).</param>
        /// <returns>El mensaje para el operador.</returns>
        public static string Describe(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            return exception switch
            {
                ProtectedValueKeyNotFoundException missing =>
                    "Falta la clave de la referencia '" + (missing.Kid ?? "(sin kid)") + "'. Definí la variable de" +
                    " entorno " + EnvironmentVariableFor(missing.Kid) + " (o registrá el hook del host) con la clave" +
                    " de 32 bytes en Base64, y volvé a intentar. El valor NO se protegió.",

                ProtectedValueKeyProviderException provider =>
                    "El proveedor de claves falló al resolver la referencia '" + (provider.Kid ?? "(sin kid)") + "'." +
                    " Revisá la variable de entorno " + EnvironmentVariableFor(provider.Kid) + " (¿está en Base64 y" +
                    " mide 32 bytes?) o el almacén del hook del host. El valor NO se protegió." +
                    Inner(provider),

                ProtectedValueIntegrityException integrity =>
                    "El valor de la clave de configuración '" + integrity.ConfigurationKey + "' no verifica su" +
                    " integridad (kid '" + (integrity.Kid ?? "(sin kid)") + "'). Volvé a protegerlo con la clave" +
                    " activa: el valor anterior no se puede resolver. El valor NO se protegió." + Inner(integrity),

                ProtectedValueException policy =>
                    "El motor rechazó la operación para la clave de configuración '" + policy.ConfigurationKey + "'" +
                    " (kid '" + (policy.Kid ?? "(sin kid)") + "'): " + policy.Message,

                _ => UnknownPrefix + exception.Message,
            };
        }

        /// <summary>
        /// El nombre de la variable de entorno del proveedor por defecto para una referencia, o una guía si la
        /// referencia no tiene forma admisible.
        /// </summary>
        /// <param name="kid">Referencia de la clave, si se conoce.</param>
        /// <returns>El nombre de la variable o la guía.</returns>
        private static string EnvironmentVariableFor(string? kid)
        {
            if (!string.IsNullOrEmpty(kid) && ProtectedValueFormat.IsValidKid(kid))
            {
                return EnvironmentValueKeyProvider.VariableName(kid!);
            }

            return "(la variable del proveedor por defecto)";
        }

        /// <summary>La causa original, para que el operador pueda escalarla sin que el mensaje lleve material.</summary>
        /// <param name="exception">La excepción del motor.</param>
        /// <returns>El texto de la causa, si la hay.</returns>
        private static string Inner(Exception exception)
        {
            return exception.InnerException == null
                ? string.Empty
                : " Causa original: " + exception.InnerException.Message;
        }
    }
}

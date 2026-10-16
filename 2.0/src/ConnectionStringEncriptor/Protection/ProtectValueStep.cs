using System;
using Cooperator.Framework.Library.Cryptography.ProtectedValues;

namespace ConnectionStringEncriptor.Protection
{
    /// <summary>
    /// El <b>paso único</b> de la utilidad (SPEC-261016-0043 §6/A.2): dado un <b>valor</b> y el <b>nombre completo de
    /// la clave de configuración de destino</b> —que es el <b>AAD</b> del valor—, produce el valor protegido con el
    /// esquema vigente y la <b>clave activa</b>, delegando <b>íntegramente</b> en el motor del Bolt 022.
    /// <list type="bullet">
    ///   <item><b>No</b> elige ni configura algoritmo ni esquema, y <b>no</b> conoce claves: el <c>kid</c> y el
    ///         material los aporta la provisión de claves del motor (entorno por defecto, hook del host).</item>
    ///   <item><b>No</b> implementa ninguna primitiva criptográfica, <b>no</b> guarda constantes de derivación y
    ///         <b>no</b> compone el valor por su cuenta: el valor protegido lo devuelve el motor.</item>
    ///   <item><b>No</b> ofrece una segunda ruta: proteger <b>es</b> este paso (ni formato viejo, ni un modo que
    ///         evite el AAD).</item>
    /// </list>
    /// <para>
    /// El valor de entrada es <b>material sensible transitorio</b>: este tipo no lo escribe, no lo registra y no lo
    /// guarda en ningún estado (§6/B.1).
    /// </para>
    /// </summary>
    public sealed class ProtectValueStep
    {
        private readonly AesGcmValueResolver _engine;

        /// <summary>Inicializa el paso sobre el motor de protección (el del Bolt 022).</summary>
        /// <param name="engine">El resolutor del esquema vigente, con su registro de claves.</param>
        public ProtectValueStep(AesGcmValueResolver engine)
        {
            _engine = engine ?? throw new ArgumentNullException(nameof(engine));
        }

        /// <summary>El motor con el que protege este paso (la única implementación que la utilidad consume).</summary>
        public AesGcmValueResolver Engine => _engine;

        /// <summary>
        /// Protege un valor con la clave activa del motor, atándolo a la clave de configuración de destino.
        /// </summary>
        /// <param name="destinationConfigurationKey">Nombre completo de la clave de configuración de destino (el AAD).</param>
        /// <param name="value">El valor a proteger (se trata como texto; no se recorta ni se interpreta).</param>
        /// <returns>El valor protegido con su <c>kid</c> y su esquema.</returns>
        /// <exception cref="ArgumentException">La clave de destino está vacía o tiene una forma inadmisible.</exception>
        /// <exception cref="ProtectedValueException">El motor no puede proteger: el error viaja tipado (§6/B.2).</exception>
        public ProtectionResult Protect(string destinationConfigurationKey, string value)
        {
            string destination = RequireDestinationKey(destinationConfigurationKey);

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // La protección la hace EL MOTOR: la clave de destino viaja como AAD (ADR-007 §3.1) y la clave activa
            // sale de SU provisión. Acá no hay algoritmo, ni clave, ni composición propia.
            string protectedValue = _engine.Protect(destination, value);

            return new ProtectionResult(
                protectedValue,
                _engine.Keys.ActiveKid ?? string.Empty,
                ProtectionResult.CurrentScheme);
        }

        /// <summary>
        /// Valida la forma mínima de la clave de configuración de destino: el AAD no puede estar vacío ni llevar
        /// espacios en los extremos (el nombre identifica el <b>slot</b> del valor).
        /// </summary>
        /// <param name="destinationConfigurationKey">Clave candidata.</param>
        /// <returns>La clave normalizada.</returns>
        private static string RequireDestinationKey(string destinationConfigurationKey)
        {
            if (destinationConfigurationKey == null)
            {
                throw new ArgumentNullException(nameof(destinationConfigurationKey));
            }

            string destination = destinationConfigurationKey;
            if (destination.Length == 0 || destination.Trim().Length == 0)
            {
                throw new ArgumentException(
                    "La clave de configuración de DESTINO es obligatoria: es el AAD que ata el valor a su slot. Sin" +
                    " ella el valor protegido no se podría resolver en su destino (ADR-007 §3.1).",
                    nameof(destinationConfigurationKey));
            }

            if (!string.Equals(destination, destination.Trim(), StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "La clave de configuración de destino no puede empezar ni terminar con espacios: el nombre es el" +
                    " AAD del valor y debe coincidir exactamente con la clave donde se va a usar.",
                    nameof(destinationConfigurationKey));
            }

            return destination;
        }
    }
}

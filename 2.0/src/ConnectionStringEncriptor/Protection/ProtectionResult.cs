using Cooperator.Framework.Library.Cryptography.ProtectedValues;

namespace ConnectionStringEncriptor.Protection
{
    /// <summary>
    /// El resultado del paso único (SPEC-261016-0043 §6/A.2): el <b>valor protegido</b> —el artefacto que el operador
    /// necesita—, el <b><c>kid</c> usado</b> y el <b>esquema de salida</b>.
    /// <para>
    /// <b>No expone material de clave ni el valor de entrada</b> (ADR-007 §3.2): es lo que la ventana puede mostrar.
    /// </para>
    /// </summary>
    public sealed class ProtectionResult
    {
        /// <summary>Esquema de salida que produce el motor (el vigente de la política).</summary>
        public const string CurrentScheme = ProtectedValueFormat.AesGcmScheme;

        /// <summary>Inicializa el resultado del paso.</summary>
        /// <param name="protectedValue">El valor protegido, tal como lo devolvió el motor.</param>
        /// <param name="kid">Referencia de la clave activa con la que se protegió.</param>
        /// <param name="scheme">Esquema de salida.</param>
        public ProtectionResult(string protectedValue, string kid, string scheme)
        {
            ProtectedValue = protectedValue;
            Kid = kid;
            Scheme = scheme;
        }

        /// <summary>El valor protegido: el único artefacto que el operador se lleva.</summary>
        public string ProtectedValue { get; }

        /// <summary>Referencia de la clave activa (<c>kid</c>) que el motor usó para proteger.</summary>
        public string Kid { get; }

        /// <summary>Esquema de salida del valor protegido.</summary>
        public string Scheme { get; }
    }
}

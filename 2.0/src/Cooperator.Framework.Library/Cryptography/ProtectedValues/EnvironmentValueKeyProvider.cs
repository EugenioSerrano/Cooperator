using System;

namespace Cooperator.Framework.Library.Cryptography.ProtectedValues
{
    /// <summary>
    /// Proveedor de claves por <b>variables de entorno</b>: el <b>default</b> del framework (ADR-007 §3.2).
    /// <para>
    /// <b>Contrato publicado.</b> La clave de un <c>kid</c> se aporta en la variable
    /// <c>Cooperator__ProtectedValues__Keys__&lt;kid&gt;</c> (<see cref="ProtectedValueFormat.EnvironmentKeyPrefix"/>),
    /// con la convención de jerarquía por doble guion bajo que ya usa la configuración del framework, y codificada
    /// en <b>Base64</b>: <b>32 bytes</b> (SPEC-261016-0035 §6/C.1). Una variable ausente significa «sin clave» —y un
    /// <c>kid</c> sin clave es un <b>error tipado</b>, nunca una degradación—; una variable presente pero mal formada
    /// (no Base64, o longitud distinta de 32 bytes) produce el error tipado del proveedor, <b>sin</b> el material.
    /// </para>
    /// <para>
    /// El <b>hook del host</b> es otra implementación de <see cref="IProtectedValueKeyProvider"/>: este proveedor no
    /// se consulta para los esquemas del host ni conoce su almacén.
    /// </para>
    /// </summary>
    public sealed class EnvironmentValueKeyProvider : IProtectedValueKeyProvider
    {
        private readonly Func<string, string?> _read;

        /// <summary>Inicializa el proveedor leyendo el entorno del proceso.</summary>
        public EnvironmentValueKeyProvider()
            : this(Environment.GetEnvironmentVariable)
        {
        }

        /// <summary>Inicializa el proveedor con un lector propio (pruebas y hosts con su propia fuente).</summary>
        /// <param name="environmentReader">Lector de variables de entorno.</param>
        public EnvironmentValueKeyProvider(Func<string, string?> environmentReader)
        {
            _read = environmentReader ?? throw new ArgumentNullException(nameof(environmentReader));
        }

        /// <summary>Nombre de la variable de entorno de una referencia de clave.</summary>
        /// <param name="kid">Referencia de la clave.</param>
        /// <returns>El nombre de la variable.</returns>
        public static string VariableName(string kid) => ProtectedValueFormat.EnvironmentKeyPrefix + kid;

        /// <inheritdoc />
        /// <exception cref="ProtectedValueKeyProviderException">
        /// La variable existe pero no es Base64 válido o no mide <see cref="ProtectedValueFormat.KeySize"/> bytes.
        /// </exception>
        public byte[]? TryGetKey(string kid)
        {
            if (kid == null)
            {
                throw new ArgumentNullException(nameof(kid));
            }

            if (!ProtectedValueFormat.IsValidKid(kid))
            {
                throw new ProtectedValueKeyProviderException(
                    "La referencia de clave no tiene el formato admitido ([A-Za-z0-9._-], 1..64).", string.Empty, null);
            }

            string variable = VariableName(kid);
            string? raw = _read(variable);
            if (string.IsNullOrEmpty(raw))
            {
                return null;
            }

            return ProtectedValueFormat.DecodeKey(variable, raw);
        }
    }
}

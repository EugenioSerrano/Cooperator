using System;
using Cooperator.Framework.Core.Configuration;

namespace Cooperator.Framework.Library.Cryptography.ProtectedValues
{
    /// <summary>
    /// El resolvedor de <b>identidad</b> del esquema <c>plain:</c>: devuelve la carga <b>tal cual</b>, sin
    /// interpretarla (ADR-004 §3.3, SPEC-261016-0035 §6/A.1).
    /// <para>
    /// Es parte del <b>despacho por esquema</b>: la <b>forma</b> del prefijo —que exista y que <c>plain:</c> sea
    /// explícito— es del contrato de configuración (Bolt 011); acá se atiende el esquema ya declarado, y <b>nunca</b>
    /// se usa como camino de degradación para un esquema que falló.
    /// </para>
    /// </summary>
    public sealed class PlainTextValueResolver : IProtectedValueResolver
    {
        /// <summary>Nombre del esquema que este resolutor atiende.</summary>
        public const string SchemeName = ProtectedValueFormat.PlainScheme;

        /// <inheritdoc />
        public string Scheme => SchemeName;

        /// <inheritdoc />
        public string Resolve(string key, string payload)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            // Identidad: el valor es lo que el consumidor declaró como texto plano. No se interpreta ni se valida nada más.
            return payload ?? string.Empty;
        }
    }
}

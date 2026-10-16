using System;
using Cooperator.Framework.Core.LazyLoad.Exceptions;

namespace Cooperator.Framework.Core.LazyLoad
{
    /// <summary>
    /// El <b>destino</b> de un provider: el nombre de ensamblado en su forma simple de
    /// *display name* y el <b>nombre completo del tipo calificado por namespace</b>
    /// (ADR-006 §3.1). Es el valor del contrato de nombres, la clave de la caché de
    /// providers y la base de todos los mensajes de diagnóstico.
    /// <para>
    /// <b>Validación estructural honesta.</b> Ambos valores son obligatorios y no vacíos, y
    /// el tipo debe estar <b>calificado por namespace</b> (contener al menos un namespace).
    /// La versión, la cultura y el token <b>no</b> se exigen ni se verifican: si vienen en
    /// el valor del ensamblado, se aceptan <b>como documentación</b> y la resolución usa su
    /// forma simple (el framework no impone binding; ADR-001 §3.3).
    /// </para>
    /// <para>
    /// <b>La clave no colisiona.</b> La igualdad es estructural sobre el par (ensamblado
    /// simple, tipo completo): los pares <c>("A.B", "C")</c> y <c>("A", "B.C")</c> —que la
    /// concatenación con puntos del 1.4 hacía indistinguibles— son dos claves distintas.
    /// </para>
    /// </summary>
    public sealed class ProviderName : IEquatable<ProviderName>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName">Nombre de ensamblado (forma simple o display name completo).</param>
        /// <param name="typeName">Nombre completo del tipo, calificado por namespace.</param>
        /// <param name="expectedKey">
        /// Clave del catálogo que aportó los valores, cuando provienen de la configuración
        /// (ADR-004 §3.2): se incluye en el diagnóstico de valor ausente.
        /// </param>
        public ProviderName(string? assemblyName, string? typeName, string? expectedKey = null)
        {
            if (string.IsNullOrWhiteSpace(assemblyName))
            {
                throw new ProviderNotConfiguredException(DescribeMissing("nombre de ensamblado", expectedKey));
            }

            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ProviderNotConfiguredException(DescribeMissing("nombre completo del tipo", expectedKey));
            }

            var simple = SimpleAssemblyName(assemblyName!);
            if (simple.Length == 0)
            {
                throw new ProviderNotConfiguredException(DescribeMissing("nombre de ensamblado", expectedKey));
            }

            if (!IsNamespaceQualified(typeName!))
            {
                throw new ProviderTypeNotFoundException(typeName!.Trim(), simple, availableProviders: null);
            }

            DeclaredAssemblyName = assemblyName!.Trim();
            AssemblyName = simple;
            TypeName = typeName!.Trim();
        }

        /// <summary>Valor declarado del ensamblado, tal cual vino (puede traer versión/cultura/token como documentación).</summary>
        public string DeclaredAssemblyName { get; }

        /// <summary>Nombre de ensamblado en su forma simple: lo que la resolución usa.</summary>
        public string AssemblyName { get; }

        /// <summary>Nombre completo del tipo, calificado por namespace.</summary>
        public string TypeName { get; }

        /// <summary>Representación canónica del destino (para mensajes y evidencia).</summary>
        public string Key => AssemblyName + "|" + TypeName;

        /// <summary>
        /// Si el valor del ensamblado trae versión, cultura o token (documentación aceptada).
        /// </summary>
        public bool DeclaredNameCarriesBindingDocumentation =>
            !string.Equals(DeclaredAssemblyName, AssemblyName, StringComparison.Ordinal);

        /// <summary>Forma simple del display name: lo anterior a la primera coma.</summary>
        public static string SimpleAssemblyName(string assemblyName)
        {
            var value = assemblyName.Trim();
            var comma = value.IndexOf(',', StringComparison.Ordinal);
            return (comma < 0 ? value : value.Substring(0, comma)).Trim();
        }

        /// <summary>Si el nombre del tipo está calificado por namespace (contiene un namespace).</summary>
        public static bool IsNamespaceQualified(string typeName)
        {
            var value = typeName.Trim();
            var lastDot = value.LastIndexOf('.');
            return lastDot > 0 && lastDot < value.Length - 1;
        }

        /// <inheritdoc />
        public bool Equals(ProviderName? other) =>
            other is not null
            && string.Equals(AssemblyName, other.AssemblyName, StringComparison.Ordinal)
            && string.Equals(TypeName, other.TypeName, StringComparison.Ordinal);

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as ProviderName);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (StringComparer.Ordinal.GetHashCode(AssemblyName) * 397)
                    ^ StringComparer.Ordinal.GetHashCode(TypeName);
            }
        }

        /// <inheritdoc />
        public override string ToString() => $"{TypeName} (ensamblado '{AssemblyName}')";

        private static string DescribeMissing(string what, string? expectedKey) =>
            expectedKey is null or ""
                ? $"Falta el {what} del provider: el nombre es obligatorio y explícito (ADR-006 §3.1)."
                : $"Falta el {what} del provider: la clave esperada es '{expectedKey}' del catálogo (ADR-004 §3.2).";
    }
}

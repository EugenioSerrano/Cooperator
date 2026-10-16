using System;
using System.Collections.Generic;
using System.Linq;
using Cooperator.Framework.Core.Exceptions;

namespace Cooperator.Framework.Core.LazyLoad.Exceptions
{
    /// <summary>
    /// El ensamblado del provider se resolvió, pero el tipo declarado no existe en él.
    /// <para>
    /// ADR-006 §3.3, tercera fila: el diagnóstico lleva el <b>nombre completo del tipo</b>
    /// y el <b>ensamblado resuelto</b>, y —cuando los hay— los tipos de ese ensamblado que
    /// <b>sí</b> implementan <see cref="ILazyProvider"/>, que es el dato que corrige el
    /// nombre en un paso. Un valor <b>no calificado por namespace</b> cae acá: el framework
    /// exige la forma de ADR-006 §3.1 y no busca "algún tipo con ese nombre".
    /// </para>
    /// </summary>
    public sealed class ProviderTypeNotFoundException : CoreException
    {
        /// <summary>
        /// 
        /// </summary>
        public ProviderTypeNotFoundException()
            : base("El ensamblado del provider no contiene el tipo declarado.")
        {
            TypeName = string.Empty;
            AssemblyName = string.Empty;
            AvailableProviders = Array.Empty<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ProviderTypeNotFoundException(string message)
            : base(message)
        {
            TypeName = string.Empty;
            AssemblyName = string.Empty;
            AvailableProviders = Array.Empty<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ProviderTypeNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
            TypeName = string.Empty;
            AssemblyName = string.Empty;
            AvailableProviders = Array.Empty<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName">Nombre completo del tipo declarado.</param>
        /// <param name="assemblyName">Ensamblado resuelto (su display name).</param>
        /// <param name="availableProviders">Tipos de ese ensamblado que sí implementan <see cref="ILazyProvider"/>.</param>
        public ProviderTypeNotFoundException(string typeName, string assemblyName, IEnumerable<string>? availableProviders)
            : base(BuildMessage(typeName, assemblyName, availableProviders))
        {
            TypeName = typeName;
            AssemblyName = assemblyName;
            AvailableProviders = availableProviders is null ? Array.Empty<string>() : availableProviders.ToArray();
        }

        /// <summary>Nombre completo del tipo declarado.</summary>
        public string TypeName { get; }

        /// <summary>Ensamblado en el que se buscó el tipo.</summary>
        public string AssemblyName { get; }

        /// <summary>Tipos del ensamblado que sí implementan <see cref="ILazyProvider"/> (puede estar vacío).</summary>
        public IReadOnlyList<string> AvailableProviders { get; }

        private static string BuildMessage(string typeName, string assemblyName, IEnumerable<string>? availableProviders)
        {
            var candidates = availableProviders?.ToArray() ?? Array.Empty<string>();
            var tail = candidates.Length > 0
                ? $" El ensamblado declara estos tipos que sí implementan {nameof(ILazyProvider)}: {string.Join(", ", candidates)}."
                : $" El ensamblado '{assemblyName}' no declara ningún tipo que implemente {nameof(ILazyProvider)}.";

            return $"El ensamblado '{assemblyName}' no contiene el tipo '{typeName}'.{tail} El nombre del tipo debe ser el COMPLETO, calificado por namespace (ADR-006 §3.1).";
        }
    }
}

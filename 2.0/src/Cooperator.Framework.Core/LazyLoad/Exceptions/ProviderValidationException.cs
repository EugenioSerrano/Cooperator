using System;
using System.Collections.Generic;
using System.Linq;
using Cooperator.Framework.Core.Exceptions;

namespace Cooperator.Framework.Core.LazyLoad.Exceptions
{
    /// <summary>
    /// El resultado <b>agregado</b> de la validación de arranque (ADR-006 §3.3): todos los
    /// providers que no resolvieron, cada uno con su <b>excepción tipada</b> y su contexto,
    /// en un solo objeto capturable. La validación <b>no</b> corta en el primer fallo.
    /// </summary>
    public sealed class ProviderValidationException : CoreException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="failures">Todos los fallos de la pasada de validación.</param>
        public ProviderValidationException(IReadOnlyList<ProviderValidationFailure> failures)
            : base(BuildMessage(failures))
        {
            Failures = failures ?? throw new ArgumentNullException(nameof(failures));
        }

        /// <summary>Todos los fallos de la validación, en el orden en que se pidieron.</summary>
        public IReadOnlyList<ProviderValidationFailure> Failures { get; }

        private static string BuildMessage(IReadOnlyList<ProviderValidationFailure>? failures)
        {
            if (failures is null || failures.Count == 0)
            {
                return "La validación de arranque no reportó fallos.";
            }

            var detail = string.Join(
                Environment.NewLine,
                failures.Select(failure => $" - '{failure.Entity}': {failure.Error.GetType().Name}: {failure.Error.Message}"));

            return $"La validación de arranque de providers encontró {failures.Count} entidad(es) sin resolver:{Environment.NewLine}{detail}";
        }
    }

    /// <summary>
    /// Un fallo de la validación de arranque: la entidad y la excepción tipada que su
    /// provider produjo (siempre una del cuadro de ADR-006 §3.3).
    /// </summary>
    public sealed class ProviderValidationFailure
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity">Entidad cuya resolución falló.</param>
        /// <param name="error">La excepción tipada del cuadro de ADR-006 §3.3.</param>
        public ProviderValidationFailure(string entity, CoreException error)
        {
            Entity = entity;
            Error = error;
        }

        /// <summary>Entidad cuya resolución falló.</summary>
        public string Entity { get; }

        /// <summary>La excepción tipada del cuadro de diagnósticos.</summary>
        public CoreException Error { get; }
    }
}

using System;
using System.Collections.Generic;
using Cooperator.Framework.Core.Exceptions;
using Cooperator.Framework.Core.LazyLoad.Exceptions;

namespace Cooperator.Framework.Core.LazyLoad
{
    /// <summary>
    /// La <b>validación de arranque</b> de providers (ADR-006 §3.3): <b>opt-in</b>, resuelve
    /// <b>todos</b> los providers que se le indican en <b>una sola pasada</b> —sin cortar en
    /// el primero— y devuelve (o lanza, en su variante estricta) el <b>listado completo</b>
    /// de los que no resolvieron, cada uno con su excepción tipada y su contexto.
    /// <para>
    /// Una aplicación la invoca al iniciar en lugar de descubrir el problema en el primer
    /// lazy load de una request de producción. Una aplicación que no la invoque no la
    /// ejecuta: el framework no valida nada por su cuenta.
    /// </para>
    /// <para>
    /// <b>La entrada es explícita</b> (las entidades a validar). El contrato de configuración
    /// de ADR-004 §3.2 es de <b>lectura por clave</b> y no expone enumeración del catálogo,
    /// de modo que "todos los providers configurados" no puede descubrirse desde el
    /// contrato: la aplicación conoce sus entidades y las declara al arrancar.
    /// </para>
    /// </summary>
    public static class ProviderStartupValidator
    {
        /// <summary>
        /// Resuelve el provider de cada entidad indicada y devuelve <b>todos</b> los fallos
        /// tipados, en el orden en que se pidieron. No lanza por un fallo de resolución.
        /// </summary>
        /// <param name="entities">Las entidades cuyos providers se validan.</param>
        /// <returns>El listado completo de fallos (vacío si todos resolvieron).</returns>
        public static IReadOnlyList<ProviderValidationFailure> Validate(IEnumerable<string> entities)
        {
            if (entities is null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            var failures = new List<ProviderValidationFailure>();

            foreach (var entity in entities)
            {
                try
                {
                    // El mismo camino que usará el lazy load en runtime: si resuelve acá,
                    // ya está en la caché del proceso y la resolución no se paga dos veces.
                    LazyProviderFactory.GetForEntity(entity);
                }
                catch (CoreException error)
                {
                    // Se ACUMULA en lugar de abortar: la validación reporta todos los fallos
                    // de la pasada, no el primero (ADR-006 §3.3, NFR provider-resolution-fail-fast).
                    failures.Add(new ProviderValidationFailure(entity, error));
                }
            }

            return failures;
        }

        /// <summary>
        /// La variante <b>estricta</b>: si alguna entidad no resuelve, lanza
        /// <see cref="ProviderValidationException"/> con <b>todos</b> los fallos en un solo
        /// objeto capturable. Es la forma de usarla en un arranque fail-fast.
        /// </summary>
        /// <param name="entities">Las entidades cuyos providers se validan.</param>
        public static void ValidateOrThrow(IEnumerable<string> entities)
        {
            var failures = Validate(entities);
            if (failures.Count > 0)
            {
                throw new ProviderValidationException(failures);
            }
        }
    }
}

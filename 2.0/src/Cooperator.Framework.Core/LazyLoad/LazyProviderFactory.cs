using System;
using System.Collections.Generic;
using Cooperator.Framework.Core.Configuration;
using Cooperator.Framework.Core.Exceptions;
using Cooperator.Framework.Core.LazyLoad.Exceptions;

namespace Cooperator.Framework.Core.LazyLoad
{
    /// <summary>
    /// La <b>fachada de la carga</b> de providers (ADR-006 §3.1, SPEC §6/A.3). Conserva la
    /// entrada por <b>nombre explícito</b> que ya usaba el código generado y agrega el
    /// <b>camino por entidad</b>, que lee los nombres del catálogo de configuración
    /// (ADR-004 §3.2). Ninguna de las dos entradas acepta rutas.
    /// </summary>
    public static class LazyProviderFactory
    {
        /// <summary>
        /// Devuelve el provider del destino declarado por <b>nombre explícito</b>: el
        /// nombre de ensamblado (forma simple de display name) y el nombre completo del tipo
        /// calificado por namespace. Es la entrada que usa el código generado.
        /// </summary>
        /// <param name="assemblyName">Nombre de ensamblado del provider.</param>
        /// <param name="className">Nombre completo del tipo del provider, calificado por namespace.</param>
        /// <returns>La instancia del provider; nunca <c>null</c>.</returns>
        public static ILazyProvider Get(string? assemblyName, string? className) =>
            ProviderCache.Get(new ProviderName(assemblyName, className));

        /// <summary>
        /// Devuelve el provider de una <b>entidad</b>, leyendo sus dos claves del catálogo
        /// de configuración (ADR-004 §3.2): <c>Cooperator:LazyLoad:Providers:&lt;entidad&gt;:Assembly</c>
        /// y <c>…:Class</c>. Es el camino donde la ausencia de configuración tiene
        /// diagnóstico propio, con la entidad y la <b>clave esperada</b>.
        /// </summary>
        /// <param name="entity">Nombre de la entidad.</param>
        /// <returns>La instancia del provider; nunca <c>null</c>.</returns>
        public static ILazyProvider GetForEntity(string entity)
        {
            if (string.IsNullOrWhiteSpace(entity))
            {
                throw new ArgumentException("La entidad es obligatoria para resolver su provider.", nameof(entity));
            }

            var assemblyKey = ConfigurationKeys.LazyLoad.AssemblyFor(entity);
            var classKey = ConfigurationKeys.LazyLoad.ClassFor(entity);

            var assemblyName = ReadCatalogValue(assemblyKey, entity);
            var className = ReadCatalogValue(classKey, entity);

            // La clave que falta se nombra con exactitud: el valor presente pero vacío es
            // "no configurado" igual que la clave ausente (ADR-006 §3.3, primera fila).
            if (string.IsNullOrWhiteSpace(assemblyName))
            {
                throw new ProviderNotConfiguredException(entity, assemblyKey);
            }

            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ProviderNotConfiguredException(entity, classKey);
            }

            return ProviderCache.Get(new ProviderName(assemblyName, className));
        }

        private static string ReadCatalogValue(string key, string entity)
        {
            try
            {
                return CooperatorSettings.Current.GetString(key);
            }
            catch (CooperatorConfigurationException)
            {
                // La fuente de los nombres es el contrato de configuración (ADR-004 §3.2).
                // Nunca se cae a App.config ni a la sección custom: esa ruta se retira.
                throw new ProviderNotConfiguredException(entity, key);
            }
        }
    }
}

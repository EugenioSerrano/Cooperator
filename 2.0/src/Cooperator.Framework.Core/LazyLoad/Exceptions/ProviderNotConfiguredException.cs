using System;
using Cooperator.Framework.Core.Exceptions;

namespace Cooperator.Framework.Core.LazyLoad.Exceptions
{
    /// <summary>
    /// La entidad no tiene provider configurado: falta la clave del catálogo
    /// (ADR-004 §3.2) que debía declarar su ensamblado o su clase.
    /// <para>
    /// ADR-006 §3.3, primera fila: el diagnóstico lleva la <b>entidad</b> sin provider
    /// configurado y la <b>clave esperada</b> del catálogo, para que se corrija la
    /// configuración sin depurar. Nunca se degrada a un provider por defecto en silencio.
    /// </para>
    /// </summary>
    public sealed class ProviderNotConfiguredException : CoreException
    {
        /// <summary>
        /// 
        /// </summary>
        public ProviderNotConfiguredException()
            : base("La entidad no tiene provider configurado.")
        {
            Entity = string.Empty;
            ExpectedKey = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ProviderNotConfiguredException(string message)
            : base(message)
        {
            Entity = string.Empty;
            ExpectedKey = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ProviderNotConfiguredException(string message, Exception innerException)
            : base(message, innerException)
        {
            Entity = string.Empty;
            ExpectedKey = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity">Entidad sin provider configurado.</param>
        /// <param name="expectedKey">Clave esperada del catálogo (ADR-004 §3.2).</param>
        public ProviderNotConfiguredException(string entity, string expectedKey)
            : base($"La entidad '{entity}' no tiene provider configurado: falta la clave '{expectedKey}' del catálogo (ADR-004 §3.2).")
        {
            Entity = entity;
            ExpectedKey = expectedKey;
        }

        /// <summary>Entidad sin provider configurado.</summary>
        public string Entity { get; }

        /// <summary>Clave del catálogo que debía declararlo.</summary>
        public string ExpectedKey { get; }
    }
}

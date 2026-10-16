using System;
using System.Collections.Generic;
using Cooperator.Framework.Core.Exceptions;

namespace Cooperator.Framework.Core.LazyLoad.Exceptions
{
    /// <summary>
    /// El ensamblado del provider no se pudo resolver por la vía del host.
    /// <para>
    /// ADR-006 §3.3, segunda fila: el diagnóstico lleva el <b>nombre de ensamblado
    /// intentado</b>, si hay <b>hook del host registrado</b> y la <b>causa subyacente</b>
    /// del runtime (<see cref="System.IO.FileNotFoundException"/> /
    /// <see cref="System.IO.FileLoadException"/>), que es lo que distingue "falta la
    /// dependencia declarada" de "la declaró mal".
    /// </para>
    /// </summary>
    public sealed class ProviderAssemblyNotFoundException : CoreException
    {
        /// <summary>
        /// 
        /// </summary>
        public ProviderAssemblyNotFoundException()
            : base("No se pudo resolver el ensamblado del provider.")
        {
            AssemblyName = string.Empty;
            HookRegistered = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ProviderAssemblyNotFoundException(string message)
            : base(message)
        {
            AssemblyName = string.Empty;
            HookRegistered = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ProviderAssemblyNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
            AssemblyName = string.Empty;
            HookRegistered = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName">Nombre de ensamblado intentado.</param>
        /// <param name="hookRegistered">Si el host tenía registrado su hook.</param>
        /// <param name="innerException">Causa subyacente del runtime.</param>
        public ProviderAssemblyNotFoundException(string assemblyName, bool hookRegistered, Exception innerException)
            : base(
                $"No se pudo resolver el ensamblado '{assemblyName}' del provider"
                + (hookRegistered
                    ? " (hay un hook del host registrado)."
                    : " (sin hook del host registrado: se usó la resolución por defecto del framework, que no sondea directorios; declare el ensamblado como dependencia o registre el hook)."),
                innerException)
        {
            AssemblyName = assemblyName;
            HookRegistered = hookRegistered;
        }

        /// <summary>Nombre de ensamblado intentado.</summary>
        public string AssemblyName { get; }

        /// <summary>Si el host tenía registrado su hook cuando se intentó resolver.</summary>
        public bool HookRegistered { get; }
    }
}

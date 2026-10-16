using System;
using Cooperator.Framework.Core.Exceptions;

namespace Cooperator.Framework.Core.LazyLoad.Exceptions
{
    /// <summary>
    /// El <b>hook del host</b> (<see cref="IAssemblyProvider"/>) falló al resolver el provider.
    /// <para>
    /// ADR-006 §3.3, quinta fila: el diagnóstico dice explícitamente que el fallo
    /// <b>vino del hook</b> —no de la resolución por defecto del framework— y lleva la
    /// <b>excepción original</b> del host como causa, porque la política de carga de ese
    /// camino es del host y su diagnóstico también.
    /// </para>
    /// </summary>
    public sealed class ProviderResolutionFailedException : CoreException
    {
        /// <summary>
        /// 
        /// </summary>
        public ProviderResolutionFailedException()
            : base("El hook del host falló al resolver el provider.")
        {
            AssemblyName = string.Empty;
            TypeName = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ProviderResolutionFailedException(string message)
            : base(message)
        {
            AssemblyName = string.Empty;
            TypeName = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ProviderResolutionFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
            AssemblyName = string.Empty;
            TypeName = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName">Ensamblado declarado para el provider.</param>
        /// <param name="typeName">Tipo declarado para el provider.</param>
        /// <param name="hostException">Excepción original del host.</param>
        public ProviderResolutionFailedException(string assemblyName, string typeName, Exception hostException)
            : base(
                $"El hook del host (IAssemblyProvider) falló al resolver el provider '{typeName}' del ensamblado '{assemblyName}': el fallo vino del hook, no de la resolución por defecto del framework.",
                hostException)
        {
            AssemblyName = assemblyName;
            TypeName = typeName;
        }

        /// <summary>Ensamblado declarado para el provider.</summary>
        public string AssemblyName { get; }

        /// <summary>Tipo declarado para el provider.</summary>
        public string TypeName { get; }

        /// <summary>El fallo provino del hook del host (y no de la resolución por defecto).</summary>
        public bool FromHostHook => true;

        /// <summary>Excepción original del host.</summary>
        public Exception? HostException => InnerException;
    }
}

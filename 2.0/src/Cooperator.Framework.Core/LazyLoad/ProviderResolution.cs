using System;

namespace Cooperator.Framework.Core.LazyLoad
{
    /// <summary>
    /// El <b>punto de fijación única</b> del hook del host (ADR-006 §3.1) y el acceso a la
    /// estrategia de resolución vigente. Mismo patrón de fijación al arranque que la
    /// configuración portable (ADR-004 §3.2).
    /// <para>
    /// <b>Fijación única.</b> El hook se registra <b>una vez</b>; un segundo registro es un
    /// error de programa y se rechaza (el comportamiento elegido se documenta y se prueba,
    /// no se deja indeterminado). Al registrarlo se <b>reinicia la caché</b>: cambiar la
    /// estrategia de resolución no puede dejar instancias resueltas por la anterior.
    /// </para>
    /// <para>
    /// <b>Registro tardío.</b> Un host que registre el hook <b>después</b> de que hubo
    /// resoluciones por la vía por defecto obtiene la estrategia nueva para las
    /// resoluciones siguientes, porque la caché se reinicia en el acto. La semántica es
    /// determinista y está cubierta por la prueba del hook.
    /// </para>
    /// </summary>
    public static class ProviderResolution
    {
        private static readonly object Gate = new object();
        private static IAssemblyProvider? assemblyProvider;

        /// <summary>El hook vigente, o <c>null</c> si el host no registró ninguno.</summary>
        public static IAssemblyProvider? AssemblyProvider
        {
            get
            {
                lock (Gate)
                {
                    return assemblyProvider;
                }
            }
        }

        /// <summary>Si el host tiene un hook registrado.</summary>
        public static bool IsHookRegistered => AssemblyProvider is not null;

        /// <summary>
        /// Registra el hook del host. Es una operación <b>única</b> por proceso: un segundo
        /// registro lanza <see cref="InvalidOperationException"/> (el framework no elige
        /// entre dos políticas de carga).
        /// </summary>
        /// <param name="provider">La política de carga del host.</param>
        public static void Register(IAssemblyProvider provider)
        {
            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            lock (Gate)
            {
                if (assemblyProvider is not null)
                {
                    throw new InvalidOperationException(
                        "El hook del host ya está registrado: la fijación es única por proceso (ADR-006 §3.1). "
                        + "Usar ProviderResolution.Reset() para volver a fijarlo en pruebas o cuando el host recarga sus providers.");
                }

                assemblyProvider = provider;
                ProviderCache.Reset();
            }
        }

        /// <summary>
        /// Quita el hook y reinicia la caché (para pruebas y para hosts que recargan sus
        /// providers). Deja el proceso en la resolución por defecto.
        /// </summary>
        public static void Reset()
        {
            lock (Gate)
            {
                assemblyProvider = null;
                ProviderCache.Reset();
            }
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Cooperator.Framework.Core.LazyLoad
{
    /// <summary>
    /// La <b>caché de providers</b> (ADR-006 §3.2): <b>una instancia por provider y por
    /// proceso</b>, <b>thread-safe</b>, con <b>clave por nombre calificado</b> y
    /// <b>reinicio explícito</b>.
    /// <para>
    /// <b>Por qué una instancia por provider.</b> El provider del cliente mantiene sus
    /// propios cachés internos: recrearlo por llamada es exactamente lo que esta caché
    /// evita. La instancia se construye <b>una sola vez</b> aun con N hilos pidiendo el
    /// mismo provider a la vez (la resolución corre dentro de un <see cref="Lazy{T}"/> con
    /// publicación exclusiva, no fuera de él).
    /// </para>
    /// <para>
    /// <b>La clave no colisiona.</b> Es el <see cref="ProviderName"/> —el par estructurado
    /// (ensamblado simple, tipo completo) con igualdad estructural— y no la concatenación
    /// con puntos del 1.4, que hacía indistinguibles <c>("A.B","C")</c> y <c>("A","B.C")</c>.
    /// </para>
    /// <para>
    /// <b>Sin envenenamiento.</b> Si la resolución falla, la excepción <b>se propaga</b> y
    /// la entrada se descarta: el siguiente intento vuelve a resolver. Una configuración
    /// corregida en caliente no queda cacheada como "ausente".
    /// </para>
    /// </summary>
    public static class ProviderCache
    {
        private static readonly ConcurrentDictionary<ProviderName, Lazy<ILazyProvider>> Instances =
            new ConcurrentDictionary<ProviderName, Lazy<ILazyProvider>>();

        /// <summary>
        /// Devuelve la instancia del provider del destino indicado, resolviéndola si es la
        /// primera vez en el proceso. Un fallo de resolución no queda cacheado.
        /// </summary>
        /// <param name="name">El destino del provider (clave de la caché).</param>
        /// <returns>La instancia del provider; nunca <c>null</c>.</returns>
        public static ILazyProvider Get(ProviderName name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var entry = Instances.GetOrAdd(
                name,
                static key => new Lazy<ILazyProvider>(
                    () => ProviderResolver.Resolve(key),
                    LazyThreadSafetyMode.ExecutionAndPublication));

            try
            {
                return entry.Value;
            }
            catch
            {
                // El fallo NO se cachea: se descarta la entrada para que el próximo intento
                // vuelva a resolver (ADR-006 §3.2). Bajo carrera, descartar una entrada ya
                // publicada sólo cuesta una resolución más — nunca un fallo cacheado.
                Instances.TryRemove(name, out _);
                throw;
            }
        }

        /// <summary>Reinicia la caché: la próxima resolución vuelve a construir la instancia.</summary>
        public static void Reset() => Instances.Clear();

        /// <summary>Cantidad de providers resueltos y vivos en el proceso (evidencia de la caché).</summary>
        public static int Count => Instances.Count;

        /// <summary>Si el destino ya está resuelto en la caché del proceso.</summary>
        public static bool Contains(ProviderName name) => Instances.ContainsKey(name);
    }
}

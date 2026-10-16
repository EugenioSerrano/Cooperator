using System;
using System.Threading;
using Cooperator.Framework.Core;
using Cooperator.Framework.Core.LazyLoad;

namespace Cooperator.LazyFixtures.Declared
{
    /// <summary>
    /// El provider del caso normal: una clase del ensamblado de reglas del cliente, como la
    /// que la plantilla `DefaultLazyProvider` genera. Cuenta sus instancias para poder
    /// probar "una instancia por provider y por proceso" (ADR-006 §3.2).
    /// </summary>
    public sealed class SampleLazyProvider : ILazyProvider
    {
        /// <summary>Cantidad de instancias construidas en el proceso (evidencia de la caché).</summary>
        public static int Instances;

        /// <summary>
        /// 
        /// </summary>
        public SampleLazyProvider()
        {
            Interlocked.Increment(ref Instances);
        }

        /// <inheritdoc />
        public IUniqueIdentifiable GetEntity(Type child, IUniqueIdentifiable identifier) =>
            throw new NotSupportedException("Este doble de prueba no resuelve entidades; sólo se construye.");

        /// <inheritdoc />
        public object GetList(Type child, IUniqueIdentifiable parent) =>
            throw new NotSupportedException("Este doble de prueba no resuelve listas; sólo se construye.");

        /// <summary>Reinicia el contador de instancias (evidencia de la caché).</summary>
        public static void ResetInstances() => Interlocked.Exchange(ref Instances, 0);
    }

    /// <summary>Un segundo provider válido del mismo ensamblado (para destinos distintos).</summary>
    public sealed class SecondLazyProvider : ILazyProvider
    {
        /// <inheritdoc />
        public IUniqueIdentifiable GetEntity(Type child, IUniqueIdentifiable identifier) =>
            throw new NotSupportedException("Este doble de prueba no resuelve entidades; sólo se construye.");

        /// <inheritdoc />
        public object GetList(Type child, IUniqueIdentifiable parent) =>
            throw new NotSupportedException("Este doble de prueba no resuelve listas; sólo se construye.");
    }

    /// <summary>Tipo público, construible… pero que <b>no</b> implementa el contrato del provider.</summary>
    public sealed class NotAProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public NotAProvider()
        {
        }
    }

    /// <summary>Tipo abstracto que sí implementa el contrato: no se puede construir.</summary>
    public abstract class AbstractProvider : ILazyProvider
    {
        /// <inheritdoc />
        public IUniqueIdentifiable GetEntity(Type child, IUniqueIdentifiable identifier) => throw new NotSupportedException();

        /// <inheritdoc />
        public object GetList(Type child, IUniqueIdentifiable parent) => throw new NotSupportedException();
    }

    /// <summary>Tipo correcto pero sin constructor público sin parámetros.</summary>
    public sealed class NoParameterlessCtorProvider : ILazyProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="required"></param>
        public NoParameterlessCtorProvider(string required)
        {
            _ = required;
        }

        /// <inheritdoc />
        public IUniqueIdentifiable GetEntity(Type child, IUniqueIdentifiable identifier) => throw new NotSupportedException();

        /// <inheritdoc />
        public object GetList(Type child, IUniqueIdentifiable parent) => throw new NotSupportedException();
    }

    /// <summary>Tipo correcto cuyo constructor sin parámetros NO es público: el binder por defecto no lo construye.</summary>
    public sealed class InternalCtorProvider : ILazyProvider
    {
        internal InternalCtorProvider()
        {
        }

        /// <inheritdoc />
        public IUniqueIdentifiable GetEntity(Type child, IUniqueIdentifiable identifier) => throw new NotSupportedException();

        /// <inheritdoc />
        public object GetList(Type child, IUniqueIdentifiable parent) => throw new NotSupportedException();
    }

    /// <summary>
    /// Tipo compatible cuyo constructor <b>estático</b> falla: el fallo es del tipo, no del
    /// framework (el framework encontró el tipo, verificó la compatibilidad y la
    /// construcción falló adentro).
    /// </summary>
    public sealed class FailingStaticCtorProvider : ILazyProvider
    {
        static FailingStaticCtorProvider()
        {
            throw new InvalidOperationException("el constructor estático de este provider de prueba falla a propósito");
        }

        /// <inheritdoc />
        public IUniqueIdentifiable GetEntity(Type child, IUniqueIdentifiable identifier) => throw new NotSupportedException();

        /// <inheritdoc />
        public object GetList(Type child, IUniqueIdentifiable parent) => throw new NotSupportedException();
    }
}

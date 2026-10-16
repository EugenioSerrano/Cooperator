using System;
using Cooperator.Framework.Core;
using Cooperator.Framework.Core.LazyLoad;

namespace Cooperator.LazyFixtures.Undeclared
{
    /// <summary>
    /// El provider del escenario "plugin en una carpeta": un provider válido cuyo ensamblado
    /// existe en el despliegue pero <b>no</b> está declarado como dependencia de la
    /// aplicación. La resolución por defecto del framework no lo encuentra (no sondea
    /// directorios); el hook del host —que conoce su carpeta— sí.
    /// </summary>
    public sealed class UndeclaredLazyProvider : ILazyProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public UndeclaredLazyProvider()
        {
        }

        /// <inheritdoc />
        public IUniqueIdentifiable GetEntity(Type child, IUniqueIdentifiable identifier) =>
            throw new NotSupportedException("Este doble de prueba no resuelve entidades; sólo se construye.");

        /// <inheritdoc />
        public object GetList(Type child, IUniqueIdentifiable parent) =>
            throw new NotSupportedException("Este doble de prueba no resuelve listas; sólo se construye.");
    }
}

namespace Cooperator.Framework.Core.LazyLoad
{
    /// <summary>
    /// El <b>hook del host</b> (ADR-006 §3.1): la política de carga de ensamblados de la
    /// aplicación, registrada <b>una vez</b> al arranque. Cuando está registrado, el
    /// framework <b>no</b> usa su resolución por defecto.
    /// <para>
    /// El host decide con ella lo que el framework no hace: resolver con un
    /// <c>AssemblyLoadContext</c> propio, cargar desde una carpeta de plugins (con
    /// <c>AssemblyDependencyResolver</c>), o devolver una instancia ya construida por su
    /// contenedor de inyección de dependencias. Esa es <b>su</b> política de confianza.
    /// </para>
    /// <para>
    /// Una implementación que falle debe lanzar: el framework traduce el fallo a
    /// <see cref="Exceptions.ProviderResolutionFailedException"/> diciendo que vino del
    /// hook, con la excepción original como causa. Devolver <c>null</c> es un fallo.
    /// </para>
    /// </summary>
    public interface IAssemblyProvider
    {
        /// <summary>
        /// Devuelve la instancia del provider para el destino indicado.
        /// </summary>
        /// <param name="name">El destino: ensamblado simple + tipo completo calificado (ADR-006 §3.1).</param>
        /// <returns>La instancia del provider; nunca <c>null</c>.</returns>
        ILazyProvider CreateProvider(ProviderName name);
    }
}

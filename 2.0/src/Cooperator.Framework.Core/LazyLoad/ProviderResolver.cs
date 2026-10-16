using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Cooperator.Framework.Core.LazyLoad.Exceptions;

namespace Cooperator.Framework.Core.LazyLoad
{
    /// <summary>
    /// La resolución de un provider por su <b>nombre explícito</b> (ADR-006 §3.1).
    /// <para>
    /// Con hook del host registrado, la instancia la aporta el hook y el framework no usa
    /// su vía por defecto. Sin hook, la vía por defecto es <b>la natural del host</b>:
    /// <c>Assembly.Load(new AssemblyName(&lt;nombre simple&gt;))</c> y el tipo por su
    /// <b>nombre completo</b>, delegando en el ALC por defecto. No hay sondeo de
    /// directorios, no hay <c>LoadFrom</c>/<c>LoadFile</c>, no hay búsqueda por nombre
    /// simple entre los ensamblados cargados y no hay rutas: el provider del caso normal es
    /// una <b>dependencia declarada</b> de la aplicación.
    /// </para>
    /// <para>
    /// Cada modo de fallo produce su excepción del cuadro de ADR-006 §3.3, con el contexto
    /// que permite corregirlo sin depurar. Nunca se devuelve <c>null</c> y nunca se prueba
    /// con otro nombre.
    /// </para>
    /// </summary>
    public static class ProviderResolver
    {
        /// <summary>
        /// Resuelve el provider del destino indicado: por el hook del host si está
        /// registrado, o por la vía por defecto del host si no.
        /// </summary>
        /// <param name="name">El destino del provider.</param>
        /// <returns>La instancia del provider; nunca <c>null</c>.</returns>
        public static ILazyProvider Resolve(ProviderName name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var hook = ProviderResolution.AssemblyProvider;
            return hook is not null
                ? ResolveWithHook(name, hook)
                : ResolveWithHost(name);
        }

        private static ILazyProvider ResolveWithHook(ProviderName name, IAssemblyProvider hook)
        {
            ILazyProvider? provider;
            try
            {
                provider = hook.CreateProvider(name);
            }
            catch (Exception hostException)
            {
                throw new ProviderResolutionFailedException(name.AssemblyName, name.TypeName, hostException);
            }

            if (provider is null)
            {
                throw new ProviderResolutionFailedException(
                    name.AssemblyName,
                    name.TypeName,
                    new InvalidOperationException("El hook devolvió null: un provider que no se puede construir es un error, no una ausencia (ADR-006 §3.3)."));
            }

            return provider;
        }

        private static ILazyProvider ResolveWithHost(ProviderName name)
        {
            Assembly assembly;
            try
            {
                assembly = Assembly.Load(new AssemblyName(name.AssemblyName));
            }
            catch (Exception exception) when (exception is FileNotFoundException or FileLoadException or BadImageFormatException)
            {
                throw new ProviderAssemblyNotFoundException(name.AssemblyName, ProviderResolution.IsHookRegistered, exception);
            }

            var type = assembly.GetType(name.TypeName, throwOnError: false);
            if (type is null)
            {
                throw new ProviderTypeNotFoundException(name.TypeName, DisplayName(assembly), AvailableProviders(assembly));
            }

            var reason = IncompatibilityReason(type);
            if (reason is not null)
            {
                throw new ProviderTypeIncompatibleException(name.TypeName, reason);
            }

            return (ILazyProvider)Activator.CreateInstance(type)!;
        }

        /// <summary>
        /// El motivo por el que un tipo no sirve como provider, o <c>null</c> si sí sirve.
        /// La verificación ocurre <b>antes</b> de construir (ADR-006 §3.3, cuarta fila).
        /// </summary>
        public static string? IncompatibilityReason(Type type)
        {
            if (!typeof(ILazyProvider).IsAssignableFrom(type))
            {
                return ProviderTypeIncompatibleException.DoesNotImplementContract;
            }

            if (type.IsAbstract || type.IsInterface)
            {
                return ProviderTypeIncompatibleException.IsAbstractOrInterface;
            }

            if (type.GetConstructor(Type.EmptyTypes) is null)
            {
                return ProviderTypeIncompatibleException.HasNoPublicParameterlessConstructor;
            }

            return null;
        }

        /// <summary>Los tipos del ensamblado que implementan el contrato del provider (para el diagnóstico).</summary>
        private static string[] AvailableProviders(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes()
                    .Where(candidate => typeof(ILazyProvider).IsAssignableFrom(candidate) && candidate.FullName is not null)
                    .Select(candidate => candidate.FullName!)
                    .OrderBy(candidate => candidate, StringComparer.Ordinal)
                    .ToArray();
            }
            catch (ReflectionTypeLoadException exception)
            {
                return exception.Types
                    .Where(candidate => candidate is not null && typeof(ILazyProvider).IsAssignableFrom(candidate) && candidate.FullName is not null)
                    .Select(candidate => candidate!.FullName!)
                    .OrderBy(candidate => candidate, StringComparer.Ordinal)
                    .ToArray();
            }
        }

        private static string DisplayName(Assembly assembly) =>
            assembly.FullName ?? assembly.GetName().Name ?? "(ensamblado sin nombre)";
    }
}

using System;
using System.Collections.Generic;
using Cooperator.Framework.Core.Configuration;

namespace Cooperator.Framework.Library.Cryptography.ProtectedValues
{
    /// <summary>
    /// El <b>punto de registro y resolución por esquema</b> (ADR-007 §3.1, SPEC-261016-0035 §6/A.1): el framework
    /// registra <c>plain:</c> (identidad) y <c>aesgcm:</c>, y el <b>host</b> registra los suyos (<c>dpapi:</c>,
    /// <c>x509:</c>, <c>kv:</c>, …) por el <b>mismo</b> punto, con el patrón de <b>fijación única al arranque</b>.
    /// El framework <b>no</b> implementa los esquemas del host.
    /// <para>
    /// La <b>frontera</b> con el contrato de configuración (Bolt 011) es explícita: la <b>forma</b> del prefijo
    /// —que exista, que <c>plain:</c> sea explícito, que un prefijo desconocido sea error tipado con la clave y el
    /// prefijo ofensores— es de la configuración (ADR-004 §3.3); <b>este</b> punto resuelve los esquemas
    /// <b>registrados</b> y produce el error de «esquema sin resolvedor registrado» cuando no lo están.
    /// </para>
    /// <para>
    /// Un valor <b>sin</b> prefijo produce el error del <b>contrato</b>
    /// (<see cref="CooperatorConfigurationException"/>): nunca se asume texto plano y nunca se prueba un descifrado
    /// alternativo ante un fallo.
    /// </para>
    /// </summary>
    public sealed class ProtectedValueResolverRegistry
    {
        private readonly Dictionary<string, IProtectedValueResolver> _resolvers = new Dictionary<string, IProtectedValueResolver>(StringComparer.Ordinal);
        private bool _fixed;

        /// <summary>
        /// Inicializa el punto de registro con los esquemas del <b>framework</b>: <c>plain:</c> (identidad) y
        /// <c>aesgcm:</c> (el mecanismo, con su registro de claves).
        /// </summary>
        /// <param name="keys">Registro de claves del mecanismo.</param>
        /// <returns>El punto de registro listo para que el host agregue los suyos.</returns>
        public static ProtectedValueResolverRegistry CreateForFramework(ProtectedValueKeyRegistry keys)
        {
            ProtectedValueResolverRegistry registry = new ProtectedValueResolverRegistry();
            registry.Register(new PlainTextValueResolver());
            registry.Register(new AesGcmValueResolver(keys));
            return registry;
        }

        /// <summary>Esquemas registrados, en orden canónico.</summary>
        public IReadOnlyCollection<string> Schemes
        {
            get
            {
                List<string> schemes = new List<string>(_resolvers.Keys);
                schemes.Sort(StringComparer.Ordinal);
                return schemes;
            }
        }

        /// <summary>Indica si el punto de registro ya quedó fijado (no admite más registros).</summary>
        public bool IsFixed => _fixed;

        /// <summary>
        /// Registra el resolvedor de un esquema. Es el punto por el que el <b>host</b> aporta los suyos.
        /// </summary>
        /// <param name="resolver">El resolvedor del esquema.</param>
        /// <exception cref="ProtectedValueSchemeException">
        /// El punto de registro ya está fijado (fijación única al arranque), el esquema está repetido o el resolvedor
        /// no declara esquema.
        /// </exception>
        public void Register(IProtectedValueResolver resolver)
        {
            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            if (_fixed)
            {
                throw new ProtectedValueSchemeException(
                    "El punto de registro de resolvedores ya está FIJADO (una sola vez al arranque): el esquema '" +
                    resolver.Scheme + "' no se puede registrar tarde. Un mismo esquema no puede tener dos respuestas" +
                    " distintas dentro del proceso.", string.Empty, null);
            }

            string scheme = resolver.Scheme ?? string.Empty;
            if (scheme.Length == 0)
            {
                throw new ProtectedValueSchemeException("El resolvedor no declara el esquema que atiende.", string.Empty, null);
            }

            if (_resolvers.ContainsKey(scheme))
            {
                throw new ProtectedValueSchemeException(
                    "El esquema '" + scheme + "' ya tiene un resolvedor registrado: el registro es único y no se" +
                    " reemplaza en caliente.", string.Empty, null);
            }

            _resolvers.Add(scheme, resolver);
        }

        /// <summary>Fija el punto de registro: a partir de acá sólo se resuelve (ADR-004 §3.2, ADR-006 §3.1).</summary>
        public void Freeze() => _fixed = true;

        /// <summary>Devuelve el resolvedor de un esquema registrado.</summary>
        /// <param name="scheme">Esquema declarado por el valor.</param>
        /// <returns>El resolvedor.</returns>
        /// <exception cref="ProtectedValueSchemeException">El esquema no tiene resolvedor registrado.</exception>
        public IProtectedValueResolver Require(string scheme)
        {
            if (scheme == null)
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            if (!_resolvers.TryGetValue(scheme, out IProtectedValueResolver? resolver))
            {
                throw new ProtectedValueSchemeException(
                    "El esquema '" + scheme + "' NO tiene resolvedor registrado en el punto de registro. El framework" +
                    " atiende 'plain' y 'aesgcm'; un esquema del host lo registra el host, y un esquema sin resolvedor" +
                    " no se intenta descifrar a ciegas.", string.Empty, null);
            }

            return resolver;
        }

        /// <summary>
        /// Resuelve un valor crudo —con su prefijo— por el resolvedor de su esquema.
        /// </summary>
        /// <param name="configurationKey">Clave de configuración que contiene el valor (también el AAD del mecanismo).</param>
        /// <param name="rawValue">Valor crudo, tal como llega de la configuración.</param>
        /// <returns>El valor resuelto.</returns>
        /// <exception cref="CooperatorConfigurationException">El valor no declara prefijo de esquema (el contrato).</exception>
        /// <exception cref="ProtectedValueSchemeException">El esquema no tiene resolvedor registrado.</exception>
        public string Resolve(string configurationKey, string rawValue)
        {
            if (configurationKey == null)
            {
                throw new ArgumentNullException(nameof(configurationKey));
            }

            if (!ProtectedValueFormat.TrySplitScheme(rawValue, out string scheme, out string payload))
            {
                // El error es del CONTRATO de configuración (ADR-004 §3.3, Bolt 011): un valor sin prefijo no se
                // asume texto plano. Se construye la excepción del contrato; no se reimplementa su validación.
                throw new CooperatorConfigurationException(
                    "El valor de configuración de la clave '" + configurationKey + "' no declara un prefijo de esquema." +
                    " Un valor sin prefijo NO se asume texto plano: declaralo explícitamente como '" +
                    ProtectedValueFormat.PlainScheme + ProtectedValueFormat.SchemeSeparator + "' (o con el esquema del" +
                    " resolutor que corresponda).");
            }

            IProtectedValueResolver resolver;
            try
            {
                resolver = Require(scheme);
            }
            catch (ProtectedValueSchemeException exception)
            {
                throw new ProtectedValueSchemeException(exception.Message, configurationKey, exception.Kid, exception);
            }

            try
            {
                return resolver.Resolve(configurationKey, payload);
            }
            catch (ProtectedValueException)
            {
                throw;
            }
            catch (Exception exception)
            {
                // Un resolvedor del HOST que falla se reporta como error tipado del host, con la excepción original.
                throw new ProtectedValueKeyProviderException(
                    "El resolvedor del esquema '" + scheme + "' (del host) falló al resolver la clave de configuración" +
                    " '" + configurationKey + "'. La causa original viaja como excepción interna.",
                    configurationKey, null, exception);
            }
            finally
            {
                Freeze();
            }
        }
    }

    /// <summary>
    /// El <b>punto de registro del proceso</b>: el patrón de <b>fijación única al arranque</b> (ADR-007 §3.1,
    /// ADR-004 §3.2, ADR-006 §3.1).
    /// <para>
    /// Se inicializa una sola vez con el proveedor de claves y la clave activa, el host registra sus esquemas y el
    /// punto se fija: un segundo <see cref="Initialize"/> o un registro posterior son <b>error tipado</b>, no una
    /// respuesta distinta para el mismo esquema dentro del proceso. Un host que necesite otra configuración la fija
    /// **al arranque**, que es lo que el patrón pide.
    /// </para>
    /// </summary>
    public static class ProtectedValueResolvers
    {
        private static readonly object Gate = new object();
        private static ProtectedValueResolverRegistry? _current;

        /// <summary>Indica si el punto de registro del proceso ya se inicializó.</summary>
        public static bool IsInitialized
        {
            get
            {
                lock (Gate)
                {
                    return _current != null;
                }
            }
        }

        /// <summary>
        /// Inicializa el punto de registro del proceso con los esquemas del framework y la clave activa.
        /// </summary>
        /// <param name="keyProvider">Proveedor de claves (entorno por defecto; el hook del host es otra implementación).</param>
        /// <param name="activeKid">Referencia de la clave activa (con la que se protege).</param>
        /// <param name="overlapUntil">Fecha de la ventana de solapamiento, si hay una rotación declarada.</param>
        /// <returns>El registro de claves fijado.</returns>
        /// <exception cref="ProtectedValueSchemeException">El punto ya estaba inicializado (fijación única).</exception>
        public static ProtectedValueKeyRegistry Initialize(IProtectedValueKeyProvider keyProvider, string activeKid, DateTimeOffset? overlapUntil = null)
        {
            if (keyProvider == null)
            {
                throw new ArgumentNullException(nameof(keyProvider));
            }

            lock (Gate)
            {
                if (_current != null)
                {
                    throw new ProtectedValueSchemeException(
                        "El punto de registro de resolvedores del proceso ya está inicializado: la fijación es única al" +
                        " arranque y no se reemplaza en caliente.", string.Empty, null);
                }

                ProtectedValueKeyRegistry keys = new ProtectedValueKeyRegistry(keyProvider);
                keys.SetActive(activeKid, overlapUntil);
                _current = ProtectedValueResolverRegistry.CreateForFramework(keys);
                return keys;
            }
        }

        /// <summary>Registra un esquema del <b>host</b> en el punto de registro del proceso.</summary>
        /// <param name="resolver">El resolvedor del esquema del host.</param>
        /// <exception cref="ProtectedValueSchemeException">El punto no está inicializado o ya está fijado.</exception>
        public static void Register(IProtectedValueResolver resolver)
        {
            lock (Gate)
            {
                if (_current == null)
                {
                    throw new ProtectedValueSchemeException(
                        "El punto de registro no está inicializado: fijá primero la configuración con Initialize(...).",
                        string.Empty, null);
                }

                _current.Register(resolver);
            }
        }

        /// <summary>Fija el punto de registro del proceso (después de registrar los esquemas del host).</summary>
        public static void Freeze()
        {
            lock (Gate)
            {
                if (_current == null)
                {
                    throw new ProtectedValueSchemeException(
                        "El punto de registro no está inicializado: fijá primero la configuración con Initialize(...).",
                        string.Empty, null);
                }

                _current.Freeze();
            }
        }

        /// <summary>Resuelve un valor crudo por el punto de registro del proceso.</summary>
        /// <param name="configurationKey">Clave de configuración que contiene el valor (también el AAD del mecanismo).</param>
        /// <param name="rawValue">Valor crudo, con su prefijo.</param>
        /// <returns>El valor resuelto.</returns>
        /// <exception cref="ProtectedValueSchemeException">El punto no está inicializado.</exception>
        public static string Resolve(string configurationKey, string rawValue)
        {
            ProtectedValueResolverRegistry registry;
            lock (Gate)
            {
                registry = _current ?? throw new ProtectedValueSchemeException(
                    "El punto de registro de resolvedores no está inicializado: no hay esquemas que resuelvan el valor.",
                    configurationKey, null);
            }

            return registry.Resolve(configurationKey, rawValue);
        }
    }
}

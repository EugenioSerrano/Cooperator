using System;

namespace Cooperator.Framework.Library.Cryptography.ProtectedValues
{
    /// <summary>
    /// El registro de claves por <c>kid</c> con sus <b>dos roles</b> (ADR-007 §3.2): una clave <b>activa</b> —con la
    /// que se <b>protege</b>— y el conjunto de claves <b>de lectura</b> —las que el resolutor acepta al resolver,
    /// servidas por el <see cref="IProtectedValueKeyProvider"/>—.
    /// <para>
    /// La <b>ventana de solapamiento</b> de una rotación (<see cref="OverlapUntil"/>) se declara acá, con
    /// <b>fecha</b>: agregar la clave nueva como activa y dejar la anterior en lectura es una operación de
    /// configuración; <b>re-proteger</b> los valores es la operación de la herramienta (Bolt 023), no de este Bolt.
    /// Un <c>kid</c> sin clave disponible produce <see cref="ProtectedValueKeyNotFoundException"/> y <b>nunca</b> se
    /// prueba con otra clave ni se degrada a texto plano.
    /// </para>
    /// <para>El material <b>no</b> se guarda acá: vive en el proveedor (entorno u hook del host).</para>
    /// </summary>
    public sealed class ProtectedValueKeyRegistry
    {
        private readonly IProtectedValueKeyProvider _provider;

        /// <summary>Inicializa el registro con su proveedor de claves.</summary>
        /// <param name="provider">Proveedor de claves (entorno por defecto; el hook del host es otra implementación).</param>
        public ProtectedValueKeyRegistry(IProtectedValueKeyProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>Referencia de la clave con la que se protege, o <c>null</c> si no se fijó ninguna.</summary>
        public string? ActiveKid { get; private set; }

        /// <summary>Fecha hasta la que la clave anterior sigue declarada en la ventana de solapamiento, si la hay.</summary>
        public DateTimeOffset? OverlapUntil { get; private set; }

        /// <summary>Proveedor con el que se resuelven las claves de lectura.</summary>
        public IProtectedValueKeyProvider Provider => _provider;

        /// <summary>
        /// Fija la clave <b>activa</b> por su referencia y declara la ventana de solapamiento de una rotación.
        /// </summary>
        /// <param name="kid">Referencia de la clave activa.</param>
        /// <param name="overlapUntil">Fecha hasta la que la clave anterior se mantiene en lectura, si corresponde.</param>
        /// <exception cref="ProtectedValueKeyNotFoundException">La referencia no tiene clave disponible.</exception>
        public void SetActive(string kid, DateTimeOffset? overlapUntil = null)
        {
            if (kid == null)
            {
                throw new ArgumentNullException(nameof(kid));
            }

            // Falla al fijar, no al proteger: una clave activa sin material es un error de configuración explícito.
            byte[] key = RequireKey(kid, "clave activa");
            CryptographicMaterial.Clear(key);

            ActiveKid = kid;
            OverlapUntil = overlapUntil;
        }

        /// <summary>Devuelve la clave de la referencia activa.</summary>
        /// <returns>La clave de 32 bytes.</returns>
        /// <exception cref="ProtectedValueKeyNotFoundException">No hay clave activa fijada, o no tiene material.</exception>
        public byte[] RequireActiveKey()
        {
            string? kid = ActiveKid;
            if (kid == null)
            {
                throw new ProtectedValueKeyNotFoundException(
                    "No hay clave ACTIVA fijada: proteger exige la referencia explícita de clave (kid) y su proveedor.",
                    string.Empty, null);
            }

            return RequireKey(kid, "clave activa");
        }

        /// <summary>Devuelve la clave de lectura de una referencia (la que un valor declara).</summary>
        /// <param name="kid">Referencia de la clave.</param>
        /// <returns>La clave de 32 bytes.</returns>
        /// <exception cref="ProtectedValueKeyNotFoundException">La referencia no tiene clave disponible.</exception>
        public byte[] RequireReadKey(string kid) => RequireKey(kid, "clave de lectura");

        /// <summary>Indica si la referencia tiene clave disponible (sin exponer el material).</summary>
        /// <param name="kid">Referencia de la clave.</param>
        /// <returns><c>true</c> si el proveedor la aporta.</returns>
        public bool HasKey(string kid)
        {
            if (kid == null)
            {
                throw new ArgumentNullException(nameof(kid));
            }

            try
            {
                byte[]? key = _provider.TryGetKey(kid);
                if (key == null)
                {
                    return false;
                }

                CryptographicMaterial.Clear(key);
                return true;
            }
            catch (ProtectedValueException)
            {
                return false;
            }
        }

        private byte[] RequireKey(string kid, string role)
        {
            if (!ProtectedValueFormat.IsValidKid(kid))
            {
                throw new ProtectedValueKeyNotFoundException(
                    "La referencia de clave de la " + role + " no tiene el formato admitido ([A-Za-z0-9._-], 1..64).",
                    string.Empty, null);
            }

            byte[]? key;
            try
            {
                key = _provider.TryGetKey(kid);
            }
            catch (ProtectedValueException)
            {
                throw;
            }
            catch (Exception exception)
            {
                // El fallo viene del proveedor del host: se reporta como tal, con la excepción original y sin material.
                throw new ProtectedValueKeyProviderException(
                    "El proveedor de claves del host falló al resolver la " + role + " '" + kid +
                    "'. La causa original viaja como excepción interna.", string.Empty, kid, exception);
            }

            if (key == null)
            {
                throw new ProtectedValueKeyNotFoundException(
                    "La " + role + " '" + kid + "' no tiene clave disponible. Un kid sin clave es un error: no se" +
                    " prueba con otra clave y no se degrada a texto plano.", string.Empty, kid);
            }

            if (key.Length != ProtectedValueFormat.KeySize)
            {
                CryptographicMaterial.Clear(key);
                throw new ProtectedValueKeyProviderException(
                    "La " + role + " '" + kid + "' no mide " + ProtectedValueFormat.KeySize + " bytes.", string.Empty, kid);
            }

            return key;
        }
    }
}

using System;

namespace Cooperator.Framework.Core.Configuration
{
    /// <summary>
    /// Contrato de configuración del framework: <b>valores nombrados</b>, y nada más.
    /// <para>
    /// ADR-004 §3.2: el contrato es de valores nombrados —el framework <b>no</b> conoce archivos, rutas ni
    /// formatos— y vive en <c>Cooperator.Framework.Core</c> (capa base, cero dependencias de terceros) porque lo
    /// consumen <c>Data</c> y <c>Library</c>.
    /// </para>
    /// <para>
    /// La implementación por defecto (sólo variables de entorno) está en <see cref="EnvironmentConfiguration"/> y
    /// la instancia vigente en <see cref="CooperatorSettings.Current"/>.
    /// </para>
    /// </summary>
    public interface ICooperatorConfiguration
    {
        /// <summary>
        /// Lee una clave obligatoria.
        /// </summary>
        /// <param name="key">Clave del catálogo (ver <see cref="ConfigurationKeys"/>).</param>
        /// <returns>El valor de la clave.</returns>
        /// <exception cref="CooperatorConfigurationException">La clave no tiene valor y no tiene default.</exception>
        string GetString(string key);

        /// <summary>
        /// Lee una clave con default documentado.
        /// </summary>
        /// <param name="key">Clave del catálogo.</param>
        /// <param name="defaultValue">Valor a devolver si la clave no tiene valor.</param>
        /// <returns>El valor de la clave, o <paramref name="defaultValue"/>.</returns>
        string GetString(string key, string defaultValue);

        /// <summary>
        /// Lee una clave entera obligatoria.
        /// </summary>
        /// <param name="key">Clave del catálogo.</param>
        /// <returns>El valor entero de la clave.</returns>
        /// <exception cref="CooperatorConfigurationException">
        /// La clave no tiene valor, o el valor no es un entero.
        /// </exception>
        int GetInt(string key);

        /// <summary>
        /// Lee una clave entera con default documentado.
        /// </summary>
        /// <param name="key">Clave del catálogo.</param>
        /// <param name="defaultValue">Valor a devolver si la clave no tiene valor.</param>
        /// <returns>El valor entero de la clave, o <paramref name="defaultValue"/>.</returns>
        /// <exception cref="CooperatorConfigurationException">El valor no es un entero.</exception>
        int GetInt(string key, int defaultValue);

        /// <summary>
        /// Entrega el valor <b>crudo, con su prefijo de esquema</b>, de la cadena de conexión pedida.
        /// <para>
        /// ADR-004 §3.3: el prefijo es <b>obligatorio</b>; un valor sin prefijo <b>no</b> se asume texto plano y un
        /// prefijo desconocido se rechaza. La capa que consume el valor es quien invoca al
        /// <see cref="IProtectedValueResolver"/> con el esquema y la carga.
        /// </para>
        /// </summary>
        /// <param name="name">Nombre de la cadena (por defecto <see cref="ConfigurationKeys.ConnectionStrings.DefaultName"/>).</param>
        /// <returns>El valor crudo, con su prefijo (por ejemplo <c>plain:Server=…</c>).</returns>
        /// <exception cref="CooperatorConfigurationException">
        /// La cadena no tiene valor, no declara prefijo, o declara un prefijo desconocido.
        /// </exception>
        string GetConnectionString(string name);
    }
}

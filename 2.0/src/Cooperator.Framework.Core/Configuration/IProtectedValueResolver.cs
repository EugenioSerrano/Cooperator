using System;

namespace Cooperator.Framework.Core.Configuration
{
    /// <summary>
    /// <b>Seam</b> hacia el resolutor de valores protegidos (ADR-007 §3, ADR-004 §3.3).
    /// <para>
    /// El proveedor de configuración entrega el valor <b>crudo, con su prefijo</b>, y <b>la capa que lo consume</b>
    /// —<c>Data</c> para las cadenas de conexión, el tooling para el reverse— es la que invoca al resolutor con el
    /// esquema y la carga. El resolutor no se ejecuta en la configuración (que no sabe de esquemas) ni en el
    /// framework entero (que no conoce el algoritmo).
    /// </para>
    /// <para>
    /// En esta entrega el único esquema registrado es <see cref="ConnectionStringValue.PlainScheme"/>
    /// (identidad). La implementación <c>aesgcm:&lt;kid&gt;:&lt;base64&gt;</c>, los esquemas del host por hook y los
    /// parámetros criptográficos son del Bolt 022 (ADR-007).
    /// </para>
    /// </summary>
    public interface IProtectedValueResolver
    {
        /// <summary>
        /// Esquema que este resolutor atiende (sin el separador; por ejemplo <c>plain</c>).
        /// </summary>
        string Scheme { get; }

        /// <summary>
        /// Resuelve la carga del valor (lo que sigue al prefijo <c>{esquema}:</c>).
        /// </summary>
        /// <param name="key">Clave de la que proviene el valor, para el diagnóstico.</param>
        /// <param name="payload">Carga del valor, ya sin el prefijo.</param>
        /// <returns>El valor resuelto (para <c>plain:</c>, la identidad de la carga).</returns>
        /// <exception cref="CooperatorConfigurationException">La carga no se puede resolver.</exception>
        string Resolve(string key, string payload);
    }
}

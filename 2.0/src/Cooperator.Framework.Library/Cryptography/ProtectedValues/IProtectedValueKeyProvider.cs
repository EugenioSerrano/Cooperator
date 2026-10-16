namespace Cooperator.Framework.Library.Cryptography.ProtectedValues
{
    /// <summary>
    /// <b>Contrato del proveedor de claves</b> (ADR-007 §3.2): el framework consume la <b>referencia</b> de la clave
    /// (<c>kid</c>) y el <b>material</b> lo aporta quien lo tiene.
    /// <para>
    /// El proveedor por <b>defecto</b> es <see cref="EnvironmentValueKeyProvider"/> (variables de entorno) y el
    /// <b>hook del host</b> es una implementación propia del consumidor (DPAPI sobre un archivo protegido por el SO,
    /// Key Vault/KMS, un gestor corporativo, un HSM). Un fallo del proveedor del host produce el error tipado
    /// correspondiente con la referencia intentada y <b>sin</b> el material.
    /// </para>
    /// <para>
    /// La implementación <b>nunca</b> devuelve una clave recortada, derivada de una constante ni completada con
    /// ceros: o tiene la clave de 32 bytes, o no tiene clave.
    /// </para>
    /// </summary>
    public interface IProtectedValueKeyProvider
    {
        /// <summary>
        /// Devuelve la clave de 32 bytes de la referencia, o <c>null</c> cuando esa referencia no está provista.
        /// </summary>
        /// <param name="kid">Referencia de la clave.</param>
        /// <returns>La clave, o <c>null</c> si el proveedor no la tiene.</returns>
        byte[]? TryGetKey(string kid);
    }
}

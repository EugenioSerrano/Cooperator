using System;

namespace Cooperator.Framework.Library.Cryptography.ProtectedValues
{
    /// <summary>
    /// Limpieza del material de clave en memoria: el framework no lo guarda (vive en el proveedor) y las copias
    /// temporales se borran en cuanto dejan de usarse (ADR-007 §3.2).
    /// <para>
    /// <c>Array.Clear</c> no es una garantía absoluta —el runtime puede haber movido una copia— y **no** se
    /// presenta como tal: es la reducción de la ventana de exposición, que es lo que la política pide de una
    /// biblioteca portable sin acceso a memoria fijada.
    /// </para>
    /// </summary>
    internal static class CryptographicMaterial
    {
        /// <summary>Borra el contenido de un arreglo de bytes, si lo hay.</summary>
        /// <param name="bytes">Material a borrar.</param>
        internal static void Clear(byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return;
            }

            Array.Clear(bytes, 0, bytes.Length);
        }
    }
}

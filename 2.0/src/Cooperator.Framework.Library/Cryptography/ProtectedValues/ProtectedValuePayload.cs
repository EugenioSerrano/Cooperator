namespace Cooperator.Framework.Library.Cryptography.ProtectedValues
{
    /// <summary>
    /// Las partes de una carga protegida ya parseada: el <c>kid</c> de la referencia de clave y la composición
    /// publicada <c>nonce | tag | ciphertext</c> (SPEC-261016-0035 §6/A.2).
    /// <para>
    /// No se expone en la superficie como API para <b>construir</b> valores —eso invitaría a armarlos a mano, que es
    /// el camino por el que un consumidor comete un error de seguridad—: la composición la hace
    /// <see cref="AesGcmValueResolver"/> al proteger.
    /// </para>
    /// </summary>
    public sealed class ProtectedValuePayload
    {
        internal ProtectedValuePayload(string kid, byte[] nonce, byte[] tag, byte[] ciphertext)
        {
            Kid = kid;
            Nonce = nonce;
            Tag = tag;
            Ciphertext = ciphertext;
        }

        /// <summary>Referencia de la clave con la que se protegió el valor.</summary>
        public string Kid { get; }

        /// <summary>Nonce de <see cref="ProtectedValueFormat.NonceSize"/> bytes, aleatorio por operación.</summary>
        public byte[] Nonce { get; }

        /// <summary>Tag de autenticación de <see cref="ProtectedValueFormat.TagSize"/> bytes.</summary>
        public byte[] Tag { get; }

        /// <summary>Texto cifrado.</summary>
        public byte[] Ciphertext { get; }
    }
}

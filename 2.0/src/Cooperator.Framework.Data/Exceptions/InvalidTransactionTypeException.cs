using System;

namespace Cooperator.Framework.Data.Exceptions
{
    /// <summary>
    /// Un objeto de transacción o de conexión que llega al <b>borde público</b> de la capa <b>no es del proveedor
    /// elegido</b> (<c>Microsoft.Data.SqlClient</c>): el punto único de conversión lo dice con el <b>tipo recibido</b>
    /// y el <b>esperado</b>, en vez de dejar que un <c>InvalidCastException</c> genérico explote a mitad de una
    /// operación (SPEC-261016-0020 §6/B.1; ADR-005 §3.2).
    /// </summary>
    public sealed class InvalidTransactionTypeException : Exception
    {
        /// <summary>Inicializa la excepción sin contexto (constructor canónico).</summary>
        public InvalidTransactionTypeException()
            : this(string.Empty)
        {
        }

        /// <summary>Inicializa la excepción con un mensaje (constructor canónico).</summary>
        /// <param name="message">Mensaje del error.</param>
        public InvalidTransactionTypeException(string message)
            : base(message)
        {
            ReceivedType = string.Empty;
            ExpectedType = string.Empty;
            Origin = string.Empty;
        }

        /// <summary>Inicializa la excepción con un mensaje y su causa (constructor canónico).</summary>
        /// <param name="message">Mensaje del error.</param>
        /// <param name="innerException">Causa original.</param>
        public InvalidTransactionTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
            ReceivedType = string.Empty;
            ExpectedType = string.Empty;
            Origin = string.Empty;
        }

        /// <summary>Inicializa la excepción con el tipo recibido, el esperado y el punto que pidió la conversión.</summary>
        /// <param name="receivedType">Nombre del tipo del objeto recibido.</param>
        /// <param name="expectedType">Nombre del tipo esperado.</param>
        /// <param name="origin">Punto de la capa que pidió la conversión.</param>
        public InvalidTransactionTypeException(string receivedType, string expectedType, string origin)
            : base("La capa de datos espera un objeto de tipo '" + expectedType + "' del proveedor Microsoft.Data.SqlClient" +
                   " y recibió '" + receivedType + "' (punto: " + origin + "). El framework usa UN solo proveedor: no hay" +
                   " conversión a un motor alternativo (ADR-005 §3.2).")
        {
            ReceivedType = receivedType;
            ExpectedType = expectedType;
            Origin = origin;
        }

        /// <summary>Tipo del objeto recibido en el borde.</summary>
        public string ReceivedType { get; }

        /// <summary>Tipo que la capa esperaba.</summary>
        public string ExpectedType { get; }

        /// <summary>Punto de la capa que pidió la conversión.</summary>
        public string Origin { get; }
    }
}

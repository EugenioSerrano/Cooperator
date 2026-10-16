using System;
using System.Data.Common;
using Cooperator.Framework.Data.Exceptions;
using Microsoft.Data.SqlClient;

namespace Cooperator.Framework.Data.Casting
{
    /// <summary>
    /// El <b>punto único de conversión</b> del borde público de la capa (SPEC-261016-0020 §6/B.1; ADR-005 §3.2):
    /// recibe <see cref="DbTransaction"/>/<see cref="DbConnection"/>, valida que sean del <b>proveedor elegido</b>
    /// (<c>Microsoft.Data.SqlClient</c>) y los entrega al interior <b>ya tipados</b>.
    /// <list type="bullet">
    ///   <item>Es el <b>único</b> lugar del paquete donde ocurre una conversión de tipos de ADO.NET: el NFR
    ///         <i>No hidden casts</i> (ADR-005 §5) exige <b>0</b> ocurrencias fuera de aquí.</item>
    ///   <item>Si el objeto <b>no</b> es del proveedor, lanza <see cref="InvalidTransactionTypeException"/> con el
    ///         <b>tipo recibido</b> y el <b>esperado</b> — nunca un <c>InvalidCastException</c> genérico a mitad de
    ///         una operación.</item>
    ///   <item>La superficie pública conserva su <b>forma BCL-typed</b> (<see cref="DbConnection"/>/
    ///         <see cref="DbTransaction"/>) para el código generado: eso es <b>forma de firma</b>, no una promesa de
    ///         multi-motor (ADR-005 §3.3) — un provider alternativo <b>no</b> va a funcionar.</item>
    /// </list>
    /// </summary>
    public static class TransactionAdapter
    {
        /// <summary>
        /// Valida y convierte una transacción del borde público al tipo del proveedor.
        /// </summary>
        /// <param name="transaction">La transacción recibida (puede ser <c>null</c>: no toda operación tiene una).</param>
        /// <param name="origin">Punto de la capa que pide la conversión, para el diagnóstico.</param>
        /// <returns>La transacción del proveedor, o <c>null</c> si no se recibió ninguna.</returns>
        /// <exception cref="InvalidTransactionTypeException">La transacción no es del proveedor elegido.</exception>
        public static SqlTransaction? AsSqlTransaction(DbTransaction? transaction, string origin)
        {
            if (transaction == null)
            {
                return null;
            }

            if (transaction is SqlTransaction sqlTransaction)
            {
                return sqlTransaction;
            }

            throw new InvalidTransactionTypeException(transaction.GetType().FullName ?? transaction.GetType().Name, typeof(SqlTransaction).FullName!, origin);
        }

        /// <summary>
        /// Valida y convierte una conexión del borde público al tipo del proveedor.
        /// </summary>
        /// <param name="connection">La conexión recibida.</param>
        /// <param name="origin">Punto de la capa que pide la conversión, para el diagnóstico.</param>
        /// <returns>La conexión del proveedor.</returns>
        /// <exception cref="InvalidTransactionTypeException">La conexión no es del proveedor elegido.</exception>
        public static SqlConnection AsSqlConnection(DbConnection? connection, string origin)
        {
            if (connection == null)
            {
                throw new InvalidTransactionTypeException(string.Empty, typeof(SqlConnection).FullName!, origin);
            }

            if (connection is SqlConnection sqlConnection)
            {
                return sqlConnection;
            }

            throw new InvalidTransactionTypeException(connection.GetType().FullName ?? connection.GetType().Name, typeof(SqlConnection).FullName!, origin);
        }
    }
}

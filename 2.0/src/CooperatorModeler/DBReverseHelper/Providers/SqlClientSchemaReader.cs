using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Cooperator.Framework.Modeler.DBReverseHelper.Providers
{
    /// <summary>
    /// El <b>lector de esquema propio</b> del reverse (SPEC-261016-0027 §6/B.1): ejecuta las consultas de esquema
    /// con el <b>provider único</b> de la 2.0 (<c>Microsoft.Data.SqlClient</c>, ADR-005 §3.1) y devuelve los
    /// <b>mismos</b> <see cref="DataSet"/> que el armado del modelo ya consume —el texto de las consultas y la forma
    /// de los resultados no cambian, y eso es lo que la paridad verifica (AC-2)—.
    /// <para>
    /// <b>La conexión es del componente</b> (AC-3): se crea desde la cadena ya resuelta y se <b>dispone</b> en la
    /// misma lectura. No acepta ni modifica una conexión del llamador —su API no las recibe— y no queda ninguna
    /// conexión abierta al terminar.
    /// </para>
    /// <para>
    /// El reverse es un <b>lector</b>: sólo ejecuta consultas de catálogo, nunca escribe (SPEC-261016-0027 §10).
    /// </para>
    /// </summary>
    public sealed class SqlClientSchemaReader
    {
        private readonly string _connectionString;

        /// <summary>Crea el lector con la cadena ya resuelta (la aporta <c>ConnectionStringResolver</c>).</summary>
        /// <param name="connectionString">Cadena de conexión resuelta; nunca vive en el código.</param>
        public SqlClientSchemaReader(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        /// <summary>
        /// Ejecuta un texto T-SQL de lectura y devuelve su <see cref="DataSet"/>, con la conexión creada y
        /// dispuesta dentro de la llamada.
        /// </summary>
        /// <param name="sqlText">El texto de la consulta de esquema (el de 1.4, sin cambios).</param>
        /// <returns>El conjunto de datos con la forma que el armado del modelo consume.</returns>
        public DataSet ExecuteSQLText(string sqlText)
        {
            if (sqlText == null)
            {
                throw new ArgumentNullException(nameof(sqlText));
            }

            DataSet results = new DataSet();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(sqlText, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        connection.Open();
                        adapter.Fill(results);
                    }
                }
            }

            return results;
        }
    }
}

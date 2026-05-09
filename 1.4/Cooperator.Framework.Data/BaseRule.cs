/*-
*       Copyright (c) 2006-2007 Eugenio Serrano, Daniel Calvin.
*       All rights reserved.
*
*       Redistribution and use in source and binary forms, with or without
*       modification, are permitted provided that the following conditions
*       are met:
*       1. Redistributions of source code must retain the above copyright
*          notice, this list of conditions and the following disclaimer.
*       2. Redistributions in binary form must reproduce the above copyright
*          notice, this list of conditions and the following disclaimer in the
*          documentation and/or other materials provided with the distribution.
*       3. Neither the name of copyright holders nor the names of its
*          contributors may be used to endorse or promote products derived
*          from this software without specific prior written permission.
*
*       THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
*       "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
*       TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
*       PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL COPYRIGHT HOLDERS OR CONTRIBUTORS
*       BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
*       CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
*       SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
*       INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
*       CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
*       ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
*       POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Cooperator.Framework.Data
{
	/// <summary>
	/// Base class that provide database access for inheritors
	/// </summary>
	public abstract class BaseRule
	{

		/// <summary>
		/// Class contructor
		/// </summary>
		protected internal BaseRule() {
            DataBaseHelper = new InternalDBHelper(this);
		}


		/// <summary>
		/// Default connection string key name.
		/// </summary>
        protected virtual string ConnectionStringKeyName 
            => "CooperatorConnectionString";


	    /// <summary>
        /// Provide access to all database operations
        /// </summary>
        public InternalDBHelper DataBaseHelper { get; }

	    /// <summary>
        /// Provide access to all database operations
        /// </summary>        
        public class InternalDBHelper
        {
            
            internal BaseRule parent;

            private InternalDBHelper()
            {
            }
            /// <summary>
            /// This class is instantiated inside BaseRule
            /// </summary>
            /// <param name="parentClass"></param>
            internal InternalDBHelper(BaseRule parentClass)
            {
                parent = parentClass;
            }

            private static readonly Dictionary<string, string> ConnectionStringsDictionary = new Dictionary<string, string>();

            /// <summary>
            /// Connection string stored in app.config or web.config 
            /// For security reasons, it is hidden outside assembly
            /// </summary>
            /// <returns>Connection string</returns>
            internal string ConnectionString
            {
                get
                {
                    lock (ConnectionStringsDictionary)
                    {
                        if (!ConnectionStringsDictionary.ContainsKey(parent.ConnectionStringKeyName))
                        {
                            string connectionString;
                            try
                            {
                                // get string from configuration file.
                                connectionString = ConfigurationManager.ConnectionStrings[parent.ConnectionStringKeyName].ConnectionString;
                                if (connectionString.ToLowerInvariant().Contains("initial catalog") || 
                                    connectionString.ToLowerInvariant().Contains("database") || 
                                    connectionString.ToLowerInvariant().Contains("data source"))
                                {
                                    // not encrypted.
                                }
                                else
                                {
                                    // maybe encryted, try to decrypt
                                    var tempConStr = Library.Cryptography.Decrypt(connectionString);
                                    if (tempConStr.ToLowerInvariant().Contains("initial catalog") || 
                                        tempConStr.ToLowerInvariant().Contains("database") ||
                                        tempConStr.ToLowerInvariant().Contains("data source"))
                                    {
                                        // encrypted.
                                        connectionString = tempConStr;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exceptions.ConnectionStringNotFoundException(
                                    "No se ha encontrado un connection string para: " + parent.ConnectionStringKeyName + "\r\n\r\n" + ex.Message);
                            }
                            ConnectionStringsDictionary.Add(parent.ConnectionStringKeyName, connectionString);
                        }
                    }

                    return ConnectionStringsDictionary[parent.ConnectionStringKeyName];
                }
            }

            /// <summary>
            /// Set a connection string. Used for database access without configuration file.
            /// </summary>
            /// <param name="value"></param>
            public void SetConnectionStringValue(string value) {
                var connectionString = value;

                // If exist, remove it from dictionary
                if (ConnectionStringsDictionary.ContainsKey(parent.ConnectionStringKeyName))
                    ConnectionStringsDictionary.Remove(parent.ConnectionStringKeyName);

                // Add string to dictionary
                ConnectionStringsDictionary.Add(parent.ConnectionStringKeyName, connectionString);            
            }


            /// <summary>
            /// Begin a transaction. If Isolation Level is not specified, Read Committed is used
            /// </summary>
            /// <returns>DbTransaction object</returns>
            public DbTransaction GetAndBeginTransaction()
            {
                return GetAndBeginTransaction(IsolationLevel.ReadCommitted);
            }


            /// <summary>
            /// Begin a transaction. If Isolation Level is not specified, Read Committed is used
            /// </summary>
            /// <param name="isolationLevel">Transaction isolation level</param>
            /// <returns>DbTransaction object</returns>
            public DbTransaction GetAndBeginTransaction(IsolationLevel isolationLevel)
            {

                // Right now we are used only SQL Server.
                var conn = new SqlConnection(ConnectionString);
                conn.Open();

                SqlTransaction tran = null;
                // To fix the "System.Data.SqlClient.SqlTransaction.Zombie()" error
                for (var i = 0; i < 9; i++)
                {
                    tran = conn.BeginTransaction(isolationLevel);
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (tran != null)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(0);
                }

                return tran;
            }

            /// <summary>
            /// Get a new open connection
            /// </summary>
            /// <returns></returns>
            public DbConnection GetNewConnection()
            {
                var conn = new SqlConnection(ConnectionString);
                conn.Open();

                return conn;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="spName"></param>
            /// <returns></returns>
            public static string correctSpName(string spName)
            {
                if (spName.StartsWith("["))
                {
                    return spName;
                }
                else
                {
                    return "[" + spName + "]";
                }
            }


            #region ExposeSQLHelperMethods

            /// <summary>			
            /// Execute a SQL Text that return a scalar (1 x 1)
            /// </summary>
            /// <param name="commandText">Sql command</param>
            /// <returns>Object with scalar value</returns>
            public object ExecuteScalarBySQLText(string commandText)
            {
                return SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, commandText);
            }

            /// <summary>
            /// Execute a SQL Text that return a scalar (1 x 1)
            /// </summary>
            /// <param name="transaction">DbTransaction object</param>
            /// <param name="commandText">Sql command</param>
            /// <returns>Object with scalar value</returns>
            public object ExecuteScalarBySQLText(DbTransaction transaction, string commandText)
            {
                return SqlHelper.ExecuteScalar((SqlTransaction)transaction, CommandType.Text, commandText);
            }

            /// <summary>
            /// Execute a Stored Procedure that return a scalar (1 x 1)
            /// </summary>
            /// <param name="spName">Stored procedure name</param>
            /// <param name="parameterValues">Stored procedure parameters</param>
            /// <returns>Object with scalar value</returns>
            public object ExecuteScalarByStoredProcedure(string spName, params object[] parameterValues)
            {
                return SqlHelper.ExecuteScalar(ConnectionString, correctSpName(spName), parameterValues);
            }

            /// <summary>
            /// Execute a Stored Procedure that return a scalar (1 x 1)
            /// </summary>
            /// <param name="transaction">DbTransaction object</param>
            /// <param name="spName">Stored Procedure name</param>
            /// <param name="parameterValues">Stored Procedure parameters</param>
            /// <returns>Object with scalar value</returns>
            public object ExecuteScalarByStoredProcedure(DbTransaction transaction, string spName, params object[] parameterValues)
            {
                return SqlHelper.ExecuteScalar((SqlTransaction)transaction, correctSpName(spName), parameterValues);
            }





            /// <summary>
            /// Execute a SQL Command that returns no value
            /// </summary>
            /// <param name="commandText">Sql command</param>
            /// <returns>Number of affected rows</returns>
            public int ExecuteNoQueryBySQLText(string commandText)
            {
                return SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, commandText);
            }

            /// <summary>
            /// Execute a SQL Command that returns no value
            /// </summary>
            /// <param name="transaction">DbTransaction object</param>
            /// <param name="commandText">Sql command</param>
            /// <returns>Number of affected rows</returns>
            public int ExecuteNoQueryBySQLText(DbTransaction transaction, string commandText)
            {
                return SqlHelper.ExecuteNonQuery((SqlTransaction)transaction, CommandType.Text, commandText);
            }

            /// <summary>
            /// Execute a Stored Procedure that returns no value
            /// </summary>
            /// <param name="spName">Stored Procedure name</param>
            /// <param name="parameterValues">Stored Procedure parameters</param>
            /// <returns>Number of affected rows</returns>
            public int ExecuteNoQueryByStoredProcedure(string spName, params object[] parameterValues)
            {
                return SqlHelper.ExecuteNonQuery(ConnectionString, correctSpName(spName), parameterValues);
            }

            /// <summary>
            /// Execute a Stored Procedure that returns no value
            /// </summary>
            /// <param name="transaction">DbTransaction object</param>
            /// <param name="spName">Stored Procedure name</param>
            /// <param name="parameterValues">Stored Procedure parameters</param>
            /// <returns>Number of affected rows</returns>
            public int ExecuteNoQueryByStoredProcedure(DbTransaction transaction, string spName, params object[] parameterValues)
            {
                return SqlHelper.ExecuteNonQuery((SqlTransaction)transaction, correctSpName(spName), parameterValues);
            }





            /// <summary>
            /// Get a DbDataReader by execute a SQL Command
            /// </summary>
            /// <param name="conn"></param>
            /// <param name="commandText">Sql Command</param>
            /// <returns>DbDataReader</returns>
            public DbDataReader ExecuteReaderBySQLText(DbConnection conn, string commandText)
            {
                return SqlHelper.ExecuteReader((SqlConnection)conn, CommandType.Text, commandText);
            }

            /// <summary>
            /// Get a DbDataReader by execute a SQL Command
            /// </summary>
            /// <param name="commandText">Sql Command</param>
            /// <returns>DbDataReader</returns>
            public DbDataReader ExecuteReaderBySQLText(string commandText)
            {
                return SqlHelper.ExecuteReader(this.ConnectionString, CommandType.Text, commandText);
            }


            /// <summary>
            /// Get a DbDataReader by execute a SQL Command
            /// </summary>
            /// <param name="transaction">DbTransaction object</param>
            /// <param name="commandText">Sql Command</param>
            /// <returns>DbDataReader</returns>
            public DbDataReader ExecuteReaderBySQLText(DbTransaction transaction, string commandText)
            {
                return SqlHelper.ExecuteReader((SqlTransaction)transaction, CommandType.Text, commandText);
            }

            /// <summary>
            /// Get a DbDataReader by execute a Stored Procedure
            /// </summary>
            /// <param name="conn"></param>
            /// <param name="spName">Stored Procedure name</param>
            /// <param name="parameterValues">Stored Procedure parameters</param>
            /// <returns>DbDataReader</returns>
            public DbDataReader ExecuteReaderByStoredProcedure(DbConnection conn, string spName, params object[] parameterValues)
            {
                return SqlHelper.ExecuteReader((SqlConnection)conn, correctSpName(spName), parameterValues);
            }


            /// <summary>
            /// Get a DbDataReader by execute a Stored Procedure
            /// </summary>
            /// <param name="spName">Stored Procedure name</param>
            /// <param name="parameterValues">Stored Procedure parameters</param>
            /// <returns>DbDataReader</returns>
            public DbDataReader ExecuteReaderByStoredProcedure(string spName, params object[] parameterValues)
            {
                return SqlHelper.ExecuteReader(this.ConnectionString , correctSpName(spName), parameterValues);
            }

            /// <summary>
            /// Get a DbDataReader by execute a Stored Procedure
            /// </summary>
            /// <param name="transaction">DbTransaction object</param>
            /// <param name="spName">Stored Procedure name</param>
            /// <param name="parameterValues">Stored Procedure parameters</param>
            /// <returns>DbDataReader</returns>
            public DbDataReader ExecuteReaderByStoredProcedure(DbTransaction transaction, string spName, params object[] parameterValues)
            {
                return SqlHelper.ExecuteReader((SqlTransaction)transaction, correctSpName(spName), parameterValues);
            }




            /// <summary>
            /// Fill a Dataset by execute SQL Command
            /// </summary>
            /// <param name="commandText">Sql command</param>
            /// <param name="dataSet">Dataset to fill</param>
            /// <param name="tableNames">Names of tables in dataset</param>
            public void FillDataSetBySQLText(string commandText, DataSet dataSet, string[] tableNames)
            {
                SqlHelper.FillDataSet(ConnectionString, CommandType.Text, commandText, dataSet, tableNames);
            }

            /// <summary>
            /// Fill a Dataset by execute SQL Command
            /// </summary>
            /// <param name="transaction">DbTransaction object</param>
            /// <param name="commandText">Sql command</param>
            /// <param name="dataSet">Dataset to fill</param>
            /// <param name="tableNames">Names of tables in dataset</param>
            public void FillDataSetBySQLText(DbTransaction transaction, string commandText, DataSet dataSet, string[] tableNames)
            {
                SqlHelper.FillDataSet((SqlTransaction)transaction, CommandType.Text, commandText, dataSet, tableNames);
            }

            /// <summary>
            /// Fill a Dataset by execute Stored Procedure
            /// </summary>
            /// <param name="spName">Stored Procedure name</param>
            /// <param name="dataSet">Dataset to fill</param>
            /// <param name="tableNames">Names of tables in dataset</param>
            /// <param name="parameterValues">Stored Procedure parameters</param>
            public void FillDataSetByStoredProcedure(string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
            {
                SqlHelper.FillDataSet(ConnectionString, correctSpName(spName), dataSet, tableNames, parameterValues);
            }

            /// <summary>
            /// Fill a Dataset by execute Stored Procedure
            /// </summary>
            /// <param name="transaction">DbTransaction object</param>
            /// <param name="spName">Stored Procedure name</param>
            /// <param name="dataSet">Dataset to fill</param>
            /// <param name="tableNames">Names of tables in dataset</param>
            /// <param name="parameterValues">Stored Procedure parameters</param>
            public void FillDataSetByStoredProcedure(DbTransaction transaction, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
            {
                SqlHelper.FillDataSet((SqlTransaction)transaction, correctSpName(spName), dataSet, tableNames, parameterValues);
            }

            #endregion

        }

        /// <summary>
        /// Checks for Sql Injection in query
        /// </summary>
        protected void CheckForSqlInjection(string sqlQueryTest)
        {
            sqlQueryTest = sqlQueryTest.ToLowerInvariant();
            if (!sqlQueryTest.StartsWith("select "))
                throw new Exceptions.DangerousQueryException("The query text must start with the SELECT sentence");

            if (
                sqlQueryTest.Contains("/*") ||
                sqlQueryTest.Contains("--") ||
                sqlQueryTest.Contains(" exec ") ||
                sqlQueryTest.Contains("(exec") ||
                sqlQueryTest.Contains("(drop") ||
                sqlQueryTest.Contains(" drop ") ||
                sqlQueryTest.Contains("(delete") ||
                sqlQueryTest.Contains(" delete ")
                )
            {
                throw new Exceptions.DangerousQueryException("Dangerous script detected in query");
            }

            if (!sqlQueryTest.Contains(" from "))
            {
                throw new Exceptions.DangerousQueryException("The query text must contains the FROM clausule");
            }

            if (!sqlQueryTest.Contains(TableName.ToLowerInvariant()))
            {
                throw new Exceptions.DangerousQueryException("The FROM clausule must contains the table name");
            }
        }
        /// <summary>
        /// Max number of rows in ObjectList
        /// </summary>
        protected virtual int MaxRowsInObjectList 
            => 30000;


	    /// <summary>
        /// The max number of records to keep in cache for readonly tables.
        /// </summary>
        protected virtual int MaxObjectsInCacheForReadOnlyTable 
            => 5000;

	    /// <summary>
        /// If the table is read only
        /// </summary>
        protected virtual bool IsReadOnly()
        {
            return false;
        }

        /// <summary>
        /// If the table is cacheable
        /// </summary>
        protected virtual bool IsCacheable()
        {
            return false;
        }


        /// <summary>
        /// Enables the GetObjectBySQLText and GetObjectListBySQLText methods
        /// </summary>
        protected virtual bool SQLQueriesEnabled()
        {
            return false;
        }





        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string StoredProceduresPrefix() { return "";}

        /// <summary>
        /// Table name of matching Object or Entity
        /// </summary>
        protected virtual string TableName 
            => "";

	    /// <summary>
        /// 
        /// </summary>
        protected virtual string RuleName 
            => "";
	}
}

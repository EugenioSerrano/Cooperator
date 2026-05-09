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
using System.Data.SqlClient;
using System.Data.Common;
using System.Reflection;
using Cooperator.Framework.Core;
using System.Web;

namespace Cooperator.Framework.Data
{

    /// <summary>
    /// Base class for Gateways and Mappers
    /// </summary>
    /// <typeparam name="T">Object or Entity type</typeparam>
    /// <typeparam name="T2">objectList or EntityList type</typeparam>
    public abstract class BaseGateway<T, T2> : BaseRule, IBaseGateway<T, T2> where T : IObject, new()
        where T2 : IObjectList<T>, new()
    {


        #region Events

        /// <summary>
        /// Event raised before insert Object or Entity
        /// </summary>
        public event EventHandler<InsertEventArgs<T>> BeforeInsert;

        /// <summary>
        /// Event raised before delete Object or Entity
        /// </summary>
        public event EventHandler<DeleteEventArgs<T>> BeforeDelete;

        /// <summary>
        /// Event raised before save Object or Entity
        /// </summary>
        public event EventHandler<SaveEventArgs<T>> BeforeSave;

        /// <summary>
        /// Event raised before update Object or Entity
        /// </summary>
        public event EventHandler<UpdateEventArgs<T, T2>> BeforeUpdate;


        #endregion


        #region Abstract and private members




        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void CompleteEntity(T entity)
        { }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual byte[] GetAssemblyToken()
        {
            return null;
        }


        private static int _checkForTokenCounter;
        private void CheckForToken()
        {
            // Si ya pasamos 200 veces por aqui, no preguntamos mas.
            // Esto es 200 veces en cualquier mapper/gateway (porque es un campo static en una clase generica)
            if (_checkForTokenCounter > 200)
            {
                return;
            }

            _checkForTokenCounter++;

            if (HttpContext.Current != null)
            {
                // Es web, no chequeamos
                return;
            }

            var assemblyToken = GetAssemblyToken();
            if (assemblyToken == null)
            {
                // No se sobreescribió el metodo GetAssemblyToken(), por lo tanto no pedimos mas nada
                _checkForTokenCounter = 201;
                return;
            }

            if (Assembly.GetEntryAssembly() == null)
            {
                // Cuando el Gateway o Mapper es llamado por un Test, este objecto queda vacio, lo dejamos pasar sin error.
                _checkForTokenCounter = 201;
                return;
            }

            if (GetPublicKeyTokenFromArray(assemblyToken) != GetEntryAssemblyTokenString())
            {
                throw new Exceptions.InsufficientSecurityRightsException("This application is not authorized to use this library");
            }
        }

        private static string _entryAssemblyTokenString;
        private static string GetEntryAssemblyTokenString()
        {
            if (string.IsNullOrWhiteSpace(_entryAssemblyTokenString))
            {
                var entryAssembly = Assembly.GetEntryAssembly();

                _entryAssemblyTokenString = GetPublicKeyTokenFromArray(
                    entryAssembly.GetName().GetPublicKeyToken());
            }

            return _entryAssemblyTokenString;
        }


        private static string GetPublicKeyTokenFromArray(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return "";
            }

            var publicKeyToken = string.Empty;
            for (var i = 0; i < bytes.GetLength(0); i++)
            {
                publicKeyToken += $"{bytes[i]:x2}";
            }

            return publicKeyToken;
        }

        /// <summary>
        /// Overwrite this function to give or forbid access for acctions in mapper
        /// </summary>
        protected virtual bool CheckForSecurityRights(SecurityRights action, T2 objectListOrEntityList)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="row"></param>
        protected abstract void HydrateFields(DbDataReader reader, T row);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        protected abstract object[] GetFieldsForInsert(T row);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        protected abstract object[] GetFieldsForDelete(T row);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        protected abstract object[] GetFieldsForUpdate(T row);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="parameters"></param>
        protected abstract void UpdateObjectFromOutputParams(T row, object[] parameters);


        #endregion


        #region GetOne

        /// <summary>
        /// Get one Object or Entity by execute stored procedure: [TableName]_GetOne
        /// </summary>
        /// <param name="primaryKey">PrimaryKey of record to retrive</param>
        /// <returns>Object or Entity</returns>
        public T GetOne(IUniqueIdentifiable primaryKey)
        {
            var row = GetOne(null, primaryKey);
            return row;
        }

        /// <summary>
        /// Get one Object or Entity by execute stored procedure: [TableName]_GetOne
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="primaryKey">PrimaryKey of record to retrive</param>
        /// <returns>Object or Entity</returns>
        public T GetOne(DbTransaction transaction, IUniqueIdentifiable primaryKey)
        {
            CheckForToken();

            if (!CheckForSecurityRights(SecurityRights.Read, default(T2)))
            {
                throw new Exceptions.InsufficientSecurityRightsException("Your's security rihgts are not good enough for this operation");
            }

            // Si es readonly, y ademas es cacheable y no estoy en una transaccion
            if ((IsReadOnly() || IsCacheable()) && transaction == null)
            {
                lock (_getAllCacheLock)
                {
                    if (_getAllCache == null) this.GetAll();
                    if (_getAllCacheDictionary.ContainsKey(UniqueIdentifierHelper.UniqueIdentifierToString(primaryKey)))
                    {
                        return (T)((ICloneable)_getAllCacheDictionary[
                            UniqueIdentifierHelper.UniqueIdentifierToString(primaryKey)]).Clone();
                    }

                    return default(T);
                }
            }

            T results;

            DbDataReader myreader;
            if (transaction == null)
            {
                using (var conn = DataBaseHelper.GetNewConnection())
                {
                    myreader = DataBaseHelper.ExecuteReaderByStoredProcedure(conn, StoredProceduresPrefix() + TableName + "_GetOne", primaryKey.Identifier());
                    results = GetObject(myreader);
                }
            }
            else
            {
                myreader = DataBaseHelper.ExecuteReaderByStoredProcedure(transaction, StoredProceduresPrefix() + TableName + "_GetOne", primaryKey.Identifier());
                results = GetObject(myreader);
            }
            return results;

        }

        #endregion


        private T GetObject(DbDataReader reader)
        {
            if (!reader.Read())
            {
                reader.Close();
                return default(T);
            }
            var row = new T();
            try
            {
                HydrateFields(reader, row);
            }
            catch (Exception)
            {
                reader.Close();
                throw new Exceptions.InvalidMappingException("The query schema do not match with the object mapping.");
            }

            reader.Close();

            CompleteEntity(row);
            row.State = ObjectState.Restored;

            return row;
        }



        /// <summary>
        /// Reset the cache if it is a cacheable table.
        /// </summary>
        public void ResetCache()
        {
            if (!IsCacheable())
            {
                throw new Exception("The table is not cacheable.");
            }


            if (HttpContext.Current == null)
            {
                _getAllCache = default(T2);
                _getAllCacheDictionary = null;
            }
            else
            {
                var key = $"Coop_DAL_{RuleName}GetAll_Cache";
                lock (_getAllCacheLock)
                    if (HttpContext.Current.Cache[key] != null) HttpContext.Current.Cache.Remove(key);

                key = $"Coop_DAL_{RuleName}GetAllDictionary_Cache";
                lock (_getAllCacheDictionaryLock)
                    if (HttpContext.Current.Cache[key] != null) HttpContext.Current.Cache.Remove(key);
            }
        }






        #region GetAll





        ///// <summary>
        ///// GetAll cache for ReadOnlyTables.
        ///// </summary>
        //private static T2 _getAllCache;
        //private static Dictionary<string, T> _getAllCacheDictionary;







        private T2 _getAllCache;
        private readonly object _getAllCacheLock = new object();
        private T2 GetAllCache
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return _getAllCache;
                }

                var key = $"Coop_DAL_{RuleName}GetAll_Cache";
                lock (_getAllCacheLock)
                {
                    if (HttpContext.Current.Cache[key] == null)
                    {
                        return default(T2);
                    }
                    if (_getAllCache == null)
                    {
                        _getAllCache = (T2)HttpContext.Current.Cache[key];
                    }

                    return _getAllCache;
                }
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    _getAllCache = value;
                }
                else
                {
                    var key = $"Coop_DAL_{RuleName}GetAll_Cache";
                    lock (_getAllCacheLock)
                    {
                        _getAllCache = value;
                        if (HttpContext.Current.Cache[key] == null)
                        {
                            HttpContext.Current.Cache.Add(key, _getAllCache, null, DateTime.Now.AddHours(1),
                                System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High,
                                null);
                        }
                        else
                        {
                            HttpContext.Current.Cache[key] = _getAllCache;
                        }
                    }
                }
            }
        }


        private Dictionary<string, T> _getAllCacheDictionary;
        readonly object _getAllCacheDictionaryLock = new object();
        private Dictionary<string, T> GetAllCacheDictionary
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    return _getAllCacheDictionary;
                }

                var key = $"Coop_DAL_{RuleName}GetAllDictionary_Cache";
                lock (_getAllCacheDictionaryLock)
                {
                    if (HttpContext.Current.Cache[key] == null)
                    {
                        return default(Dictionary<string, T>);
                    }

                    if (_getAllCacheDictionary == null)
                    {
                        _getAllCacheDictionary = (Dictionary<string, T>)HttpContext.Current.Cache[key];
                    }
                    return _getAllCacheDictionary;
                }
            }
            set
            {
                if (HttpContext.Current == null)
                {
                    _getAllCacheDictionary = value;
                }
                else
                {
                    var key = $"Coop_DAL_{RuleName}GetAllDictionary_Cache";
                    lock (_getAllCacheDictionaryLock)
                    {
                        _getAllCacheDictionary = value;
                        if (HttpContext.Current.Cache[key] == null)
                        {
                            HttpContext.Current.Cache.Add(key, _getAllCacheDictionary, null,
                                System.DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration,
                                System.Web.Caching.CacheItemPriority.High, null);
                        }
                        else
                        {
                            HttpContext.Current.Cache[key] = _getAllCacheDictionary;
                        }
                    }
                }
            }
        }







        /// <summary>
        /// Get all rows or entities by execute stored procedure: [TableName]_GetAll
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <returns>objectList or entity list</returns>
        public T2 GetAll(DbTransaction transaction)
        {
            CheckForToken();

            if (!CheckForSecurityRights(SecurityRights.Read, default(T2)))
            {
                throw new Exceptions.InsufficientSecurityRightsException("Your's security rihgts are not good enough for this operation");
            }

            T2 results;
            DbDataReader myReader;

            if (IsReadOnly() || IsCacheable())
            {
                lock (_getAllCacheLock)
                {
                    if (_getAllCache != null)
                    {
                        var newList = new T2();
                        foreach (T item in _getAllCache)
                        {
                            newList.Add((T)((ICloneable)item).Clone());
                        }

                        return newList;
                    }

                    if (transaction == null)
                    {
                        using (var conn = DataBaseHelper.GetNewConnection())
                        {
                            myReader = DataBaseHelper.ExecuteReaderByStoredProcedure(conn, StoredProceduresPrefix() + TableName + "_GetAll");
                            results = GetObjectList(myReader);
                        }
                    }
                    else
                    {
                        myReader = DataBaseHelper.ExecuteReaderByStoredProcedure(transaction, StoredProceduresPrefix() + TableName + "_GetAll");
                        results = GetObjectList(myReader);
                    }

                    if (results.Count > MaxObjectsInCacheForReadOnlyTable)
                    {
                        throw new Exceptions.TooMuchRowsInQueryException(
                            $"The table {TableName} is marked as ReadOnly or as Cacheable, but the table has more of {MaxObjectsInCacheForReadOnlyTable} records. You can override the method: MaxObjectsInCacheForReadOnlyTable, or set this table as No ReadOnly or as No Cacheable");
                    }

                    _getAllCache = results;
                    _getAllCacheDictionary = new Dictionary<string, T>();
                    foreach (var item in _getAllCache)
                    {
                        _getAllCacheDictionary.Add(
                            UniqueIdentifierHelper.UniqueIdentifierToString((IUniqueIdentifiable)item), item);
                    }


                    // Como es read only o cacheable, devolvemos una lista clonada
                    var newList2 = new T2();
                    foreach (var item in results)
                    {
                        newList2.Add((T)((ICloneable)item).Clone());
                    }

                    return newList2;
                }

            }


            // No es read only 
            if (transaction == null)
            {
                using (var conn = DataBaseHelper.GetNewConnection())
                {
                    myReader = DataBaseHelper.ExecuteReaderByStoredProcedure(conn, StoredProceduresPrefix() + TableName + "_GetAll");
                    results = GetObjectList(myReader);
                }
            }
            else
            {
                myReader = DataBaseHelper.ExecuteReaderByStoredProcedure(transaction, StoredProceduresPrefix() + TableName + "_GetAll");
                results = GetObjectList(myReader);
            }

            return results;
        }


        /// <summary>
        /// Get all rows or entities by execute stored procedure: [TableName]_GetAll
        /// </summary>
        /// <returns>objectList or entity list</returns>
        public T2 GetAll()
        {
            return GetAll(null);
        }

        #endregion

        private T2 GetObjectList(DbDataReader reader)
        {
            var counter = 0;
            var myArray = new T[MaxRowsInObjectList];
            while (reader.Read())
            {
                myArray[counter] = new T();
                try
                {
                    HydrateFields(reader, myArray[counter]);
                }
                catch (Exception)
                {
                    reader.Close();
                    throw new Exceptions.InvalidMappingException("The query schema do not match with the object mapping.");
                }
                counter += 1;

                if (counter >= MaxRowsInObjectList)
                {
                    reader.Close();
                    throw new Exceptions.TooMuchRowsInQueryException(
                        $"The query exceeds the max of {MaxRowsInObjectList} records.");
                }
            }
            reader.Close();
            var objectList = new T2();
            objectList.AddRange(myArray);

            myArray = null;
            objectList.RemoveRange(counter, MaxRowsInObjectList - counter);
            if (objectList.Count > 15000)
            {
                GC.Collect();
            }

            foreach (var row in objectList)
            {
                CompleteEntity(row);
                row.State = ObjectState.Restored;
            }

            return objectList;
        }

        #region ExecuteNonQuery

        /// <summary>
        /// Ejecuta ExecuteNonQuery de SqlHelper y gestiona las excepciones
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="storedProcedureName"></param>
        /// <param name="parameterValues"></param>
        private void ExecuteNonQuery(DbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            try
            {
                int rowAfecteds;
                if (transaction != null)
                {
                    rowAfecteds = SqlHelper.ExecuteNonQuery((SqlTransaction)transaction,
                        InternalDBHelper.correctSpName(storedProcedureName), parameterValues);
                }
                else
                {
                    rowAfecteds = SqlHelper.ExecuteNonQuery(DataBaseHelper.ConnectionString,
                        InternalDBHelper.correctSpName(storedProcedureName), parameterValues);
                }
                if (rowAfecteds == 0)
                {
                    throw new Exceptions.NoRowAffectedException();
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Message)
                {
                    case "CONCURRENCE ERROR":
                        throw new Exceptions.RowConcurrenceException();

                    case "ROW DO NOT EXIST":
                        throw new Exceptions.RowDoNotExistException();
                }

                throw;
            }
        }

        #endregion


        #region GetByParent

        /// <summary>
        /// Get rows by parent
        /// </summary>
        public T2 GetByParent(IUniqueIdentifiable parent)
        {
            return GetByParent(null, parent);
        }

        /// <summary>
        /// Get rows by parent
        /// </summary>
        public T2 GetByParent(DbTransaction transaction, IUniqueIdentifiable parent)
        {
            CheckForToken();

            if (!CheckForSecurityRights(SecurityRights.Read, default(T2)))
            {
                throw new Exceptions.InsufficientSecurityRightsException("Your's security rihgts are not good enough for this operation");
            }

            T2 results;
            DbDataReader myReader;
            if (transaction == null)
            {
                using (var conn = DataBaseHelper.GetNewConnection())
                {
                    myReader = DataBaseHelper.ExecuteReaderByStoredProcedure(
                        conn, StoredProceduresPrefix() + TableName + "_GetByParent", parent.Identifier());
                    results = GetObjectList(myReader);
                }

            }
            else
            {
                myReader = DataBaseHelper.ExecuteReaderByStoredProcedure(
                    transaction, StoredProceduresPrefix() + TableName + "_GetByParent", parent.Identifier());
                results = GetObjectList(myReader);
            }

            return results;
        }

        #endregion


        #region GetObjectListByAnyStoredProcedure


        /// <summary>
        /// Get objectList or EntityList by execute any stored procedure
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="storedProcedureName">Stored Procedure name</param>
        /// <param name="parameterValues">Stored Procedure parameters</param>
        /// <returns>objectList or EntityList</returns>
        protected T2 GetObjectListByAnyStoredProcedure(DbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            CheckForToken();

            if (!CheckForSecurityRights(SecurityRights.Read, default(T2)))
            {
                throw new Exceptions.InsufficientSecurityRightsException("Your's security rihgts are not good enough for this operation");
            }

            T2 results;
            DbDataReader myReader;
            if (transaction == null)
            {
                using (var conn = DataBaseHelper.GetNewConnection())
                {
                    myReader = DataBaseHelper.ExecuteReaderByStoredProcedure(conn, storedProcedureName, parameterValues);
                    results = GetObjectList(myReader);
                }

            }
            else
            {
                myReader = DataBaseHelper.ExecuteReaderByStoredProcedure(transaction, storedProcedureName, parameterValues);
                results = GetObjectList(myReader);
            }

            return results;
        }



        /// <summary>
        /// Get objectList or EntityList by execute any stored procedure
        /// </summary>
        /// <param name="storedProcedureName">Stored Procedure name</param>
        /// <param name="parameterValues">Stored Procedure parameters</param>
        /// <returns>objectList or EntityList</returns>
        protected T2 GetObjectListByAnyStoredProcedure(string storedProcedureName, params object[] parameterValues)
        {
            return GetObjectListByAnyStoredProcedure(null, storedProcedureName, parameterValues);
        }



        #endregion


        #region GetObjectByAnyStoredProcedure

        /// <summary>
        /// Get one Object or Entity by execute any stored procedure
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="storedProcedureName">Stored Procedure name</param>
        /// <param name="parameterValues">Stored Procedure parameter</param>
        /// <returns>Object or Entity</returns>
        protected T GetObjectByAnyStoredProcedure(DbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            CheckForToken();

            if (!CheckForSecurityRights(SecurityRights.Read, default(T2)))
            {
                throw new Exceptions.InsufficientSecurityRightsException("Your's security rihgts are not good enough for this operation");
            }

            T results;
            DbDataReader myReader;
            if (transaction == null)
            {
                using (var conn = DataBaseHelper.GetNewConnection())
                {
                    myReader = DataBaseHelper.ExecuteReaderByStoredProcedure(conn, storedProcedureName, parameterValues);
                    results = GetObject(myReader);
                }
            }
            else
            {
                myReader = DataBaseHelper.ExecuteReaderByStoredProcedure(transaction, storedProcedureName, parameterValues);
                results = GetObject(myReader);
            }

            return results;
        }

        /// <summary>
        /// Get one Object or Entity by execute any stored procedure
        /// </summary>
        /// <param name="storedProcedureName">Stored Procedure name</param>
        /// <param name="parameterValues">Stored Procedure parameter</param>
        /// <returns>Object or Entity</returns>
        protected T GetObjectByAnyStoredProcedure(string storedProcedureName, params object[] parameterValues)
        {
            return GetObjectByAnyStoredProcedure(null, storedProcedureName, parameterValues);
        }



        #endregion


        #region GetObjectListBySQLText


        /// <summary>
        /// Get objectList or EntityList by execute a SQL Text
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="sqlQueryText">SQL Query Text</param>
        /// <returns>objectList or EntityList</returns>
        protected T2 GetObjectListBySQLText(DbTransaction transaction, string sqlQueryText)
        {
            CheckForToken();

            if (!CheckForSecurityRights(SecurityRights.Read, default(T2)))
            {
                throw new Exceptions.InsufficientSecurityRightsException("Your's security rihgts are not good enough for this operation");
            }

            if (!SQLQueriesEnabled()) throw new Exceptions.SqlQueriesDisabledException("The queries are not enabled in this gateway or mapper");
            CheckForSqlInjection(sqlQueryText);
            T2 results;

            DbDataReader myReader;
            if (transaction == null)
            {
                using (var conn = DataBaseHelper.GetNewConnection())
                {
                    myReader = DataBaseHelper.ExecuteReaderBySQLText(conn, sqlQueryText);
                    results = GetObjectList(myReader);
                }
            }
            else
            {
                myReader = DataBaseHelper.ExecuteReaderBySQLText(transaction, sqlQueryText);
                results = GetObjectList(myReader);
            }
            return results;
        }



        /// <summary>
        /// Get objectList or EntityList by execute a SQL Text
        /// </summary>
        /// <param name="sqlQueryText">SQL Query Text</param>
        /// <returns>objectList or EntityList</returns>
        protected T2 GetObjectListBySQLText(string sqlQueryText)
        {
            return GetObjectListBySQLText(null, sqlQueryText);
        }

        #endregion


        #region GetObjectBySQLText


        /// <summary>
        /// Get Object or Entity by execute SQL Text
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="sqlQueryText">SQL Query Text</param>
        /// <returns>objectList or EntityList</returns>
        protected T GetObjectBySQLText(DbTransaction transaction, string sqlQueryText)
        {
            CheckForToken();

            if (!CheckForSecurityRights(SecurityRights.Read, default(T2)))
            {
                throw new Exceptions.InsufficientSecurityRightsException("Your's security rihgts are not good enough for this operation");
            }

            if (!SQLQueriesEnabled())
            {
                throw new Exceptions.SqlQueriesDisabledException("The queries are not enabled in this gateway or mapper");
            }
            CheckForSqlInjection(sqlQueryText);
            T results;

            DbDataReader myReader;
            if (transaction == null)
            {
                using (DbConnection conn = DataBaseHelper.GetNewConnection())
                {
                    myReader = DataBaseHelper.ExecuteReaderBySQLText(conn, sqlQueryText);
                    results = GetObject(myReader);
                }
            }
            else
            {
                myReader = DataBaseHelper.ExecuteReaderBySQLText(transaction, sqlQueryText);
                results = GetObject(myReader);
            }
            return results;
        }



        /// <summary>
        /// Get Object or Entity by execute SQL Text
        /// </summary>
        /// <param name="sqlQueryText">SQL Query Text</param>
        /// <returns>objectList or EntityList</returns>
        protected T GetObjectBySQLText(string sqlQueryText)
        {
            return GetObjectBySQLText(null, sqlQueryText);
        }

        #endregion




        #region Insert

        /// <summary>
        /// Insert a Object or Entity in related table.
        /// </summary>
        /// <param name="row">Object or Entity to insert</param>	
        public virtual void Insert(T row)
        {
            this.Insert(null, row);
        }

        /// <summary>
        /// Insert a Object or Entity in related table.
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="row">Object or Entity to insert</param>
        public virtual void Insert(DbTransaction transaction, T row)
        {
            CheckForToken();

            var tempList = new T2 { row };
            if (!CheckForSecurityRights(SecurityRights.Insert, tempList))
            {
                throw new Exceptions.InsufficientSecurityRightsException("Your's security rihgts are not good enough for this operation");
            }

            // Check if table is Read Only
            if (IsReadOnly())
            {
                throw new Exceptions.TableIsReadOnlyException();
            }

            // Raise BeforeInsert event
            var e = new InsertEventArgs<T>(false, row);
            OnBeforeInsert(e);
            if (e.Cancel)
            {
                throw new Exceptions.InsertCanceledException();
            }


            //Execute validate if applicable
            if (row is IValidable validableObject)
            {
                validableObject.Validate();
            }

            // Insert
            var spName = "";

            spName = StoredProceduresPrefix() + TableName + "_Insert";
            var params2 = GetFieldsForInsert(row);

            ExecuteNonQuery(transaction, spName, params2);

            //Update Object or Entity from output params 
            UpdateObjectFromOutputParams(row, params2);
            row.State = ObjectState.Updated;

            if (IsCacheable())
            {
                ResetCache();
            }
        }

        /// <summary>
        /// Raise BeforeInsert event, if applicable
        /// </summary>
        /// <param name="e"></param>
        protected void OnBeforeInsert(InsertEventArgs<T> e)
        {
            BeforeInsert?.Invoke(this, e);
        }

        #endregion


        #region Append

        /// <summary>
        /// Append a objectList or EntityList in related table. All objects in objectList or EntityList are inserted in a transaction.
        /// </summary>
        /// <param name="objectList">objectList or EntityList to update.</param>
        public virtual void Append(T2 objectList)
        {
            Append(null, objectList);
        }


        /// <summary>
        /// Append a objectList or EntityList in related table. All objects in objectList or EntityList are inserted in a transaction.
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="objectList">objectList or EntityList to update.</param>
        public virtual void Append(DbTransaction transaction, T2 objectList)
        {
            CheckForToken();

            if (!CheckForSecurityRights(SecurityRights.Insert, objectList))
            {
                throw new Exceptions.InsufficientSecurityRightsException("Your's security rihgts are not good enough for this operation");
            }

            foreach (var item in objectList)
            {
                item.State = ObjectState.New;
            }

            //Now, clear deleted items 
            foreach (T item in ((IObjectList<T>)objectList).DeletedItems())
            {
                item.State = ObjectState.New;
            }


            Update(transaction, objectList);
        }

        #endregion


        #region Delete

        /// <summary>
        /// Delete a Object or Entity from related table.
        /// </summary>
        /// <param name="row">Object or Entity to delete.</param>
        public virtual void Delete(T row)
        {
            Delete(null, row);
        }

        /// <summary>
        /// Delete a Object or Entity from related table.
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="row">Object or Entity to delete.</param>
        public virtual void Delete(DbTransaction transaction, T row)
        {
            CheckForToken();

            var tempList = new T2 { row };
            if (!CheckForSecurityRights(SecurityRights.Delete, tempList))
            {
                throw new Exceptions.InsufficientSecurityRightsException("Your's security rihgts are not good enough for this operation");
            }

            // Check if table is Read Only
            if (IsReadOnly())
            {
                throw new Exceptions.TableIsReadOnlyException();
            }

            // Raise BeforeDelete event
            var e = new DeleteEventArgs<T>(false, row);
            OnBeforeDelete(e);
            if (e.Cancel)
            {
                throw new Exceptions.DeleteCanceledException();
            }

            //Delete
            var spName = "";

            spName = StoredProceduresPrefix() + TableName + "_Delete";
            var params2 = GetFieldsForDelete(row);

            ExecuteNonQuery(transaction, spName, params2);

            row.State = ObjectState.Updated;

            if (IsCacheable())
            {
                ResetCache();
            }
        }

        /// <summary>
        ///  Raise BeforeDelete event, if applicable
        /// </summary>
        /// <param name="e"></param>
        protected void OnBeforeDelete(DeleteEventArgs<T> e)
        {
            BeforeDelete?.Invoke(this, e);
        }

        #endregion



        #region Remove

        /// <summary>
        /// Append a objectList or EntityList in related table. All objects in objectList or EntityList are inserted in a transaction.
        /// </summary>
        /// <param name="objectList">objectList or EntityList to update.</param>
        public virtual void Remove(T2 objectList)
        {
            Remove(null, objectList);
        }


        /// <summary>
        /// Append a objectList or EntityList in related table. All objects in objectList or EntityList are inserted in a transaction.
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="objectList">objectList or EntityList to update.</param>
        public virtual void Remove(DbTransaction transaction, T2 objectList)
        {
            CheckForToken();

            if (!CheckForSecurityRights(SecurityRights.Delete, objectList))
            {
                throw new Exceptions.InsufficientSecurityRightsException("Your's security rihgts are not good enough for this operation");
            }

            var newObjectList = new T2();

            foreach (var item in objectList)
            {
                item.State = ObjectState.Deleted;
                ((IObjectList<T>)newObjectList).DeletedItems().Add(item);
            }

            Update(transaction, newObjectList);
        }

        #endregion




        #region Save

        /// <summary>
        /// Update a Object or Entity in related table.
        /// </summary>
        /// <param name="row">Object or Entity to update</param>	
        public virtual void Save(T row)
        {
            Save(null, row);
        }

        /// <summary>
        /// Update a Object or Entity in related table.
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="row">Object or Entity to update</param>
        public virtual void Save(DbTransaction transaction, T row)
        {
            CheckForToken();

            var tempList = new T2 { row };
            if (!CheckForSecurityRights(SecurityRights.Update, tempList))
            {
                throw new Exceptions.InsufficientSecurityRightsException("Your's security rihgts are not good enough for this operation");
            }

            // Check if table is Read Only
            if (IsReadOnly())
            {
                throw new Exceptions.TableIsReadOnlyException();
            }

            // Raise BeforeSave event
            var e = new SaveEventArgs<T>(false, row);
            OnBeforeSave(e);
            if (e.Cancel)
            {
                throw new Exceptions.UpdateCanceledException();
            }

            // Execute validate if applicable
            if (row is IValidable validableObject)
            {
                validableObject.Validate();
            }

            // if dont have changes, return
            if (!ObjectStateHelper.IsNewOrDeletedOrModified(row))
            {
                return;
            }

            // Update
            var spName = "";

            spName = StoredProceduresPrefix() + TableName + "_Update";
            var params2 = GetFieldsForUpdate(row);

            ExecuteNonQuery(transaction, spName, params2);

            //Update Object or Entity from output params 
            UpdateObjectFromOutputParams(row, params2);
            row.State = ObjectState.Updated;

            if (IsCacheable())
            {
                ResetCache();
            }
        }


        /// <summary>
        /// Raise BeforeSave event, if applicable
        /// </summary>
        /// <param name="e"></param>
        protected void OnBeforeSave(SaveEventArgs<T> e)
        {
            BeforeSave?.Invoke(this, e);
        }

        #endregion


        #region Update

        /// <summary>
        /// Update a objectList or EntityList in related table. All objects in objectList or EntityList are updated in a transaction.
        /// </summary>
        /// <param name="objectList">objectList or EntityList to update.</param>
        public virtual void Update(T2 objectList)
        {
            Update(null, objectList);
        }


        /// <summary>
        /// Update a objectList or EntityList in related table. All objects in objectList or EntityList are updated in a transaction.
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="objectList">objectList or EntityList to update.</param>
        public virtual void Update(DbTransaction transaction, T2 objectList)
        {
            CheckForToken();

            if (!CheckForSecurityRights(SecurityRights.Update, objectList))
            {
                throw new Exceptions.InsufficientSecurityRightsException("Your's security rihgts are not good enough for this operation");
            }

            // Check if table is Read Only
            if (IsReadOnly()) throw new Exceptions.TableIsReadOnlyException();

            // Raise BeforeUpdate event
            var e = new UpdateEventArgs<T, T2>(false, objectList);
            OnBeforeUpdate(e);
            if (e.Cancel)
            {
                throw new Exceptions.UpdateCanceledException();
            }

            // cheks if there are something to update
            var needUpdate = false;
            foreach (var currentRow in objectList)
            {
                if (ObjectStateHelper.IsNewOrDeletedOrModified(currentRow))
                {
                    needUpdate = true;
                    break;
                }
            }

            var deletedItems = ((IObjectList<T>)objectList).DeletedItems();
            if (deletedItems.Count != 0)
            {
                needUpdate = true;
            }

            if (needUpdate)
            {
                // Execute, Insert, Delete or Update for each row in objectList, in a transaction.
                var localTransaction = (transaction == null);
                DbConnection conn = null;

                if (localTransaction)
                {
                    transaction = DataBaseHelper.GetAndBeginTransaction();
                    conn = transaction.Connection;
                }
                try
                {
                    foreach (T currentRow in objectList)
                    {
                        if (ObjectStateHelper.IsNew(currentRow))
                        {
                            if (!ObjectStateHelper.IsDeleted(currentRow))
                            {
                                Insert(transaction, currentRow);
                            }
                        }
                        else if (ObjectStateHelper.IsModified(currentRow))
                        {
                            Save(transaction, currentRow);
                        }
                        else if (ObjectStateHelper.IsDeleted(currentRow))
                        {
                            Delete(transaction, currentRow);
                            ObjectStateHelper.SetAsDeleted(currentRow);
                        }
                    }

                    foreach (T currentRow in deletedItems)
                    {
                        if (!ObjectStateHelper.IsNew(currentRow))
                        {
                            Delete(transaction, currentRow);
                            ObjectStateHelper.SetAsDeleted(currentRow);
                        }
                    }

                    // End the transaction if we start it.                    
                    if (localTransaction)
                    {
                        transaction.Commit();
                    }

                    // remove deleted items from collection
                    for (var i = objectList.Count - 1; i >= 0; i--)
                    {
                        if (ObjectStateHelper.IsDeleted(objectList[i]))
                        {
                            objectList.RemoveAt(i);
                        }
                    }

                    // remove item from deleted items collection
                    ((IObjectList<T>)objectList).ResetDeletedItems();

                }
                catch (SqlException ex)
                {
                    if (localTransaction)
                    {
                        transaction.Rollback();
                    }

                    switch (ex.Message)
                    {
                        case "CONCURRENCE ERROR":
                            throw new Exceptions.RowConcurrenceException();

                        case "ROW DO NOT EXIST":
                            throw new Exceptions.RowDoNotExistException();

                        default:
                            throw;
                    }
                }
                catch (Exception)
                {
                    if (localTransaction)
                    {
                        transaction.Rollback();
                    }
                    throw;
                }
                finally
                {
                    if (localTransaction)
                    {
                        conn.Close();
                        transaction.Dispose();
                    }
                }
            }

            if (IsCacheable())
            {
                ResetCache();
            }

        }

        /// <summary>
        /// Raise BeforeUpdate event, if applicable
        /// </summary>
        /// <param name="e"></param>
        protected void OnBeforeUpdate(UpdateEventArgs<T, T2> e)
        {
            BeforeUpdate?.Invoke(this, e);
        }

        #endregion






    }



    //----------------- EventArgs classes -------------------



    /// <summary>
    /// Event argument for BeforeInsert event
    /// </summary>
    /// <typeparam name="T">Object or Entity type</typeparam>
    public class InsertEventArgs<T> : EventArgs where T : IObject
    {
        /// <summary>
        /// Event argument for BeforeInsert event
        /// </summary>
        /// <param name="cancel">If cancel operation</param>
        /// <param name="row">Object or Entity to insert</param>
        public InsertEventArgs(bool cancel, T row)
        {
            Cancel = cancel;
            Row = row;
        }

        /// <summary>
        /// If cancel operation
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Object or Entity to insert 
        /// </summary>
        public T Row { get; }
    }


    /// <summary>
    /// Event argument for BeforeDelete event
    /// </summary>
    public class DeleteEventArgs<T> : EventArgs where T : IObject
    {
        /// <summary>
        /// Event argument for Delete events
        /// </summary>
        /// <param name="cancel">If cancel operation</param>
        /// <param name="row">Object or Entity to delete</param>
        public DeleteEventArgs(bool cancel, T row)
        {
            Cancel = cancel;
            Row = row;
        }

        /// <summary>
        /// If cancel operation
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Object to delete
        /// </summary>
        public T Row { get; }
    }

    /// <summary>
    /// Event argument for BeforeSave event
    /// </summary>
    /// <typeparam name="T">Object or Entity type</typeparam>
    public class SaveEventArgs<T> : EventArgs where T : IObject
    {
        internal readonly T _row;

        /// <summary>
        /// Event argument for BeforeSave event
        /// </summary>
        /// <param name="cancel">If cancel operation</param>
        /// <param name="row">Object or Entity to save</param>
        public SaveEventArgs(bool cancel, T row)
        {
            Cancel = cancel;
            _row = row;
        }

        /// <summary>
        /// If cancel operation
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Object or Entity to save
        /// </summary>
        public T Row => _row;
    }


    /// <summary>
    /// Event argument for BeforeUpdate events
    /// </summary>
    /// <typeparam name="T">Object or Entity type</typeparam>
    /// <typeparam name="T2">objectList or EntityList type</typeparam>
    public class UpdateEventArgs<T, T2> : EventArgs
        where T : IObject
        where T2 : IObjectList<T>
    {
        /// <summary>
        /// Event argument for BeforeUpdate events
        /// </summary>
        /// <param name="cancel">If cancel operation</param>
        /// <param name="objectList">objectList or EntityList to update</param>
        public UpdateEventArgs(bool cancel, T2 objectList)
        {
            Cancel = cancel;
            ObjectList = objectList;
        }

        /// <summary>
        /// If cancel operation
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// objectList or EntityList to update
        /// </summary>
        public T2 ObjectList { get; }
    }

}

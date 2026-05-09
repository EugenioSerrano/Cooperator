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
using Cooperator.Framework.Core;
using System.Web;

namespace Cooperator.Framework.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public abstract class BaseLoader<E,T, T2> : BaseRule
        where T : IObject, new()
        where E : T, new()
        where T2 : IObjectList<E>, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="ObjectListOrEntityList"></param>
        /// <returns></returns>
        protected virtual bool CheckForSecurityRights(SecurityRights action, T2 ObjectListOrEntityList)
        {
            return true;
        }

        #region Abstract and private members




        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void CompleteEntity(E entity)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="row"></param>
        protected abstract void HydrateFields(DbDataReader reader, T row);


        #endregion


        #region GetOne

        /// <summary>
        /// Get one Object or Entity by execute stored procedure: [TableName]_GetOne
        /// </summary>
        /// <param name="primaryKey">PrimaryKey of record to retrive</param>
        /// <returns>Object or Entity</returns>
        public E GetOne(IUniqueIdentifiable primaryKey)
        {
            E row = GetOne(null, primaryKey);
            return row;
        }

        /// <summary>
        /// Get one Object or Entity by execute stored procedure: [TableName]_GetOne
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="primaryKey">PrimaryKey of record to retrive</param>
        /// <returns>Object or Entity</returns>
        public E GetOne(DbTransaction transaction, IUniqueIdentifiable primaryKey)
        {
            if (!CheckForSecurityRights(SecurityRights.Read, default(T2)))
            {
                throw new Exceptions.InsufficientSecurityRightsException ("Your's security rihgts are not good enough for this operation");
            }

            if (IsReadOnly() || IsCacheable())
            {
                if (_getAllCache == null) this.GetAll();
                if (_getAllCacheDictionary.ContainsKey(UniqueIdentifierHelper.UniqueIdentifierToString(primaryKey)))
                    return (E)((ICloneable)_getAllCacheDictionary[UniqueIdentifierHelper.UniqueIdentifierToString(primaryKey)]).Clone();
                else
                    return default(E);
            }

            E results = default(E);
            DbConnection conn = DataBaseHelper.GetNewConnection();

            DbDataReader Myreader;
            if (transaction == null)
            {
                Myreader = DataBaseHelper.ExecuteReaderByStoredProcedure(conn, StoredProceduresPrefix() + TableName + "_GetOne", primaryKey.Identifier());
            }
            else
            {
                Myreader = DataBaseHelper.ExecuteReaderByStoredProcedure(transaction, StoredProceduresPrefix() + TableName + "_GetOne", primaryKey.Identifier());
            }

            results = GetRow(Myreader);
            conn.Close();

            return results;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private E GetRow(DbDataReader reader)
        {
            if (!reader.Read())
            {
                reader.Close();
                return default(E);
            }
            E row = new E();
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
                throw new System.Exception("The table is not cacheable.");


            if (HttpContext.Current == null)
            {
                _getAllCache = default(T2);
                _getAllCacheDictionary = null;
            }
            else
            {
                string key = string.Format("Coop_DAL_{0}GetAll_Cache", RuleName);
                lock (_getAllCacheLock)
                    if (HttpContext.Current.Cache[key] != null) HttpContext.Current.Cache.Remove(key);

                key = string.Format("Coop_DAL_{0}GetAllDictionary_Cache", RuleName);
                lock (_getAllCacheDictionaryLock)
                    if (HttpContext.Current.Cache[key] != null) HttpContext.Current.Cache.Remove(key);
            }
        }






        #region GetAll


        private T2 _getAllCache;
        object _getAllCacheLock = new object();
        private T2 getAllCache
        {
            get
            {
                if (HttpContext.Current == null)
                    return _getAllCache;
                else
                {
                    string key = string.Format("Coop_DAL_{0}GetAll_Cache", RuleName);
                    lock (_getAllCacheLock)
                    {
                        if (HttpContext.Current.Cache[key] == null)
                            return default(T2);
                        else
                        {
                            if (_getAllCache == null)
                                _getAllCache = (T2)HttpContext.Current.Cache[key];
                            return _getAllCache;
                        }
                    }
                }
            }
            set
            {
                if (HttpContext.Current == null)
                    _getAllCache = value;
                else
                {
                    string key = string.Format("Coop_DAL_{0}GetAll_Cache", RuleName);
                    lock (_getAllCacheLock)
                    {
                        _getAllCache = value;
                        if (HttpContext.Current.Cache[key] == null)
                            HttpContext.Current.Cache.Add(key, _getAllCache, null, System.DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
                        else
                            HttpContext.Current.Cache[key] = _getAllCache;
                    }
                }
            }
        }


        private Dictionary<string, T> _getAllCacheDictionary;
        object _getAllCacheDictionaryLock = new object();
        private Dictionary<string, T> getAllCacheDictionary
        {
            get
            {
                if (HttpContext.Current == null)
                    return _getAllCacheDictionary;
                else
                {
                    string key = string.Format("Coop_DAL_{0}GetAllDictionary_Cache", RuleName);
                    lock (_getAllCacheDictionaryLock)
                    {
                        if (HttpContext.Current.Cache[key] == null)
                            return default(Dictionary<string, T>);
                        else
                        {
                            if (_getAllCacheDictionary == null)
                                _getAllCacheDictionary = (Dictionary<string, T>)HttpContext.Current.Cache[key];
                            return _getAllCacheDictionary;
                        }
                    }
                }
            }
            set
            {
                if (HttpContext.Current == null)
                    _getAllCacheDictionary = value;
                else
                {
                    string key = string.Format("Coop_DAL_{0}GetAllDictionary_Cache", RuleName);
                    lock (_getAllCacheDictionaryLock)
                    {
                        _getAllCacheDictionary = value;
                        if (HttpContext.Current.Cache[key] == null)
                            HttpContext.Current.Cache.Add(key, _getAllCacheDictionary, null, System.DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
                        else
                            HttpContext.Current.Cache[key] = _getAllCacheDictionary;
                    }
                }
            }
        }






        /// <summary>
        /// Get all rows or entities by execute stored procedure: [TableName]_GetAll
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <returns>ObjectList or entity list</returns>
        public T2 GetAll(DbTransaction transaction)
        {
            if (!CheckForSecurityRights(SecurityRights.Read, default(T2)))
            {
                throw new Exceptions.InsufficientSecurityRightsException ("Your's security rihgts are not good enough for this operation");
            }

            if ((IsReadOnly() || IsCacheable()) && (_getAllCache != null))
            {
                T2 newList = new T2();
                foreach (E item in _getAllCache)
                    newList.Add((E)((ICloneable)item).Clone());

                return newList;
            }

            T2 results = default(T2);

            DbDataReader MyReader;
            if (transaction == null)
            {
                using (DbConnection conn = DataBaseHelper.GetNewConnection())
                {
                    MyReader = DataBaseHelper.ExecuteReaderByStoredProcedure(conn, StoredProceduresPrefix() + TableName + "_GetAll");
                    results = GetObjectList(MyReader);
                }

            }
            else
            {
                MyReader = DataBaseHelper.ExecuteReaderByStoredProcedure(transaction, StoredProceduresPrefix() + TableName + "_GetAll");
                results = GetObjectList(MyReader);
            }

            if (IsReadOnly() || IsCacheable())
            {
                if (results.Count > MaxObjectsInCacheForReadOnlyTable)
                    throw new Exceptions.TooMuchRowsInQueryException(string.Format("The table {0} is marked as ReadOnly or as Cacheable, but the table has more of {1} records. You can override the method: MaxObjectsInCacheForReadOnlyTable, or set this table as No ReadOnly or as No Cacheable", this.TableName, this.MaxObjectsInCacheForReadOnlyTable));

                _getAllCache = results;
                _getAllCacheDictionary = new Dictionary<string, T>();
                foreach (T item in _getAllCache)
                    _getAllCacheDictionary.Add(UniqueIdentifierHelper.UniqueIdentifierToString((IUniqueIdentifiable)item), item);

            }

            return results;
        }


        /// <summary>
        /// Get all rows or entities by execute stored procedure: [TableName]_GetAll
        /// </summary>
        /// <returns>ObjectList or entity list</returns>
        public T2 GetAll()
        {
            return GetAll(null);
        }

        #endregion

        private T2 GetObjectList(DbDataReader reader)
        {
            int counter = 0;
            E[] MyArray = new E[MaxRowsInObjectList];
            while (reader.Read())
            {
                MyArray[counter] = new E();
                try
                {
                    HydrateFields(reader,MyArray[counter]);
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
                    throw new Exceptions.TooMuchRowsInQueryException(string.Format("The query exceeds the max of {0} records.", MaxRowsInObjectList));
                }
            }
            reader.Close();
            T2 ObjectList = new T2();
            ObjectList.AddRange(MyArray);

            MyArray = null;
            ObjectList.RemoveRange(counter, MaxRowsInObjectList - counter);
            if (ObjectList.Count > 15000) GC.Collect();

            foreach (E row in ObjectList)
            {
                CompleteEntity(row);
                row.State = ObjectState.Restored;
            }

            return ObjectList;
        }


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
            if (!CheckForSecurityRights(SecurityRights.Read, default(T2)))
            {
                throw new Exceptions.InsufficientSecurityRightsException ("Your's security rihgts are not good enough for this operation");
            }

            T2 results = default(T2);
            DbConnection conn = DataBaseHelper.GetNewConnection();

            DbDataReader MyReader;
            if (transaction == null)
            {
                MyReader = DataBaseHelper.ExecuteReaderByStoredProcedure(conn, StoredProceduresPrefix() + TableName + "_GetByParent", parent.Identifier());
            }
            else
            {
                MyReader = DataBaseHelper.ExecuteReaderByStoredProcedure(transaction, StoredProceduresPrefix() + TableName + "_GetByParent", parent.Identifier());
            }
            results = GetObjectList(MyReader);

            conn.Close();


            return results;
        }

        #endregion


        #region GetObjectListByAnyStoredProcedure


        /// <summary>
        /// Get ObjectList or EntityList by execute any stored procedure
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="storedProcedureName">Stored Procedure name</param>
        /// <param name="parameterValues">Stored Procedure parameters</param>
        /// <returns>ObjectList or EntityList</returns>
        protected T2 GetObjectListByAnyStoredProcedure(DbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            if (!CheckForSecurityRights(SecurityRights.Read, default(T2)))
            {
                throw new Exceptions.InsufficientSecurityRightsException ("Your's security rihgts are not good enough for this operation");
            }

            T2 results = default(T2);
            DbConnection conn = DataBaseHelper.GetNewConnection();

            DbDataReader MyReader;
            if (transaction == null)
            {
                MyReader = DataBaseHelper.ExecuteReaderByStoredProcedure(conn, storedProcedureName, parameterValues);
            }
            else
            {
                MyReader = DataBaseHelper.ExecuteReaderByStoredProcedure(transaction, storedProcedureName, parameterValues);
            }

            results = GetObjectList(MyReader);
            conn.Close();


            return results;
        }



        /// <summary>
        /// Get ObjectList or EntityList by execute any stored procedure
        /// </summary>
        /// <param name="storedProcedureName">Stored Procedure name</param>
        /// <param name="parameterValues">Stored Procedure parameters</param>
        /// <returns>ObjectList or EntityList</returns>
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
        protected E GetObjectByAnyStoredProcedure(DbTransaction transaction, string storedProcedureName, params object[] parameterValues)
        {
            if (!CheckForSecurityRights(SecurityRights.Read, default(T2)))
            {
                throw new Exceptions.InsufficientSecurityRightsException ("Your's security rihgts are not good enough for this operation");
            }

            E results = default(E);
            DbConnection conn = DataBaseHelper.GetNewConnection();

            DbDataReader MyReader;
            if (transaction == null)
            {
                MyReader = DataBaseHelper.ExecuteReaderByStoredProcedure(conn, storedProcedureName, parameterValues);
            }
            else
            {
                MyReader = DataBaseHelper.ExecuteReaderByStoredProcedure(transaction, storedProcedureName, parameterValues);
            }

            results = GetRow(MyReader);

            conn.Close();


            return results;
        }

        /// <summary>
        /// Get one Object or Entity by execute any stored procedure
        /// </summary>
        /// <param name="storedProcedureName">Stored Procedure name</param>
        /// <param name="parameterValues">Stored Procedure parameter</param>
        /// <returns>Object or Entity</returns>
        protected E GetObjectByAnyStoredProcedure(string storedProcedureName, params object[] parameterValues)
        {
            return GetObjectByAnyStoredProcedure(null, storedProcedureName, parameterValues);
        }


        #endregion


        #region GetObjectListBySQLText


        /// <summary>
        /// Get ObjectList or EntityList by execute a SQL Text
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="sqlQueryText">SQL Query Text</param>
        /// <returns>ObjectList or EntityList</returns>
        protected T2 GetObjectListBySQLText(DbTransaction transaction, string sqlQueryText)
        {
            if (!CheckForSecurityRights(SecurityRights.Read, default(T2)))
            {
                throw new Exceptions.InsufficientSecurityRightsException ("Your's security rihgts are not good enough for this operation");
            }

            if (!SQLQueriesEnabled()) throw new Exceptions.SqlQueriesDisabledException("The queries are not enabled in this gateway or mapper");
            CheckForSqlInjection(sqlQueryText);
            DbConnection conn = DataBaseHelper.GetNewConnection();

            DbDataReader MyReader;
            if (transaction == null)
            {
                MyReader = DataBaseHelper.ExecuteReaderBySQLText(conn, sqlQueryText);
            }
            else
            {
                MyReader = DataBaseHelper.ExecuteReaderBySQLText(transaction, sqlQueryText);
            }
            T2 results = GetObjectList(MyReader);
            conn.Close();
            return results;
        }



        /// <summary>
        /// Get ObjectList or EntityList by execute a SQL Text
        /// </summary>
        /// <param name="sqlQueryText">SQL Query Text</param>
        /// <returns>ObjectList or EntityList</returns>
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
        /// <returns>ObjectList or EntityList</returns>
        protected E GetObjectBySQLText(DbTransaction transaction, string sqlQueryText)
        {
            if (!CheckForSecurityRights(SecurityRights.Read, default(T2)))
            {
                throw new Exceptions.InsufficientSecurityRightsException ("Your's security rihgts are not good enough for this operation");
            }

            if (!SQLQueriesEnabled()) throw new Exceptions.SqlQueriesDisabledException("The queries are not enabled in this gateway or mapper");
            CheckForSqlInjection(sqlQueryText);
            DbConnection conn = DataBaseHelper.GetNewConnection();
            DbDataReader MyReader;
            if (transaction == null)
            {
                MyReader = DataBaseHelper.ExecuteReaderBySQLText(conn, sqlQueryText);
            }
            else
            {
                MyReader = DataBaseHelper.ExecuteReaderBySQLText(transaction, sqlQueryText);
            }
            E results = GetRow(MyReader);
            conn.Close();
            return results;
        }



        /// <summary>
        /// Get Object or Entity by execute SQL Text
        /// </summary>
        /// <param name="sqlQueryText">SQL Query Text</param>
        /// <returns>ObjectList or EntityList</returns>
        protected E GetObjectBySQLText(string sqlQueryText)
        {
            return GetObjectBySQLText(null, sqlQueryText);
        }

        #endregion


    }
}

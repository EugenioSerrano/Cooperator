#nullable disable // SPEC-261016-0020 6/C.5: archivo APORTADO del 1.4 con diagnosticos de nulabilidad del build (28) - unica mecanica de build autorizada sobre el aporte.

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

using Cooperator.Framework.Data.Caching;

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

            // SPEC-261016-0020 6/C.6: sin cache en 2.0.0: se consulta la base (sustitucion: Bolt 013).

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
        /// El caché de la capa: el <b>contrato</b> <see cref="ICacheProvider"/>, no una implementación concreta
        /// (ADR-004 §3.1). Por defecto es el proveedor del proceso; una aplicación con contenedor propio —o una
        /// prueba con un doble— lo reemplaza.
        /// </summary>
        protected virtual ICacheProvider CacheProvider => CacheRegions.Provider;

        /// <summary>
        /// Reset the cache if it is a cacheable table.
        /// </summary>
        public void ResetCache()
        {
            if (!IsCacheable())
                throw new System.Exception("The table is not cacheable.");

            // SPEC-261016-0019 6/B.1 (AC-6): se remueven exactamente las dos claves de esta regla y ninguna otra.
            CacheProvider.Remove(CacheKey.CollectionAll(TableName));
            CacheProvider.Remove(CacheKey.CollectionDictionary(TableName));
        }

        /// <summary>Lee la colección completa de la base: el productor del punto de caché de colecciones.</summary>
        /// <param name="transaction">Transacción en curso, si la hay.</param>
        /// <returns>La colección completa.</returns>
        private T2 ReadAllObjects(DbTransaction transaction)
        {
            if (transaction == null)
            {
                using (DbConnection conn = DataBaseHelper.GetNewConnection())
                {
                    return GetObjectList(DataBaseHelper.ExecuteReaderByStoredProcedure(conn, StoredProceduresPrefix() + TableName + "_GetAll"));
                }
            }

            return GetObjectList(DataBaseHelper.ExecuteReaderByStoredProcedure(transaction, StoredProceduresPrefix() + TableName + "_GetAll"));
        }






        #region GetAll


        // SPEC-261016-0020 6/C.6: las propiedades getAllCache/getAllCacheDictionary (cache del host y su espejo) se retiran.






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



            // SPEC-261016-0019 6/B.1: cuando la tabla es read only o cacheable el punto usa el CONTRATO con su clave
            // y su TTL publicados; el resto de las tablas se sigue leyendo de la base sin cachear.
            T2 results = IsReadOnly() || IsCacheable()
                ? CacheProvider.GetOrCreate(
                    CacheKey.CollectionAll(TableName),
                    () => ReadAllObjects(transaction),
                    CacheRegions.CollectionsTtl)
                : ReadAllObjects(transaction);

            if (IsReadOnly() || IsCacheable())
            {
                if (results.Count > MaxObjectsInCacheForReadOnlyTable)
                    throw new Exceptions.TooMuchRowsInQueryException(string.Format("The table {0} is marked as ReadOnly or as Cacheable, but the table has more of {1} records. You can override the method: MaxObjectsInCacheForReadOnlyTable, or set this table as No ReadOnly or as No Cacheable", this.TableName, this.MaxObjectsInCacheForReadOnlyTable));



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

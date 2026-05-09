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
using System.Data.Common;
using Cooperator.Framework.Core;

namespace Cooperator.Framework.Data
{

    /// <summary>
    /// 
    /// </summary>
    public interface IBaseGateway
    {

        /// <summary>
        /// Reset the cache if it is a cacheable table.
        /// </summary>
        void ResetCache();

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public interface IBaseGateway<T, T2> : IBaseGateway, IGateway<T, T2>
        where T : IObject, new()
        where T2 : IObjectList<T>, new()
    {
        /// <summary>
        /// Event raised before insert Object or Entity
        /// </summary>
        event EventHandler<InsertEventArgs<T>> BeforeInsert;

        /// <summary>
        /// Event raised before delete Object or Entity
        /// </summary>
        event EventHandler<DeleteEventArgs<T>> BeforeDelete;

        /// <summary>
        /// Event raised before save Object or Entity
        /// </summary>
        event EventHandler<SaveEventArgs<T>> BeforeSave;

        /// <summary>
        /// Event raised before update Object or Entity
        /// </summary>
        event EventHandler<UpdateEventArgs<T, T2>> BeforeUpdate;

        /// <summary>
        /// Get one Object or Entity by execute stored procedure: [TableName]_GetOne
        /// </summary>
        /// <param name="primaryKey">PrimaryKey of record to retrive</param>
        /// <returns>Object or Entity</returns>
        new T GetOne(IUniqueIdentifiable primaryKey);

        /// <summary>
        /// Get one Object or Entity by execute stored procedure: [TableName]_GetOne
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="primaryKey">PrimaryKey of record to retrive</param>
        /// <returns>Object or Entity</returns>
        T GetOne(DbTransaction transaction, IUniqueIdentifiable primaryKey);


        /// <summary>
        /// Get all rows or entities by execute stored procedure: [TableName]_GetAll
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <returns>objectList or entity list</returns>
        T2 GetAll(DbTransaction transaction);

        /// <summary>
        /// Get all rows or entities by execute stored procedure: [TableName]_GetAll
        /// </summary>
        /// <returns>objectList or entity list</returns>
        T2 GetAll();

        /// <summary>
        /// Get rows by parent
        /// </summary>
        new T2 GetByParent(IUniqueIdentifiable parent);

        /// <summary>
        /// Get rows by parent
        /// </summary>
        T2 GetByParent(DbTransaction transaction, IUniqueIdentifiable parent);

        /// <summary>
        /// Insert a Object or Entity in related table.
        /// </summary>
        /// <param name="row">Object or Entity to insert</param>	
        new void Insert(T row);

        /// <summary>
        /// Insert a Object or Entity in related table.
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="row">Object or Entity to insert</param>
        void Insert(DbTransaction transaction, T row);

        /// <summary>
        /// Append a objectList or EntityList in related table. All objects in objectList or EntityList are inserted in a transaction.
        /// </summary>
        /// <param name="objectList">objectList or EntityList to update.</param>
        void Append(T2 objectList);

        /// <summary>
        /// Append a objectList or EntityList in related table. All objects in objectList or EntityList are inserted in a transaction.
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="objectList">objectList or EntityList to update.</param>
        void Append(DbTransaction transaction, T2 objectList);

        /// <summary>
        /// Delete a Object or Entity from related table.
        /// </summary>
        /// <param name="row">Object or Entity to delete.</param>
        new void Delete(T row);

        /// <summary>
        /// Delete a Object or Entity from related table.
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="row">Object or Entity to delete.</param>
        void Delete(DbTransaction transaction, T row);

        /// <summary>
        /// Append a objectList or EntityList in related table. All objects in objectList or EntityList are inserted in a transaction.
        /// </summary>
        /// <param name="objectList">objectList or EntityList to update.</param>
        void Remove(T2 objectList);

        /// <summary>
        /// Append a objectList or EntityList in related table. All objects in objectList or EntityList are inserted in a transaction.
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="objectList">objectList or EntityList to update.</param>
        void Remove(DbTransaction transaction, T2 objectList);

        /// <summary>
        /// Update a Object or Entity in related table.
        /// </summary>
        /// <param name="row">Object or Entity to update</param>	
        new void Save(T row);

        /// <summary>
        /// Update a Object or Entity in related table.
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="row">Object or Entity to update</param>
        void Save(DbTransaction transaction, T row);

        /// <summary>
        /// Update a objectList or EntityList in related table. All objects in objectList or EntityList are updated in a transaction.
        /// </summary>
        /// <param name="objectList">objectList or EntityList to update.</param>
        new void Update(T2 objectList);

        /// <summary>
        /// Update a objectList or EntityList in related table. All objects in objectList or EntityList are updated in a transaction.
        /// </summary>
        /// <param name="transaction">DbTransaction object</param>
        /// <param name="objectList">objectList or EntityList to update.</param>
        void Update(DbTransaction transaction, T2 objectList);
    }

}
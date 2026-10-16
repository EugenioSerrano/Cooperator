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
using System.Text;
using Cooperator.Framework.Core;

namespace Cooperator.Framework.Core
{
    /// <summary>
    /// Interface for Objects and entity collections
    /// </summary>
    /// <typeparam name="T">Row or entity type</typeparam>
    public interface IObjectList<T>: IList<T> where T : IObject
    {

        /// <summary>
        /// Get items removed from list
        /// </summary>
        /// <returns></returns>
        IObjectList<T> DeletedItems();

        /// <summary>
        /// Clear the deletedItems list.
        /// </summary>
        void ResetDeletedItems();
        
        /// <summary>
        /// Replace Object or Entity in list, based on UniqueIdentifier
        /// </summary>
        /// <param name="replacingEntity">New Object or Entity</param>
        void Replace(T replacingEntity);

		/// <summary>
		/// Remove items in range
		/// </summary>
		/// <param name="index">Items from</param>
		/// <param name="count">Number of items to remove</param>
		void RemoveRange(int index, int count);

		/// <summary>
		/// Add items from a IEnumerable collection
		/// </summary>
		/// <param name="collection">Collection to Add</param>
		void AddRange(IEnumerable<T> collection);

        /// <summary>
        /// Get a item from list searching by UniqueIdentifier.
        /// </summary>
        /// <param name="itemToSearch">Item to search for.</param>
        /// <returns>Row or Entity, null if item not found.</returns>
        T GetItem(T itemToSearch);

        /// <summary>
        /// Get a item from list searching by UniqueIdentifier.
        /// </summary>
        /// <param name="itemToSearch">Item to search for.</param>
        /// <returns>Row or Entity, null if item not found.</returns>
        T GetItem(IUniqueIdentifiable itemToSearch);

        /// <summary>
        /// Returns a Dataset containing data from Objectset o entitylist.
        /// </summary>
        /// <returns></returns>
        System.Data.DataSet ToDataSet();
    }
}

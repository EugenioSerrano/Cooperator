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

namespace Cooperator.Framework.Core
{

    /// <summary>
    /// Class with help methods for IUniqueIdentifier implementation
    /// </summary>
    public static class UniqueIdentifierHelper
    {
        /// <summary>
        /// Checks if two rows or entities are the same, based on UniqueIdentifier
        /// </summary>
        /// <param name="rowOrEntity1">First Row or Entity</param>
        /// <param name="rowOrEntity2">Second Row or Entity</param>
        /// <returns></returns>
        public static bool IsSameObject(IUniqueIdentifiable rowOrEntity1, IUniqueIdentifiable rowOrEntity2)
        {
            return UniqueIdentifierToString(rowOrEntity1).Equals(UniqueIdentifierToString((IUniqueIdentifiable)rowOrEntity2));
        }


        /// <summary>
        /// Checks if two entities are the same, based on UniqueIdentifier
        /// </summary>
        /// <param name="obj1">First Row or Entity</param>
        /// <param name="obj2">Second Row or Entity</param>
        /// <returns></returns>
        public static bool IsTheSameKey(IUniqueIdentifiable obj1, IUniqueIdentifiable obj2)
        {
            string i1 = ArrayToString(obj1.Identifier());
            string i2 = ArrayToString(obj2.Identifier());
            return i1 == i2;
        }

        /// <summary>
        /// Returns a string with a concatenation of Unique Identifier values.
        /// </summary>
        /// <param name="rowOrEntity">Row or Entity</param>
        /// <returns></returns>
        public static string UniqueIdentifierToString(IUniqueIdentifiable rowOrEntity)
        {
            if (rowOrEntity == null) throw new System.ArgumentNullException("rowOrEntity", "This parameter can not be null");
            return rowOrEntity.GetType().FullName + ArrayToString(rowOrEntity.Identifier());
        }

        private static string ArrayToString(object[] array)
        {
            StringBuilder acum = new StringBuilder("{");
            string sep = "";
            foreach (object o in array)
            {
                if (o == null)
                {
                    acum.Append(sep);
                    acum.Append("null");
                }
                else
                {
                    acum.Append(sep);
                    acum.Append(o.ToString());
                }
                sep = ",";
            }
            acum.Append( "}");
            return acum.ToString();
        }

        /// <summary>
        /// Returns a objects array with a concatenation of Unique Identifier values plus parameters values
        /// </summary>
        /// <param name="uniqueIdentifier">Unique Identifier</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        public static object[] ComposeIdentifier(IUniqueIdentifiable uniqueIdentifier, params object[] parameters) {
            if (uniqueIdentifier == null) throw new System.ArgumentNullException("uniqueIdentifier","This parameter can not be null");
            
            object[] newArray = new object[(uniqueIdentifier.Identifier().Length + parameters.Length )];

            int counter = 0;
            foreach (object obj in uniqueIdentifier.Identifier())
            {
                newArray[counter] = obj; counter++;
            }

            foreach (object obj in parameters)
            {
                newArray[counter] = obj; counter++;
            }

            return newArray;
        }

    }
}

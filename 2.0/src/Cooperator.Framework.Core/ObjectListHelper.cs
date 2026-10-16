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
using System.Reflection;

namespace Cooperator.Framework.Core
{
    /// <summary>
    /// Helper methods to materialize a RowSet into a <see cref="System.Data.DataSet"/>.
    /// </summary>
    public class ObjectListHelper<T, T2>
        where T : IObject
        where T2 : List<T>
    {

        /// <summary>
        /// Fill a dataset from a RowSet
        /// </summary>
        public void FillDataSet(System.Data.DataSet dataSet, T2 rowSet)
        {
            if (dataSet.Tables.Count == 0)
                throw new Exceptions.InvalidDataSetException("The Dataset has no tables.");

            if (rowSet.Count == 0)
                return;

            //object firstObject = rowSet[0];

            dataSet.Tables[0].Clear();
            PropertyInfo[] propertyArray = new PropertyInfo[dataSet.Tables[0].Columns.Count];

            int counter = 0;

            try
            {
                //create array with properties with column names
                foreach (System.Data.DataColumn column in dataSet.Tables[0].Columns)
                {
                    // SPEC-261016-0034 §6/C y AC-5 (ADR-006 §3.3): la MISMA verificación que el punto del comparador de
                // orden; el fallo nombra la columna, el tipo y el punto (el catch genérico de más abajo ya no lo tapa).
                propertyArray[counter] = Mapping.TypeIdentification.RequireProperty(typeof(T), column.ColumnName, "ObjectListHelper:FillDataSet");
                    counter += 1;
                }

                foreach (T myObj in rowSet)
                {
                    System.Data.DataRow myRow = dataSet.Tables[0].NewRow();

                    counter = 0;
                    foreach (System.Data.DataColumn column in dataSet.Tables[0].Columns)
                    {

                        if (propertyArray[counter].GetValue(myObj, null) == null)
                        {
                            myRow[column.ColumnName] = System.DBNull.Value;
                        }
                        else
                        {
                            myRow[column.ColumnName] = propertyArray[counter].GetValue(myObj, null);
                        }
                        counter += 1;
                    }

                    dataSet.Tables[0].Rows.Add(myRow);
                }
            }
            catch (Exceptions.TypeResolutionException)
            {
                // SPEC-261016-0034 §6/C (AC-5): el diagnóstico de la identificación de la propiedad NO se tapa con
                // el error genérico de esquema — la causa (qué columna, qué tipo y en qué punto) es lo que hace
                // accionable el fallo. El error genérico queda sólo para lo que realmente es un desajuste.
                throw;
            }
            catch (Exception)
            {
                throw new Exceptions.InvalidDataSetException("The Dataset schema do not match with RowSet");
            }
        }

        private static string SqlName(string Tipo)
        {
            switch (Tipo)
            {
                case "System.String":
                    return "Char(255)";
                case "System.Decimal":
                    return "decimal";
                case "System.Int32":
                    return "int";
                case "System.DateTime":
                    return "date";
                case "System.Boolean":
                    return "char(5)";
                default:
                    return "char(255)";
            }
        }
    }
}

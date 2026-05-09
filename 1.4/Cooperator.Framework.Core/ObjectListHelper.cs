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

using System.Data.OleDb;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Cooperator.Framework.Core
{
    /// <summary>
    /// Helper methods for RowSet exportation.
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
                    propertyArray[counter] = typeof(T).GetProperty(column.ColumnName);
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
            catch (Exception)
            {
                throw new Exceptions.InvalidDataSetException("The Dataset schema do not match with RowSet");
            }
        }




        private static Object xlsLock = new Object();


        /// <summary>
        /// Create a Excel file from Row Set.
        /// </summary>
        public void CreateExcelFile(T2 rowSet, string spreadsheetName, string targetFileName)
        {
            lock (xlsLock)
            {
                string FileNameTemp = "";

                //Create connection string.
                OleDbConnection conn;
                using (conn = new OleDbConnection())
                {

                    FileNameTemp = targetFileName + "TMP.xls";
                    string ConStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + FileNameTemp + @"; Extended Properties=""Excel 8.0;HDR=YES""";

                    //If file existe, delete.
                    if (System.IO.File.Exists(targetFileName))
                    {
                        System.IO.File.Delete(targetFileName);
                    }

                    //Create new file
                    conn.ConnectionString = ConStr;
                    conn.Open();
                    var cmd1 = new OleDbCommand()
                    {
                        Connection = conn
                    };


                    // Create array with row properties.
                    PropertyInfo[] propertyArray = typeof(T).GetProperties();

                    // Count number of no-System properties.
                    int noIncludeProperties = 0;
                    foreach (PropertyInfo prop in propertyArray)
                    {
                        string propName = prop.Name.ToLower(System.Globalization.CultureInfo.InvariantCulture);
                        if (!((prop.PropertyType.ToString().StartsWith("System"))
                            && propName != "state"
                            && prop.CanRead
                            ))
                        {
                            noIncludeProperties += 1;
                        }
                    }

                    // Create new array with exportable properties
                    PropertyInfo[] propertyArray2 = new PropertyInfo[propertyArray.Length - noIncludeProperties];

                    int counter = 0;
                    foreach (PropertyInfo prop in propertyArray)
                    {
                        string propName = prop.Name.ToLower(System.Globalization.CultureInfo.InvariantCulture);
                        if ((prop.PropertyType.ToString().StartsWith("System"))
                            && propName != "state"
                            && prop.CanRead
                            )
                        {
                            propertyArray2[counter] = prop;
                            counter += 1;
                        }
                    }


                    // Now, we use propertyArray2
                    bool firstColumn = false;
                    System.Text.StringBuilder miSb, miSb2;
                    string quote;

                    //Build string to create the sheet
                    miSb = new System.Text.StringBuilder("CREATE TABLE [");
                    miSb.Append(spreadsheetName);
                    miSb.Append("] ( ");
                    firstColumn = true;
                    foreach (PropertyInfo prop in propertyArray2)
                    {
                        if (firstColumn)
                        {
                            miSb.Append(" [");
                            firstColumn = false;
                        }
                        else
                        {
                            miSb.Append(", [");
                        }
                        miSb.Append(prop.Name);
                        miSb.Append("] ");
                        miSb.Append(SqlName(prop.PropertyType.ToString()));
                    }
                    miSb.Append(")");
                    // Create the sheet
                    cmd1.CommandText = miSb.ToString();
                    cmd1.ExecuteNonQuery();

                    //Now, create all the rows
                    foreach (T myObj in rowSet)
                    {

                        miSb = new System.Text.StringBuilder("INSERT INTO [");
                        miSb.Append(spreadsheetName);
                        miSb.Append("] ( ");
                        firstColumn = true;
                        foreach (PropertyInfo prop in propertyArray2)
                        {
                            if (prop.GetValue(myObj, null) != null)
                            {
                                if (firstColumn)
                                {
                                    miSb.Append(" [");
                                    firstColumn = false;
                                }
                                else
                                {
                                    miSb.Append(", [");
                                }
                                miSb.Append(prop.Name);
                                miSb.Append("] ");
                            }
                        }
                        miSb.Append(") values (");
                        firstColumn = true;
                        foreach (PropertyInfo prop in propertyArray2)
                        {
                            if (prop.GetValue(myObj, null) != null)
                            {

                                string strPropType = prop.PropertyType.ToString();
                                if (strPropType.StartsWith("System.Nullable"))
                                {
                                    strPropType = strPropType.Substring(18, (strPropType.Length - 19));
                                }

                                if (strPropType == "System.String" ||
                                strPropType == "System.DateTime" ||
                                strPropType == "System.Boolean")
                                {
                                    quote = @"""";
                                }
                                else
                                {
                                    quote = "";
                                }
                                if (firstColumn)
                                {
                                    miSb.Append(quote);
                                    firstColumn = false;
                                }
                                else
                                {
                                    miSb.Append(", " + quote);
                                }
                                miSb2 = new System.Text.StringBuilder(prop.GetValue(myObj, null).ToString());
                                miSb2.Replace('\"', '\'');
                                miSb2.Replace(',', '.');
                                miSb.Append(miSb2.ToString());
                                miSb.Append(quote);
                            }
                        }
                        miSb.Append(")");

                        // Insert Row
                        cmd1.CommandText = miSb.ToString();
                        cmd1.ExecuteNonQuery();
                    }

                }  // Dispose de connection

                //Rename file to final name.
                System.IO.File.Move(FileNameTemp, targetFileName);

                GC.Collect();

                System.Threading.Thread.Sleep(100);
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

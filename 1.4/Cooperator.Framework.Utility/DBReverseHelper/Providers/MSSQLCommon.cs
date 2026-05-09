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

namespace Cooperator.Framework.Utility.DBReverseHelper.Providers.MSSQL
{
    /// <summary>
    /// 
    /// </summary>
    public enum SP_HelpTablesEnum
    {
        /// <summary>
        /// 
        /// </summary>
        Name = 0,

        /// <summary>
        /// 
        /// </summary>
        Column_name = 1,

        /// <summary>
        /// 
        /// </summary>
        Identity = 2,

        /// <summary>
        /// 
        /// </summary>
        RowGuidCol = 3,

        /// <summary>
        /// 
        /// </summary>
        Data_located_on_filegroup = 4,

        /// <summary>
        /// 
        /// </summary>
        index_name = 5,

        /// <summary>
        /// 
        /// </summary>
        constraint_type = 6,

        /// <summary>
        /// 
        /// </summary>
        referenced_by_fk = 7
    }

    

    /// <summary>
    /// 
    /// </summary>
    public class MSSQLTypeConverter : IDBTypeConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nativeType"></param>
        /// <returns></returns>
        public Boolean NativeTypeExist(string nativeType)
        {
            switch (nativeType.ToLower())
            {

                case "nchar":
                case "ntext":
                case "sysname":
                case "nvarchar":
                case "char":
                case "text":
                case "varchar":
                case "timestamp":
                case "bigint":
                case "int":
                case "smallint":
                case "tinyint":
                case "bit":
                case "smallmoney":
                case "money":
                case "float":
                case "real":
                case "decimal":
                case "numeric":
                case "varbinary":
                case "binary":
                case "xml":
                case "smalldatetime":
                case "datetime":
                case "date":
                case "image":
                case "uniqueidentifier":
                case "sql_variant":
                    return true;
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nativeType"></param>
        /// <returns></returns>
        public Type NativeType2CLRType(string nativeType)
        {
            Type t = null;
            switch (nativeType.ToLower())
            {
                case "xml":
                case "char":
                case "nchar":
                case "ntext":
                case "nvarchar":
                case "sysname":
                case "text":
                case "varchar":
                    t = typeof(String);
                    break;
                case "bigint":
                    t = typeof(Int64);
                    break;
                case "int":
                    t = typeof(Int32);
                    break;
                case "smallint":
                    t = typeof(Int16);
                    break;
                case "tinyint":
                    t = typeof(Byte);
                    break;
                case "bit":
                    t = typeof(Boolean);
                    break;
                case "smallmoney":
                case "money":
                case "decimal":
                case "numeric":
                    t = typeof(Decimal);
                    break;
                case "float":
                case "real":
                    t = typeof(Double);
                    break;
                case "smalldatetime":
                case "datetime":
                case "date":
                    t = typeof(DateTime);
                    break;
                case "image":
                case "timestamp":
                case "varbinary":
                case "binary":
                    t = typeof(Byte[]);
                    break;
                case "uniqueidentifier":
                    t = typeof(System.Guid);
                    break;
                case "sql_variant":
                    t = typeof(System.Object);
                    break;
                default:
                    throw new Exceptions.UnknownDBTypeExeception();
            }
            return t;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nativeType"></param>
        /// <param name="nativeLength"></param>
        /// <returns></returns>
        public Nullable<int> GetLength(string nativeType, int nativeLength)
        {
            Nullable<int> rValue = null;
            switch (nativeType.ToLower())
            {
                case "nchar":
                case "ntext":
                case "sysname":
                case "nvarchar":
                    rValue = nativeLength == -1 ? -1 : nativeLength / 2;
                    break;
                case "char":
                case "text":
                case "varchar":
                case "timestamp":
                    rValue = nativeLength;
                    break;
                case "bigint":
                case "int":
                case "smallint":
                case "tinyint":
                case "bit":
                case "smallmoney":
                case "money":
                case "float":
                case "real":
                    break;
                case "decimal":
                case "numeric":
                    rValue = nativeLength;
                    break;
                case "varbinary":
                case "binary":
                case "xml":
                    rValue = nativeLength == -1 ? -1 : nativeLength;
                    break;
                case "smalldatetime":
                case "datetime":
                case "date":
                case "image":
                case "uniqueidentifier":
                case "sql_variant":
                    rValue = null;
                    break;
                default:
                    throw new Exceptions.UnknownDBTypeExeception();
            }
            return rValue;
        }


    }
}


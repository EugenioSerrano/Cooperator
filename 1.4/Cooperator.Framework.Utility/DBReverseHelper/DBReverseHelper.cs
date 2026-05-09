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

namespace Cooperator.Framework.Utility.DBReverseHelper
{

    /// <summary>
    /// 
    /// </summary>
    public interface IDBReverseProvider
    {
        /// <summary>
        /// 
        /// </summary>
        IDBTypeConverter TypeConverter { set;get;}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        DataBase GetSchema();
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IDBTypeConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nativeType"></param>
        /// <returns></returns>
        Boolean NativeTypeExist(string nativeType);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nativeType"></param>
        /// <returns></returns>
        Type NativeType2CLRType(string nativeType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nativeType"></param>
        /// <param name="nativeLength"></param>
        /// <returns></returns>
        Nullable<int> GetLength(string nativeType, int nativeLength);
    }


    /// <summary>
    /// 
    /// </summary>
    public class DBReverseEngine
    {
        /// <summary>
        /// 
        /// </summary>
        public IDBReverseProvider dbReverseProvider = null;

        /// <summary>
        /// 
        /// </summary>
        public IDBTypeConverter dbTypeConverter = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engineFactory"></param>
        public DBReverseEngine(DBReverseEngineFactory engineFactory)
        {
            dbReverseProvider = engineFactory.CreateDBReverseProvider();
            dbTypeConverter = engineFactory.CreateDBTypeConverter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataBase GetSchema()
        {
            dbReverseProvider.TypeConverter = dbTypeConverter;
            DataBase db = dbReverseProvider.GetSchema();
            foreach (DBTable dbt in db.Tables)
                foreach (DBForeignKey dbfk in dbt.ForeignKeys)
                {
                    if (dbt.PrimaryKey != null)
                    {
                        int level = dbt.PrimaryKey.Fields.Count;
                        switch (DBParentsHelper.IsFKInPK(dbfk, dbt.PrimaryKey))
                        {
                            case IncludedEnum.ieIncludedAndEqual:
                                dbfk.IsOneToOneKey = true;
                                break;
                            case IncludedEnum.ieIncludedButNotEqual:
                                if (level > dbt.ParentLevel)
                                {
                                    dbfk.IsParentKey = true;
                                    dbt.Parent = dbfk.RelatedTableName;
                                    dbt.ParentLevel = level;
                                    dbt.ParentForeignKey = dbfk;
                                }
                                break;
                            case IncludedEnum.ieNotIncluded:
                                dbfk.IsReferenceKey = true;
                                break;
                        }
                    }
                }

            foreach (DBTable dbt in db.Views)
                foreach (DBForeignKey dbfk in dbt.ForeignKeys)
                {
                    if (dbt.PrimaryKey != null)
                    {
                        int level = dbt.PrimaryKey.Fields.Count;
                        switch (DBParentsHelper.IsFKInPK(dbfk, dbt.PrimaryKey))
                        {
                            case IncludedEnum.ieIncludedAndEqual:
                                dbfk.IsOneToOneKey = true;
                                break;
                            case IncludedEnum.ieIncludedButNotEqual:
                                if (level > dbt.ParentLevel)
                                {
                                    dbfk.IsParentKey = true;
                                    dbt.Parent = dbfk.RelatedTableName;
                                    dbt.ParentLevel = level;
                                    dbt.ParentForeignKey = dbfk;
                                }
                                break;
                            case IncludedEnum.ieNotIncluded:
                                dbfk.IsReferenceKey = true;
                                break;
                        }
                    }
                }

            return db;
        }
    }

    /// <summary>
    /// Clase abstracta de la que heredan las factories de los distintos providers.
    /// </summary>
    public abstract class DBReverseEngineFactory
    {
        string _connectionString = "";
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString
        {
            set { _connectionString = value; }
            get { return _connectionString; }
        }
        /// <summary>
        /// Crea un provider
        /// </summary>
        /// <returns></returns>
        public abstract IDBReverseProvider CreateDBReverseProvider();



        

        /// <summary>
        /// Crea el TypeConverter asociado al provider
        /// </summary>
        /// <returns></returns>
        public abstract IDBTypeConverter CreateDBTypeConverter();

    }

    /// <summary>
    /// 
    /// </summary>
    public enum IncludedEnum
    {
        /// <summary>
        /// 
        /// </summary>
        ieNotIncluded = 0,
        /// <summary>
        /// 
        /// </summary>
        ieIncludedButNotEqual = 1,
        /// <summary>
        /// 
        /// </summary>
        ieIncludedAndEqual = 2

    }

    /// <summary>
    /// 
    /// </summary>
    public static class DBParentsHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fk"></param>
        /// <param name="pk"></param>
        /// <returns></returns>
        public static IncludedEnum IsFKInPK(DBForeignKey fk, DBPrimaryKey pk)
        {
            string pks = DBParentsHelper.KeyToString(pk);
            string fks = DBParentsHelper.KeyToString(fk);
            if (fks == pks)
            {
                // Is Aggregation... or not
                return IncludedEnum.ieIncludedAndEqual;
            }
            else
            {
                if (pks.IndexOf(fks) == -1)
                    return IncludedEnum.ieNotIncluded;
                else
                    return IncludedEnum.ieIncludedButNotEqual;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fk"></param>
        /// <param name="pk"></param>
        /// <returns></returns>
        public static List<DBFieldKey> PartialKey(DBForeignKey fk, DBPrimaryKey pk)
        {
            List<DBFieldKey> ptKey = new List<DBFieldKey>();
            string s = DBParentsHelper.KeyToString(fk);
            foreach (DBFieldKey dpk in pk.Fields)
            {
                if (s.IndexOf(";" + dpk.Name + ";") == -1)
                    ptKey.Add(dpk);
            }
            return ptKey;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbfs"></param>
        /// <returns></returns>
        public static string KeyToString(DBForeignKey dbfs)
        {
            string s = "";
            string sep = ";";
            foreach (DBFieldKey n in dbfs.Fields)
            {
                s = s + sep + n.Name;
                sep = ";";
            }
            s = s + sep;
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbfs"></param>
        /// <returns></returns>
        public static string KeyToString(DBPrimaryKey dbfs)
        {
            string s = "";
            string sep = ";";
            foreach (DBFieldKey n in dbfs.Fields)
            {
                s = s + sep + n.Name;
                sep = ";";
            }
            s = s + sep;
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbfs"></param>
        /// <returns></returns>
        public static string KeyToString(List<DBFieldKey> dbfs)
        {
            string s = "";
            string sep = ";";
            foreach (DBFieldKey n in dbfs)
            {
                s = s + sep + n.Name;
                sep = ";";
            }
            s = s + sep;
            return s;
        }


    }
}

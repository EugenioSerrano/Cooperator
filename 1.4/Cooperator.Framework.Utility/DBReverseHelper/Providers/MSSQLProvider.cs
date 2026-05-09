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
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections.Generic;
//using MPEData = Microsoft.Practices.EnterpriseLibrary.Data;

namespace Cooperator.Framework.Utility.DBReverseHelper.Providers.MSSQL
{
    /// <summary>
    /// Factory de MSSQLProvider. Devuelve MSSQLProvider y MSSQLTypeConverter
    /// </summary>
    public class MSSQLReverseFactory : DBReverseEngineFactory
    {

        private string _connetionString = "";
        
        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        public MSSQLReverseFactory(string connectionString)
        {
            _connetionString = connectionString;
        }

        /// <summary>
        /// Metodo que crea el provider.
        /// </summary>
        /// <returns>Un objeto del tipo MSSQLProvider</returns>
        public override IDBReverseProvider CreateDBReverseProvider()
        {
            return new MSSQLProvider(_connetionString);
        }

        /// <summary>
        /// Metodo que crea el TypeConverter de este proveedor
        /// </summary>
        /// <returns>Un objeto del tipo MSSQLTypeConverter</returns>
        public override IDBTypeConverter CreateDBTypeConverter()
        {
            return new MSSQLTypeConverter();
        }
    }

    /// <summary>
    /// Provider para Microsoft SQL Server.
    /// </summary>
    public class MSSQLProvider : IDBReverseProvider
    {
        private IDBTypeConverter typeConverter = null;
        private string _connectionString = "";

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public MSSQLProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// 
        /// </summary>
        public IDBTypeConverter TypeConverter
        {
            get
            {
                return this.typeConverter;
            }
            set
            {
                this.typeConverter = value;
            }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DataBase GetSchema()
        {


            DataBase db = new DataBase();
                db.Name = GetDBName();
                GetTables(db);
                GetViews(db);
            return db;

            //DataBase db = new DataBase();
            //try
            //{
            //    db.Name = GetDBName();
            //    GetTables(db);
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
            //return db;
        }
        
        private string GetDBName()
        {
            StringBuilder mySB = new StringBuilder("select db_name()");
            DataAccess Da = new DataAccess(_connectionString);
            using(DataSet ds=Da.ExecuteSQLText(mySB.ToString()))
            {
                return ds.Tables[0].Rows[0].ItemArray[0].ToString();
            
            }
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        private DataSet GetSP_Help(string objectName)
        {
            StringBuilder mySB = new StringBuilder("exec sp_help");
            if (!String.IsNullOrEmpty(objectName)) {
                mySB.AppendFormat("'{0}'", objectName);
            }

            DataAccess Da = new DataAccess(_connectionString);
            return Da.ExecuteSQLText(mySB.ToString());
        }


        private DataSet GetTableNames()
        {
            DataAccess Da = new DataAccess(_connectionString);
            return Da.ExecuteSQLText("select table_schema + '.' + table_name as Name from INFORMATION_SCHEMA.TABLES where table_type ='BASE TABLE'  and not table_name in ('dtproperties','sysdiagrams') order by table_schema, table_name");
        }

        private DataSet GetViewNames()
        {
            DataAccess Da = new DataAccess(_connectionString);
            return Da.ExecuteSQLText("select table_schema + '.' + table_name as Name from INFORMATION_SCHEMA.TABLES where table_type ='VIEW' and not table_name in ('sysconstraints','syssegments') order by table_schema, table_name");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private Boolean GetTables(DataBase db)
        {
            using (DataSet ds = GetTableNames())
            {
                DataTable t = ds.Tables[0];
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string name = (String)dr["Name"];
                    if (name.StartsWith("dbo.")) name = name.Substring(4);

                    DBTable dbt = new DBTable(name);
                    if (!dbt.Name.EndsWith("sysdiagrams"))
                    {
                        db.Tables.Add(dbt);
                        using (DataSet dst = GetSP_Help(dbt.Name))
                        {
                            GetTableFields(
                                dbt,
                                dst.Tables[(int)SP_HelpTablesEnum.Column_name],
                                dst.Tables[(int)SP_HelpTablesEnum.Identity],
                                dst.Tables[(int)SP_HelpTablesEnum.RowGuidCol]
                                );

                            if (dst.Tables.Count - 1 >= (int)SP_HelpTablesEnum.index_name)
                                GetTablePKandIndexes(dbt, dst.Tables[(int)SP_HelpTablesEnum.index_name]);
                        }
                        GetTableFKeys(dbt);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private Boolean GetViews(DataBase db)
        {
            using (DataSet ds = GetViewNames())
            {
                DataTable t = ds.Tables[0];
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string name = (String)dr["Name"];
                    if (name.StartsWith("dbo.")) name = name.Substring(4);

                    DBTable dbt = new DBTable(name);
                    if (!dbt.Name.EndsWith("sysdiagrams"))
                    {
                        db.Tables.Add(dbt);
                        using (DataSet dst = GetSP_Help(dbt.Name))
                        {
                            GetTableFields(
                                dbt,
                                dst.Tables[(int)SP_HelpTablesEnum.Column_name],
                                dst.Tables[(int)SP_HelpTablesEnum.Identity],
                                dst.Tables[(int)SP_HelpTablesEnum.RowGuidCol]
                                );

                            if (dst.Tables.Count - 1 >= (int)SP_HelpTablesEnum.index_name)
                                GetTablePKandIndexes(dbt, dst.Tables[(int)SP_HelpTablesEnum.index_name]);
                        }
                        GetTableFKeys(dbt);
                    }
                }
            }
            return true;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="myTable"></param>
        /// <param name="tbInfo"></param>
        /// <param name="tbIdentity"></param>
        /// <param name="tbRowGuidCol"></param>
        /// <returns></returns>
        private Boolean GetTableFields(DBTable myTable, DataTable tbInfo, DataTable tbIdentity, DataTable tbRowGuidCol)
        {
            //Type ut = null;
            foreach (DataRow dr in tbInfo.Rows)
            {
                bool isIdentity = false;
                foreach (DataRow dr2 in tbIdentity.Rows)
                {
                    if ((string)dr2["Identity"] == (string)dr["Column_name"])
                    {
                        isIdentity = true;
                        break;
                    }
                }

                bool isRowGuid = false;
                foreach (DataRow dr2 in tbRowGuidCol.Rows)
                {
                    if ((string)dr2["RowGuidCol"] == (string)dr["Column_name"])
                    {
                        isRowGuid = true;
                        break;
                    }
                }

                bool isRowVersion = ((string)dr["Type"]).ToLower() == "timestamp" ? true : false;

                bool isComputed = ((string)dr["Computed"] == "no" ? false : true);


                if (this.typeConverter.NativeTypeExist((string)dr["Type"]))
                {
                    myTable.Fields.Add(new DBField(
                        (string)dr["Column_name"],
                        this.TypeConverter.NativeType2CLRType((string)dr["Type"]),
                        (string)dr["Type"],
                        this.TypeConverter.GetLength((string)dr["Type"],
                        (int)dr["Length"]),
                        int.Parse( "0"+dr["Prec"].ToString()),
                        int.Parse("0" + (string)dr["Scale"].ToString()),
                        (string)dr["Nullable"] == "no" ? false : true,
                        isRowVersion,
                        isRowVersion || isRowGuid || isIdentity || isComputed ,
                        isIdentity,
                        isRowGuid,
                        isComputed
                        ));
                }
                else
                {
                    string sType=GetUnknownTypeNativeName((string)dr["Type"]);
                    myTable.Fields.Add(new DBField(
                        (string)dr["Column_name"],
                        this.TypeConverter.NativeType2CLRType(sType),
                        (string)dr["Type"],
                        null,
                        int.Parse("0" + (string)dr["Prec"].ToString()),
                        int.Parse("0" + (string)dr["Scale"].ToString()),
                        (string)dr["Nullable"] == "no" ? false : true,
                        isRowVersion,
                        isRowVersion || isRowGuid || isIdentity || isComputed,
                        isIdentity,
                        isRowGuid,
                        isComputed 
                        ));

                }

            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myTable"></param>
        /// <param name="tbInfo"></param>
        /// <returns></returns>
        private Boolean GetTablePKandIndexes(DBTable myTable, DataTable tbInfo)
        {
            foreach (DataRow dr in tbInfo.Rows)
            {
                bool hasPK = false;
                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    if (dr.Table.Columns[i].ColumnName.ToLowerInvariant() == "index_description")
                    {
                        hasPK = true;
                        break;
                    }
                }
                if (hasPK)
                {

                    if (((string)dr["index_description"]).ToLower().IndexOf("primary key") != -1)
                    {
                        // Pk
                        myTable.PrimaryKey = new DBPrimaryKey((string)dr["index_name"]);
                        string[] fkfArray = (((string)dr["index_keys"]).Split(",".ToCharArray()));
                        foreach (string s in fkfArray)
                        {
                            int i = s.IndexOf("(-)");
                            OrderEnum oe = i == -1 ? OrderEnum.Asc : OrderEnum.Desc;
                            string sF;
                            i = s.IndexOf("(");
                            if (i != -1)
                                sF = s.Substring(0, i);
                            else
                                sF = s;
                            myTable.PrimaryKey.Fields.Add(new DBFieldKey(sF.Trim(), oe));
                        }
                    }
                    else
                    {
                        // IDX
                        DBKey<DBFieldKey> idx = new DBKey<DBFieldKey>();
                        idx.Name = (string)dr["index_name"];
                        myTable.Indexes.Add(idx);
                        string[] fkfArray = (((string)dr["index_keys"]).Split(",".ToCharArray()));
                        foreach (string s in fkfArray)
                        {
                            int i = s.IndexOf("(-)");

                            OrderEnum oe = i == -1 ? OrderEnum.Asc : OrderEnum.Desc;
                            string sF;
                            i = s.IndexOf("(");
                            if (i != -1)
                                sF = s.Substring(0, i);
                            else
                                sF = s;
                            idx.Fields.Add(new DBFieldKey(sF.Trim(), oe));
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unknownTypeName"></param>
        /// <returns></returns>
        private string GetUnknownTypeNativeName(string  unknownTypeName)
        {
            DataAccess Da = new DataAccess(_connectionString);
            try
            {
                using (DataSet ds = Da.ExecuteSQLText(string.Format("EXEC sp_help '{0}'", unknownTypeName)))
                {
                    return (string)ds.Tables[0].Rows[0]["Storage_type"];
                }
            }
            catch
            {
                return unknownTypeName;
            }
        }




        private Boolean GetTableFKeys(DBTable myTable)
        {

            DataAccess Da = new DataAccess(_connectionString);

            string tableName = "";
            string schemaName = "";
            if (myTable.Name.IndexOf('.') == -1)
            {
                tableName = myTable.Name;
            }
            else
            {
                tableName = myTable.Name.Substring(myTable.Name.IndexOf('.') + 1);
                schemaName = myTable.Name.Substring(0, myTable.Name.IndexOf('.'));
            }

            if (string.IsNullOrEmpty(schemaName)) {

                // Get the FK for a table without schema
                using (DataSet ds = Da.ExecuteSQLText(string.Format("EXEC sp_fkeys @fktable_name = '{0}'", myTable.Name)))
                {
                    //DataTable t = ds.Tables[0];

                    ds.Tables[0].DefaultView.Sort = "FK_NAME";

                    DBForeignKey fk = null;
                    foreach (DataRowView dr in ds.Tables[0].DefaultView)
                    {
                        if ((fk == null) || ((string)dr["FK_NAME"] != fk.Name))
                        {
                            string schemaNameAndDot = (string)dr["PKTABLE_OWNER"] + '.';
                            if (schemaNameAndDot.ToLower().StartsWith("dbo.")) schemaNameAndDot = "";

                            fk = new DBForeignKey((string)dr["FK_NAME"], schemaNameAndDot + (string)dr["PKTABLE_NAME"]);
                            myTable.ForeignKeys.Add(fk);
                        }
                        fk.Fields.Add(new DBFieldFKey((string)dr["FKCOLUMN_NAME"], OrderEnum.Unknown, (string)dr["PKCOLUMN_NAME"]));
                    }
                }


            } else {

                // Get the FK for a table with schema
                using (DataSet ds = Da.ExecuteSQLText(string.Format("EXEC sp_fkeys @fktable_name = '{0}', @fktable_owner='{1}'", tableName, schemaName)))
                {
                    //DataTable t = ds.Tables[0];
                    
                    ds.Tables[0].DefaultView.Sort = "FK_NAME";
                    
                    DBForeignKey fk = null;
                    foreach (DataRowView dr in ds.Tables[0].DefaultView)
                    {
                        if ((fk == null) || ((string)dr["FK_NAME"] != fk.Name))
                        {
                            string schemaNameAndDot = (string)dr["PKTABLE_OWNER"] + '.';
                            if (schemaNameAndDot.ToLower().StartsWith("dbo.")) schemaNameAndDot = "";

                            fk = new DBForeignKey((string)dr["FK_NAME"], schemaNameAndDot + (string)dr["PKTABLE_NAME"]);
                            myTable.ForeignKeys.Add(fk);
                        }
                        fk.Fields.Add(new DBFieldFKey((string)dr["FKCOLUMN_NAME"], OrderEnum.Unknown, (string)dr["PKCOLUMN_NAME"]));
                    }
                }

            }


            return true;
        }

        /// <summary>
        /// Esta clase hereda de BaseRule para poder acceder a la base de datos.
        /// </summary>
        private class DataAccess : Cooperator.Framework.Data.BaseRule 
        {
            public DataAccess(string connectionString) {
                base.DataBaseHelper.SetConnectionStringValue(connectionString);
            }

            // Al fin este no lo use porque SQLHelper no permite llamar a los Stored Procedures del sistema. :-(
            //internal DataSet ExecuteStoredProcedure(string spName, params object[] parameterValues)
            //{
            //    DataSet MyDataset = new DataSet();
            //    base.DataBaseHelper.FillDataSetByStoredProcedure(spName, MyDataset, new string[] { }, parameterValues);
            //    return MyDataset;
            //}

            internal DataSet ExecuteSQLText(string SQLText)
            {
                DataSet MyDataset = new DataSet();
                base.DataBaseHelper.FillDataSetBySQLText(SQLText, MyDataset, new string[] { });
                return MyDataset;

            }
        }
    }
}

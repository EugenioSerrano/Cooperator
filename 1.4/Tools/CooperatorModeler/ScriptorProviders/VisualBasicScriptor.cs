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
using System.IO;
using System.Text;
using Cooperator.Framework.Utility.DBReverseHelper;
using Cooperator.Framework.Utility.CodeGeneratorHelper;

public abstract class VisualBasicScriptor : ClassesBaseScriptor
{
    public VisualBasicScriptor()
        : base()
    {
    }

    /// <summary>
    /// Used by: Entity.Auto.cs
    /// </summary>
    protected override string AGGREGATIONS_AND_TYPES_FOR_IMAPPEABLE(EntityNode entity)
    {
        StringBuilder sb = new StringBuilder();
        string sep = "";
        foreach (PropertyNode currentProperty in entity.Children)
        {
            if (currentProperty.NativeType == "None" && currentProperty.GenerateProperty)
            {
                string entitiesOrObjects = "";
                if (currentProperty.CLRType.EndsWith("Object") || currentProperty.CLRType.EndsWith("ObjectList")) entitiesOrObjects = "Objects.";
                else entitiesOrObjects = "Entities.";
                sb.Append(sep + currentProperty.GenerateAs + " As " + entitiesOrObjects + currentProperty.CLRType.Replace("[]", "()"));
                sep = ", ";
            }
        }
        return sb.ToString();
    }


    protected override string SET_AGGREGATIONS(EntityNode entity)
    {
        StringBuilder sb = new StringBuilder();
        string sep = "";
        foreach (PropertyNode currentProperty in entity.Children)
        {
            if (currentProperty.NativeType == "None" && currentProperty.GenerateProperty)
            {
                if (currentProperty.GenerateAsLazyLoad)
                {
                    string entityOrObject = "";
                    if (currentProperty.CLRType.EndsWith("Object") || currentProperty.CLRType.EndsWith("ObjectList"))
                        entityOrObject = "Objects.";
                    else
                        entityOrObject = "Entities.";

                    sb.Append(sep + "Dim " + currentProperty.GenerateAs + " As " + entityOrObject+ currentProperty.CLRType.Replace("[]", "()") + " = Nothing 'Lazy load");
                }
                else
                {
                    if (currentProperty.IsEntity)
                    {
                        //Ejercicios EjerciciosEntity = Mappers.EjerciciosMapper.Instance().GetOne(entity.CuitEmpresa, entity.NumeroEjercicio);
                        string param = ""; string sep2 = "";
                        string ifNull = "";
                        bool haveNullableFields = false;
                        string sep3 = "";
                        foreach (RelatedField relField in currentProperty.RelatedFields)
                        {
                            foreach (PropertyNode currentProperty2 in entity.Children)
                            {
                                if (currentProperty2.Name == relField.Name)
                                {
                                    bool byRelection = ((currentProperty2.IsPrimaryKey || currentProperty2.IsForeignKey) && !currentProperty2.GenerateProperty);
                                    string convertTo = "Convert.To" + currentProperty2.CLRType.Replace("System.", "").Replace("[]", "()");
                                    if (currentProperty2.IsNullable)
                                    {
                                        haveNullableFields = true;
                                        if (currentProperty2.CLRType == "System.String")
                                        {
                                            if (byRelection)
                                            {
                                                param += sep2 + convertTo + "(_" + relField.GenerateAsForName + "FieldInfo.GetValue(entity))";
                                                ifNull += sep3 + "Not String.IsNullOrEmpty(Convert.ToString(_" + relField.GenerateAsForName + "FieldInfo.GetValue(entity)))";
                                            }
                                            else
                                            {
                                                param += sep2 + "entity." + relField.GenerateAsForName;
                                                ifNull += sep3 + "Not String.IsNullOrEmpty(entity." + relField.GenerateAsForName + ")";
                                            }
                                        }
                                        else
                                        {
                                            if (byRelection)
                                            {
                                                param += sep2 + convertTo + "(_" + relField.GenerateAsForName + "FieldInfo.GetValue(entity))";
                                                ifNull += sep3 + "_" + relField.GenerateAsForName + "FieldInfo.GetValue(entity) IsNot Nothing ";
                                            }
                                            else
                                            {
                                                param += sep2 + "entity." + relField.GenerateAsForName + ".Value";
                                                ifNull += sep3 + "entity." + relField.GenerateAsForName + ".HasValue";
                                            }
                                        }
                                        sep2 = ", ";
                                        sep3 = " And ";
                                    }
                                    else
                                    {
                                        if (byRelection)
                                            param += sep2 + convertTo + "(_" + relField.GenerateAsForName + "FieldInfo.GetValue(entity))";
                                        else
                                            param += sep2 + "entity." + relField.GenerateAsForName;
                                        sep2 = ", ";
                                    }
                                }
                            }
                        }
                        string entityOrObject = "";
                        if (currentProperty.CLRType.EndsWith("Object") || currentProperty.CLRType.EndsWith("ObjectList"))
                            entityOrObject = "Objects.";
                        else
                            entityOrObject = "Entities.";
                        if (currentProperty.IsOneToOneRelation)
                        {
                            string gatewayOrMapper = (currentProperty.GenerateAsType == CollectionTypeEnum.Entity) ? "Mappers.{0}Mapper.Instance().GetOne(entity)" : "Gateways.{0}Gateway.Instance().GetOne(entity)";
                            string entityName = currentProperty.Parent.GenerateAs;
                            string relatedEntityName = DomainTreeHelper.GetFullGenerateAsForEntity(currentProperty.Parent.Parent, currentProperty.RelatedTableName);
                            sb.Append(sep + "Dim " + currentProperty.GenerateAs + " As " + entityOrObject + currentProperty.CLRType.Replace("[]", "()") + " = " + string.Format(gatewayOrMapper, relatedEntityName, entityName));
                        }
                        else
                        {
                            string gatewayOrMapper = (currentProperty.GenerateAsType == CollectionTypeEnum.Entity) ? " = Mappers.{0}Mapper.Instance().GetOne({1})\r\n" : " = Gateways.{0}Gateway.Instance().GetOne({1})\r\n";
                            sb.Append(sep + "Dim " + currentProperty.GenerateAs + " As " + entityOrObject + currentProperty.CLRType.Replace("[]", "()") + " = Nothing\r\n");
                            string relatedEntityName = DomainTreeHelper.GetFullGenerateAsForEntity(currentProperty.Parent.Parent, currentProperty.RelatedTableName);
                            if (haveNullableFields) sb.Append("If " + ifNull + " Then\r\n");
                            sb.Append(sep + currentProperty.GenerateAs + string.Format(gatewayOrMapper, relatedEntityName, param));
                            if (haveNullableFields) sb.Append("End If\r\n");
                        }
                    }
                    else
                    {
                        //AsientosDetalleCollection = Gateways.AsientosDetalleGateway.Instance().GetByAsientos(entity);

                        string GetByXXXX = GET_GETBYXXXXX_FOR_RELATEDTABLE(currentProperty.RelatedTableName, currentProperty.RelatedFields, currentProperty.Parent.Parent);

                        string gatewayOrMapper = (currentProperty.GenerateAsType == CollectionTypeEnum.EntityList) ? "Mappers.{0}Mapper.Instance().GetBy{1}(entity)" : "Gateways.{0}Gateway.Instance().GetBy{1}(entity)";
                        string entityName = currentProperty.Parent.GenerateAs;
                        string collectionName = DomainTreeHelper.GetFullGenerateAsForEntity(currentProperty.Parent.Parent, currentProperty.RelatedTableName);
                        sb.Append(sep + "Dim " + currentProperty.GenerateAs + " As " + currentProperty.CLRType.Replace("[]","()") + " = " + string.Format(gatewayOrMapper, collectionName, GetByXXXX));
                    }
                }
                sep = "\r\n";
            }
        }
        return sb.ToString();
    }


    /// <summary>
    /// Used by: Object.Auto.cs
    /// </summary>
    /// <returns></returns>
    protected override string OVERRIDE_TO_STRING(EntityNode entity)
    {
        if (entity.ToStringInfo.OverrideToString)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sParams = new StringBuilder();

            sb.Append("Public Overrides Function ToString() As String\r\n");
            foreach (string s in entity.ToStringInfo.ToStringParams)
            {
                string sP = (entity.FindInChildrenByName(s) as PropertyNode).GenerateAs;
                sParams.Append(sParams.Length == 0 ? sP : "," + sP);
            }
            sb.Append("return String.Format(\"" + entity.ToStringInfo.StringFotmat + "\"," + sParams.ToString() + ")\r\n");
            sb.Append("End Function\r\n");
            return sb.ToString();
        }
        else
            return "";
    }



    /// <summary>
    /// Used by: Object.Auto.cs
    /// </summary>
    /// <returns></returns>
    protected override string FIELDS_DEFINITION_FOR_OBJECTS(EntityNode entity)
    {
        StringBuilder sb = new StringBuilder();
        foreach (PropertyNode currentProperty in entity.Children)
        {
            if (currentProperty.NativeType != "None" && (currentProperty.GenerateProperty || currentProperty.IsPrimaryKey || currentProperty.IsForeignKey))
            {
                if (currentProperty.IsNullable && currentProperty.CLRType != "System.String" && currentProperty.CLRType != "System.Object" && currentProperty.CLRType != "System.Byte[]")
                    sb.Append("Protected _" + currentProperty.GenerateAs + " As System.Nullable(Of " + currentProperty.CLRType.Replace("[]","()") + ") \r\n");
                else
                    sb.Append("Protected _" + currentProperty.GenerateAs + " As "+ currentProperty.CLRType.Replace("[]","()") + "\r\n");
            }
        }
        return sb.ToString();
    }


    /// <summary>
    /// Used by: Object.Auto.cs
    /// </summary>
    /// <returns></returns>
    protected override string UPDATE_OBJECT_FROM_OUT_PARAMS(EntityNode entity)
    {
        StringBuilder sb = new StringBuilder();
        int counter = 0;
        foreach (PropertyNode currentProperty in entity.Children)
        {
            bool mustGenerate = true;
            if (currentProperty.IsRowVersion) mustGenerate = entity.GenerateAsVersionable;

            if (currentProperty.NativeType != "None" && (currentProperty.GenerateProperty || currentProperty.IsPrimaryKey) && currentProperty.IsAutoGenerated && mustGenerate)
                sb.AppendFormat("_{0} = CType(parameters({2}), {1})\r\n", currentProperty.GenerateAs, currentProperty.CLRType.Replace("[]","()"), counter);

            counter++;
        }
        return sb.ToString();
    }


    /// <summary>
    /// Used by: Object.Auto.cs
    /// </summary>
    /// <returns></returns>
    protected override string PARAMETERS_FOR_RELATED_FIELD(List<RelatedField> relatedFields, EntityNode entity)
    {
        StringBuilder sb = new StringBuilder();
        string sep = "_";
        foreach (PropertyNode prop in entity.Children)
        {
            foreach (RelatedField relField in relatedFields)
            {
                if (prop.Name == relField.Name)
                {
                    sb.Append(sep);
                    sb.Append(relField.GenerateAsForName);
                    if (prop.IsNullable && prop.CLRType != "System.String")
                        sb.Append(".Value");

                    sep = ", _";
                }
            }
        }
        return sb.ToString();
    }


    /// <summary>
    /// Used by: Object.Auto.cs, Entity.Auto.cs
    /// </summary>
    protected override string SET_AUTO_VALUES_FOR_OBJECTS_CTOR(EntityNode entity)
    {
        StringBuilder sb = new StringBuilder();
        foreach (PropertyNode currentProperty in entity.Children)
        {
            if (currentProperty.IsPrimaryKey && currentProperty.IsAutoGenerated)
            {
                string typeName = currentProperty.CLRType.Replace("System.", "");
                if (currentProperty.CLRType == "Byte[]") typeName = "ByteArray";
                sb.Append("\t\t\t_" + currentProperty.GenerateAs + " =  ValuesGenerator.Get" + typeName + "\r\n");
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Used by: Object.Auto.cs, Entity.Auto.cs
    /// </summary>
    protected override string PK_SET_VALUES_FOR_OBJECTS_CTOR(EntityNode entity)
    {
        StringBuilder sb = new StringBuilder();
        foreach (PropertyNode currentProperty in entity.Children)
        {
            if (currentProperty.NativeType != "None" && (currentProperty.GenerateProperty || currentProperty.IsPrimaryKey) && currentProperty.IsPrimaryKey && !currentProperty.IsRowVersion)
            {
                sb.Append("\t\t\t_" + currentProperty.GenerateAs + " = " + currentProperty.GenerateAs + "\r\n");
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Used by: Object.Auto.cs
    /// </summary>
    protected override string PK_PARAMS_FOR_OBJECTS_CTOR(EntityNode entity)
    {
        string sep = "";
        StringBuilder sb = new StringBuilder();
        foreach (PropertyNode currentProperty in entity.Children)
        {
            if (currentProperty.NativeType != "None" && (currentProperty.GenerateProperty || currentProperty.IsPrimaryKey) && currentProperty.IsPrimaryKey && !currentProperty.IsRowVersion)
            {
                if (currentProperty.IsNullable && currentProperty.CLRType != "System.String" && currentProperty.CLRType != "System.Object" && currentProperty.CLRType != "System.Byte[]")
                    sb.Append(sep + "ByVal " + currentProperty.GenerateAs + " As System.Nullable(Of " + currentProperty.CLRType + ") ");
                else
                    sb.Append(sep + "ByVal " + currentProperty.GenerateAs + " As "+ currentProperty.CLRType.Replace("[]","()"));
                sep = ", _\r\n\t\t\t";
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Used by: Object.Auto.cs
    /// </summary>
    protected override string ALL_FIELDS_PARAMS_FOR_OBJECTS_CTOR(EntityNode entity)
    {
        string sep = "";
        StringBuilder sb = new StringBuilder();
        foreach (PropertyNode currentProperty in entity.Children)
        {
            if (currentProperty.NativeType != "None" && (currentProperty.GenerateProperty || currentProperty.IsPrimaryKey) && !currentProperty.IsRowVersion)
            {
                if (currentProperty.IsNullable && currentProperty.CLRType != "System.String" && currentProperty.CLRType != "System.Object" && currentProperty.CLRType != "System.Byte[]")
                    sb.Append(sep + "ByVal " + currentProperty.GenerateAs + " As System.Nullable(Of " + currentProperty.CLRType + ")");
                else
                    sb.Append(sep + "ByVal " + currentProperty.GenerateAs + " As " + currentProperty.CLRType.Replace("[]","()"));
                sep = ", _\r\n\t\t\t";
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Used by: Object.Auto.cs, Entity.Auto.cs
    /// </summary>
    protected override string ALL_FIELDS_SET_VALUES_FOR_OBJECTS_CTOR(EntityNode entity)
    {
        StringBuilder sb = new StringBuilder();
        foreach (PropertyNode currentProperty in entity.Children)
        {
            if (currentProperty.NativeType != "None" && (currentProperty.GenerateProperty || currentProperty.IsPrimaryKey) && !currentProperty.IsRowVersion)
            {
                sb.Append("\t\t\t_" + currentProperty.GenerateAs + " = " + currentProperty.GenerateAs + "\r\n");
            }
        }
        return sb.ToString();
    }



    /// <summary>
    /// Used by: Mapper.Auto.cs
    /// </summary>
    protected override string PARAMETERS_FOR_DATABASEQUERY(DatabaseQuery query, bool addComma)
    {

        if (query.Parameters == null) return "";
        if (String.IsNullOrEmpty(query.Parameters.Trim())) return "";

        StringBuilder sb = new StringBuilder();
        if (addComma) sb.Append(", ");
        try
        {
            string[] parameters = query.Parameters.Split(',');
            string comma = "";
            foreach (string param in parameters)
            {
                string param2 = param.Trim();
                bool isNull = false;
                if (param2.ToLower().EndsWith("=null") || param2.ToLower().EndsWith("= null"))
                {
                    isNull = true;
                    param2 = param2.Replace("= null", "");
                    param2 = param2.Replace("=null", "");
                    param2 = param2.Replace("= Null", "");
                    param2 = param2.Replace("=Null", "");
                    param2 = param2.Replace("= NULL", "");
                    param2 = param2.Replace("=NULL", "");
                }
                  
                string name = param2.Substring(0, param2.IndexOf(' ')).Trim().Replace("@", "");
                string paramType = param2.Substring(param2.IndexOf(' ')).Trim().ToLower();

                int posi = paramType.IndexOf("(");
                if (posi != -1) paramType = paramType.Substring(0, posi);

                // Only SQL for the moment...
                Cooperator.Framework.Utility.DBReverseHelper.Providers.MSSQL.MSSQLTypeConverter tc = new Cooperator.Framework.Utility.DBReverseHelper.Providers.MSSQL.MSSQLTypeConverter();
                string CLRType = tc.NativeType2CLRType(paramType).ToString();

                if (isNull && CLRType != "System.String")
                    sb.AppendFormat("{0}ByVal {1} As Nullable(Of {2})", comma, name, CLRType);
                else
                    sb.AppendFormat("{0}ByVal {1} As {2}", comma, name, CLRType);
                comma = ", ";
            }
        }
        catch (Exception)
        {
            throw new System.Exception("Invalid Database Query parameter(s). Query: " + query.QueryName);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Used by: Mapper.Auto.cs
    /// </summary>
    protected override string PARAMETERS_VALUES_FOR_DATABASEQUERY(DatabaseQuery query)
    {
        if (query.Parameters == null) return "";
        if (String.IsNullOrEmpty(query.Parameters.Trim())) return "";

        StringBuilder sb = new StringBuilder();
        try
        {
            string[] parameters = query.Parameters.Split(',');
            foreach (string param in parameters)
            {
                string param2 = param.Trim();
                string name = param2.Substring(0, param2.IndexOf(' ')).Trim().Replace("@", "");
                sb.AppendFormat(", {0}", name);
            }
        }
        catch (Exception)
        {
            throw new System.Exception("Invalid Database Query parameter(s). Query: " + query.QueryName);
        }

        return sb.ToString();

    }

}

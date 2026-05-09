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
using Cooperator.Framework.Utility.DBReverseHelper;
using Cooperator.Framework.Utility.CodeGeneratorHelper;

public abstract class ClassesBaseScriptor : ScriptorBaseProvider
{
    public ClassesBaseScriptor()
        : base()
    {
    }

    protected abstract string OVERRIDE_TO_STRING(EntityNode entity);

    protected string CORRECT_NAME(string name)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(name);
        sb.Replace(" ", "_");
        sb.Replace("-", "_");
        return sb.ToString();
    }


    protected string GET_GETBYXXXXX_FOR_RELATEDTABLE(string RelatedTableName, List<RelatedField> relatedFields, BaseTreeNode model )
    {
        foreach (EntityNode currentEntity in model.Children)
        {
            if (currentEntity.Name == RelatedTableName)
            {
                foreach (PropertyNode currentProperty in currentEntity.Children)
                {
                    if (currentProperty.IsEntity && currentProperty.RelatedFields.Count == relatedFields.Count)
                    {
                        bool isTheSame = true;
                        int count = 0;
                        foreach (RelatedField field1 in currentProperty.RelatedFields)
                        {
                            if (field1.Name != relatedFields[count].RelatedName)
                                isTheSame = false;
                            count++;
                        }
                        if (isTheSame)
                            return currentProperty.GenerateAs.Replace("Entity", "");
                    }
                }
            }
        }
        throw new ApplicationException("Parent Entity not found. Related table name:" + RelatedTableName );
    }



    /// <summary>
    /// Used by: Entity.Auto.cs
    /// </summary>
    protected abstract string AGGREGATIONS_AND_TYPES_FOR_IMAPPEABLE(EntityNode entity);


    /// <summary>
    /// Used by: Mapper.Auto.cs
    /// </summary>
    protected string AGGREGATIONS_NAMES_FOR_IMAPPEABLE(EntityNode entity)
    {
        StringBuilder sb = new StringBuilder();
        string sep = "";
        foreach (PropertyNode currentProperty in entity.Children)
        {
            if (currentProperty.NativeType == "None" && currentProperty.GenerateProperty)
            {
                sb.Append(sep + currentProperty.GenerateAs);
                sep = ", ";
            }
        }
        return sb.ToString();
    }


    /// <summary>
    /// Used by: Mapper.Auto.cs
    /// </summary>
    protected abstract string SET_AGGREGATIONS(EntityNode entity);

    
    /// <summary>
    /// Used by: Object.Auto.cs
    /// </summary>
    protected abstract string FIELDS_DEFINITION_FOR_OBJECTS(EntityNode entity);

    /// <summary>
    /// Used by: Object.Auto.cs
    /// </summary>
    protected abstract string UPDATE_OBJECT_FROM_OUT_PARAMS(EntityNode entity);

    /// <summary>
    /// Used by: Object.Auto.cs
    /// </summary>
    /// <returns></returns>
    protected abstract string PARAMETERS_FOR_RELATED_FIELD(List<RelatedField> relatedFields, EntityNode entity);

    /// <summary>
    /// Used by: Object.Auto.cs, Entity.Auto.cs
    /// </summary>
    protected abstract string SET_AUTO_VALUES_FOR_OBJECTS_CTOR(EntityNode entity);

    /// <summary>
    /// Used by: Object.Auto.cs, Entity.Auto.cs
    /// </summary>
    protected abstract string PK_SET_VALUES_FOR_OBJECTS_CTOR(EntityNode entity);

    /// <summary>
    /// Used by: Object.Auto.cs
    /// </summary>
    protected abstract string PK_PARAMS_FOR_OBJECTS_CTOR(EntityNode entity);

    /// <summary>
    /// Used by: Object.Auto.cs
    /// </summary>
    protected abstract string ALL_FIELDS_PARAMS_FOR_OBJECTS_CTOR(EntityNode entity);

    /// <summary>
    /// Used by: Object.Auto.cs, Entity.Auto.cs
    /// </summary>
    protected abstract string ALL_FIELDS_SET_VALUES_FOR_OBJECTS_CTOR(EntityNode entity);


    /// <summary>
    /// Used by: Mapper.Auto.cs
    /// </summary>
    protected abstract string PARAMETERS_FOR_DATABASEQUERY(DatabaseQuery query, bool addComma);

    /// <summary>
    /// Used by: Mapper.Auto.cs
    /// </summary>
    protected abstract string PARAMETERS_VALUES_FOR_DATABASEQUERY(DatabaseQuery query);

}

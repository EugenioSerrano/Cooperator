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

public abstract class DBBaseScriptor : ScriptorBaseProvider
{
    public DBBaseScriptor()
        : base()
	{
	}

    protected abstract string CORRECT_TABLENAME(string EntityName);


    protected string CORRECT_NAME(string name)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(name);
        sb.Replace(" ", "_");
        sb.Replace("-", "_");
        return sb.ToString();
    }

    /// <summary>
    /// Used by: DescriptionFunction.cs
    /// </summary>
    protected abstract string PK_AS_PARAMETERS_FROM_RELATED_FIELDS(List<RelatedField> pkCollection);


    /// <summary>
    /// Used by: DescriptionFunction.cs
    /// </summary>
    protected abstract string PK_AS_WHERE_FROM_RELATED_FIELDS(List<RelatedField> pkCollection);

    /// <summary>
    /// Used by:  Insert.cs
    /// </summary>
    protected abstract string ALL_FIELDS_AS_PARAMETERS(EntityNode entity);

    /// <summary>
    /// Used by: Insert.cs
    /// </summary>
    protected abstract string ALL_FIELDS_SEPARATED_BY_COMMA(EntityNode entity);

    /// <summary>
    /// Used by: Insert.cs
    /// </summary>
    protected abstract string SET_GUID_VALUES(EntityNode entity);

    /// <summary>
    /// Used by: Insert.cs
    /// </summary>
    protected abstract string ALL_VARIABLES_OF_FIELDS_SEPARATED_BY_COMMA(EntityNode entity);

    /// <summary>
    /// Used by: Insert.cs
    /// </summary>
    protected abstract string SET_IDENTITY_VALUES(EntityNode entity);

    /// <summary>
    /// Used by: GetOne.cs
    /// </summary>
    protected abstract string PK_AS_PARAMETERS(EntityNode entity);

        /// <summary>
    /// Used by: Delete.cs
    /// </summary>
    protected abstract string PK_AS_PARAMETERS_PLUS_ROWVERSION(EntityNode entity);


    /// <summary>
    /// Used by: GetAll.cs, GetOne.cs
    /// </summary>
    protected abstract string ALL_FIELDS_FOR_SELECT(EntityNode entity, Dictionary<string, string> parameters);

    /// <summary>
    /// Used by: GetAll.cs, GetOne.cs
    /// </summary>
    protected abstract string ALL_FIELDS_FOR_SELECT(EntityNode entity, Dictionary<string, string> parameters, bool includeTableName);

    /// <summary>
    /// Used by: GetOne.cs
    /// </summary>
    protected abstract string PK_AS_WHERE(EntityNode entity);

    /// <summary>
    /// Used by: Delete.cs
    /// </summary>
    protected abstract string PK_AS_WHERE_PLUS_ROWVERSION(EntityNode entity);


    /// <summary>
    /// Used by: Update.cs, Delete.Cs
    /// </summary>
    protected abstract string PK_FIELDS_FOR_SELECT(EntityNode entity);

    /// <summary>
    /// Used by: Update.cs
    /// </summary>
    protected abstract string ALL_FIELDS_MORE_PK_AS_PARAMETERS(EntityNode entity);

    /// <summary>
    /// Used by: Update.cs
    /// </summary>
    protected abstract string SET_ALL_FIELD_BY_VARIABLES(EntityNode entity);

    /// <summary>
    /// Used by: Update.cs
    /// </summary>
    protected abstract string PK_PK_AS_WHERE(EntityNode entity);

    /// <summary>
    /// Used by: Insert.cs, Update.cs
    /// </summary>
    protected abstract string SET_ROWVERSION(EntityNode entity);


}

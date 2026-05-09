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
using System.ComponentModel;
using Cooperator.Framework.Utility.DBReverseHelper;
using Cooperator.Framework.Utility.DBReverseHelper.Providers.MSSQL;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Drawing.Design;


namespace Cooperator.Framework.Utility.DBReverseHelper
{
    /// <summary>
    /// 
    /// </summary>
    public enum CollectionTypeEnum { 
        /// <summary>
        /// 
        /// </summary>
        NotApplicable,
        /// <summary>
        /// 
        /// </summary>
        Entity,
        /// <summary>
        /// 
        /// </summary>
        Object,
        /// <summary>
        /// 
        /// </summary>
        EntityList,
        /// <summary>
        /// 
        /// </summary>
        ObjectList
    }        
    
    
    /// <summary>
    /// 
    /// </summary> 
    public static class DomainTreeHelper
    {

        /// <summary>
        /// Actualiza un arbol de entidades en base al esquema de la base.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="oldTree"></param>
        /// <returns></returns>
        public static BaseTreeNode UpdateDomainTree(string connectionString, BaseTreeNode oldTree)
        {
            // Obtenemos un nuevo arbol
            BaseTreeNode newTree = GetDomainTree(connectionString, oldTree );

            // Y recorremos el viejo y aquellos nodos con el mismo nombre lo reemplazamos.
            // De manera que solo queden sin modificarse los nuevos.
            EntityNode oldRoot = (EntityNode)oldTree;
            EntityNode newRoot = (EntityNode)newTree;


            // Antes que nada actualizamos GenerateAsChildOf
            foreach (EntityNode oldEntity in oldRoot.Children)
            {
                foreach (EntityNode newEntity in newRoot.Children)
                {
                    if (newEntity.Name == oldEntity.Name)
                    {
                        foreach (string candidate in newEntity.ParentCandidates)
                        {
                            if (candidate == oldEntity.GenerateAsChildOf)
                            {
                                newEntity.GenerateAsChildOf = oldEntity.GenerateAsChildOf;
                            }
                        }
                    }
                }
            }            
            
            foreach (EntityNode oldEntity in oldRoot.Children)
            {
                //Buscamos esta entidad en el arbol nuevo
                foreach (EntityNode newEntity in newRoot.Children)
                {
                    // Si la entidad existe en el arbol nuevo, actualizamos sus miembros
                    if (newEntity.Name == oldEntity.Name)
                    {
                        newEntity.GenerateAs = oldEntity.GenerateAs;
                        newEntity.GenerateEntity = oldEntity.GenerateEntity;
                        newEntity.GenerateObject = oldEntity.GenerateObject;
                        newEntity.GenerateAsReadOnly = oldEntity.GenerateAsReadOnly;
                        newEntity.GenerateAsCacheable = oldEntity.GenerateAsCacheable;
                        //newEntity.GenerateAsVersionable = oldEntity.GenerateAsVersionable;
                        newEntity.GenerateSpDelete = oldEntity.GenerateSpDelete;
                        newEntity.GenerateSpDeleteByParents = oldEntity.GenerateSpDeleteByParents;
                        newEntity.GenerateSpDescriptionFunction = oldEntity.GenerateSpDescriptionFunction;
                        newEntity.GenerateSpGetAll = oldEntity.GenerateSpGetAll;
                        newEntity.GenerateSpGetByParent = oldEntity.GenerateSpGetByParent;
                        newEntity.GenerateSpGetByParents = oldEntity.GenerateSpGetByParents;
                        newEntity.GenerateSpGetOne = oldEntity.GenerateSpGetOne;
                        newEntity.GenerateSpInsert = oldEntity.GenerateSpInsert;
                        newEntity.GenerateSpUpdate = oldEntity.GenerateSpUpdate;
                        if (string.IsNullOrEmpty(oldEntity.OrderBy))
                            newEntity.OrderBy = "";
                        else
                            newEntity.OrderBy = oldEntity.OrderBy;
                        newEntity.ToStringInfo = oldEntity.ToStringInfo;

                        newEntity.Namespace = oldEntity.Namespace;

                        // copy queries collection
                        foreach (DatabaseQuery oldQuery in oldEntity.Queries)
                        {
                            DatabaseQuery newQuery = new DatabaseQuery();
                            newQuery.GenerateQuery = oldQuery.GenerateQuery;
                            newQuery.Parameters = oldQuery.Parameters;
                            newQuery.QueryName = oldQuery.QueryName;
                            newQuery.QueryText = oldQuery.QueryText;
                            newQuery.ReturnType = oldQuery.ReturnType;
                            newQuery.ScalarCLRType = oldQuery.ScalarCLRType;

                            newEntity.Queries.Add(newQuery);
                        }

                        // newEntity.PrimaryKeyFields  //Esta no actualizo, porque tiene que tomar la nueva 

                        // Ahora actualizamos las propiedades de la entidad.
                        foreach (PropertyNode oldProperty in oldEntity.Children)
                        {
                            foreach (PropertyNode newProperty in newEntity.Children)
                            {
                                if (newProperty.Name == oldProperty.Name)
                                {
                                    // Es la misma propiedad. Debo actualizar.
                                    newProperty.GenerateAs = oldProperty.GenerateAs;
                                    newProperty.GenerateAsDescriptionField = oldProperty.GenerateAsDescriptionField;
                                    newProperty.GenerateAsLazyLoad = oldProperty.GenerateAsLazyLoad;
                                    newProperty.GenerateAsReadOnly = oldProperty.GenerateAsReadOnly;
                                    newProperty.GenerateAsType = oldProperty.GenerateAsType;
                                    newProperty.GenerateProperty = oldProperty.GenerateProperty;
                                }
                            }
                        }
                    }
                }
            }


            // correct generation names for related fields
            CorrectGenerationNamesForRelatedFields(newTree);



            // Devolvemos el nuevo
            return newTree;
        }



        /// <summary>
        /// Obtiene el arbol de entidades usando el esquema de la base.
        /// </summary>
        /// <returns></returns>
        public static BaseTreeNode GetDomainTree(string connectionString, BaseTreeNode oldDomainTree)
        {

            MSSQLReverseFactory factory = new MSSQLReverseFactory(connectionString);
            DataBase databaseSchema = (new DBReverseEngine(factory)).GetSchema();

            // REVISAR
            // Se agregan las Views como si fueran tablas para engañar al Modeler
            foreach (DBTable view in databaseSchema.Views)
                databaseSchema.Tables.Add(view);


            // Creamos el nodo raiz.
            EntityNode root = new EntityNode(null, "Model");

            // Creamos un nodo por cada tabla. (Representan las clases a crearse)
            foreach (DBTable t in databaseSchema.Tables)
            {

                // Verificamos si el modelo viejo tenia las mismas relaciones, y si si las tenia, pero en otra ubicacion, acomodamos nuestra lista de FK
                if (oldDomainTree != null)
                {
                    List<DBForeignKey> newForeingKeyList = new List<DBForeignKey>();

                    EntityNode oldEntity = DomainTreeHelper.FindEntityInTree(oldDomainTree, t.Name);
                    if (oldEntity != null)
                    {
                        foreach (PropertyNode oldProperty in oldEntity.Children)
                        {
                            foreach (DBForeignKey fk in t.ForeignKeys)
                            {
                                if (oldProperty.IsEntity && oldProperty.RelatedTableName == fk.RelatedTableName && oldProperty.RelatedFields.Count == fk.Fields.Count)
                                {
                                    int fc = 0;
                                    bool equal = true;
                                    foreach (RelatedField rf in oldProperty.RelatedFields)
                                    {
                                        if (rf.Name != fk.Fields[fc].Name) equal = false;
                                        fc += 1;
                                    }
                                    //Si es la misma relacion, entonces, la agregamos a la nueva lista
                                    if (equal) newForeingKeyList.Add(fk);
                                }
                            }
                        }
                    }
                    // Ahora verificamos si hay que cambiar de lugar las FK en base a la nueva lista
                    if (newForeingKeyList.Count > 0)
                    {
                        // Primero borramos de las FK originales las que ya agregamos a la lista
                        foreach (DBForeignKey fk in newForeingKeyList)
                            t.ForeignKeys.Remove(fk);

                        // Ahora agregamos a la lista de las nuevas FK las que quedaron sin borrar
                        foreach (DBForeignKey fk in t.ForeignKeys)
                            newForeingKeyList.Add (fk);

                        // Finalmente reemplazamos la lista vieja, por la nueva
                        t.ForeignKeys.Clear();
                        foreach (DBForeignKey fk in newForeingKeyList)
                            t.ForeignKeys.Add(fk);
                    }
                }

                
                // Ahora agregamos el nodo de esta tabla
                ADDNodeFromDBObject(databaseSchema, root, t);

            }    // foreach (DBTable t in databaseSchema.Tables)



            // Correct GenerateAs for reserved words
            foreach (EntityNode currentEntity in root.Children)
            {
                currentEntity.GenerateAs = currentEntity.GenerateAs.Replace(" ", "");
                if (currentEntity.GenerateAs.ToLowerInvariant().Trim() == "date") currentEntity.GenerateAs += "_0";
                if (currentEntity.GenerateAs.ToLowerInvariant().Trim() == "state") currentEntity.GenerateAs += "_0";
                if (currentEntity.GenerateAs.ToLowerInvariant().Trim() == "class") currentEntity.GenerateAs += "_0";
                foreach (PropertyNode currentProperty in currentEntity.Children)
                {
                    currentProperty.GenerateAs = currentProperty.GenerateAs.Replace(" ", "");
                    if (currentProperty.GenerateAs.ToLowerInvariant().Trim() == "date") currentProperty.GenerateAs += "_0";
                    if (currentProperty.GenerateAs.ToLowerInvariant().Trim() == "state") currentProperty.GenerateAs += "_0";
                    if (currentProperty.GenerateAs.ToLowerInvariant().Trim() == "class") currentProperty.GenerateAs += "_0";
                }
            }


            // Checks for IsVersionable 
            foreach (EntityNode currentEntity in root.Children)
                foreach (PropertyNode currentProperty in currentEntity.Children)
                    if (currentProperty.IsRowVersion) currentEntity.GenerateAsVersionable = true;

            CorrectSelectedCollections(root);

            // Correct GenerateEntity
            foreach (EntityNode currentEntity in root.Children)
            {
                EntityNode entity = DomainTreeHelper.FindEntityInTree(currentEntity.Parent, currentEntity.GenerateAsChildOf);
                if (entity != null) entity.GenerateEntity = true;
            }

            return root;

        }

        /// <summary>
        /// Genera un EntityNode para un objeto de la base de datos del Tipo DBTable
        /// En esta categoría entran Tablas y Vistas de la Base de datos.
        /// </summary>
        /// <param name="databaseSchema">Schema de la base de datos que contiene el objeto</param>
        /// <param name="root">Nodo padre</param>
        /// <param name="dbTable">Objeto a agregar</param>
        private static void ADDNodeFromDBObject(DataBase databaseSchema, EntityNode root, DBTable dbTable)
        {
            // Esto nodo representa una entidad.
            EntityNode newEntity = new EntityNode(root, dbTable.Name);


            if (dbTable.PrimaryKey == null || dbTable.PrimaryKey.Fields == null || dbTable.PrimaryKey.Fields.Count == 0)
            {
                newEntity.GenerateAsReadOnly = true;
                newEntity.GenerateSpDelete = false;
                newEntity.GenerateSpDeleteByParents = false;
                newEntity.GenerateSpDescriptionFunction = false;
                newEntity.GenerateSpGetByParent = false;
                newEntity.GenerateSpGetByParents = false;
                newEntity.GenerateSpGetOne = false;
                newEntity.GenerateSpInsert = false;
                newEntity.GenerateSpUpdate = false;
            }


            // Creamos la lista de claves.
            List<string> pklist = new List<string>();
            if (dbTable.PrimaryKey != null)
            {
                foreach (DBFieldKey pkf in dbTable.PrimaryKey.Fields)
                {
                    pklist.Add(pkf.Name);
                }
            }

            newEntity.PrimaryKeyFields = pklist;
            root.Add(newEntity);

            // Y agregamos todas las propiedades a este nodo.
            foreach (DBField f in dbTable.Fields)
            {
                // Verificamos si este campo es parte de la PK
                bool isPrimaryKey = false;
                if (dbTable.PrimaryKey != null)
                {
                    foreach (DBFieldKey pkf in dbTable.PrimaryKey.Fields)
                    {
                        if (pkf.Name.ToLower() == f.Name.ToLower())
                        {
                            isPrimaryKey = true;
                            break;
                        }
                    }
                }

                List<string> parentCandidatesList = new List<string>();
                parentCandidatesList.Add("None");
                //newEntity.GenerateAsChildOf = "None";

                // Verify if this field is part of FK
                bool isForeignKey = false;
                if (dbTable.ForeignKeys != null)
                {
                    foreach (DBForeignKey fk in dbTable.ForeignKeys)
                    {
                        if (!parentCandidatesList.Contains(fk.RelatedTableName))
                        {
                            parentCandidatesList.Add(fk.RelatedTableName);
                            foreach (DBFieldKey fkfield in fk.Fields)
                            {
                                if (fkfield.Name.ToLower() == f.Name.ToLower())
                                {
                                    isForeignKey = true;
                                    if (fk.IsParentKey)
                                    {
                                        newEntity.GenerateAsChildOf = fk.RelatedTableName;
                                    }
                                    break;
                                }
                            }
                        }

                        foreach (DBFieldKey fkfield in fk.Fields)
                        {
                            if (fkfield.Name.ToLower() == f.Name.ToLower())
                            {
                                isForeignKey = true;
                                break;
                            }
                        }

                    }
                }

                newEntity.ParentCandidates = parentCandidatesList.ToArray();
                if (string.IsNullOrEmpty(newEntity.GenerateAsChildOf))
                {
                    newEntity.GenerateAsChildOf = "None";
                }

                int nativeLen = 0;
                if (f.Length.HasValue)
                {
                    nativeLen = f.Length.Value;
                }
                int nativePrecision = 0;
                if (f.Precision.HasValue)
                {
                    nativePrecision = f.Precision.Value;
                }
                int nativeScale = 0;
                if (f.Scale.HasValue)
                {
                    nativeScale = f.Scale.Value;
                }



                PropertyNode newProperty = new PropertyNode(
                    newEntity,
                    f.Name,
                    f.UniversalType.ToString(),
                    f.NativeType,
                    nativeLen,
                    nativePrecision,
                    nativeScale,
                    isPrimaryKey,
                    isForeignKey,
                    f.RowVersion,
                    f.Nullable,
                    f.AutoGenerated,
                    f.Identity,
                    f.RowGuid,
                    f.Computed,
                    false,
                    false,
                    false,
                    "",
                    false,
                    false);

                if (newEntity.GenerateAsReadOnly)
                    newProperty.GenerateAsReadOnly = true;

                newEntity.Add(newProperty);
            }

            // Ahora si esta tabla tiene padres, agregamos una propiedad con la descripcion de cada padre.
            if (dbTable.ForeignKeys != null)
            {
                foreach (DBForeignKey fk in dbTable.ForeignKeys)
                {
                    PropertyNode newProperty = new PropertyNode(
                        newEntity,
                        fk.RelatedTableNameWithoutSchema + "String",
                        "System.String",
                        "varchar",
                        100,
                        0,
                        0,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        true,
                        fk.RelatedTableName,
                        false,
                        false);

                    foreach (DBFieldFKey field in fk.Fields)
                    {
                        // en field tengo el nombre del campo clave, tengo que obtener que tipo es.
                        foreach (DBTable tabl in databaseSchema.Tables)
                        {
                            if (tabl.Name == fk.RelatedTableName)
                            {
                                foreach (DBField tablField in tabl.Fields)
                                {
                                    if (tablField.Name == field.RelatedFieldName)
                                    {
                                        int fieldLen = 0;
                                        if (tablField.Length.HasValue)
                                        {
                                            fieldLen = tablField.Length.Value;
                                        }
                                        int fieldPrec = 0;
                                        if (tablField.Precision.HasValue)
                                        {
                                            fieldPrec = tablField.Precision.Value;
                                        }
                                        int fieldScl = 0;
                                        if (tablField.Scale.HasValue)
                                        {
                                            fieldScl = tablField.Scale.Value;
                                        }

                                        RelatedField relField = new RelatedField(field.Name, field.RelatedFieldName, tablField.UniversalType.ToString(), tablField.NativeType, fieldLen, fieldPrec, fieldScl);
                                        newProperty.RelatedFields.Add(relField);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    int counter = 1;
                    while (true)
                    {
                        try
                        {
                            newEntity.Add(newProperty);

                            newProperty.GenerateProperty = fk.IsParentKey;
                            break;
                        }
                        catch (Cooperator.Framework.Utility.Exceptions.NodeAlreadyExistException)
                        {
                            // Este nodo ya existe, cambiamos el nombre e intentamos de nuevo
                            counter++;
                            PropertyNode newProperty2 = new PropertyNode(
                                newEntity,
                                fk.RelatedTableNameWithoutSchema + counter.ToString() + "String",
                                newProperty.CLRType,
                                newProperty.NativeType,
                                newProperty.NativeLength,
                                newProperty.NativePrecision,
                                newProperty.NativeScale,
                                newProperty.IsPrimaryKey,
                                newProperty.IsForeignKey,
                                newProperty.IsRowVersion,
                                newProperty.IsNullable,
                                newProperty.IsAutoGenerated,
                                newProperty.IsIdentity,
                                newProperty.IsRowGuid,
                                newProperty.IsComputed,
                                newProperty.IsCollection,
                                newProperty.IsEntity,
                                newProperty.IsDescriptionField,
                                newProperty.RelatedTableName,
                                newProperty.IsOneToOneRelation,
                                false);

                            newProperty2.RelatedFields.AddRange(newProperty.RelatedFields);
                            newProperty = newProperty2;

                        }
                    }

                }
            }

            // Agregamos una coleccion a cada tabla que tenga hijos.
            foreach (DBTable tabl in databaseSchema.Tables)
            {
                // Si no es la misma tabla. Busco coleccion de FK para ver si me referencia
                if (tabl.ForeignKeys != null)
                {
                    foreach (DBForeignKey fk in tabl.ForeignKeys)
                    {
                        if (fk.RelatedTableName == dbTable.Name)
                        {
                            string AggregationOrCollection = "Collection";
                            bool isEntity = false;
                            bool isCollection = true;
                            bool isOneToOne = false;
                            string clrType = "ObjectList";
                            if (fk.IsOneToOneKey)
                            {
                                // is a Aggregation
                                AggregationOrCollection = "Aggregation";
                                isEntity = true;
                                isCollection = false;
                                isOneToOne = true;
                                clrType = "Object";
                            }

                            // tiene que tener una coleccion
                            PropertyNode newProperty = new PropertyNode(
                                newEntity,
                                tabl.NameWithoutSchema + AggregationOrCollection,
                                tabl.Name + clrType,
                                "None",
                                0,
                                0,
                                0,
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                false,
                                isCollection,
                                isEntity,
                                false,
                                tabl.Name,
                                isOneToOne,
                                true);

                            //newProperty.GenerateAsLazyLoad = false;
                            foreach (DBFieldFKey field in fk.Fields)
                            {
                                // en field tengo el nombre del campo clave, tengo que obtener que tipo es.
                                foreach (DBTable tabl2 in databaseSchema.Tables)
                                {
                                    if (tabl2.Name == fk.RelatedTableName)
                                    {
                                        foreach (DBField tablField in tabl.Fields)
                                        {
                                            //if (tablField.Name == field.RelatedFieldName)
                                            if (tablField.Name == field.Name)
                                            {
                                                int fieldLen = 0;
                                                if (tablField.Length.HasValue)
                                                {
                                                    fieldLen = tablField.Length.Value;
                                                }
                                                int fieldPrec = 0;
                                                if (tablField.Precision.HasValue)
                                                {
                                                    fieldPrec = tablField.Precision.Value;
                                                }
                                                int fieldScl = 0;
                                                if (tablField.Scale.HasValue)
                                                {
                                                    fieldScl = tablField.Scale.Value;
                                                }

                                                RelatedField relField = new RelatedField(field.RelatedFieldName, field.Name, tablField.UniversalType.ToString(), tablField.NativeType, fieldLen, fieldPrec, fieldScl);
                                                newProperty.RelatedFields.Add(relField);
                                                //break;
                                            }
                                        }
                                    }
                                }

                            }
                            int counter = 1;
                            while (true)
                            {
                                try
                                {
                                    newEntity.Add(newProperty);
                                    // Si tiene una collection esta clase tambien debe tener su controller.
                                    // newEntity.GenerateEntity = true;

                                    newProperty.GenerateProperty = fk.IsParentKey;
                                    break;
                                }
                                catch (Cooperator.Framework.Utility.Exceptions.NodeAlreadyExistException)
                                {
                                    // Este nodo ya esta agregado, cambiamos el nombre e intentamos de nuevo
                                    counter++;
                                    PropertyNode newProperty2 = new PropertyNode(
                                        newEntity,
                                        tabl.NameWithoutSchema + counter.ToString() + AggregationOrCollection,
                                        newProperty.CLRType,
                                        newProperty.NativeType,
                                        newProperty.NativeLength,
                                        newProperty.NativePrecision,
                                        newProperty.NativeScale,
                                        newProperty.IsPrimaryKey,
                                        newProperty.IsForeignKey,
                                        newProperty.IsRowVersion,
                                        newProperty.IsNullable,
                                        newProperty.IsAutoGenerated,
                                        newProperty.IsIdentity,
                                        newProperty.IsRowGuid,
                                        newProperty.IsComputed,
                                        newProperty.IsCollection,
                                        newProperty.IsEntity,
                                        newProperty.IsDescriptionField,
                                        newProperty.RelatedTableName,
                                        newProperty.IsOneToOneRelation,
                                        true );

                                    newProperty2.GenerateAsLazyLoad = false;
                                    newProperty2.RelatedFields.AddRange(newProperty.RelatedFields);
                                    newProperty = newProperty2;

                                }
                            }
                        }
                    }
                }
            }

            // Ahora si esta tabla tiene Tablas refrenciadas, agregamos una propiedad al tipo refrencia.
            if (dbTable.ForeignKeys != null)
            {
                foreach (DBForeignKey fk in dbTable.ForeignKeys)
                {
                    PropertyNode newProperty = new PropertyNode(
                        newEntity,
                        fk.RelatedTableNameWithoutSchema + "Entity",
                        fk.RelatedTableNameWithoutSchema + "Object",
                        "None",
                        0,
                        0,
                        0,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        true,
                        false,
                        fk.RelatedTableName,
                        false,
                        true);

                    foreach (DBFieldFKey field in fk.Fields)
                    {
                        // en field tengo el nombre del campo clave, tengo que obtener que tipo es.
                        foreach (DBTable tabl in databaseSchema.Tables)
                        {
                            if (tabl.Name == fk.RelatedTableName)
                            {
                                foreach (DBField tablField in tabl.Fields)
                                {
                                    if (tablField.Name == field.RelatedFieldName)
                                    {
                                        int fieldLen = 0;
                                        if (tablField.Length.HasValue)
                                        {
                                            fieldLen = tablField.Length.Value;
                                        }
                                        int fieldPrec = 0;
                                        if (tablField.Precision.HasValue)
                                        {
                                            fieldPrec = tablField.Precision.Value;
                                        }
                                        int fieldScl = 0;
                                        if (tablField.Scale.HasValue)
                                        {
                                            fieldScl = tablField.Scale.Value;
                                        }

                                        RelatedField relField = new RelatedField(field.Name, field.RelatedFieldName, tablField.UniversalType.ToString(), tablField.NativeType, fieldLen, fieldPrec, fieldScl);

                                        newProperty.RelatedFields.Add(relField);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    //newProperty.GenerateAsLazyLoad = false;
                    newProperty.GenerateProperty = fk.IsReferenceKey;

                    int counter = 1;
                    while (true)
                    {
                        try
                        {
                            newEntity.Add(newProperty);
                            break;
                        }
                        catch (Cooperator.Framework.Utility.Exceptions.NodeAlreadyExistException)
                        {
                            // Este nodo ya esta agregado, cambiamos el nombre e intentamos de nuevo
                            counter++;
                            PropertyNode newProperty2 = new PropertyNode(
                                newEntity,
                                fk.RelatedTableNameWithoutSchema + counter.ToString() + "Entity",
                                newProperty.CLRType,
                                newProperty.NativeType,
                                newProperty.NativeLength,
                                newProperty.NativePrecision,
                                newProperty.NativeScale,
                                newProperty.IsPrimaryKey,
                                newProperty.IsForeignKey,
                                newProperty.IsRowVersion,
                                newProperty.IsNullable,
                                newProperty.IsAutoGenerated,
                                newProperty.IsIdentity,
                                newProperty.IsRowGuid,
                                newProperty.IsComputed,
                                newProperty.IsCollection,
                                newProperty.IsEntity,
                                newProperty.IsDescriptionField,
                                newProperty.RelatedTableName,
                                newProperty.IsOneToOneRelation,
                                newProperty.IsLazyLoadable);

                            newProperty2.GenerateAsLazyLoad = false;
                            newProperty2.GenerateProperty = fk.IsReferenceKey;
                            newProperty2.RelatedFields.AddRange(newProperty.RelatedFields);
                            newProperty = newProperty2;
                        }
                    }
                }
            }

            // Revisamos todas las propiedades y elegimos la primera que sea string como descripcion.
            bool found = false;
            foreach (PropertyNode prop in newEntity.Children)
            {
                if (prop.CLRType == "System.String" && string.IsNullOrEmpty(prop.RelatedTableName))
                {
                    prop.GenerateAsDescriptionField = true;
                    found = true;
                    break;
                }
            }
            // Si no hay ningun string marcamos la primer propiedad que encontremos
            if (!found)
            {
                foreach (PropertyNode prop in newEntity.Children)
                {
                    prop.GenerateAsDescriptionField = true;
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelTree"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static string GetDescriptionFieldNameForEntity(BaseTreeNode modelTree, string entityName)
        {
            foreach (EntityNode node in modelTree.Children)
                if (node.Name == entityName)
                    foreach (PropertyNode currentProperty in node.Children)
                        if (currentProperty.GenerateAsDescriptionField)
                            return currentProperty.GenerateAs;

            throw new System.ArgumentException("Entity '" + entityName + "' Not found", "entityName");
        }
        
        
        /// <summary>
        /// Returns GenerateAs string for a Entity.
        /// </summary>
        public static string GetGenerateAsForEntity(BaseTreeNode modelTree, string entityName)
        {
            foreach (EntityNode node in modelTree.Children)
                if (node.Name == entityName) return node.GenerateAs;
            throw new System.ArgumentException("Entity '" + entityName + "' Not found", "entityName");
        }


        /// <summary>
        /// Returns GenerateAs string for a Entity.
        /// </summary>
        public static string GetFullGenerateAsForEntity(BaseTreeNode modelTree, string entityName)
        {
            foreach (EntityNode node in modelTree.Children)
                if (node.Name == entityName) return node.FullGenerateAs;
            throw new System.ArgumentException("Entity '" + entityName + "' Not found", "entityName");
        }

        /// <summary>
        /// Returns GenerateAs string for a Property
        /// </summary>
        public static string GetGenerateAsForProperty(BaseTreeNode modelTree, string entityName, string propertyName)
        {
            foreach (EntityNode node in modelTree.Children)
            {
                if (node.Name == entityName)
                {
                    foreach (PropertyNode propNode in node.Children)
                        if (propNode.Name == propertyName) return propNode.GenerateAs;
                    // if not found.
                    throw new System.ArgumentException("Property '" + propertyName + "' Not found", "propertyName");
                } 
            }
            throw new System.ArgumentException("Entity '" + entityName + "' Not found", "entityName");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelTree"></param>
        /// <param name="entityName"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static PropertyNode FindPropertyInTree(BaseTreeNode modelTree, string entityName, string propertyName)
        {
            foreach (EntityNode currentEntity in modelTree.Children)
            {
                if (currentEntity.Name == entityName)
                {
                    foreach (PropertyNode currentProperty in currentEntity.Children)
                    {
                        if (currentProperty.Name == propertyName)
                        {
                            return currentProperty;
                        }
                    }
                }
            }
            return null;
        }


        private static Dictionary<string, string> _NamespaceCollectionDictionary = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<string, string>.KeyCollection GetNamespaceCollection(EntityNode rootNode)
        {
            if (_NamespaceCollectionDictionary.Count==0)
            {
                foreach (EntityNode currentEntity in rootNode.Children)
                {
                    if (!string.IsNullOrEmpty(currentEntity.Namespace) && (currentEntity.GenerateObject || currentEntity.GenerateEntity))
                    {
                        if (!_NamespaceCollectionDictionary.ContainsKey(currentEntity.Namespace))
                            _NamespaceCollectionDictionary.Add(currentEntity.Namespace, currentEntity.Namespace);
                    }
                }
            }
            return _NamespaceCollectionDictionary.Keys;

        }

        /// <summary>
        /// 
        /// </summary>
        public static void ResetNamespaceCollection()
        {
            _NamespaceCollectionDictionary.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelTree"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public static EntityNode FindEntityInTree(BaseTreeNode modelTree, string entityName)
        {
            foreach (EntityNode currentEntity in modelTree.Children)
            {
                if (currentEntity.Name == entityName)
                {
                    return currentEntity;
                }
            }
            return null;
        }


        /// <summary>
        /// Correct CLRType of Collections and Entities when GenerateAs changes
        /// </summary>
        public static void CorrectCLRType(BaseTreeNode modelTree)
        {
            foreach (EntityNode ent in modelTree.Children)
            {
                foreach (PropertyNode prop in ent.Children)
                {
                    if (prop.IsCollection)
                    {
                        switch (prop.GenerateAsType)
                        {
                            case CollectionTypeEnum.EntityList:
                                prop.SetCLRType(GetFullGenerateAsForEntity(modelTree, prop.RelatedTableName) + "List");
                                break;
                            case CollectionTypeEnum.ObjectList:
                                prop.SetCLRType(GetFullGenerateAsForEntity(modelTree, prop.RelatedTableName) + "ObjectList");
                                break;
                            default:
                                break;
                        }
                    }
                    if (prop.IsEntity)
                    {
                        switch (prop.GenerateAsType)
                        {
                            case CollectionTypeEnum.Entity:
                                prop.SetCLRType(GetFullGenerateAsForEntity(modelTree, prop.RelatedTableName) );
                                break;
                            case CollectionTypeEnum.Object:
                                prop.SetCLRType(GetFullGenerateAsForEntity(modelTree, prop.RelatedTableName) + "Object");
                                break;
                            default:
                                break;
                        }
                    }
                }                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelTree"></param>
        public static void CorrectSelectedCollections(BaseTreeNode modelTree)
        {        
            foreach (EntityNode currentEntity in modelTree.Children)
            {
                foreach (PropertyNode currentProperty in currentEntity.Children)
                {
                    if ((currentProperty.IsCollection || (currentProperty.IsEntity && currentProperty.IsOneToOneRelation )) && currentProperty.GenerateProperty)
                    {
                        foreach (EntityNode currentEntity2 in modelTree.Children)
                        {
                            if (currentEntity2.Name == currentProperty.RelatedTableName && currentEntity2.GenerateAsChildOf != currentEntity.Name)
                            {
                                currentProperty.GenerateProperty = false;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelTree"></param>
        public static void CorrectGenerationNamesForRelatedFields(BaseTreeNode modelTree)
        {
            foreach (EntityNode currentEntity in modelTree.Children)
            {
                foreach (PropertyNode currentProperty in currentEntity.Children)
                {
                    if (currentProperty.RelatedFields.Count >0)
                    {
                        foreach (RelatedField f in currentProperty.RelatedFields)
                        {
                            string GenerateAsForName = GetGenerateAsForProperty(modelTree, currentEntity.Name, f.Name);
                            string GenerateAsForRelatedName = GetGenerateAsForProperty(modelTree, currentProperty.RelatedTableName, f.RelatedName);
                            f.SetGenerateAsForName(GenerateAsForName);
                            f.SetGenerateAsForRelatedName(GenerateAsForRelatedName);                            
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// Clase base de la cual heredan los nodos del arbol. (Entidad o Atributo) Es abstacta.
    /// </summary>
    [Serializable]
    public abstract class BaseTreeNode
    {
        private string name;
      
        /// <summary>
        /// Constructor del nodo.
        /// </summary>
        /// <param name="name">Nombre de este nodo</param>
        public BaseTreeNode(string name)
        {
            this.name = name;
        }


        private bool _haveChanges;
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public bool HaveChanges
        {
            get { return _haveChanges; }
            set { _haveChanges = value; }
        }



        /// <summary>
        /// Nombre del nodo.
        /// </summary>
        [Description("Original name of entity or property")]
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Permite agregar un item(hijo) a este nodo. Puede ser una nueva entidad o un atributo.
        /// </summary>
        /// <param name="node">El nodo a agregar</param>
        public abstract void Add(BaseTreeNode node);

        /// <summary>
        /// Elimina un item(hijo) de este nodo.
        /// </summary>
        /// <param name="node">El nodo a eliminar</param>
        public abstract void Remove(BaseTreeNode node);



        /// <summary>
        /// La lista de hijos de este Nodo.
        /// </summary>
        [Browsable(false)]
        public abstract List<BaseTreeNode> Children { get;}


        /// <summary>
        /// Busca un nodo hijo por nombre
        /// </summary>
        /// <param name="name">Nombre del nodo a buscar</param>
        /// <returns>El nodo cuyo nombre coincida con el pasado como parametro</returns>
        public BaseTreeNode FindInChildrenByName(string name)
        {
            foreach (BaseTreeNode node in this.Children)
            {
                if (node.Name == name)
                {
                    return node;
                }
            }
            return null;
        }


    }


    /// <summary>
    /// Nodo que representa una Entidad.
    /// </summary>
    [Serializable]
    public class EntityNode : BaseTreeNode
    {
        private List<BaseTreeNode> children = new List<BaseTreeNode>();

        /// <summary>
        /// Contructor de la clase
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="name">Nombre de la entidad</param>
        public EntityNode(EntityNode parentNode, string name)
            : base(name)
        {
            _Parent = parentNode;

            // Obtenemos el namespace si la tabla contiene un .
            string nameSpace = "";
            int point = name.IndexOf('.');
            if (point != -1)
            {
                nameSpace = name.Substring(0, point);
                name = name.Substring(point + 1);
            }

            _GenerateAs = name;
            _Namespace = nameSpace;
        }

        EntityNode _Parent;
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public EntityNode Parent
        {
            get { return _Parent; }
        }

        /// <summary>
        /// Permite agregar un item(hijo) a este nodo. Puede ser una nueva entidad o un atributo.
        /// </summary>
        /// <param name="node">El nodo a agregar</param>
        public override void Add(BaseTreeNode node)
        {
            foreach (BaseTreeNode oldNode in children)
            {
                if (oldNode.Name.Trim().ToLowerInvariant() == node.Name.Trim().ToLowerInvariant())
                    throw new Exceptions.NodeAlreadyExistException("Ya existe un nodo con ese nombre");
            }
            children.Add(node);
        }

        /// <summary>
        /// Elimina un item(hijo) de este nodo.
        /// </summary>
        /// <param name="node">El nodo a eliminar</param>
        public override void Remove(BaseTreeNode node)
        {
            children.Remove(node);
        }

        /// <summary>
        /// La lista de hijos de este Nodo.
        /// </summary>
        public override List<BaseTreeNode> Children
        {
            get { return this.children; }
        }

        private string _GenerateAs = "";
        /// <summary>
        /// 
        /// </summary>
        [Description("Generation name of entity")]
        public string GenerateAs
        {
            get { return _GenerateAs; }
            set {

                if (value.IndexOf('.') == -1)
                {
                    if (_GenerateAs != value) base.HaveChanges = true;
                    _GenerateAs = value;
                    DomainTreeHelper.CorrectCLRType(this.Parent);
                }
            }
        }

        private bool _GenerateObjectList = true;
        /// <summary>
        /// 
        /// </summary>
        [Description("Generate ObjectList class and stored procedures for this item"), DefaultValue(true)]
        public bool GenerateObject
        {
            get { return _GenerateObjectList; }
            set
            {
                if (_GenerateObjectList != value) base.HaveChanges = true;
                _GenerateObjectList = value;
            }
        }


        private bool _GenerateEntity = false;
        /// <summary>
        /// 
        /// </summary>
        [Description("Generate entity class for this item"), DefaultValue(true)]
        public bool GenerateEntity
        {
            get { return _GenerateEntity; }
            set {
                if (_GenerateEntity != value) base.HaveChanges = true;
                _GenerateEntity = value;
            }
        }



        private bool _GenerateAsCacheable = false;
        /// <summary>
        /// 
        /// </summary>
        [Description("Hold all records of this table in cache"), DefaultValue(false)]
        public bool GenerateAsCacheable
        {
            get { return _GenerateAsCacheable; }
            set
            {
                if (_GenerateAsCacheable != value) base.HaveChanges = true;
                _GenerateAsCacheable = value;
            }
        }



        private bool _GenerateSpDescriptionFunction = true;
        /// <summary>
        /// 
        /// </summary>
        [Description("Generate description function in data base"), DefaultValue(true)]
        public bool GenerateSpDescriptionFunction
        {
            get { return _GenerateSpDescriptionFunction; }
            set
            {
                if (_GenerateSpDescriptionFunction != value) base.HaveChanges = true;
                _GenerateSpDescriptionFunction = value;
            }
        }


        private bool _GenerateSpDelete = true;
        /// <summary>
        /// 
        /// </summary>
        [Description("Generate delete stored procedure in data base"), DefaultValue(true)]
        public bool GenerateSpDelete
        {
            get { return _GenerateSpDelete; }
            set
            {
                if (_GenerateSpDelete != value) base.HaveChanges = true;
                _GenerateSpDelete = value && this.PrimaryKeyFields.Count != 0; ;
            }
        }


        private bool _GenerateSpDeleteByParents = true;
        /// <summary>
        /// 
        /// </summary>
        [Description("Generate delete by parents stored procedure in data base"), DefaultValue(true)]
        public bool GenerateSpDeleteByParents
        {
            get { return _GenerateSpDeleteByParents; }
            set
            {
                if (_GenerateSpDeleteByParents != value) base.HaveChanges = true;
                _GenerateSpDeleteByParents = value && this.PrimaryKeyFields.Count != 0; ;
            }
        }


        private bool _GenerateSpGetAll = true;
        /// <summary>
        /// 
        /// </summary>
        [Description("Generate GetAll stored procedure in data base"), DefaultValue(true)]
        public bool GenerateSpGetAll
        {
            get { return _GenerateSpGetAll; }
            set
            {
                if (_GenerateSpGetAll != value) base.HaveChanges = true;
                _GenerateSpGetAll = value;
            }
        }


        private bool _GenerateSpGetByParent = true;
        /// <summary>
        /// 
        /// </summary>
        [Description("Generate GetByParent stored procedure in data base"), DefaultValue(true)]
        public bool GenerateSpGetByParent
        {
            get { return _GenerateSpGetByParent; }
            set
            {
                if (_GenerateSpGetByParent != value) base.HaveChanges = true;
                _GenerateSpGetByParent = value;
            }
        }


        private bool _GenerateSpGetByParents = true;
        /// <summary>
        /// 
        /// </summary>
        [Description("Generate GetByParents stored procedure in data base"), DefaultValue(true)]
        public bool GenerateSpGetByParents
        {
            get { return _GenerateSpGetByParents; }
            set
            {
                if (_GenerateSpGetByParents != value) base.HaveChanges = true;
                _GenerateSpGetByParents = value ; 
            }
        }



        private bool _GenerateSpGetOne = true;
        /// <summary>
        /// 
        /// </summary>
        [Description("Generate GetOne stored procedure in data base"), DefaultValue(true)]
        public bool GenerateSpGetOne
        {
            get { return _GenerateSpGetOne; }
            set
            {
                if (_GenerateSpGetOne != value) base.HaveChanges = true;
                _GenerateSpGetOne = value && this.PrimaryKeyFields.Count != 0;
            }
        }


        private bool _GenerateSpInsert = true;
        /// <summary>
        /// 
        /// </summary>
        [Description("Generate Insert stored procedure in data base"), DefaultValue(true)]
        public bool GenerateSpInsert
        {
            get { return _GenerateSpInsert; }
            set
            {
                if (_GenerateSpInsert != value) base.HaveChanges = true;
                _GenerateSpInsert = value && this.PrimaryKeyFields.Count != 0; 
            }
        }

        private bool _GenerateSpUpdate = true;
        /// <summary>
        /// 
        /// </summary>
        [Description("Generate Update stored procedure in data base"), DefaultValue(true)]
        public bool GenerateSpUpdate
        {
            get { return _GenerateSpUpdate; }
            set
            {
                if (_GenerateSpUpdate != value) base.HaveChanges = true;
                _GenerateSpUpdate = value && this.PrimaryKeyFields.Count != 0; 
            }
        }





        private List<string> _PrimaryKeyFields = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        [Description("Collection of fields part of primary key")]
        public List<string> PrimaryKeyFields
        {
            get { return _PrimaryKeyFields; }
            set
            {
                if (_PrimaryKeyFields != value) base.HaveChanges = true;
                _PrimaryKeyFields = value;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Description("Number of native property selected to generate plus Primary and Foreign fields")]
        public int SelectedNativePropertiesCount
        {
            get
            {
                int counter = 0;
                foreach (PropertyNode currentProperty in this.Children)
                {
                    //if ((currentProperty.GenerateProperty || currentProperty.IsPrimaryKey || currentProperty.IsForeignKey) && currentProperty.NativeType != "None" && !currentProperty.IsDescriptionField ) counter++;
                    if (currentProperty.IsUsedByInsertAndUpdate() && currentProperty.NativeType != "None" && !currentProperty.IsDescriptionField) counter++;
                }
                return counter;
            }
        }

        private bool _GenerateAsReadOnly = false;
        /// <summary>
        /// 
        /// </summary>
        [Description("Define this table as Read Only. All the records will be cahed on memory"),DefaultValue(false)]
        public bool GenerateAsReadOnly
        {
            get { return _GenerateAsReadOnly; }
            set
            {
                if (_GenerateAsReadOnly != value) base.HaveChanges = true;
                _GenerateAsReadOnly = value ;
            }
        }

        private bool _GenerateAsVersionable = false;
        /// <summary>
        /// 
        /// </summary>
        [Description("If generate row version controls for concurrence."), DefaultValue(false)]
        public bool GenerateAsVersionable
        {
            get { return _GenerateAsVersionable; }
            set
            {
                if (_GenerateAsVersionable != value) base.HaveChanges = true;
                if (value)
                {
                    bool hasRowversionColumn = false;
                    foreach (PropertyNode currentProperty in this.Children)
                        if (currentProperty.IsRowVersion) hasRowversionColumn = true;
                    if (!hasRowversionColumn) return;
                }                            
                _GenerateAsVersionable = value;
            }
        }



        private string _Namespace = "";
        /// <summary>
        /// Namespace for this entity
        /// </summary>
        [Description("Namespace for this entity."), DefaultValue("")]
        public string Namespace
        {
            get
            {
                return _Namespace;
            }
            set
            {
                if (value == null)
                {
                    _Namespace = "";
                }
                else
                {
                    if (value.IndexOf('.') == -1)
                    {
                        if (_Namespace != value) base.HaveChanges = true;
                        _Namespace = value;
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public string FormattedNamespace
        {
            get
            {
                if (string.IsNullOrEmpty(_Namespace))
                    return "";
                else
                    return "." + _Namespace;
            }            
        }


        /// <summary>
        /// 
        /// </summary>
        [Browsable (false )]
        public string NameWithoutSchema
        {
            get
            {
                if (base.Name.IndexOf('.') == -1)
                    return base.Name;
                else
                    return base.Name.Substring(base.Name.IndexOf('.') + 1);
            }
        }


        /// <summary>
        /// Generation name of entity with namespace
        /// </summary>
        [Description("Generation name of entity with namespace")]
        public string FullGenerateAs
        {
            get
            {
                if (string.IsNullOrEmpty(_Namespace))
                    return _GenerateAs;
                else
                    return _Namespace + '.' + _GenerateAs;
            }
        }



        private string _OrderBy = "";
        /// <summary>
        /// ORDER BY used by all stored procedures
        /// </summary>
        [Description("ORDER BY used by all stored procedures."), DefaultValue("")]
        public string OrderBy
        {
            get { return _OrderBy; }
            set
            {
                if (_OrderBy != value) base.HaveChanges = true;
                _OrderBy = value;
            }
        }

        private string[] _ParentCandidates = { "None" };

        /// <summary>
        /// 
        /// </summary>
        [Browsable (false)]
        public string[] ParentCandidates
        {
            get { return _ParentCandidates; }
            set
            {
                if (_ParentCandidates != value) base.HaveChanges = true;
                _ParentCandidates = value;
            }
        }


        private string _GenerateAsChildOf;

        /// <summary>
        /// 
        /// </summary>
        [Description("Defines the parent table of this table."), DefaultValue("None")]
        [EditorAttribute(typeof(Cooperator.Framework.Utility.DBReverseHelper.PropertyEditors.DropDownListEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string GenerateAsChildOf
        {
            get
            {
                Cooperator.Framework.Utility.DBReverseHelper.PropertyEditors.DropDownListEditor.stringList = _ParentCandidates;
                return _GenerateAsChildOf;
            }
            set
            {
                foreach (string candidate in _ParentCandidates)
                {
                    if (value == candidate)
                    {
                        if (_GenerateAsChildOf != value)
                        {
                            base.HaveChanges = true;
                            _GenerateAsChildOf = value;
                            // Refresh Collections
                            DomainTreeHelper.CorrectSelectedCollections(this.Parent);
                        }
                    }
                }
            }
        }

        private List<DatabaseQuery> _Queries = new List<DatabaseQuery>();
        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Description("Defines queries for this table.")]
        public List<DatabaseQuery> Queries
        {
            get
            {
                foreach (DatabaseQuery item in _Queries)
                {
                    item.ItemChanged -= new EventHandler<EventArgs>(DatabaseQueryItemChanged);
                    item.ItemChanged += new EventHandler<EventArgs>(DatabaseQueryItemChanged);
                }                
                return _Queries;
            }
        }


        private void DatabaseQueryItemChanged(object sender, EventArgs e)
        {
            this.HaveChanges = true;
        }

        private ToStringStruct toStringInfo=new ToStringStruct();
        /// <summary>
        /// 
        /// </summary>
        [Description("Defines ToStringInfo for override ToString() in class.")]
        [EditorAttribute(typeof(Cooperator.Framework.Utility.DBReverseHelper.PropertyEditors.ToStringInfoEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public ToStringStruct ToStringInfo
        {
            get {
                if (toStringInfo == null) toStringInfo = new ToStringStruct();
                toStringInfo.Properties = new List<string>();
                toStringInfo.Properties.Clear();
                foreach (PropertyNode btn in this.children)
                    if(btn.GenerateProperty && btn.GenerateAsType==CollectionTypeEnum.NotApplicable)
                        toStringInfo.Properties.Add(btn.Name);
                return toStringInfo; 
            }
            set { toStringInfo = value; }
        }
	

    }


    



    /// <summary>
    /// Nodo que representa una Propiedad de una entidad
    /// </summary>
    [Serializable]
    public class PropertyNode : BaseTreeNode
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="name"></param>
        /// <param name="clrType"></param>
        /// <param name="nativeType"></param>
        /// <param name="nativeLength"></param>
        /// <param name="nativePrecision"></param>
        /// <param name="nativeScale"></param>
        /// <param name="isPrimaryKey"></param>
        /// <param name="isForeignKey"></param>
        /// <param name="isRowVersion"></param>
        /// <param name="isNullable"></param>
        /// <param name="isAutoGenerated"></param>
        /// <param name="isIdentity"></param>
        /// <param name="isRowGuid"></param>
        /// <param name="isComputed"></param>
        /// <param name="isCollection"></param>
        /// <param name="isEntity"></param>
        /// <param name="isDescriptionField"></param>
        /// <param name="relatedTableName"></param>
        /// <param name="isOneToOneRelation"></param>
        /// <param name="isLazyLoadable"></param>
        public PropertyNode(
            EntityNode parentNode, 
            string name, 
            string clrType, 
            string nativeType, 
            int nativeLength,
            int nativePrecision,
            int nativeScale, 
            bool isPrimaryKey, 
            bool isForeignKey, 
            bool isRowVersion, 
            bool isNullable, 
            bool isAutoGenerated, 
            bool isIdentity, 
            bool isRowGuid, 
            bool isComputed, 
            bool isCollection, 
            bool isEntity, 
            bool isDescriptionField, 
            string relatedTableName,
            bool isOneToOneRelation,
            bool isLazyLoadable)
            : base(name)
        {
            _parent = parentNode;
            _GenerateAs = name;
            _CLRType = clrType;
            _NativeType = nativeType;
            _NativeLength = nativeLength;
            _NativePrecision = nativePrecision;
            _NativeScale = nativeScale;
            _IsPrimaryKey = isPrimaryKey;
            _IsAutoGenerated = isAutoGenerated;
            _IsRowVersion = isRowVersion;
            _RelatedTableName = relatedTableName;
            _IsNullable = isNullable;
            _IsIdentity = isIdentity;
            _IsRowGuid = isRowGuid;
            _IsComputed = isComputed;
            if (isCollection) _GenerateAsType = CollectionTypeEnum.ObjectList;
            if (isEntity) _GenerateAsType = CollectionTypeEnum.Object;
            _IsCollection = isCollection;
            _IsEntity = isEntity;
            _IsDescriptionField = isDescriptionField;
            if (isDescriptionField || isEntity || isCollection || isAutoGenerated) _GenerateAsReadOnly = true;
            _IsForeignKey = isForeignKey;
            _IsOneToOneRelation = isOneToOneRelation;
            _IsLazyLoadable = isLazyLoadable;
            _GenerateAsLazyLoad = isCollection || IsEntity;
            if (!_IsLazyLoadable) _GenerateAsLazyLoad = false;
        }

        /// <summary>
        /// Permite agregar un item(hijo) a este nodo. Puede ser solamente un atributo.
        /// </summary>
        /// <param name="node">El nodo a agregar</param>
        public override void Add(BaseTreeNode node)
        {
            throw new Exceptions.CannotAddOrRemoveItemException("No puede agregar un item a un nodo del tipo PropertyNode");
        }

        /// <summary>
        /// Elimina un item(hijo) de este nodo.
        /// </summary>
        /// <param name="node">El nodo a eliminar</param>
        public override void Remove(BaseTreeNode node)
        {
            throw new Exceptions.CannotAddOrRemoveItemException("No puede eliminar un item de un nodo del tipo PropertyNode");
        }


        private EntityNode _parent;
        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public EntityNode Parent
        {
            get { return _parent; }
        }


        internal void SetCLRType(string clrType)
        {
            _CLRType = clrType;
        }



        /// <summary>
        /// La lista de hijos de este Nodo.
        /// </summary>
        public override List<BaseTreeNode> Children
        {
            // Una lista vacia.
            get { return new List<BaseTreeNode>(); }
        }

        private string _GenerateAs = "";
        /// <summary>
        /// 
        /// </summary>
        [Description("Generation name of property")]
        public string GenerateAs
        {
            get { return _GenerateAs; }
            set
            {
                if (_IsEntity && value.ToLowerInvariant().Trim() == "entity") return;

                if (_GenerateAs != value)
                {
                    base.HaveChanges = true;
                    DomainTreeHelper.CorrectCLRType(this.Parent.Parent);
                }
                _GenerateAs = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [Description("Generation fullname of property")]
        public string FullGenerateAs
        {
            get
            {
                string ns = this.Parent.Namespace;
                if (string.IsNullOrWhiteSpace(ns))
                    return this.GenerateAs;
                else
                    return ns + "." + this.GenerateAs;
            }
        }



        private bool _GenerateProperty = true;
        /// <summary>
        /// 
        /// </summary>
        [Description("Include this property in generation"), DefaultValue(true)]
        public bool GenerateProperty
        {
            get { return _GenerateProperty; }
            set
            {
                if (_GenerateProperty != value)
                    base.HaveChanges = true;

                // if we have a association, this is a entity
                if (value && this.Parent.GenerateObject && (this.IsEntity && !this._IsOneToOneRelation))
                    this.Parent.GenerateEntity = true;


                if (this._IsCollection || (this._IsEntity && this._IsOneToOneRelation ))
                {
                    if (value)
                    {
                        foreach (EntityNode currentEntity in this.Parent.Parent.Children)
                        {
                            if (currentEntity.Name == this.RelatedTableName && currentEntity.GenerateAsChildOf == this.Parent.Name)
                            {
                                _GenerateProperty = true;
                                if (this.Parent.GenerateObject) this.Parent.GenerateEntity = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        _GenerateProperty = false;
                    }
                }
                else
                {
                    _GenerateProperty = value;
                }
            }
        }

        private bool _IsAutoGenerated = false;
        /// <summary>
        /// 
        /// </summary>
        [Description("Field is Read Only"), DefaultValue(false)]
        public bool IsAutoGenerated
        {
            get { return _IsAutoGenerated; }
        }


        private bool _GenerateAsReadOnly = false;
        /// <summary>
        /// 
        /// </summary>
        [Description("Property will be Read Only"), DefaultValue(false)]
        public bool GenerateAsReadOnly
        {
            get { return _GenerateAsReadOnly; }
            set
            {
                if (_GenerateAsReadOnly != value) base.HaveChanges = true;
                _GenerateAsReadOnly = _IsDescriptionField || _IsEntity || _IsCollection || _IsAutoGenerated || value;
            }
        }


        private bool _IsNullable = false;
        /// <summary>
        /// 
        /// </summary>
        [Description("Property will be Nullable"), DefaultValue(false)]
        public bool IsNullable
        {
            get { return _IsNullable; }
        }


        private string _CLRType = "";
        /// <summary>
        /// 
        /// </summary>
        [Description("CLR Type of this property")]
        public string CLRType
        {
            get { return _CLRType; }
        }




        private string _NativeType = "";
        /// <summary>
        /// 
        /// </summary>
        [Description("Native yype of this property")]
        public string NativeType
        {
            get { return _NativeType; }
        }


        private int _NativeLength = 0;
        /// <summary>
        /// 
        /// </summary>
        [Description("Native length of this property")]
        [Browsable(true)]
        public int NativeLength
        {
            get { return _NativeLength; }
        }

        private int _NativePrecision = 0;
        /// <summary>
        /// 
        /// </summary>
        [Description("Native precision of this property")]
        [Browsable(true)]
        public int NativePrecision
        {
            get { return _NativePrecision; }
        }

        private int _NativeScale = 0;
        /// <summary>
        /// 
        /// </summary>
        [Description("Native scale of this property")]
        [Browsable(true)]
        public int NativeScale
        {
            get { return _NativeScale; }
        }


        private bool _GenerateAsLazyLoad = false;
        /// <summary>
        /// 
        /// </summary>
        [Description("Entity is loaded in LazyLoad fashion"), DefaultValue(false)]
        public bool GenerateAsLazyLoad
        {
            get { return _GenerateAsLazyLoad; }
            set
            {
                if (_GenerateAsLazyLoad != value) base.HaveChanges = true;
                if (value)
                {
                    if (_IsLazyLoadable) _GenerateAsLazyLoad = true;
                }
                else
                {
                    _GenerateAsLazyLoad = false;
                }
            }
        }


        private List<RelatedField> _RelatedFields = new List<RelatedField>();
        /// <summary>
        /// 
        /// </summary>
        [Description("Collection of fields needed to get the collection")]
        public List<RelatedField> RelatedFields
        {
            get { return _RelatedFields; }
        }


        private bool _GenerateAsProtected = false;
        /// <summary>
        /// 
        /// </summary>
        [Description("If this property will be generated as protected access level."), DefaultValue(false)]
        public bool GenerateAsProtected
        {
            get
            {
                return _GenerateAsProtected;
            }
            set
            {
                if (_NativeType == "None") value = false;
                if (_GenerateAsProtected != value) base.HaveChanges = true;
                _GenerateAsProtected = value;
            }
        }

        private bool _IsPrimaryKey = false;
        /// <summary>
        /// 
        /// </summary>
        [Description("If the field is part of Primary Key"), DefaultValue(false)]
        public bool IsPrimaryKey
        {
            get { return _IsPrimaryKey; }
        }


        private bool _IsForeignKey = false;
        /// <summary>
        /// 
        /// </summary>
        [Description("If the field is part of Foreign Key"), DefaultValue(false)]
        public bool IsForeignKey
        {
            get { return _IsForeignKey; }
        }


        private bool _GenerateAsDescriptionField = false;
        /// <summary>
        /// 
        /// </summary>
        [Description("If this field is the representative column of table"), DefaultValue(false)]
        public bool GenerateAsDescriptionField
        {
            get { return _GenerateAsDescriptionField; }
            set
            {
                // Cant Turn Off
                if (!value) return;

                if (_GenerateAsDescriptionField != value) base.HaveChanges = true;

                // If Turn On a related field...
                if (value && _RelatedTableName != "")
                {
                    _GenerateAsDescriptionField = false;
                    return;
                }
                _GenerateAsDescriptionField = value;

                if (_GenerateAsDescriptionField)
                {
                    // Turn off all GenerateAsDescriptionField in this property
                    foreach (PropertyNode prop in this.Parent.Children)
                        if (prop.Name != this.Name) prop.SetGenerateAsDescriptionFieldValue(false);
                }
            }
        }

        private void SetGenerateAsDescriptionFieldValue(bool value) {
            if (_GenerateAsDescriptionField != value) base.HaveChanges = true;
            _GenerateAsDescriptionField = value;
        }


        private bool _IsRowVersion = false;
        /// <summary>
        /// 
        /// </summary>
        [Description("This field is RowVersion"), DefaultValue(false)]
        public bool IsRowVersion
        {
            get { return _IsRowVersion; }
        }

        private bool _IsRowGuid;
        /// <summary>
        /// 
        /// </summary>
        [Description("This field is RowGuid"), DefaultValue(false)]
        public bool IsRowGuid
        {
            get { return _IsRowGuid; }
        }

        private bool _IsIdentity;
        /// <summary>
        /// 
        /// </summary>
        [Description("This field is Identity"), DefaultValue(false)]
        public bool IsIdentity
        {
            get { return _IsIdentity; }
        }

        private bool _IsComputed;
        /// <summary>
        /// 
        /// </summary>
        [Description("This field is Computed"), DefaultValue(false)]
        public bool IsComputed
        {
            get { return _IsComputed; }
        }

        private string _RelatedTableName = "";
        /// <summary>
        /// 
        /// </summary>
        [Description("Related table name of this item")]
        public string RelatedTableName
        {
            get { return _RelatedTableName; }
        }


        private CollectionTypeEnum _GenerateAsType = CollectionTypeEnum.NotApplicable;
        /// <summary>
        /// 
        /// </summary>
        [Description("Type of collection to generate")]
        [DefaultValue(CollectionTypeEnum.NotApplicable)]
        public CollectionTypeEnum GenerateAsType
        {
            get { return _GenerateAsType; }
            set {
                if (_GenerateAsType == value) return;

                base.HaveChanges = true;

                if (_IsCollection)
                {
                    if (value == CollectionTypeEnum.EntityList || value==CollectionTypeEnum.ObjectList)
                    {
                        _GenerateAsType = value;

                        _CLRType = DomainTreeHelper.GetFullGenerateAsForEntity(this.Parent.Parent, this.RelatedTableName) + value.ToString().Replace("EntityList", "List");

                        if (value == CollectionTypeEnum.EntityList)
                        {
                            EntityNode relatedNode = DomainTreeHelper.FindEntityInTree(this.Parent.Parent, this.RelatedTableName);
                            relatedNode.GenerateObject = true;
                            relatedNode.GenerateEntity = true;
                        }
                    }
                }

                if (_IsEntity)
                {
                    if (_IsOneToOneRelation)
                    {
                        if (value == CollectionTypeEnum.Entity || value == CollectionTypeEnum.Object)
                        {
                            _GenerateAsType = value;
                            string lastPart = (value == CollectionTypeEnum.Object) ? "Object" : "";
                            _CLRType = DomainTreeHelper.GetFullGenerateAsForEntity(this.Parent.Parent, this.RelatedTableName ) + lastPart;
                        }
                    }
                    else
                    {
                        if (value == CollectionTypeEnum.Entity || value == CollectionTypeEnum.Object)
                        {
                            _GenerateAsType = value;
                            string lastPart = (value == CollectionTypeEnum.Object) ? "Object" : "";
                            _CLRType = DomainTreeHelper.GetFullGenerateAsForEntity(this.Parent.Parent, this.RelatedTableName ) + lastPart;
                        }

                    }

                    if (value == CollectionTypeEnum.Entity)
                    {
                        EntityNode relatedNode = DomainTreeHelper.FindEntityInTree(this.Parent.Parent, this.RelatedTableName);
                        relatedNode.GenerateObject = true;
                        relatedNode.GenerateEntity = true;
                    }
                }
            }
        }

        private bool _IsCollection;
        /// <summary>
        /// 
        /// </summary>
        [Description("This property is collection of another type"), DefaultValue(false)]
        public bool IsCollection
        {
            get { return _IsCollection; }
        }


        private bool _IsEntity;
        /// <summary>
        /// 
        /// </summary>
        [Description("This property is a parent type"), DefaultValue(false)]
        public bool IsEntity
        {
            get { return _IsEntity; }
        }

        private bool _IsDescriptionField;
        /// <summary>
        /// 
        /// </summary>
        [Description("This field show the description field of related tabla as string "), DefaultValue(false)]
        public bool IsDescriptionField
        {
            get { return _IsDescriptionField; }
        }



        private bool _IsOneToOneRelation;
        /// <summary>
        /// 
        /// </summary>
        [Description("If this entity have a 'One to One' relationship with the main entity"), DefaultValue(false)]
        public bool IsOneToOneRelation
        {
            get { return _IsOneToOneRelation; }
        }


        private bool _IsLazyLoadable;
        /// <summary>
        /// 
        /// </summary>
        [Description("If this entity can be load in Lazy Load fashion"), DefaultValue(true)]
        public bool IsLazyLoadable
        {
            get { return _IsLazyLoadable; }
        }

        /// <summary>
        /// If this entity can used for Insert and Update (stored procedures and IMappeableTramoObject.GetFieldsForInsert() and IMappeableTramoObject.GetFieldsForUpdate()
        /// </summary>
        public bool IsUsedByInsertAndUpdate()
        {
            return ((this.GenerateProperty || this.IsPrimaryKey || this.IsForeignKey) && !(this.GenerateAsReadOnly && !this.IsAutoGenerated && !this.IsPrimaryKey && !this.IsForeignKey));
        }

    }









    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RelatedField
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="relatedName"></param>
        /// <param name="type"></param>
        /// <param name="nativeType"></param>
        /// <param name="nativeLength"></param>
        /// <param name="nativePrecision"></param>
        /// <param name="nativeScale"></param>
        public RelatedField(string name, string relatedName, string type, string nativeType, int nativeLength, int nativePrecision, int nativeScale)
        {
            _Name = name;
            _GenerateAsForName = name;
            _RelatedName = relatedName;
            _GenerateAsForRelatedName = relatedName;
            _Type = type;
            _NativeType = nativeType;
            _NativeLength = nativeLength;
            _NativePrecision = nativePrecision;
            _NativeScale = nativeScale;
        }

        private string _GenerateAsForName = "";
        /// <summary>
        /// 
        /// </summary>
        public string GenerateAsForName
        {
            get { return _GenerateAsForName; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetGenerateAsForName(string value)
        {
            _GenerateAsForName = value;
        }

        private string _GenerateAsForRelatedName = "";
        /// <summary>
        /// 
        /// </summary>
        public string GenerateAsForRelatedName
        {
            get { return _GenerateAsForRelatedName; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void SetGenerateAsForRelatedName(string value)
        {
            _GenerateAsForRelatedName = value;
        }

        private string _Name = "";
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return _Name; }
        }

        private string _RelatedName = "";
        /// <summary>
        /// 
        /// </summary>
        public string RelatedName
        {
            get { return _RelatedName; }
        }

        private string _Type = "";
        /// <summary>
        /// 
        /// </summary>
        public string Type
        {
            get { return _Type; }
        }

        private string _NativeType = "";
        /// <summary>
        /// 
        /// </summary>
        public string NativeType
        {
            get { return _NativeType; }
        }

        private int _NativeLength = 0;
        /// <summary>
        /// 
        /// </summary>
        public int NativeLength
        {
            get { return _NativeLength; }
        }

        private int _NativePrecision = 0;
        /// <summary>
        /// 
        /// </summary>
        public int NativePrecision
        {
            get { return _NativePrecision; }
        }

        private int _NativeScale = 0;
        /// <summary>
        /// 
        /// </summary>
        public int NativeScale
        {
            get { return _NativeScale; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _Name + " : " + _RelatedName;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ToStringStruct
    {
        
        
        /// <summary>
        /// 
        /// </summary>
        public ToStringStruct() { }


        private bool overrideToString=false;
        /// <summary>
        /// 
        /// </summary>
        public bool OverrideToString
        {
            get { return overrideToString; }
            set { overrideToString = value; }
        }
	

        private string stringFotmat;
        /// <summary>
        /// 
        /// </summary>
        public string StringFotmat
        {
            get { return stringFotmat; }
            set { stringFotmat = value; }
        }

        private List<String> toStringParams=new List<string>();
        /// <summary>
        /// 
        /// </summary>
        public List<String> ToStringParams
        {
            get { return toStringParams; }
            set { toStringParams = value; }
        }



        private List<String> properties;
        /// <summary>
        /// 
        /// </summary>
        public List<String> Properties
        {
            get { return properties; }
            set { properties = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.overrideToString)
            {
                string s="";
                string sep="";
                foreach(string sx in this.toStringParams)
                {
                    s+=sep+sx;
                    sep=",";
                }
                
                return string.Format("String.Format(\"{0}\", {1})", this.StringFotmat,s );
            }
            else
                return "Not override ToString()";
        }

    }

}

/// <summary>
/// 
/// </summary>
public enum DatabaseQueryReturnType
{
    /// <summary>
    /// Return many records
    /// </summary>
    ManyRecords,
    /// <summary>
    /// Return one record
    /// </summary>
    OneRecord,
    /// <summary>
    /// Return scalar type value
    /// </summary>
    Scalar,
    /// <summary>
    /// Return DataReader
    /// </summary>
    DataReader,
    /// <summary>
    /// Return DataTable
    /// </summary>
    DataTable,
    /// <summary>
    /// Return number of affected rows
    /// </summary>
    NoQuery
}


/// <summary>
/// 
/// </summary>
[Serializable]
public class DatabaseQuery
{
    private string _QueryName;
    private string _Parameters;
    private string _QueryText;
    private bool _GenerateQuery = true;

    /// <summary>
    /// 
    /// </summary>
    public DatabaseQuery()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public event EventHandler<System.EventArgs> ItemChanged;

    /// <summary>
    /// 
    /// </summary>
    [DefaultValue(true)]
    public bool GenerateQuery
    {
        get { return _GenerateQuery; }
        set
        {
            System.EventArgs e = new System.EventArgs();
            if (ItemChanged != null) ItemChanged(this, e);

            _GenerateQuery = value;

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public string QueryName
    {
        get { return _QueryName; }
        set {
            System.EventArgs e = new System.EventArgs();
            if (ItemChanged != null) ItemChanged(this, e);

            _QueryName = value; 
        
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Description("")]
    public string Parameters
    {
        get { return _Parameters; }
        set {
            System.EventArgs e = new System.EventArgs();
            if (ItemChanged != null) ItemChanged(this, e);

            _Parameters = value; 
        
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [EditorAttribute(typeof(Cooperator.Framework.Utility.DBReverseHelper.PropertyEditors.StringEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public string QueryText
    {
        get { return _QueryText; }
        set {
            System.EventArgs e = new System.EventArgs();
            if (ItemChanged != null) ItemChanged(this, e);

            _QueryText = value; 
        }
    }

    private DatabaseQueryReturnType _ReturnType = DatabaseQueryReturnType.ManyRecords;
    /// <summary>
    /// 
    /// </summary>
    public DatabaseQueryReturnType ReturnType
    {
        get { return _ReturnType; }
        set { _ReturnType = value; }
    }

    private string _ScalarCLRType = "System.Int32";
    /// <summary>
    /// 
    /// </summary>
    public string ScalarCLRType
    {
        get { return _ScalarCLRType; }
        set { _ScalarCLRType = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return _QueryName;
    }
}



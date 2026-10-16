#nullable disable // SPEC-261016-0027 6/D.3-a: archivo APORTADO del 1.4 al tooling con diagnosticos de nulabilidad del build - unica mecanica de build autorizada sobre el aporte.
using System;
using System.Collections.Generic;
using Cooperator.Framework.Modeling.Model;
using Cooperator.Framework.Modeling.Reverse;
using Cooperator.Framework.Modeler.DBReverseHelper.Providers;

namespace Cooperator.Framework.Modeler.DBReverseHelper
{
    /// <summary>
    /// La MITAD REVERSE de <c>DomainTreeHelper</c> (SPEC-261016-0027 6/D.3-b): construye el arbol desde el esquema
    /// de la base. La mitad de arbol vive en <c>Cooperator.Framework.Modeling.Model</c>, con el MISMO nombre de
    /// tipo, y por eso las cuatro llamadas cruzadas van CALIFICADAS (6/D.3-b, recordatorio 1).
    /// </summary>
    public static class DomainTreeHelper
    {

        /// <summary>
        /// Actualiza un arbol de entidades en base al esquema de la base.
        /// <para>
        /// La cadena de conexion la resuelve el CONTRATO de configuracion (SPEC-261016-0027 6/C.1): el reverse no la
        /// recibe por un mecanismo propio del tool ni por un parametro provisto por un formulario.
        /// </para>
        /// </summary>
        /// <param name="oldTree">El arbol previo, con el que se fusiona el nuevo.</param>
        /// <returns>El arbol actualizado.</returns>
        public static BaseTreeNode UpdateDomainTree(BaseTreeNode oldTree) =>
            UpdateDomainTree(ConnectionStringResolver.Resolve(), oldTree);

        private static BaseTreeNode UpdateDomainTree(string connectionString, BaseTreeNode oldTree)
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
            Cooperator.Framework.Modeling.Model.DomainTreeHelper.CorrectGenerationNamesForRelatedFields(newTree);



            // Devolvemos el nuevo
            return newTree;
        }



        /// <summary>
        /// Obtiene el arbol de entidades usando el esquema de la base.
        /// </summary>
        /// <returns></returns>
        /// <summary>
        /// Construye el arbol de entidades desde el esquema de la base, con la cadena que resuelve el CONTRATO de
        /// configuracion (SPEC-261016-0027 6/C.1).
        /// </summary>
        /// <param name="oldDomainTree">El arbol previo (para heredar lo que el esquema no trae).</param>
        /// <returns>El arbol nuevo.</returns>
        public static BaseTreeNode GetDomainTree(BaseTreeNode oldDomainTree) =>
            GetDomainTree(ConnectionStringResolver.Resolve(), oldDomainTree);

        private static BaseTreeNode GetDomainTree(string connectionString, BaseTreeNode oldDomainTree)
        {

            MSSQLReverseFactory factory = new MSSQLReverseFactory(connectionString);
            DataBase databaseSchema = (new DBReverseEngine(factory)).GetSchema();

            // REVISAR
            // Se agregan las Views como si fueran tablas para enga�ar al Modeler
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

                    EntityNode oldEntity = Cooperator.Framework.Modeling.Model.DomainTreeHelper.FindEntityInTree(oldDomainTree, t.Name);
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

            Cooperator.Framework.Modeling.Model.DomainTreeHelper.CorrectSelectedCollections(root);

            // Correct GenerateEntity
            foreach (EntityNode currentEntity in root.Children)
            {
                EntityNode entity = Cooperator.Framework.Modeling.Model.DomainTreeHelper.FindEntityInTree(currentEntity.Parent, currentEntity.GenerateAsChildOf);
                if (entity != null) entity.GenerateEntity = true;
            }

            return root;

        }

        /// <summary>
        /// Genera un EntityNode para un objeto de la base de datos del Tipo DBTable
        /// En esta categor�a entran Tablas y Vistas de la Base de datos.
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
                        catch (Cooperator.Framework.Modeling.Exceptions.NodeAlreadyExistException)
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
                                catch (Cooperator.Framework.Modeling.Exceptions.NodeAlreadyExistException)
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
                        catch (Cooperator.Framework.Modeling.Exceptions.NodeAlreadyExistException)
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
}}

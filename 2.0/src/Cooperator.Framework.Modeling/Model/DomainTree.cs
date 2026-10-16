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
#nullable disable // archivo portado del 1.4 (net48, sin anotaciones): SPEC-261016-0004 §A.3

using System;
using System.Collections.Generic;
using System.Text;

namespace Cooperator.Framework.Modeling.Model
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
    /// Clase base de la cual heredan los nodos del arbol. (Entidad o Atributo) Es abstacta.
    /// </summary>
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
        public bool HaveChanges
        {
            get { return _haveChanges; }
            set { _haveChanges = value; }
        }



        /// <summary>
        /// Nombre del nodo.
        /// </summary>
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
        public string GenerateAsChildOf
        {
            get
            {
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
        public bool IsAutoGenerated
        {
            get { return _IsAutoGenerated; }
        }


        private bool _GenerateAsReadOnly = false;
        /// <summary>
        /// 
        /// </summary>
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
        public bool IsNullable
        {
            get { return _IsNullable; }
        }


        private string _CLRType = "";
        /// <summary>
        /// 
        /// </summary>
        public string CLRType
        {
            get { return _CLRType; }
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


        private bool _GenerateAsLazyLoad = false;
        /// <summary>
        /// 
        /// </summary>
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
        public List<RelatedField> RelatedFields
        {
            get { return _RelatedFields; }
        }


        private bool _GenerateAsProtected = false;
        /// <summary>
        /// 
        /// </summary>
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
        public bool IsPrimaryKey
        {
            get { return _IsPrimaryKey; }
        }


        private bool _IsForeignKey = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsForeignKey
        {
            get { return _IsForeignKey; }
        }


        private bool _GenerateAsDescriptionField = false;
        /// <summary>
        /// 
        /// </summary>
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
        public bool IsRowVersion
        {
            get { return _IsRowVersion; }
        }

        private bool _IsRowGuid;
        /// <summary>
        /// 
        /// </summary>
        public bool IsRowGuid
        {
            get { return _IsRowGuid; }
        }

        private bool _IsIdentity;
        /// <summary>
        /// 
        /// </summary>
        public bool IsIdentity
        {
            get { return _IsIdentity; }
        }

        private bool _IsComputed;
        /// <summary>
        /// 
        /// </summary>
        public bool IsComputed
        {
            get { return _IsComputed; }
        }

        private string _RelatedTableName = "";
        /// <summary>
        /// 
        /// </summary>
        public string RelatedTableName
        {
            get { return _RelatedTableName; }
        }


        private CollectionTypeEnum _GenerateAsType = CollectionTypeEnum.NotApplicable;
        /// <summary>
        /// 
        /// </summary>
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
        public bool IsCollection
        {
            get { return _IsCollection; }
        }


        private bool _IsEntity;
        /// <summary>
        /// 
        /// </summary>
        public bool IsEntity
        {
            get { return _IsEntity; }
        }

        private bool _IsDescriptionField;
        /// <summary>
        /// 
        /// </summary>
        public bool IsDescriptionField
        {
            get { return _IsDescriptionField; }
        }



        private bool _IsOneToOneRelation;
        /// <summary>
        /// 
        /// </summary>
        public bool IsOneToOneRelation
        {
            get { return _IsOneToOneRelation; }
        }


        private bool _IsLazyLoadable;
        /// <summary>
        /// 
        /// </summary>
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



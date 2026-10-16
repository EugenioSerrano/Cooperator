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
    /// Logica de arbol del modelo: mitad portable de DomainTreeHelper (SPEC-261016-0004 B.2).
    /// La mitad reverse (UpdateDomainTree/GetDomainTree/ADDNodeFromDBObject) vive en el tooling.
    /// </summary>
    public static class DomainTreeHelper
    {
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
}

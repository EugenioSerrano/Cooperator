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

#nullable disable // SPEC-261016-0034 §6/B.4: archivo APORTADO del 1.4 con diagnosticos de nulabilidad (CS8618/CS8600/CS8603) — la directiva es la unica mecanica autorizada sobre el aporte.
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Cooperator.Framework.Core
{

    ///// <summary>
    ///// Base class for Objects and entity collections
    ///// </summary>
    ///// <typeparam name="T">Row or Entity type</typeparam>
    //public class EntityList<T> : BaseRowSet<T> where T : IRow, new() 
    //{                
    //}


    /// <summary>
    /// Base class for Objects and entity collections
    /// </summary>
    public class ObjectList<T> : List<T>, IObjectList<T> //, IBindingList
        where T : IObject, new()
    {
        /// <summary>
        /// BaseRowSet based on TOutput Type
        /// </summary>
        /// <typeparam name="TOutput">Type for convertion</typeparam>
        /// <returns></returns>
        public ObjectList<TOutput> ConvertList<TOutput>() where TOutput:class,IObject,new()
        { 
            List<TOutput> lTOutput=this.ConvertAll<TOutput>(this.CurrentTto<TOutput>);
            ObjectList<TOutput> brsTOutput = new ObjectList<TOutput>();
            brsTOutput.AddRange(lTOutput);
            return brsTOutput;
        }

        ObjectList<T> _deletedItems;

        private ObjectList<T> deletedItems
        {
            get
            {
                if (_deletedItems == null) _deletedItems = new ObjectList<T>();
                return _deletedItems;
            }
        }


        IObjectList<T> IObjectList<T>.DeletedItems()
        {
            return deletedItems;
        }

        void IObjectList<T>.ResetDeletedItems()
        {
            _deletedItems = new ObjectList<T>();
        }

        /// <summary>
        /// Converter for BaseRowSet ConvertList
        /// </summary>
        /// <typeparam name="TOutput">Type for convertion result. </typeparam>
        /// <param name="p">Object to convert</param>
        /// <returns>Converter object</returns>
        public TOutput CurrentTto<TOutput>(T p)
            where TOutput : class 
        {
            TOutput result = p as TOutput;
            if (result == null)
                throw new InvalidCastException();
            
            return result;
        }

        /// <summary>
        /// Replace Object or Entity in list, based on UniqueIdentifier
        /// </summary>
        /// <param name="replacingEntity">New Object or Entity</param>
        public void Replace(T replacingEntity)
        {
            List<T> mylist = this;
            int counter = 0;
            foreach (T Obj in mylist)
            {
                if (isTheSame(Obj, replacingEntity))
                {
                    base[counter] = replacingEntity;
                    return;
                }
                counter += 1;
            }
            throw new Exceptions.ItemNotFoundInListException("Row or Entity not found in list");
        }


        /// <summary>
        /// Remove item from list. 
        /// </summary>
        /// <param name="itemToRemove">Row or Entity to remove from ObjectSet or list.</param>
        public new void Remove(T itemToRemove)
        {
            List<T> mylist = this;
            foreach (T Obj in mylist)
            {
                if (isTheSame(Obj, itemToRemove))
                {
                    ObjectStateHelper.SetAsDeleted(Obj);
                    if (!ObjectStateHelper.IsNew(Obj))
                        deletedItems.Add(Obj);
                    base.Remove(Obj);
                    return;
                }
            }
            throw new Exceptions.ItemNotFoundInListException("Row or Entity not found in list");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="match"></param>
        public new void RemoveAll(Predicate<T> match)
        {
            List<T> mylist = this;
            foreach (T Obj in mylist.FindAll(match))
            {
                ObjectStateHelper.SetAsDeleted(Obj);
                if (!ObjectStateHelper.IsNew(Obj))
                    deletedItems.Add(Obj);
            }
            base.RemoveAll(match);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public new void RemoveAt(int index)
        {
            T item = base[index];
            if (item != null)
            {
                ObjectStateHelper.SetAsDeleted(item);
                if (!ObjectStateHelper.IsNew(item))
                    deletedItems.Add(item);
            }
            base.RemoveAt(index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        public new void RemoveRange(int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                T item = base[i + index];
                if (item != null)
                {
                    ObjectStateHelper.SetAsDeleted(item);
                    if (!ObjectStateHelper.IsNew(item))
                        deletedItems.Add(item);
                }
            }
            base.RemoveRange(index, count);
        }
        

        /// <summary>
        /// Get a item from list searching by UniqueIdentifier.
        /// </summary>
        /// <param name="itemToSearch">Item to search for.</param>
        /// <returns>Row or Entity, null if item not found.</returns>
        public T GetItem(T itemToSearch)
        {

            T returnObject = default(T);
            System.Threading.Tasks.Parallel.ForEach<T>(
                this, obj =>
                {
                    if (isTheSame(obj, itemToSearch)) returnObject = obj;
                }
             );

            return returnObject;


            //foreach (T Obj in this)
            //{
            //    // This is very slow :(
            //    //if (UniqueIdentifierHelper.IsSameObject((IUniqueIdentifiable)Obj, uniqueIdentifier))
            //    //    return Obj;

            //    if (isTheSame(Obj, itemToSearch))
            //        return Obj;
            //}
            //return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemToSearch"></param>
        /// <returns></returns>
        public T GetItem(IUniqueIdentifiable itemToSearch)
        {
            T returnObject = default(T);
            System.Threading.Tasks.Parallel.ForEach<T>(
                this, obj => {
                    if (isTheSame((IUniqueIdentifiable) obj, itemToSearch))  returnObject = obj; 
                }
             );

            return returnObject;

            //foreach (T Obj in this)
            //{

            //    if (isTheSame((IUniqueIdentifiable) Obj, itemToSearch)) 
            //        return Obj;
            //}
            //return default(T);
        }




        /// <summary>
        /// Set Object state for all items in Objectset or list.
        /// </summary>
        public void SetState(ObjectState state)
        {
            List<T> mylist = this;
            foreach (T Obj in mylist)
                Obj.State = state;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        protected bool isTheSame(T obj1, T obj2)
        {
            bool _isTheSame = true;
            int counter = 0;
            foreach (object keyItem in ((IUniqueIdentifiable)obj1).Identifier())
            {
                object item = ((IUniqueIdentifiable)obj2).Identifier()[counter];
                if (keyItem != null && item != null)
                {
                    if (item.ToString() != keyItem.ToString())
                    {
                        _isTheSame = false;
                        break;
                    }
                }
                counter++;
            }
            return _isTheSame;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        protected bool isTheSame(IUniqueIdentifiable obj1, IUniqueIdentifiable obj2)
        {
            bool _isTheSame = true;
            int counter = 0;
            foreach (object keyItem in obj1.Identifier())
            {
                object item = obj2.Identifier()[counter];
                if (keyItem != null && item != null)
                {
                    if (item.ToString() != keyItem.ToString())
                    {
                        _isTheSame = false;
                        break;
                    }
                }
                counter++;
            }
            return _isTheSame;
        }

        /// <summary>
        /// Sort the list based on a property.
        /// </summary>
        /// <param name="propertyName">Property name to sort on</param>
        /// <param name="ascDesc">Descending or ascending order</param>
        public void Sort(string propertyName, bool ascDesc)
        {
            GenericComparer myComparer = new GenericComparer(typeof(T), propertyName, ascDesc);
            base.Sort(myComparer);
        }

        /// <summary>
        /// Private class used by sort
        /// </summary>
        private class GenericComparer : IComparer<T>
        {
            PropertyInfo _field;
            bool _ascDesc;
            public GenericComparer(Type type, string propertyName, bool ascDesc)
            {
                // SPEC-261016-0034 6/C y AC-5 (ADR-006 3.3): una sola verificacion, compartida con el gemelo del
                // llenado del DataSet; el fallo nombra la PROPIEDAD, el TIPO y el PUNTO que la pidio.
                _field = Mapping.TypeIdentification.RequireProperty(type, propertyName, "BaseObjectList:GenericComparer");
                _ascDesc = ascDesc;
            }

            object GetValue(object obj)
            {
                return ((PropertyInfo)_field).GetValue(obj, null);
            }

            int IComparer<T>.Compare(T obj1, T obj2)
            {
                IComparable comparer = (IComparable)GetValue(obj1);
                return comparer.CompareTo(GetValue(obj2)) * (_ascDesc ? 1 : -1);
            }

        }



        /// <summary>
        /// Returns a Dataset containing data from Objectset o entitylist.
        /// </summary>
        /// <returns>Untyped dataset</returns>
        public virtual System.Data.DataSet ToDataSet()
        {
            //create new dataset
            System.Data.DataSet dataSet = new System.Data.DataSet();

            // get properties from entity or Objectset
            PropertyInfo[] propertyArray = typeof(T).GetProperties();

            // Count non-System properties
            int noSystemProperties = 0;
            foreach (PropertyInfo prop in propertyArray)
            {
                if (prop.PropertyType.Namespace == null || !prop.PropertyType.Namespace.StartsWith("System", StringComparison.Ordinal)) noSystemProperties += 1;
                if (!prop.CanRead) noSystemProperties += 1;
            }

            // Create new array with System properties and remove state property.
            PropertyInfo[] propertyArray2 = new PropertyInfo[propertyArray.Length - noSystemProperties];
            int counter = 0;
            foreach (PropertyInfo prop in propertyArray)
            {
                string propName = prop.Name.ToLower(System.Globalization.CultureInfo.InvariantCulture);
                if ((prop.PropertyType.Namespace != null && prop.PropertyType.Namespace.StartsWith("System", StringComparison.Ordinal))
                    && propName != "state"
                    && prop.CanRead
                    )
                {
                    propertyArray2[counter] = prop;
                    counter += 1;
                }
            }

            // now we have al data in propertyArray2

            // Create datatable with her class name
            System.Data.DataTable dataTable = new System.Data.DataTable((new T()).GetType().ToString());

            // create columns for each property
            foreach (PropertyInfo prop in propertyArray2)
            {
                // SPEC-261016-0034 6/B.1 (ADR-006 3.1, F-47): el tipo de la columna sale del TIPO DISPONIBLE
                // (el PropertyType de la propiedad), no de un nombre reconstruido con Substring sobre el
                // ToString() de la reflexion. Un Nullable<T> resuelve su tipo subyacente POR API, asi que el
                // resultado deja de depender del ensamblado llamador y del formato del nombre; y si el tipo no
                // esta disponible, la identificacion FALLA con diagnostico accionable (nunca un DataColumn con
                // tipo nulo: el silencio que explotaba mas tarde).
                System.Type columnType = Mapping.TypeIdentification.UnwrapNullable(prop.PropertyType, "BaseObjectList:DataTableMapping");
                if (System.Nullable.GetUnderlyingType(prop.PropertyType) != null)
                {
                    System.Data.DataColumn myCol = new System.Data.DataColumn(prop.Name, columnType);
                    myCol.AllowDBNull = true;
                    dataTable.Columns.Add(myCol);
                }
                else
                {
                    dataTable.Columns.Add(prop.Name, columnType);
                }
            }

            // Add datatable to dataset
            dataSet.Tables.Add(dataTable);

            // Now populate de datatable from list
            foreach (T myObj in this)
            {
                System.Data.DataRow myRow = dataSet.Tables[0].NewRow();

                counter = 0;
                foreach (System.Data.DataColumn column in dataSet.Tables[0].Columns)
                {
                    if (propertyArray2[counter].GetValue(myObj, null) == null)
                    {
                        myRow[column.ColumnName] = System.DBNull.Value;
                    }
                    else
                    {
                        myRow[column.ColumnName] = propertyArray2[counter].GetValue(myObj, null);
                    }
                    counter += 1;
                }

                dataSet.Tables[0].Rows.Add(myRow);
            }

            //Returns the dataset
            return dataSet;

        }


    }
}


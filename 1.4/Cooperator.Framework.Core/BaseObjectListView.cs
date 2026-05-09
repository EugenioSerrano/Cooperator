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
using System.Reflection;
using System.ComponentModel;

namespace Cooperator.Framework.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectListView<T> : ObjectList<T>, IBindingListView
        where T : BaseObject, new()
    {

        private ObjectList<T> _originalList = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public ObjectListView(ObjectList<T> list)
        {
            _originalList = list;
            recreateList();
        }

        private void recreateList()
        {
            base.Clear();
            base.AddRange(_originalList);

            //Apply sort
            if (_sortCollection != null)
                ApplySort(_sortCollection);

            //Apply filter
            if (_Filter.ToLower().Replace(" ", "").Contains("deleted=false"))
                ApplyDeletedFilter();
        }


        /// <summary>
        /// 
        /// </summary>
        public void Synchronize()
        {
            recreateList();
        }

        #region Miembros de IBindingListView

        ListSortDescriptionCollection _sortCollection = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sorts"></param>
        public void ApplySort(ListSortDescriptionCollection sorts)
        {
            if (sorts.Count != 1)
            {
                throw new Exceptions.InvalidSortArgumentException("Only one property can be used to sort a ListView");
            }
            _sortCollection = sorts;
            _IsSorted = true;
            bool direction = (sorts[0].SortDirection == ListSortDirection.Ascending);
            base.Sort(sorts[0].PropertyDescriptor.Name, direction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="direction"></param>
        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            ListSortDescription[] sorts = new ListSortDescription[1];
            sorts[0] = new ListSortDescription(property, direction);
            ApplySort(new ListSortDescriptionCollection(sorts));
        }

        private bool _IsSorted = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsSorted
        {
            get { return _IsSorted; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ListSortDescriptionCollection SortDescriptions
        {
            get { return _sortCollection; }
        }


        /// <summary>
        /// 
        /// </summary>
        public void RemoveSort()
        {
            _sortCollection = null;
            _IsSorted = false;
            recreateList();
            // Send a IBindingList list event
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0, 0));
        }

        /// <summary>
        /// 
        /// </summary>
        public ListSortDirection SortDirection
        {
            get { return _sortCollection[0].SortDirection; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PropertyDescriptor SortProperty
        {
            get { return _sortCollection[0].PropertyDescriptor; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SupportsAdvancedSorting
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SupportsSorting
        {
            get { return true; }
        }

        private string _Filter = "";
        /// <summary>
        /// 
        /// </summary>
        public string Filter
        {
            get
            {
                return _Filter; ;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    value = "";
                else
                {
                    if (value.ToLower().Replace(" ", "").Contains("deleted=false"))
                    {
                        _Filter = value;
                        ApplyDeletedFilter();
                    }
                    else
                        throw new Exceptions.InvalidFilterArgumentException("The filter argument is not valid.");
                }
            }
        }

        private void ApplyDeletedFilter()
        {
            if (_Filter.ToLower().Replace(" ", "").Contains("deleted=false"))
            {
                for (int i = base.Count - 1; i >= 0; i--)
                {
                    if (ObjectStateHelper.IsDeleted(base[i]))
                        base.RemoveAt(i);
                }

            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void RemoveFilter()
        {
            _Filter = "";
            recreateList();
            // Send a IBindingList list event
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0, 0));
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SupportsFiltering
        {
            get { return true; }
        }


        /// <summary>
        /// Forces the filtering of the list based on the use of the specified Predicate.
        /// </summary>
        /// <param name="match"></param>
        public void ApplyFilter(Predicate<T> match)
        {
            // Restore the state without filter
            recreateList();

            for (int i = base.Count - 1; i >= 0; i--)
            {
                if (!match(base[i]))
                {
                    base.RemoveAt(i);
                }
            }

            // Send a IBindingList list event
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0, 0));
        }



        #endregion

        private ListChangedEventArgs _resetEvent = new ListChangedEventArgs(ListChangedType.Reset, -1);
        private ListChangedEventHandler _onListChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ev"></param>
        protected virtual void OnListChanged(ListChangedEventArgs ev)
        {
            if (_onListChanged != null)
                _onListChanged(this, ev);
        }

        /// <summary>
        /// 
        /// </summary>
        public event ListChangedEventHandler ListChanged
        {
            add
            {
                _onListChanged += value;
            }
            remove
            {
                _onListChanged -= value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void OnClearComplete()
        {
            OnListChanged(_resetEvent);
        }

        private BaseObject _justAddedItem;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected void OnInsertComplete(int index, BaseObject item)
        {
            // agrega los escuchadores de eventos
            item.OnCancelEdit += new CancelEditEventDelegate(CancelEditHandler);
            item.OnEndEdit += new EndEditEventDelegate(EndEditHandler);

            _justAddedItem = item;

            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }

        private void EndEditHandler(BaseObject sender, System.EventArgs e)
        {
            if (_justAddedItem == sender)
            {
                _justAddedItem = null;
            }
        }

        private void CancelEditHandler(BaseObject sender, System.EventArgs e)
        {
            if (_justAddedItem != null)
            {
                if (_justAddedItem == sender)
                {
                    //// HACK: esto es para evitar un bug raro del bindeo que 
                    //// ocurre cuando el item cancelado es el primero de la lista
                    //if (base.IndexOf((T)sender) != 0)
                    //{
                    base.Remove(((T)sender));
                    _originalList.Remove(((T)sender));
                    //}                    
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected void OnRemoveComplete(int index, BaseObject item)
        {
            // elimina el escuchador de evanto
            item.OnCancelEdit -= new CancelEditEventDelegate(CancelEditHandler);
            item.OnEndEdit -= new EndEditEventDelegate(EndEditHandler);

            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected void OnSetComplete(int index, object oldValue, object newValue)
        {
            if (oldValue != newValue)
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public new T this[int index]
        {
            get
            {
                return (T)(base[index]);
            }
            set
            {
                T oldValue = base[index];
                base[index] = value;
                OnSetComplete(index, oldValue, value);
            }
        }

        #region Miembros de IBindingList




        #region Find

        ///<summary>
        /// Finds the first object in the list matching the search criteria.
        ///</summary>
        /// <param name="property">The <see cref="PropertyDescriptor"/> to search.</param>
        /// <param name="key">Value to find in the property.</param>
        /// <returns>The index of the object in the list, -1 if not found.</returns>
        public int Find(PropertyDescriptor property, object key)
        {
            return this.Find(property, key, true);
        }

        ///<summary>
        /// Finds the first object in the list matching the search criteria.
        ///</summary>
        /// <param name="property">The <see cref="PropertyDescriptor"/> to search.</param>
        /// <param name="key">Value to find in the property.</param>
        /// <param name="ignoreCase">Perform the search on case-sensitive/insensitive mode with strings.</param>
        /// <returns>The index of the object in the list, -1 if not found.</returns>
        public int Find(PropertyDescriptor property, object key, bool ignoreCase)
        {
            return this.Find(property, key, ignoreCase, StringLike.Equal, 0);
        }

        ///<summary>
        /// Finds the first object in the list matching the search criteria.
        ///</summary>
        /// <param name="property">Property to search.</param>
        /// <param name="key">Value to find in the property.</param>
        public virtual T Find(string property, object key)
        {
            return Find(property, key, true);
        }

        ///<summary>
        /// Finds the first object in the list matching the search criteria.
        ///</summary>
        /// <param name="property">Property to search.</param>
        /// <param name="key">Value to find in the property.</param>
        /// <param name="ignoreCase">Perform the search on case-sensitive/insensitive mode with strings.</param>
        public virtual T Find(string property, object key, bool ignoreCase)
        {
            return Find(property, key, ignoreCase, StringLike.Equal, 0);
        }

        ///<summary>
        /// Finds the first object in the list matching the search criteria.
        ///</summary>
        /// <param name="property">Property to search.</param>
        /// <param name="key">Value to find in the property.</param>
        /// <param name="ignoreCase">Perform the search on case-sensitive/insensitive mode with strings.</param>
        /// <param name="stringLike">One of the <see cref="StringLike"/> enum values.</param>
        /// <param name="start">The zero-based index of the list to start the search.</param>
        public virtual T Find(string property, object key, bool ignoreCase, StringLike stringLike, int start)
        {
            PropertyDescriptorCollection oProperties = TypeDescriptor.GetProperties(typeof(T));
            PropertyDescriptor oProp = oProperties.Find(property, true);

            if (oProp != null)
            {
                int index = this.Find(oProp, key, ignoreCase, stringLike, start);

                if (index > -1)
                {
                    return this[index];
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Finds the index of the item with the specified value in the specified property descriptor.
        /// </summary>
        /// <param name="property">The <see cref="PropertyDescriptor"/> to search in.</param>
        /// <param name="key">Value to look for.</param>
        /// <param name="ignoreCase">Perform the search on case-sensitive/insensitive mode with strings.</param>
        /// <param name="stringLike">One of the <see cref="StringLike"/> enum values.</param>
        /// <param name="start">The zero-based index of the list to start the search.</param>
        /// <returns>The zero-based index of the item that matches the search-criteria. </returns>
        protected virtual int Find(PropertyDescriptor property, object key, bool ignoreCase, StringLike stringLike, int start)
        {
            for (int i = start; i < Count; i++)
            {

                T listItem = this[i];
                object tempObject = property.GetValue(listItem);
                if ((key == null) && (tempObject == null))              // If the value of the property and the key are null, this is the index.
                {
                    return i;
                }
                else if (tempObject is string && key != null)           // If the value of the property is a string, perform a string comparison.
                {
                    switch (stringLike)
                    {
                        case StringLike.Equal:
                            if (String.Compare(tempObject.ToString(), key.ToString(), ignoreCase) == 0)
                                return i;
                            break;
                        case StringLike.StartsWith:
                            if (tempObject.ToString().StartsWith(key.ToString(), ignoreCase, System.Globalization.CultureInfo.CurrentCulture))
                                return i;
                            break;
                        case StringLike.EndsWith:
                            if (tempObject.ToString().EndsWith(key.ToString(), ignoreCase, System.Globalization.CultureInfo.CurrentCulture))
                                return i;
                            break;
                        case StringLike.Contains:
                            if (tempObject.ToString().Contains(key.ToString()))
                                return i;
                            break;
                    }
                }
                else if (tempObject != null && tempObject.Equals(key))  // If the property is of another type, check with Equals.
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion Find


        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SupportsSearching
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public new void Add(T item)
        {
            base.Add(item);
            _originalList.Add(item);
            OnInsertComplete(base.IndexOf(item), item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public new void AddRange(IEnumerable<T> collection)
        {
            //base.AddRange(collection);
            //_originalList.AddRange(collection);
            foreach (T item in collection)
            {
                base.Add(item);
                _originalList.Add(item);
            }

            if (!String.IsNullOrEmpty(_Filter))
                ApplyDeletedFilter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            _originalList.Add(item);
            OnInsertComplete(index, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="collection"></param>
        public new void InsertRange(int index, IEnumerable<T> collection)
        {
            base.InsertRange(index, collection);
            //_originalList.AddRange(collection);            
            foreach (T item in collection)
            {
                _originalList.Add(item);
                index++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemToRemove"></param>
        public new void Remove(T itemToRemove)
        {
            int index = base.IndexOf(itemToRemove);
            base.Remove(itemToRemove);
            try
            {
                _originalList.Remove(itemToRemove);
            }
            catch (Exceptions.ItemNotFoundInListException)
            {
                // Como hemos tenido que sombrear algunos metodos, es posible que se haya agregado algun item
                // a la coleccion, y que nosotros no nos hayamos enterado, por eso si da ese error lo ignoramos.
            }
            OnRemoveComplete(index, itemToRemove);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="match"></param>
        public new void RemoveAll(Predicate<T> match)
        {
            _originalList.RemoveAll(match);
            base.RemoveAll(match);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public new void RemoveAt(int index)
        {
            T item = base[index];
            ObjectStateHelper.SetAsDeleted(item);
            base.RemoveAt(index);
            try
            {
                _originalList.Remove(item);
            }
            catch (Exceptions.ItemNotFoundInListException)
            {
                // Como hemos tenido que sombrear algunos metodos, es posible que se haya agregado algun item
                // a la coleccion, y que nosotros no nos hayamos enterado, por eso si da ese error lo ignoramos.
            }
            OnRemoveComplete(index, item);
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
                try
                {
                    _originalList.Remove(item);
                }
                catch (Exceptions.ItemNotFoundInListException)
                {
                    // Como hemos tenido que sombrear algunos metodos, es posible que se haya agregado algun item
                    // a la coleccion, y que nosotros no nos hayamos enterado, por eso si da ese error lo ignoramos.
                }
                base.RemoveAt(i + index);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="replacingEntity"></param>
        public new void Replace(T replacingEntity)
        {
            //int index=0;
            //T oldValue;
            base.Replace(replacingEntity);
            try
            {
                _originalList.Replace(replacingEntity);
            }
            catch (Exceptions.ItemNotFoundInListException)
            {
                // Como hemos tenido que sombrear algunos metodos, es posible que se haya agregado algun item
                // a la coleccion, y que nosotros no nos hayamos enterado, por eso si da ese error lo ignoramos.
            }


            //OnSetComplete(index, oldValue, replacingEntity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public new void SetState(ObjectState state)
        {
            _originalList.SetState(state);
            if ((state & ObjectState.Deleted) == ObjectState.Deleted)
            {
                if (_Filter.ToLower().Replace(" ", "").Contains("deleted=false"))
                    this.Clear();
            }
            else
            {
                base.SetState(state);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object AddNew()
        {
            T e = new T();
            this.Add(e);
            return e;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AllowNew
        {
            get { return true; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AllowEdit
        {
            get { return true; ; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AllowRemove
        {
            get { return true; }
        }


        /// <summary>
        /// 
        /// </summary>
        public bool SupportsChangeNotification
        {
            get { return true; }
        }


        #endregion
    }


    #region " StringLike Enum "
    /// <summary>
    /// Enumeration to specify how a string looks like in another string.
    /// </summary>
    public enum StringLike
    {
        /// <summary>
        /// The string is equal.
        /// </summary>
        Equal = 0,
        /// <summary>
        /// The string starts with.
        /// </summary>
        StartsWith,
        /// <summary>
        /// The string ends with.
        /// </summary>
        EndsWith,
        /// <summary>
        /// The string is contained.
        /// </summary>
        Contains
    }

    #endregion

}


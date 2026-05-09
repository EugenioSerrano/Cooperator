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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using Cooperator.Framework.Core.LazyLoad;

namespace Cooperator.Framework.Core
{
    // Delegates
    /// <summary>
    /// CancelEditEventDelegate Delegate definition.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks>Implements the callback when an CancelEditEvent produces.</remarks>
    public delegate void CancelEditEventDelegate(BaseObject sender, System.EventArgs e);
    /// <summary>
    ///  CancelEditEventDelegate Delegate definition.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks>Implements the callback when an BeginEditEvent produces.</remarks>
    public delegate void BeginEditEventDelegate(BaseObject sender, System.EventArgs e);
    /// <summary>
    ///  BeginEditEventDelegate Delegate definition.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks>Implements the callback when an EndEditEvent produces.</remarks>
    public delegate void EndEditEventDelegate(BaseObject sender, System.EventArgs e);

    /// <summary>
    /// Base class for Objects and Entities, implements IRow.
    /// </summary>
    [Serializable]
    public abstract class BaseObject : IObject
    {
        
        #region Miembros de IEditableObject

        /// <summary>
        /// OnCancelEdit event of CancelEditEventDelegate type.
        /// </summary>
        /// <remarks>This event is binded to CancelEditEventDelegate Delegate</remarks>
        [field: NonSerialized]
        public event CancelEditEventDelegate OnCancelEdit;

        /// <summary>
        /// OnBeginEdit event of BeginEditEventDelegate type.
        /// </summary>
        /// <remarks>This event is binded to BeginEditEventDelegate Delegate</remarks>
        [field: NonSerialized]
        public event BeginEditEventDelegate OnBeginEdit;

        /// <summary>
        /// OnBeginEdit event of EndEditEventDelegate type.
        /// </summary>
        /// <remarks>This event is binded to EndEditEventDelegate Delegate</remarks>
        [field: NonSerialized]
        public event EndEditEventDelegate OnEndEdit;

        /// <summary>
        /// This local variable isolates the boolean value if the event 
        /// is in the context of a transaction
        /// </summary>
        private bool inTxn;

        /// <summary>
        /// IEditableObject.BeginEdit implements the OnBeginEdit event call 
        /// that fires the function delegates in BeginEditEventDelegate
        /// </summary>
        /// <remarks>If in the context of a Transaction, then fire the OnBeginEdit with 
        /// a reference to itself and a new System.EventArgs() like parameters.</remarks>
        void IEditableObject.BeginEdit()
        {
            if (!inTxn)
            {
                OnBeginEdit?.Invoke(this, new EventArgs());
                inTxn = true;
            }
        }

        /// <summary>
        /// IEditableObject.CancelEdit implements the OnCancelEdit event call 
        /// that fires the function delegates in CancelEditEventDelegate
        /// </summary>
        /// <remarks>If in the context of a Transaction, then fire the OnCancelEdit with 
        /// a reference to itself and a new System.EventArgs() like parameters.</remarks>
        void IEditableObject.CancelEdit()
        {
            if (inTxn)
            {
                OnCancelEdit?.Invoke(this, new EventArgs());
                inTxn = false;
            }
        }

        /// <summary>
        /// IEditableObject.EndEdit implements the OnEndEdit event call 
        /// that fires the function delegates in EndEditEventDelegate
        /// </summary>
        /// <remarks>If in the context of a Transaction, then fire the OnEndEdit with 
        /// a reference to itself and a new System.EventArgs() like parameters.</remarks>
        void IEditableObject.EndEdit()
        {
            if (inTxn)
            {
                OnEndEdit?.Invoke(this, new EventArgs());
                inTxn = false;
            }
        }

        #endregion
        

        /// <summary>
        /// This private variable remains true while the constructor is 
        /// executing, when it finished then become false
        /// </summary>
        private bool _Initializing = true;

        /// <summary>
        /// Class constructor, set initial value state for Object or Entity
        /// </summary>
        /// <remarks>When Initialize() finished it execution, then the object 
        /// is initialized and set _Initializing = false</remarks>
        protected BaseObject()
        {
            _state = ObjectState.New;
            Initialize();
            _Initializing = false;
        }
        
        /// <summary>
        /// Called from constructor
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        /// Called only after parameterized constructor
        /// </summary>
        protected virtual void Initialized()
        {
        }
        
        /// <summary>
        /// State of row or entity
        /// </summary>
         ObjectState _state;

         /// <summary>
         /// When a property is changed, a copy of original object is stored here
         /// </summary>
        protected IObject InternalOriginalValue;
        
        /// <summary>
        /// When any property value changes, this method should be invoked.
        /// </summary>
        /// <remarks>If the object is initializing (_Initializing is true), 
        /// no clone it.
        /// If is modified for first time 
        /// ((_state &amp; ObjectState.Modified) != ObjectState.Modified ), 
        /// sets _OriginalValue and the object as modified</remarks>
        protected void PropertyModified()
        {
            if (_Initializing) return;

            if ((_state & ObjectState.Modified) != ObjectState.Modified )
            {
                SetOriginalValue();
                _state |= ObjectState.Modified;
            }
        }

        /// <summary>
        /// When a associated entity is set in the current entity, we call this method to give the posibility to correct the string that represents the object. (Description field)
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void CorrectDescriptionField(IObject entity)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        protected abstract void SetOriginalValue();
        
        /// <summary>
        /// Get original value of row or entity.
        /// </summary>
        public IObject OriginalValue()
        {
            if (ObjectStateHelper.IsModified(this))
            {
                return InternalOriginalValue;
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Return the object state.
        /// </summary>
        /// <remarks>It may value
        /// New = 1,
        /// Modified = 2,
        /// Deleted = 4,
        /// Restored = 8 and
        /// Updated = 16</remarks>
        ObjectState IObject.State
        {
            get { return _state; }
            set { _state = value; }
        }
    }
}

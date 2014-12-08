#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    XbimList.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions.Transactions.Extensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Common.XbimExtensions.Interfaces;

#endregion

namespace Xbim.XbimExtensions
{
    [IfcPersistedEntityAttribute, Serializable]
    public class XbimListUnique<T> : XbimList<T>
    {
        internal XbimListUnique(IPersistIfcEntity owner)
            : base(owner)
        {
        }

        internal XbimListUnique(IPersistIfcEntity owner, int capacity)
            : base(owner, capacity)
        {
        }

        internal XbimListUnique(IPersistIfcEntity owner, IEnumerable<T> list)
            : base(owner, list)
        {
        }


        public override string ListType
        {
            get { return "list-unique"; }
        }
    }

    /// <summary>
    ///   A set that supports list behaviour
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    [IfcPersistedEntityAttribute, Serializable]
    public class XbimListSet<T> : XbimList<T>
    {
        internal XbimListSet(IPersistIfcEntity owner)
            : base(owner)
        {
        }

        internal XbimListSet(IPersistIfcEntity owner, int capacity)
            : base(owner, capacity)
        {
        }

        internal XbimListSet(IPersistIfcEntity owner, IEnumerable<T> list)
            : base(owner, list)
        {
        }


        public override string ListType
        {
            get { return "set"; }
        }
    }

    [IfcPersistedEntityAttribute, Serializable]
    public class XbimList<T> : IList, IList<T>, IEnumerable<T>, INotifyCollectionChanged, INotifyPropertyChanged,
                               ExpressEnumerable, IXbimNoNotifyCollection
    {
        private readonly IList<T> _list;

        protected IList<T> m_List
        {
            get { return _list; }
        }

        internal XbimList(IPersistIfcEntity owner)
        {
            _list = new List<T>();
            _owningEntity = owner;
        }

        internal XbimList(IPersistIfcEntity owner, int capacity)
        {
            _list = new List<T>(capacity);
            _owningEntity = owner;
        }

        internal XbimList(IPersistIfcEntity owner, IEnumerable<T> collection)
        {
            _list = new List<T>(collection);
            _owningEntity = owner;
        }

        //public void TrimExcess()
        //{
        //    m_List..TrimExcess();
        //}

        #region INotifyPropertyChanged Members

        [field: NonSerialized]
        private event PropertyChangedEventHandler _propertyChanged;

        [NonSerialized] private static PropertyChangedEventArgs countPropChangedEventArgs =
            new PropertyChangedEventArgs("Count");

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }

        private void NotifyCountChanged(int oldValue)
        {
            PropertyChangedEventHandler propChanged = _propertyChanged;
            if (propChanged != null && oldValue != m_List.Count)
                propChanged(this, countPropChangedEventArgs);
        }

        #endregion

        #region INotifyCollectionChanged Members

        [field: NonSerialized]
        private event NotifyCollectionChangedEventHandler _collectionChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _collectionChanged += value; }
            remove { _collectionChanged -= value; }
        }

        #endregion

        private void SetItem(int index, T item)
        {
            object removed = null;
            if (index < m_List.Count)
                removed = m_List[index];
            m_List[index] = item;
            if (_owningEntity.Activated) _owningEntity.ModelOf.Activate(_owningEntity, true);
            NotifyCollectionChangedEventHandler collChanged = _collectionChanged;
            if (collChanged != null)
            {
                if (index < m_List.Count)
                    collChanged(this,
                                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, removed,
                                                                     index, index));
                else
                {
                    collChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                    NotifyCountChanged(Count-1);
                }
            }
        }

        public int IndexOf(T item)
        {
            return m_List.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            m_List.Insert(index, item);
        }

        public virtual void RemoveAt(int index)
        {
            int oldCount = m_List.Count;
            T removed = m_List[index];
            m_List.RemoveAt(index);
            if (_owningEntity.Activated) _owningEntity.ModelOf.Activate(_owningEntity, true);
            NotifyCollectionChangedEventHandler collChanged = _collectionChanged;
            if (collChanged != null)
                collChanged(this,
                            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, index));
            NotifyCountChanged(oldCount);
        }


        public T this[int index]
        {
            get { return m_List[index]; }
            set { this.SetItem(index, value); }
        }


        public virtual void Add(T item)
        {
            int oldCount = m_List.Count;
            m_List.Add(item);
            if (_owningEntity.Activated) 
                _owningEntity.ModelOf.Activate(_owningEntity, true);
            NotifyCollectionChangedEventHandler collChanged = _collectionChanged;
            if (collChanged != null)
                collChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
            NotifyCountChanged(oldCount);
        }

        public virtual void Clear()
        {
            int oldCount = m_List.Count;
            //foreach (var item in mm_List)
            //{
            //    ISupportInverse iModeRef = item as ISupportInverse;
            //    if (iModeRef != null) iModeRef.DecrementModelRefCount();
            //}
            m_List.Clear();
            if (_owningEntity.Activated) _owningEntity.ModelOf.Activate(_owningEntity, true);
            NotifyCollectionChangedEventHandler collChanged = _collectionChanged;
            if (collChanged != null)
                collChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            NotifyCountChanged(oldCount);
        }

        public bool Contains(T item)
        {
            return m_List.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_List.CopyTo(array, arrayIndex);
        }

        public virtual int Count
        {
            get { return m_List.Count; }
        }


        public virtual bool Remove(T item)
        {
            int oldCount = m_List.Count;
            bool removed = m_List.Remove(item);
            if (removed)
            {
                if (_owningEntity.Activated) _owningEntity.ModelOf.Activate(_owningEntity, true);
                NotifyCollectionChangedEventHandler collChanged = _collectionChanged;
                if (collChanged != null)
                    collChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
                NotifyCountChanged(oldCount);
            }
            return removed;
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            if (m_List.Count == 0)
                return Enumerable.Empty<T>().GetEnumerator();
            else
                return m_List.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (m_List.Count == 0)
                return Enumerable.Empty<T>().GetEnumerator();
            else
                return m_List.GetEnumerator();
        }

        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            this.Add((T)value);
            return m_List.Count - 1;
        }

        void IList.Clear()
        {
            this.Clear();
        }

        bool IList.Contains(object value)
        {
            return m_List.Contains((T) value);
        }

        int IList.IndexOf(object value)
        {
            return m_List.IndexOf((T) value);
        }

        void IList.Insert(int index, object value)
        {
            this.Insert(index, (T) value);
        }

        bool IList.IsFixedSize
        {
            get { return ((IList) m_List).IsFixedSize; }
        }

        bool IList.IsReadOnly
        {
            get { return ((IList) m_List).IsReadOnly; }
        }

        void IList.Remove(object value)
        {
            this.Remove((T) value);
        }

        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        object IList.this[int index]
        {
            get { return m_List[index]; }
            set { this.Insert(index, (T) value); }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            this.CopyTo((T[]) array, index);
        }

        int ICollection.Count
        {
            get { return m_List.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection) m_List).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection) m_List).SyncRoot; }
        }

        #endregion

        #region ExpressEnumerable Members

        public virtual string ListType
        {
            get { return "list"; }
        }

        #endregion

        #region IList<T> Members

       

        void IList<T>.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        #endregion

        #region ICollection<T> Members



        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        void ICollection<T>.Clear()
        {
            this.Clear();
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return ((IList) m_List).IsReadOnly; }
        }

       

        #endregion

        #region ExpressEnumerable Members

        void ExpressEnumerable.Add(object o)
        {
            this.Add((T)o);
        }

        #endregion

        private readonly IPersistIfcEntity _owningEntity;

        private IPersistIfcEntity OwningEntity
        {
            get { return _owningEntity; }
        }

        void IXbimNoNotifyCollection.Add(object o)
        {
            m_List.Add((T)o);
        }

        
    }
}
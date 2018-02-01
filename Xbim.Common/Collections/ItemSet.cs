using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Xbim.Common.Exceptions;

namespace Xbim.Common.Collections
{
    public abstract class ItemSet<T> : IItemSet<T>, IList
    {
        protected readonly int Property;
        protected readonly bool IsEntitySet;

        protected IModel Model { get { return OwningEntity.Model; } }

        public IPersistEntity OwningEntity { get; private set; }

        protected List<T> Internal { get; private set; }

        protected ItemSet(IPersistEntity entity, int capacity, int property)
        {
            //this will create internal list of optimal capacity
            Internal = new List<T>(capacity > 0 ? capacity : 0);
            Property = property;
            OwningEntity = entity;
            IsEntitySet = typeof(IPersistEntity).GetTypeInfo().IsAssignableFrom(typeof(T));
        }



        #region IItemSet<T> Members
        /// <summary>
        /// This function makes it possible to add nested lists if this is the case.
        /// It works like InsertAt is the object doesn't exist already. You cannot create lists with wholes.
        /// </summary>
        /// <param name="index">Index of the object</param>
        /// <returns>Object at specified index. If it is a nested list and it doesn't exist it gets created.</returns>
        public T GetAt(int index)
        {
            if (index < Count)
                return this[index];

            if (index > Count)
                throw new Exception("It is not possible to get object which is more that just the next after the last one.");

            if (!typeof(IItemSet).GetTypeInfo().IsAssignableFrom(typeof(T)))
                return default(T);

            var result = CreateNestedSet();
            Insert(index, result);
            return result;
        }

        protected T CreateNestedSet()
        {
            if (!typeof(IItemSet).GetTypeInfo().IsAssignableFrom(typeof(T)))
                throw new NotSupportedException();

            //get non-abstract type of IItemSet
            var type = GetType().GetGenericTypeDefinition();
            //get generic argument of nested item set
            var inner = typeof(T).GetTypeInfo().GetGenericArguments()[0];
            //create generic type which can be created and added to this set
            var toCreate = type.MakeGenericType(inner);

            var result = (T)Activator.CreateInstance(toCreate, BindingFlags.NonPublic | BindingFlags.Instance, null,
                new object[] { OwningEntity, 4, Property }, null);
            return result;
        }

        public void AddRange(IEnumerable<T> values)
        {
            if (Model.IsTransactional && Model.CurrentTransaction == null)
                throw new XbimException("Operation out of transaction");

            var enumerable = values as T[] ?? values.ToArray();
            if (IsEntitySet && enumerable.Any(v => v != null && !ReferenceEquals(((IPersistEntity)v).Model, Model)))
                throw new XbimException("Cross model entity assignment");


            //activate owning entity for write in case it is not active yet
            if (!OwningEntity.Activated)
                Model.Activate(OwningEntity);

            var items = values as T[] ?? enumerable.ToArray();
            Action doAction = () =>
            {
                Internal.AddRange(items);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Add, items);
                NotifyCountChanged();
            };


            if (!Model.IsTransactional)
            {
                doAction();
                return;
            }

            Action undoAction = () =>
            {
                foreach (var value in items)
                    Internal.Remove(value);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, items);
                NotifyCountChanged();
            };

            Model.CurrentTransaction.DoReversibleAction(doAction, undoAction, OwningEntity, ChangeType.Modified, Property);
        }

        public T FirstOrDefault(Func<T, bool> predicate)
        {
            return Enumerable.FirstOrDefault(this, predicate);
        }

        public TF FirstOrDefault<TF>(Func<TF, bool> predicate) where TF: T
        {
            return Enumerable.OfType<TF>(this).FirstOrDefault(predicate);
        }

        public IEnumerable<TW> Where<TW>(Func<TW, bool> predicate) where TW: T
        {
            return Enumerable.OfType<TW>(this).Where(predicate);
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyCountChanged()
        {
            if (PropertyChanged == null) return;
            PropertyChanged(this, new PropertyChangedEventArgs("Count"));
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void NotifyCollectionChanged(NotifyCollectionChangedAction action, T item)
        {
            if (CollectionChanged == null) return;
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item));
        }

        private void NotifyCollectionChanged(NotifyCollectionChangedAction action, IEnumerable<T> items)
        {
            if (CollectionChanged == null) return;
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, items));
        }

        private void NotifyCollectionChanged(NotifyCollectionChangedAction action)
        {
            if (CollectionChanged == null) return;
            CollectionChanged(this, new NotifyCollectionChangedEventArgs(action));
        }

        #endregion

        #region ICollection<T> Members

        public virtual void Add(T item)
        {
            if (Model.IsTransactional && Model.CurrentTransaction == null)
                throw new Exception("Operation out of transaction");

            if (IsEntitySet && item != null && !ReferenceEquals(((IPersistEntity)item).Model, Model))
                throw new XbimException("Cross model entity assignment");

            //activate owning entity for write in case it is not active yet
            if (!OwningEntity.Activated)
                Model.Activate(OwningEntity);

            Action doAction = () =>
            {
                Internal.Add(item);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item);
                NotifyCountChanged();
            };


            if (!Model.IsTransactional)
            {
                doAction();
                return;
            }

            Action undoAction = () =>
            {
                Internal.Remove(item);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, item);
                NotifyCountChanged();
            };

            Model.CurrentTransaction.DoReversibleAction(doAction, undoAction, OwningEntity, ChangeType.Modified, Property);
        }


        public virtual void Clear()
        {
            if (Model.IsTransactional && Model.CurrentTransaction == null)
                throw new Exception("Operation out of transaction");

            if (!OwningEntity.Activated)
                Model.Activate(OwningEntity);

            var oldItems = Internal.ToArray();
            Action doAction = () =>
            {
                Internal.Clear();
                NotifyCollectionChanged(NotifyCollectionChangedAction.Reset);
                NotifyCountChanged();
            };

            if (!Model.IsTransactional)
            {
                doAction();
                return;
            }

            Action undoAction = () =>
            {
                Internal.AddRange(oldItems);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Add, oldItems);
                NotifyCountChanged();
            };
            Model.CurrentTransaction.DoReversibleAction(doAction, undoAction, OwningEntity, ChangeType.Modified, Property);
        }

        public bool Contains(T item)
        {
            return Internal.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Internal.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Internal.Count; }
        }


        public virtual bool Remove(T item)
        {
            if (Model.IsTransactional && Model.CurrentTransaction == null)
                throw new Exception("Operation out of transaction");

            if (!OwningEntity.Activated)
                Model.Activate(OwningEntity);

            var result = false;
            Action doAction = () =>
            {
                result = Internal.Remove(item);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, item);
                NotifyCountChanged();
            };
            Action undoAction = () =>
            {
                Internal.Add(item);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item);
                NotifyCountChanged();
            };

            if (!Model.IsTransactional)
            {
                doAction();
                return true;
            }

            Model.CurrentTransaction.DoReversibleAction(doAction, undoAction, OwningEntity, ChangeType.Modified, Property);

            return result;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return Internal.Count == 0 ? Enumerable.Empty<T>().GetEnumerator() : Internal.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Internal.Count == 0 ? Enumerable.Empty<T>().GetEnumerator() : Internal.GetEnumerator();
        }

        #endregion

        #region ICollection<T> Members

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        void ICollection<T>.Clear()
        {
            Clear();
        }

        bool ICollection<T>.Contains(T item)
        {
            return Internal.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            Internal.CopyTo(array, arrayIndex);
        }

        int ICollection<T>.Count
        {
            get { return Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return ((IList<T>)Internal).IsReadOnly; }
        }

        bool ICollection<T>.Remove(T item)
        {
            return Remove(item);
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);
        }

        int ICollection.Count
        {
            get { return Internal.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)Internal).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)Internal).SyncRoot; }
        }

        #endregion

        #region IList<T> members
        public T this[int index]
        {
            get
            {
                return Internal[index];
            }
            set
            {
                if (Model.IsTransactional && Model.CurrentTransaction == null)
                    throw new XbimException("Operation out of transaction");

                if (IsEntitySet && value != null && !ReferenceEquals(((IPersistEntity)value).Model, Model))
                    throw new XbimException("Cross model entity assignment");

                if (!OwningEntity.Activated)
                    Model.Activate(OwningEntity);

                var oldValue = Internal[index];
                Action doAction = () =>
                {
                    Internal[index] = value;
                    NotifyCollectionChanged(NotifyCollectionChangedAction.Replace, value);
                };


                if (!Model.IsTransactional)
                {
                    doAction();
                    return;
                }

                Action undoAction = () =>
                {
                    Internal[index] = oldValue;
                    NotifyCollectionChanged(NotifyCollectionChangedAction.Replace, oldValue);
                };

                Model.CurrentTransaction.DoReversibleAction(doAction, undoAction, OwningEntity, ChangeType.Modified, Property);
            }
        }

        public int IndexOf(T item)
        {
            return Internal.IndexOf(item);
        }


        public void Insert(int index, T item)
        {
            if (Model.IsTransactional && Model.CurrentTransaction == null)
                throw new XbimException("Operation out of transaction");

            if (IsEntitySet && item != null && !ReferenceEquals(((IPersistEntity)item).Model, Model))
                throw new XbimException("Cross model entity assignment");

            if (!OwningEntity.Activated)
                Model.Activate(OwningEntity);

            Action doAction = () =>
            {
                Internal.Insert(index, item);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item);
                NotifyCountChanged();
            };

            if (!Model.IsTransactional)
            {
                doAction();
                return;
            }

            Action undoAction = () =>
            {
                Internal.RemoveAt(index);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, item);
                NotifyCountChanged();
            };
            Model.CurrentTransaction.DoReversibleAction(doAction, undoAction, OwningEntity, ChangeType.Modified, Property);
        }

        public void RemoveAt(int index)
        {
            var toRemove = Internal[index];
            Remove(toRemove);
        }
        #endregion

        #region IList members
        int IList.Add(object value)
        {
            if (!(value is T)) return -1;

            var v = (T)value;
            Add(v);
            return Internal.Count - 1;
        }

        bool IList.Contains(object value)
        {
            return ((IList)Internal).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return ((IList)Internal).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return Model.IsTransactional && Model.CurrentTransaction != null; }
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = value == null ? default(T) : (T)value;
            }
        }
        #endregion
    }
}

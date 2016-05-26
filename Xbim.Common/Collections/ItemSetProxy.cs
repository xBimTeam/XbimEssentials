using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Xbim.Common.Collections
{
    public class ItemSetProxy<TInner, TOuter> : IItemSet<TOuter>, IDisposable
    {
        private readonly IItemSet<TOuter> _extendedSet;
        private readonly Func<TInner, TOuter> _transformOut;
        private readonly Func<TOuter, TInner> _transformIn;
        private readonly IItemSet<TInner> _innerSet;

        public ItemSetProxy(
            IItemSet<TInner> innerSet, 
            IItemSet<TOuter> extendedSet, 
            Func<TInner, TOuter> transformOut, 
            Func<TOuter, TInner> transformIn)
        {
            _innerSet = innerSet;
            _extendedSet = extendedSet;
            _transformOut = transformOut;
            _transformIn = transformIn;

            //hook to inner events
            _innerSet.PropertyChanged += InnerSetOnPropertyChanged;
            _extendedSet.PropertyChanged += InnerSetOnPropertyChanged;
            _innerSet.CollectionChanged += InnerSetOnCollectionChanged;
            _extendedSet.CollectionChanged += ExtendedSetOnCollectionChanged;
        }

        private void ExtendedSetOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs ncArgs)
        {
            var innerCount = ((ICollection) _innerSet).Count;
            //we need to translate items to outer type before we fire it from this object
            NotifyCollectionChangedEventArgs args;
            switch (ncArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    args = new NotifyCollectionChangedEventArgs(ncArgs.Action, ncArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    args = new NotifyCollectionChangedEventArgs(ncArgs.Action, ncArgs.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    args = new NotifyCollectionChangedEventArgs(ncArgs.Action, ncArgs.NewItems, ncArgs.OldItems);
                    break;
                case NotifyCollectionChangedAction.Move:
                    args = new NotifyCollectionChangedEventArgs(ncArgs.Action, ncArgs.NewItems, ncArgs.NewStartingIndex + innerCount, ncArgs.OldStartingIndex + innerCount);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    args = new NotifyCollectionChangedEventArgs(ncArgs.Action);
                    break;
                default:
                    args = new NotifyCollectionChangedEventArgs(ncArgs.Action);
                    break;
            }
            OnCollectionChanged(args);
        }

        private void InnerSetOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs ncArgs)
        {
            //we need to translate items to outer type before we fire it from this object
            NotifyCollectionChangedEventArgs args;
            switch (ncArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    args = new NotifyCollectionChangedEventArgs(ncArgs.Action, ncArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    args = new NotifyCollectionChangedEventArgs(ncArgs.Action, ncArgs.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    args = new NotifyCollectionChangedEventArgs(ncArgs.Action, ncArgs.NewItems, ncArgs.OldItems);
                    break;
                case NotifyCollectionChangedAction.Move:
                    args = new NotifyCollectionChangedEventArgs(ncArgs.Action, ncArgs.NewItems, ncArgs.NewStartingIndex, ncArgs.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    args = new NotifyCollectionChangedEventArgs(ncArgs.Action);
                    break;
                default:
                    args = new NotifyCollectionChangedEventArgs(ncArgs.Action);
                    break;
            }
            OnCollectionChanged(args);
        }

        private void InnerSetOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged(propertyChangedEventArgs.PropertyName);
        }

        private IEnumerable<TOuter> Transformed
        {
            get { return _innerSet.Select(inner => _transformOut(inner)); }
        }


        public IEnumerator<TOuter> GetEnumerator()
        {
            return _extendedSet.Concat(Transformed).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TOuter item)
        {
            if (item == null)
                return;

            var inner = _transformIn(item);
            if (inner == null)
                _extendedSet.Add(item);
            else
                _innerSet.Add(inner);
        }

        public int Add(object value)
        {
            if (!(value is TOuter))
                return -1;

            var item = (TOuter) value;

            var inner = _transformIn(item);
            if (inner != null)
                return _innerSet.Add((object) inner);

            var index = _extendedSet.Add((object) item);
            return ((ICollection) _innerSet).Count + index;
        }

        public bool Contains(object value)
        {
            if (!(value is TOuter))
                return false;

            return _innerSet.Contains(_transformIn((TOuter) value)) || _extendedSet.Contains(value);
        }

        void IList.Clear()
        {
            if (_innerSet.Any())
                ((IList) _innerSet).Clear();
            if (_extendedSet.Any())
                ((IList) _extendedSet).Clear();
        }

        public int IndexOf(object value)
        {
            if (!(value is TOuter))
                return -1;
            var item = (TOuter) value;
            return IndexOf(item);
        }

        public void Insert(int index, object value)
        {
            if (!(value is TOuter))
                throw new ArgumentException("It isn't possible to insert object which is not " + typeof (TOuter).FullName, "value");

            Insert(index, (TOuter) value);
        }

        public void Remove(object value)
        {
            if (!(value is TOuter))
                return;

            Remove((TOuter) value);
        }

        void IList.RemoveAt(int index)
        {
            var innerCount = ((ICollection) _innerSet).Count;
            var list = index < innerCount ? _innerSet : (IList) _extendedSet;
            var i = index < innerCount ? index : index - innerCount;
            list.RemoveAt(i);
        }

        object IList.this[int index]
        {
            get { return ((IList<TOuter>) this)[index]; }
            set
            {
                if (value == null)
                {
                    ((IList<TOuter>) this)[index] = default(TOuter);
                    return;
                }

                if (!(value is TOuter))
                    throw new ArgumentException("It isn't possible to insert object which is not " + typeof (TOuter).FullName, "value");

                ((IList<TOuter>) this)[index] = (TOuter) value;
            }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        void ICollection<TOuter>.Clear()
        {
            ((IList) this).Clear();
        }

        public bool Contains(TOuter item)
        {
            if (item == null)
                return false;

            return _extendedSet.Contains(item) || _innerSet.Contains(_transformIn(item));
        }

        public void CopyTo(TOuter[] array, int arrayIndex)
        {
            var aIndex = 0;
            for (var i = arrayIndex; i < ((ICollection) this).Count; i++)
                array[aIndex++] = this[i];
        }

        public bool Remove(TOuter item)
        {
            if (item == null)
                return false;

            return _extendedSet.Remove(item) || _innerSet.Remove(_transformIn(item));
        }

        public void CopyTo(Array array, int index)
        {
            var aIndex = 0;
            for (var i = index; i < ((ICollection) this).Count; i++)
                array.SetValue(this[i], aIndex++);
        }

        int ICollection.Count
        {
            get { return ((ICollection) _extendedSet).Count + ((ICollection) _innerSet).Count; }
        }

        public object SyncRoot
        {
            get { return _innerSet.SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return _innerSet.IsSynchronized && _extendedSet.IsSynchronized; }
        }

        int ICollection<TOuter>.Count
        {
            get { return ((ICollection) this).Count; }
        }

        bool ICollection<TOuter>.IsReadOnly
        {
            get { return false; }
        }

        public int IndexOf(TOuter item)
        {
            var index = _innerSet.IndexOf(_transformIn(item));
            if (index < 0)
                index = _extendedSet.IndexOf(item);
            return index;
        }

        public void Insert(int index, TOuter item)
        {
            var innerCount = ((ICollection) _innerSet).Count;
            var isInner = index <= innerCount;
            if (isInner)
            {
                var inner = _transformIn(item);
                _innerSet.Insert(index, inner);
                return;
            }

            var i = index - innerCount;
            _extendedSet.Insert(i, item);
        }

        void IList<TOuter>.RemoveAt(int index)
        {
            ((IList) this).RemoveAt(index);
        }

        public TOuter this[int index]
        {
            get
            {
                var innerCount = ((ICollection) _innerSet).Count;
                if (index < innerCount)
                {
                    var item = ((IList<TInner>) _innerSet)[index];
                    return _transformOut(item);
                }

                return ((IList<TOuter>) _extendedSet)[index - innerCount];
            }
            set
            {
                var innerCount = ((ICollection) _innerSet).Count;
                if (index < innerCount)
                {
                    ((IList<TInner>) _innerSet)[index] = _transformIn(value);
                    return;
                }

                ((IList<TOuter>) _extendedSet)[index - innerCount] = value;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public IPersistEntity OwningEntity
        {
            get { return _innerSet.OwningEntity; }
        }

        public void AddRange(IEnumerable<TOuter> values)
        {
            foreach (var value in values)
            {
                if (value == null)
                {
                    _innerSet.Add(null);
                    continue;
                }
                var inner = _transformIn(value);
                if (inner != null)
                {
                    _innerSet.Add(inner);
                    continue;
                }

                _extendedSet.Add(value);
            }
        }

        public TOuter First
        {
            get { return _innerSet.Any() ? _transformOut(_innerSet.First) : _extendedSet.First; }
        }

        public TOuter FirstOrDefault()
        {
            var inner = _innerSet.FirstOrDefault();
            if (inner != null)
                return _transformOut(inner);
            return _extendedSet.FirstOrDefault();
        }

        public TOuter FirstOrDefault(Func<TOuter, bool> predicate)
        {
            var inner = Transformed.FirstOrDefault(predicate);
            if (inner != null)
                return inner;
            return _extendedSet.FirstOrDefault(predicate);
        }

        public TF FirstOrDefault<TF>(Func<TF, bool> predicate)
        {
            var inner = Transformed.OfType<TF>().FirstOrDefault(predicate);
            if (inner != null)
                return inner;
            return _extendedSet.FirstOrDefault(predicate);
        }

        public IEnumerable<TW> Where<TW>(Func<TW, bool> predicate)
        {
            return Transformed.OfType<TW>().Where(predicate).Concat(_extendedSet.Where(predicate));
        }

        public IEnumerable<TO> OfType<TO>()
        {
            return _extendedSet.OfType<TO>().Concat(Transformed.OfType<TO>());
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _innerSet.PropertyChanged -= InnerSetOnPropertyChanged;
            _extendedSet.PropertyChanged -= InnerSetOnPropertyChanged;
            _innerSet.CollectionChanged -= InnerSetOnCollectionChanged;
            _extendedSet.CollectionChanged -= ExtendedSetOnCollectionChanged;

            _disposed = true;
        }
    }
}

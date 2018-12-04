using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xbim.Common.Exceptions;

namespace Xbim.Common.Collections
{
    public class ExtendedItemSet<TInner, TOuter> : IItemSet<TOuter>, IList
    {
        private readonly IItemSet<TOuter> _extendedSet;
        private readonly Func<TInner, TOuter> _transformOut;
        private readonly Func<TOuter, TInner> _transformIn;
        private readonly IItemSet<TInner> _innerSet;

        private IList InnerList { get { return _innerSet as IList; } }
        private IList ExtendedList { get { return _extendedSet as IList; } }

        public ExtendedItemSet(
            IItemSet<TInner> innerSet, 
            IItemSet<TOuter> extendedSet, 
            Func<TInner, TOuter> transformOut, 
            Func<TOuter, TInner> transformIn)
        {
            _innerSet = innerSet;
            _extendedSet = extendedSet;

            if(InnerList == null || ExtendedList == null)
                throw new XbimException("Both inner and extended lists must implement IList");

            _transformOut = transformOut;
            _transformIn = transformIn;
        }

        private IEnumerable<TOuter> Transformed
        {
            get { return _innerSet.Select(inner => _transformOut(inner)); }
        }


        public IEnumerator<TOuter> GetEnumerator()
        {
            return Transformed.Concat(_extendedSet).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TOuter item)
        {
            if (item == null)
                return;

            if (_extendedSet.Any())
            {
                _extendedSet.Add(item);
                return;
            }

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
                return InnerList.Add(inner);

            var index = ExtendedList.Add(item);
            return _innerSet.Count + index;
        }

        public bool Contains(object value)
        {
            if (!(value is TOuter))
                return false;

            return _innerSet.Contains(_transformIn((TOuter) value)) || ExtendedList.Contains(value);
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
            var list = index < innerCount ? InnerList : ExtendedList;
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

            return 
                _extendedSet.Remove(item) || 
                _innerSet.Remove(_transformIn(item));
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
            get { return InnerList.SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return InnerList.IsSynchronized && ExtendedList.IsSynchronized; }
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
                    var item = _innerSet[index];
                    return _transformOut(item);
                }

                return _extendedSet[index - innerCount];
            }
            set
            {
                var innerCount = ((ICollection) _innerSet).Count;
                if (index < innerCount)
                {
                    _innerSet[index] = _transformIn(value);
                    return;
                }

                _extendedSet[index - innerCount] = value;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                _innerSet.CollectionChanged += value;
                _extendedSet.CollectionChanged += value;
            }
            remove
            {
                _innerSet.CollectionChanged -= value;
                _extendedSet.CollectionChanged -= value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _innerSet.PropertyChanged += value;
                _extendedSet.PropertyChanged += value;
            }
            remove
            {
                _innerSet.PropertyChanged -= value;
                _extendedSet.PropertyChanged -= value;
            }
        }

        public IPersistEntity OwningEntity
        {
            get { return _innerSet.OwningEntity; }
        }

        public TOuter GetAt(int index)
        {
            return _transformOut(_innerSet.GetAt(index));
        }

        public void AddRange(IEnumerable<TOuter> values)
        {
            foreach (var value in values)
            {
                if (value == null)
                {
                    InnerList.Add(null);
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

        public TOuter FirstOrDefault(Func<TOuter, bool> predicate)
        {
            return Enumerable.FirstOrDefault(this, predicate);
        }

        public TF FirstOrDefault<TF>(Func<TF, bool> predicate) where TF : TOuter
        {
            return this.OfType<TF>().FirstOrDefault(predicate);
        }

        public IEnumerable<TW> Where<TW>(Func<TW, bool> predicate) where TW : TOuter
        {
            return this.OfType<TW>().Where(predicate);
        }
    }
}

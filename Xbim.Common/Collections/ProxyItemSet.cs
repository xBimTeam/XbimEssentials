using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xbim.Common.Exceptions;

namespace Xbim.Common.Collections
{
    public class ProxyItemSet<TInner, TOuter> : IItemSet<TOuter>, IList where TInner : TOuter
    {
        private readonly IItemSet<TInner> _inner;
        private IList List { get { return _inner as IList; } }

        public ProxyItemSet(IItemSet<TInner> inner)
        {
            _inner = inner;
            if(List == null)
                throw new XbimException("Inner list has to implement IList");
        }

        public IEnumerator<TOuter> GetEnumerator()
        {
            return new ProxyEnumerator<TInner,TOuter>(_inner.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TOuter item)
        {
            _inner.Add(GetIn(item));
        }

        public int Add(object value)
        {
            return List.Add(value);
        }

        public bool Contains(object value)
        {
            return List.Contains(value);
        }

        void IList.Clear()
        {
            _inner.Clear();
        }

        public int IndexOf(object value)
        {
            return List.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            List.Insert(index, value);
        }

        public void Remove(object value)
        {
            List.Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            List.RemoveAt(index);
        }

        object IList.this[int index]
        {
            get { return List[index]; }
            set { List[index] = value; }
        }

        bool IList.IsReadOnly
        {
            get { return List.IsReadOnly; }
        }

        public bool IsFixedSize
        {
            get { return List.IsFixedSize; }
        }

        void ICollection<TOuter>.Clear()
        {
            _inner.Clear();
        }

        public bool Contains(TOuter item)
        {
            return _inner.Contains(GetIn(item));
        }

        public void CopyTo(TOuter[] array, int arrayIndex)
        {
            var result = new TInner[array.Length];
            _inner.CopyTo(result, arrayIndex);
            for (var i = 0; i < array.Length; i++)
                array[i] = result[i];
        }

        public bool Remove(TOuter item)
        {
            Check(item);
            return _inner.Remove(GetIn(item));
        }

        public void CopyTo(Array array, int index)
        {
            List.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return ((ICollection) _inner).Count; }
        }

        public object SyncRoot
        {
            get { return List.SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return List.IsSynchronized; }
        }

        int ICollection<TOuter>.Count
        {
            get { return ((ICollection)_inner).Count; }
        }

        bool ICollection<TOuter>.IsReadOnly
        {
            get { return List.IsReadOnly; }
        }

        public int IndexOf(TOuter item)
        {
            return _inner.IndexOf(GetIn(item));
        }

        public void Insert(int index, TOuter item)
        {
            _inner.Insert(index, GetIn(item));
        }

        void IList<TOuter>.RemoveAt(int index)
        {
            _inner.RemoveAt(index);
        }

        public TOuter this[int index]
        {
            get { return _inner[index]; }
            set
            {
                _inner[index] = GetIn(value);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                _inner.CollectionChanged += value;
            }
            remove
            {
                _inner.CollectionChanged -= value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _inner.PropertyChanged+= value;
            }
            remove
            {
                _inner.PropertyChanged -= value;
            }
        }

        public IPersistEntity OwningEntity
        {
            get { return _inner.OwningEntity; }
        }

        public TOuter GetAt(int index)
        {
            return _inner.GetAt(index);
        }

        public void AddRange(IEnumerable<TOuter> values)
        {
            _inner.AddRange(values.Cast<TInner>());
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

        // ReSharper disable once UnusedParameter.Local
        private static void Check(TOuter item)
        {
            if (item != null && !(item is TInner))
                throw new XbimException("Invalid type for underlying collection");
        }

        private static TInner GetIn(TOuter outer)
        {
            if (outer == null)
                return default(TInner);
            Check(outer);
            return (TInner) outer;
        }
    }

    internal class ProxyEnumerator<TInner, TOuter> : IEnumerator<TOuter> where TInner : TOuter
    {
        private readonly IEnumerator<TInner> _inner;

        public ProxyEnumerator(IEnumerator<TInner> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public bool MoveNext()
        {
            return _inner.MoveNext();
        }

        public void Reset()
        {
            _inner.Reset();
        }

        public TOuter Current
        {
            get { return _inner.Current; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}

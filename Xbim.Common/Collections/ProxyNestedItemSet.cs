using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xbim.Common.Exceptions;

namespace Xbim.Common.Collections
{
    public class ProxyNestedItemSet<TInner, TOuter> : IItemSet<IItemSet<TOuter>>, IList where TInner:TOuter
    {
        private readonly IItemSet<IItemSet<TInner>> _inner;
        private IList List { get { return _inner as IList;} }

        public ProxyNestedItemSet(IItemSet<IItemSet<TInner>> inner)
        {
            _inner = inner;
            if(List == null)
                throw  new XbimException("Inner list must implement IList");
        }

       

        public IEnumerator<IItemSet<TOuter>> GetEnumerator()
        {
            return new ProxyNestedEnumerator<TInner, TOuter>(_inner.GetEnumerator());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IItemSet<TOuter> item)
        {
            _inner.Add(GetIn(item));
        }

        public int Add(object value)
        {
            var item = value as IItemSet<TOuter>;
            return List.Add(item);
        }

        public bool Contains(object value)
        {
            return _inner.Contains(value);
        }

        void IList.Clear()
        {
            ((IList)_inner).Clear();
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
            ((IList)_inner).RemoveAt(index);
        }

        object IList.this[int index]
        {
            get { return ((IList)_inner)[index]; }
            set { ((IList)_inner)[index] = value; }
        }

        bool IList.IsReadOnly
        {
            get { return ((IList)_inner).IsReadOnly; }
        }

        public bool IsFixedSize
        {
            get { return ((IList)_inner).IsFixedSize; }
        }

        void ICollection<IItemSet<TOuter>>.Clear()
        {
            _inner.Clear();
        }

        public bool Contains(IItemSet<TOuter> item)
        {
            return _inner.Contains(GetIn(item));
        }

        public void CopyTo(IItemSet<TOuter>[] array, int arrayIndex)
        {
            var result = new IItemSet<TInner>[array.Length];
            _inner.CopyTo(result, arrayIndex);
            for (var i = 0; i < array.Length; i++)
                array[i] = new ProxyItemSet<TInner, TOuter>(result[i]);
        }

        public bool Remove(IItemSet<TOuter> item)
        {
            Check(item);
            return _inner.Remove(GetIn(item));
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_inner).CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return ((ICollection)_inner).Count; }
        }

        public object SyncRoot
        {
            get { return ((ICollection)_inner).SyncRoot; }
        }

        public bool IsSynchronized
        {
            get { return ((ICollection)_inner).IsSynchronized; }
        }

        int ICollection<IItemSet<TOuter>>.Count
        {
            get { return ((ICollection)_inner).Count; }
        }

        bool ICollection<IItemSet<TOuter>>.IsReadOnly
        {
            get { return ((IList)_inner).IsReadOnly; }
        }

        public int IndexOf(IItemSet<TOuter> item)
        {
            return _inner.IndexOf(GetIn(item));
        }

        public void Insert(int index, IItemSet<TOuter> item)
        {
            _inner.Insert(index, GetIn(item));
        }

        void IList<IItemSet<TOuter>>.RemoveAt(int index)
        {
            _inner.RemoveAt(index);
        }

        public IItemSet<TOuter> this[int index]
        {
            get { return new ProxyItemSet<TInner, TOuter>(_inner[index]); }
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
                _inner.PropertyChanged += value;
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


        public IItemSet<TOuter> GetAt(int index)
        {
            return GetOut(_inner.GetAt(index));
        }

        public void AddRange(IEnumerable<IItemSet<TOuter>> values)
        {
            _inner.AddRange(values.Cast<IItemSet<TInner>>());
        }

 

        // ReSharper disable once UnusedParameter.Local
        private static void Check(IItemSet<TOuter> items)
        {
            if (!(items.All(i => i is TInner)))
                throw new XbimException("Invalid type for underlying collection");
        }

        private static IItemSet<TInner> GetIn(IItemSet<TOuter> outer)
        {
            if (outer == null)
                throw new XbimException("Invalid underlying collection");
            Check(outer);
            return (IItemSet<TInner>)outer;
        }

        private static IItemSet<TOuter> GetOut(IItemSet<TInner> inner)
        {
            return new ProxyItemSet<TInner,TOuter>(inner);
        }

        public IItemSet<TOuter> FirstOrDefault(Func<IItemSet<TOuter>, bool> predicate)
        {
            return Enumerable.FirstOrDefault(this, predicate);
        }

        public TF FirstOrDefault<TF>(Func<TF, bool> predicate) where TF : IItemSet<TOuter>
        {
            return this.OfType<TF>().FirstOrDefault(predicate);
        }

        public IEnumerable<TW> Where<TW>(Func<TW, bool> predicate) where TW : IItemSet<TOuter>
        {
            return this.OfType<TW>().Where(predicate);
        }
    }

    internal class ProxyNestedEnumerator<TInner, TOuter> : IEnumerator<IItemSet<TOuter>> where TInner : TOuter
    {
        private readonly IEnumerator<IItemSet<TInner>> _inner;

        public ProxyNestedEnumerator(IEnumerator<IItemSet<TInner>> inner)
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

        public IItemSet<TOuter> Current
        {
            get { return new ProxyItemSet<TInner, TOuter>(_inner.Current); }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xbim.Common.Exceptions;

namespace Xbim.Common.Collections
{
    public class ProxyNestedValueSet<TInner, TOuter> : IItemSet<IItemSet<TOuter>>, IList
    {
        private readonly IItemSet<IItemSet<TInner>> _inner;
        private readonly Func<TInner, TOuter> _toOut;
        private readonly Func<TOuter, TInner> _toIn;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private IList List { get { return _inner as IList; } }

        public int Count => _inner.Count;

        public bool IsReadOnly => false;

        public IPersistEntity OwningEntity => _inner.OwningEntity;

        public bool IsFixedSize => false;

        public object SyncRoot => _inner;

        public bool IsSynchronized => true;

        object IList.this[int index] { get => this[index]; set => this[index] = value as IItemSet<TOuter>; }

        public IItemSet<TOuter> this[int index] {
            get
            {
                var iset = _inner[index];
                return new ProxyValueSet<TInner, TOuter>(iset, _toOut, _toIn);
            }
            set
            {
                if (value == null)
                {
                    _inner[index] = null;
                    return;
                }

                var iset = _inner.GetAt(index);
                iset.AddRange(value.Select(v => _toIn(v)));
            }
        }

        public ProxyNestedValueSet(IItemSet<IItemSet<TInner>> inner, Func<TInner, TOuter> toOut, Func<TOuter, TInner> toIn)
        {
            _inner = inner;
            _toOut = toOut;
            _toIn = toIn;

            if (List == null)
                throw new XbimException("Inner list must implement IList");

            // forward notifications
            _inner.CollectionChanged += (s, a) =>
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(a.Action));
            };
            _inner.PropertyChanged += (s, a) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(a.PropertyName));
            };
        }

        public IItemSet<TOuter> GetAt(int index)
        {
            var iset = _inner.GetAt(index);
            return new ProxyValueSet<TInner, TOuter>(iset, _toOut, _toIn);
        }

        public void AddRange(IEnumerable<IItemSet<TOuter>> values)
        {
            var idx = _inner.Count;
            foreach (var set in values)
            {
                _inner.GetAt(idx).AddRange(set.Select(v => _toIn(v)));
                idx++;
            }
        }

        public IItemSet<TOuter> FirstOrDefault(Func<IItemSet<TOuter>, bool> predicate)
        {
            var set = _inner.FirstOrDefault(i => predicate(new ProxyValueSet<TInner, TOuter>(i, _toOut, _toIn)));
            if (set == null)
                return null;
            return new ProxyValueSet<TInner, TOuter>(set, _toOut, _toIn);
        }

        TF IItemSet<IItemSet<TOuter>>.FirstOrDefault<TF>(Func<TF, bool> predicate)
        {
            return _inner
                .Select(i => new ProxyValueSet<TInner, TOuter>(i, _toOut, _toIn))
                .OfType<TF>()
                .FirstOrDefault(predicate);
        }

        public IEnumerable<TW> Where<TW>(Func<TW, bool> predicate) where TW : IItemSet<TOuter>
        {
            return _inner
                .Select(i => new ProxyValueSet<TInner, TOuter>(i, _toOut, _toIn))
                .OfType<TW>()
                .Where(predicate);
        }

        public int IndexOf(IItemSet<TOuter> item)
        {
            var inner = item.Select(i => _toIn(i)).ToList();
            var find = _inner.FirstOrDefault(i => inner.SequenceEqual(i));
            if (find == null)
                return -1;
            return _inner.IndexOf(find);
        }

        public void Insert(int index, IItemSet<TOuter> item)
        {
            this[index] = item;
        }

        public void RemoveAt(int index)
        {
            _inner.RemoveAt(index);
        }

        public void Add(IItemSet<TOuter> item)
        {
            _inner.GetAt(_inner.Count).AddRange(item.Select(i => _toIn(i)));
        }

        public void Clear()
        {
            _inner.Clear();
        }

        public bool Contains(IItemSet<TOuter> item)
        {
            return IndexOf(item) > -1;
        }

        public void CopyTo(IItemSet<TOuter>[] array, int arrayIndex)
        {
            for (int i = 0; i < _inner.Count; i++)
            {
                array[arrayIndex + i] = new ProxyValueSet<TInner, TOuter>(_inner[i], _toOut, _toIn);
            }
        }

        public bool Remove(IItemSet<TOuter> item)
        {
            var idx = IndexOf(item);
            if (idx < 0)
                return false;
            
            RemoveAt(idx);
            return true;
        }

        public IEnumerator<IItemSet<TOuter>> GetEnumerator()
        {
            return new ProxyEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Add(object value)
        {
            if (!(value is IItemSet<TOuter> set))
                throw new ArgumentException("Invalid type", nameof(value));

            Add(set);
            return Count - 1;
        }

        public bool Contains(object value)
        {
            if (!(value is IItemSet<TOuter> set))
                throw new ArgumentException("Invalid type", nameof(value));

            return Contains(set);
        }

        public int IndexOf(object value)
        {
            if (!(value is IItemSet<TOuter> set))
                throw new ArgumentException("Invalid type", nameof(value));

            return IndexOf(set);
        }

        public void Insert(int index, object value)
        {
            if (!(value is IItemSet<TOuter> set))
                throw new ArgumentException("Invalid type", nameof(value));

            Insert(index, set);
        }

        public void Remove(object value)
        {
            if (!(value is IItemSet<TOuter> set))
                throw new ArgumentException("Invalid type", nameof(value));

            Remove(set);
        }

        public void CopyTo(Array array, int index)
        {
            for (int i = 0; i < _inner.Count; i++)
                array.SetValue(new ProxyValueSet<TInner, TOuter>(_inner[i], _toOut, _toIn), index + i);
        }

        private class ProxyEnumerator : IEnumerator<IItemSet<TOuter>>
        {
            private ProxyNestedValueSet<TInner, TOuter> inner;
            private IItemSet<TOuter> current;
            private int currentIdx = -1;

            public ProxyEnumerator(ProxyNestedValueSet<TInner, TOuter> inner)
            {
                this.inner = inner;
            }

            public IItemSet<TOuter> Current => current;

            object IEnumerator.Current => current;

            public void Dispose()
            {
                inner = null;
            }

            public bool MoveNext()
            {
                currentIdx++;
                if (currentIdx > (inner.Count -1))
                    return false;

                current = inner[currentIdx];
                return true;
            }

            public void Reset()
            {
                currentIdx = -1;
            }
        }
    }
}

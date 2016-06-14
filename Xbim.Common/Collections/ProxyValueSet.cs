using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xbim.Common.Exceptions;

namespace Xbim.Common.Collections
{
    public class ProxyValueSet<TInner, TOuter> : IItemSet<TOuter>, IDisposable, IList 
        where TInner : struct, IExpressValueType
        where TOuter : struct, IExpressValueType
    {
        private readonly IItemSet<TInner> _inner;
        private readonly Func<TInner, TOuter> _toOut;
        private readonly Func<TOuter, TInner> _toIn;
        private IList List { get { return _inner as IList; } }



        public ProxyValueSet(IItemSet<TInner> inner, Func<TInner, TOuter> toOut, Func<TOuter, TInner> toIn)
        {
            _inner = inner;
            _toOut = toOut;
            _toIn = toIn;
            if(List == null)
                throw new XbimException("Inner list has to implement IList");

            _inner.PropertyChanged += InnerOnPropertyChanged;
            _inner.CollectionChanged += InnerOnCollectionChanged;
        }

        private void InnerOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            OnCollectionChanged(notifyCollectionChangedEventArgs);
        }

        private void InnerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged(propertyChangedEventArgs.PropertyName);
        }

        public IEnumerator<TOuter> GetEnumerator()
        {
            return new ProxyEnumerator<TInner,TOuter>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TOuter item)
        {
            _inner.Add(_toIn(item));
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
            get { return ((IList) _inner)[index]; }
            set { ((IList) _inner)[index] = value; }
        }

        bool IList.IsReadOnly
        {
            get { return ((IList) _inner).IsReadOnly; }
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
            return _inner.Contains(_toIn(item));
        }

        public void CopyTo(TOuter[] array, int arrayIndex)
        {
            var result = new TInner[array.Length];
            _inner.CopyTo(result, arrayIndex);
            for (var i = 0; i < array.Length; i++)
                array[i] = _toOut(result[i]);
        }

        public bool Remove(TOuter item)
        {
            return _inner.Remove(_toIn(item));
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
            get { return ((IList)_inner).IsReadOnly; }
        }

        public int IndexOf(TOuter item)
        {
            return _inner.IndexOf(_toIn(item));
        }

        public void Insert(int index, TOuter item)
        {
            _inner.Insert(index, _toIn(item));
        }

        void IList<TOuter>.RemoveAt(int index)
        {
            _inner.RemoveAt(index);
        }

        public TOuter this[int index]
        {
            get { return _toOut(_inner[index]); }
            set
            {
                _inner[index] = _toIn(value);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public IPersistEntity OwningEntity
        {
            get { return _inner.OwningEntity; }
        }

        public void AddRange(IEnumerable<TOuter> values)
        {
            _inner.AddRange(values.Cast<TInner>());
        }

        public TOuter First
        {
            get { return _toOut(_inner.First); }
        }

        public TOuter FirstOrDefault()
        {
            return _inner.Count == 0 ? new TOuter() : _toOut(_inner.FirstOrDefault());
        }

        public TOuter FirstOrDefault(Func<TOuter, bool> predicate)
        {
            return _inner.FirstOrDefault(predicate);
        }

        public TF FirstOrDefault<TF>(Func<TF, bool> predicate)
        {
            return _inner.FirstOrDefault(predicate);
        }

        public IEnumerable<TW> Where<TW>(Func<TW, bool> predicate)
        {
            return _inner.Where(predicate);
        }

        public IEnumerable<TO> OfType<TO>()
        {
            return _inner.OfType<TO>();
        }

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _inner.PropertyChanged -= InnerOnPropertyChanged;
            _inner.CollectionChanged -= InnerOnCollectionChanged;
            _disposed = true;
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

        private class ProxyEnumerator<TIn, TOut> : IEnumerator<TOut>
            where TIn : struct, IExpressValueType
            where TOut : struct, IExpressValueType

        {
            private readonly ProxyValueSet<TIn, TOut> _proxy;
            private readonly IEnumerator<TIn> _inner;

            public ProxyEnumerator(ProxyValueSet<TIn, TOut> proxy)
            {
                _proxy = proxy;
                _inner = proxy._inner.GetEnumerator();
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

            public TOut Current
            {
                get { return _proxy._toOut(_inner.Current); }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }

    }

    
}

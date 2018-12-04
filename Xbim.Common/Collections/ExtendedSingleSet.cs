using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Xbim.Common.Collections
{
    public class ExtendedSingleSet<TInner, TOuter> : IItemSet<TOuter>
    {
        private TInner Inner
        {
            get
            {
                return _getter();
            }
            set
            {
                var old = Inner;
                _setter(value);
                if (_collectionChanged != null)
                {
                    if (old != null)
                        _collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, old));
                    if (value != null)
                        _collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
                }
                if (_propertyChanged != null)
                {
                    if (old == null && value != null)
                        _propertyChanged(this, new PropertyChangedEventArgs("Count"));
                    if (old != null && value == null)
                        _propertyChanged(this, new PropertyChangedEventArgs("Count"));
                }

            }
        }

        private TOuter InnerOut
        {
            get
            {
                return Inner == null ? default(TOuter) : _toOut(Inner);
            }
            set
            {
                if (value == null)
                    Inner = default(TInner);
                Inner = _toIn(value);
            }
        }

        private IEnumerable<TOuter> InnerSet { get { return new[] { _toOut(_getter()) }; } }
        private readonly Func<TInner> _getter;
        private readonly Action<TInner> _setter;
        private readonly IItemSet<TOuter> _extended;
        private readonly Func<TInner, TOuter> _toOut;
        private readonly Func<TOuter, TInner> _toIn;
        private int Increment { get { return Inner == null ? 0 : 1; } }

        public ExtendedSingleSet(Func<TInner> getter, Action<TInner> setter, IItemSet<TOuter> extended,
            Func<TInner, TOuter> toOut, Func<TOuter, TInner> toIn)
        {
            _getter = getter;
            _setter = setter;
            _extended = extended;
            _toOut = toOut;
            _toIn = toIn;
        }

        public IEnumerator<TOuter> GetEnumerator()
        {
            return Inner == null ?
                _extended.GetEnumerator() :
                _extended.Concat(InnerSet).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TOuter item)
        {
            if (item == null)
                return;

            var inner = _toIn(item);
            if (Inner == null && inner != null)
            {
                Inner = inner;
                return;
            }

            _extended.Add(item);
        }

        public void Clear()
        {
            Inner = default(TInner);
            _extended.Clear();
        }

        public bool Contains(TOuter item)
        {
            if (item == null)
                return false;

            if (item.GetType().GetTypeInfo().IsClass)
            {
                return item.Equals(Inner) || _extended.Contains(item);
            }

            var inner = _toIn(item);
            return inner.Equals(Inner) || _extended.Contains(item);
        }

        public void CopyTo(TOuter[] array, int arrayIndex)
        {
            this.ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(TOuter item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _extended.Count + Increment; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int IndexOf(TOuter item)
        {
            if (item.Equals(InnerOut))
                return 0;
            return _extended.IndexOf(item) + Increment;
        }

        public void Insert(int index, TOuter item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public TOuter this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        private event NotifyCollectionChangedEventHandler _collectionChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                _extended.CollectionChanged += value;
                _collectionChanged += value;
            }
            remove
            {
                _extended.CollectionChanged -= value;
                _collectionChanged -= value;
            }
        }

        private event PropertyChangedEventHandler _propertyChanged;
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                _extended.PropertyChanged += value;
                _propertyChanged += value;
            }
            remove
            {
                _extended.PropertyChanged -= value;
                _propertyChanged -= value;
            }
        }

        public IPersistEntity OwningEntity
        {
            get { return _extended.OwningEntity; }
        }

        public TOuter GetAt(int index)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<TOuter> values)
        {
            throw new NotImplementedException();
        }

        public TOuter FirstOrDefault(Func<TOuter, bool> predicate)
        {
            return Enumerable.FirstOrDefault(this, predicate);
        }

        public TF FirstOrDefault<TF>(Func<TF, bool> predicate) where TF : TOuter
        {
            return Enumerable.OfType<TF>(this).FirstOrDefault(predicate);
        }

        public IEnumerable<TW> Where<TW>(Func<TW, bool> predicate) where TW : TOuter
        {
            return Enumerable.OfType<TW>(this).Where(predicate);
        }
    }
}

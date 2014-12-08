#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    XbimIndexedCollection.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

#endregion

namespace Xbim.XbimExtensions
{
    [Serializable]
    internal class XbimSecondaryIndex<TValue> : KeyedCollection<object, List<TValue>>
    {
        [Serializable]
        private struct NullKey
        {
        }

        private readonly PropertyInfo _indexProp;

        [NonSerialized] private static NullKey nullValue;

        [NonSerialized] private LateBoundMethod _propertyGetterCallback;

        internal LateBoundMethod PropertyGetterCallback
        {
            get
            {
                if (_propertyGetterCallback == null)
                    _propertyGetterCallback = GetPropertyGetterCallback(_indexProp);
                return _propertyGetterCallback;
            }
            set { _propertyGetterCallback = value; }
        }

        [OnDeserialized]
        internal void OnSerializedMethod(StreamingContext context)
        {
            if (_indexProp != null)
                _propertyGetterCallback = GetPropertyGetterCallback(_indexProp);
        }


        static XbimSecondaryIndex()
        {
            nullValue = new NullKey();
        }


        protected override object GetKeyForItem(List<TValue> item)
        {
            object key = _propertyGetterCallback(item.First(), null);
            if (key == null) key = nullValue;
            return key;
        }

        public string IndexedPropertyName
        {
            get { return _indexProp.Name; }
        }

        public XbimSecondaryIndex(PropertyInfo secondaryIndex)
        {
            _indexProp = secondaryIndex;
            _propertyGetterCallback = GetPropertyGetterCallback(_indexProp);
        }

        private LateBoundMethod GetPropertyGetterCallback(PropertyInfo property)
        {
            MethodInfo method = property.DeclaringType.GetMethod("get_" + property.Name, Type.EmptyTypes);
            LateBoundMethod callback = DelegateFactory.Create(method);
            return callback;
        }

        public void Add(TValue item)
        {
            object key = _propertyGetterCallback(item, null);
            if (key == null) key = nullValue;
            if (key is IEnumerable)
                foreach (object collItem in (IEnumerable) key)
                {
                    if (!Contains(key))
                    {
                        List<TValue> instances = new List<TValue>();
                        instances.Add(item);
                        Add(instances);
                    }
                    else
                        this[key].Add(item);
                }
            else
            {
                if (!Contains(key))
                {
                    List<TValue> instances = new List<TValue>();
                    instances.Add(item);
                    Add(instances);
                }
                else
                    this[key].Add(item);
            }
        }


        public new bool Remove(object item)
        {
            object key = _propertyGetterCallback(item, null);
            if (key == null) key = nullValue;
            bool ok = true;
            if (key is IEnumerable)
                foreach (object collItem in (IEnumerable) key)
                {
                    List<TValue> instances = this[collItem];
                    if (instances.Count == 1) //if left with empty collection delete whole entry
                    {
                        if (ok) ok = base.Remove(key);
                    }
                    else if (ok) ok = instances.Remove((TValue) item);
                }

            else
            {
                List<TValue> instances = this[key];
                if (instances.Count == 1) //if left with empty collection delete whole entry
                {
                    if (ok) ok = base.Remove(key);
                }
                else if (ok) ok = instances.Remove((TValue) item);
            }
            return ok;
        }
    }

    [Serializable]
    public class XbimIndexedCollection<TValue> : ICollection<TValue>
    {
        #region Fields

        private readonly Dictionary<object, TValue> _primaryIndex;
        private readonly List<TValue> _naturalIndex;

        private readonly PropertyInfo _primaryIndexProp;
        private readonly List<XbimSecondaryIndex<TValue>> _secondaryIndices;
        [NonSerialized] private LateBoundMethod _propertyGetterCallback;

        #endregion

        #region Index Related

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (_primaryIndexProp != null)
                _propertyGetterCallback = GetPropertyGetterCallback(_primaryIndexProp);
        }


        /// <summary>
        ///   Constructs a collection which is indexed by the the list of indices and has no primary index
        /// </summary>
        /// <param name = "indices"> List of properties to index</param>
        public XbimIndexedCollection(List<PropertyInfo> indices)
            : this(null, indices)
        {
        }

        public XbimIndexedCollection(List<PropertyInfo> indices, IEnumerable<TValue> collection)
            : this(null, indices)
        {
            foreach (TValue item in collection)
                Add(item);
        }

        public XbimIndexedCollection(PropertyInfo primaryIndex, List<PropertyInfo> secondaryIndices)
            : this(primaryIndex)
        {
            if (secondaryIndices != null && secondaryIndices.Count > 0)
            {
                _secondaryIndices = new List<XbimSecondaryIndex<TValue>>();
                foreach (PropertyInfo item in secondaryIndices)
                {
                    _secondaryIndices.Add(new XbimSecondaryIndex<TValue>(item));
                }
            }
        }

        public XbimIndexedCollection(PropertyInfo primaryIndex, PropertyInfo secondaryIndex)
            : this(primaryIndex)
        {
            if (secondaryIndex != null)
            {
                _secondaryIndices = new List<XbimSecondaryIndex<TValue>>();
                _secondaryIndices.Add(new XbimSecondaryIndex<TValue>(secondaryIndex));
            }
        }

        public XbimIndexedCollection(PropertyInfo primaryIndex, IEnumerable<TValue> collection)
            : this(primaryIndex)
        {
            foreach (TValue item in collection)
                Add(item);
        }

        public XbimIndexedCollection(PropertyInfo primaryIndex)

        {
            _primaryIndexProp = primaryIndex;

            if (_primaryIndexProp == null)
                _naturalIndex = new List<TValue>();
            else
            {
                _propertyGetterCallback = GetPropertyGetterCallback(_primaryIndexProp);
                _primaryIndex = new Dictionary<object, TValue>();
            }
        }


        public XbimIndexedCollection(PropertyInfo primaryIndex, List<PropertyInfo> secondaryIndices,
                                     IEnumerable<TValue> collection)
            : this(primaryIndex, secondaryIndices)
        {
            foreach (TValue item in collection)
                Add(item);
        }

        public string IndexedPropertyName
        {
            get { return _primaryIndexProp != null ? _primaryIndexProp.Name : null; }
        }

        public bool HasIndex(string indexName)
        {
            if (IndexedPropertyName == indexName && !string.IsNullOrEmpty(indexName)) //we want the primary sort order
                return true;
            if (_secondaryIndices != null)
            {
                foreach (XbimSecondaryIndex<TValue> secondary in _secondaryIndices)
                {
                    if (secondary.IndexedPropertyName == indexName && !string.IsNullOrEmpty(indexName))
                        return true;
                }
            }
            return false;
        }


        /// <summary>
        ///   Returns the values which meet the required key, an excpetion is thrown if the index is invalid, HasIndex returns true if index is valid;
        /// </summary>
        /// <param name = "indexName"></param>
        /// <param name = "hashKey"></param>
        /// <returns></returns>
        public IEnumerable<TValue> GetValues(string indexName, object hashKey)
        {
            if (!HasIndex(indexName) || string.IsNullOrEmpty(indexName))
                throw new ArgumentException("Index does not exist", "indexName");
            if (IndexedPropertyName == indexName) //we want the primary sort order
            {
                TValue result;
                if (_primaryIndex.TryGetValue(hashKey, out result))
                    yield return result;
            }
            else
            {
                foreach (XbimSecondaryIndex<TValue> idx in _secondaryIndices)
                {
                    if (idx.IndexedPropertyName == indexName)
                    {
                        if (idx.Contains(hashKey))
                        {
                            List<TValue> instances = idx[hashKey];
                            foreach (TValue item in instances)
                                yield return item;
                        }
                    }
                }
            }
        }

        #endregion

        #region ICollection<TValue> Members

        public void Add(TValue item)
        {
            if (_primaryIndex == null) //it does not have a primary index
            {
                _naturalIndex.Add(item);
            }
            else //it has a primary index
            {
                object key = _propertyGetterCallback(item, null);
                _primaryIndex.Add(key, item);
            }
            if (_secondaryIndices != null)
            {
                foreach (XbimSecondaryIndex<TValue> secondary in _secondaryIndices)
                {
                    secondary.Add(item);
                }
            }
            INotifyPropertyChanging iNotChanging = item as INotifyPropertyChanging;
            if (iNotChanging != null)
                iNotChanging.PropertyChanging += XbimIndexedCollection_PropertyChanging;
            INotifyPropertyChanged iNotChanged = item as INotifyPropertyChanged;
            if (iNotChanged != null)
                iNotChanged.PropertyChanged += XbimIndexedCollection_PropertyChanged;
        }

        private void XbimIndexedCollection_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (IndexedPropertyName == e.PropertyName) //we want the primary sort order
            {
                object key = _propertyGetterCallback(sender, null);
                _primaryIndex.Remove(key);
            }
            else if (_secondaryIndices != null)
            {
                foreach (XbimSecondaryIndex<TValue> idx in _secondaryIndices)
                {
                    if (idx.IndexedPropertyName == e.PropertyName)
                    {
                        idx.Remove(sender);
                        break;
                    }
                }
            }
        }

        private void XbimIndexedCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IndexedPropertyName == e.PropertyName) //we want the primary sort order
            {
                object key = _propertyGetterCallback(sender, null);
                _primaryIndex.Add(key, (TValue) sender);
            }
            else if (_secondaryIndices != null)
            {
                foreach (XbimSecondaryIndex<TValue> idx in _secondaryIndices)
                {
                    if (idx.IndexedPropertyName == e.PropertyName)
                    {
                        idx.Add((TValue) sender);
                        break;
                    }
                }
            }
        }

        public void Clear()
        {
            if (_primaryIndex == null)
                _naturalIndex.Clear();
            else
                _primaryIndex.Clear();
        }

        public bool Contains(TValue item)
        {
            if (_primaryIndex == null)
                return _naturalIndex.Contains(item);
            else
            {
                object key = _propertyGetterCallback(item, null);
                return _primaryIndex.ContainsKey(key);
            }
        }


        public void CopyTo(TValue[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _primaryIndex == null ? _naturalIndex.Count : _primaryIndex.Count; }
        }

        public bool IsReadOnly
        {
            get
            {
                return _primaryIndex == null
                           ? ((IList) _naturalIndex).IsReadOnly
                           : ((IDictionary) _primaryIndex).IsReadOnly;
            }
        }

        public bool Remove(TValue item)
        {
            if (_primaryIndex == null)
                return _naturalIndex.Remove(item);
            else
                return _primaryIndex.Remove(item);
        }

        #endregion

        #region IEnumerable<TValue> Members

        public IEnumerator<TValue> GetEnumerator()
        {
            if (_primaryIndex == null)
                return _naturalIndex.GetEnumerator();
            else
                return _primaryIndex.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_primaryIndex == null)
                return ((IEnumerable) _naturalIndex).GetEnumerator();
            else
                return ((IEnumerable) _primaryIndex.Values).GetEnumerator();
        }

        #endregion

        private LateBoundMethod GetPropertyGetterCallback(PropertyInfo property)
        {
            MethodInfo method = property.DeclaringType.GetMethod("get_" + property.Name, Type.EmptyTypes);
            LateBoundMethod callback = DelegateFactory.Create(method);
            return callback;
        }
    }

    #region Helper Class

    internal class XbimIndexEnumerator<TValue> : IEnumerator<TValue>
    {
        private TValue _current;
        private readonly Dictionary<object, HashSet<TValue>> _instances;
        private Dictionary<object, HashSet<TValue>>.Enumerator _keyEnumerator;
        private IEnumerator<TValue> _instanceEnumerator;

        public XbimIndexEnumerator(Dictionary<object, HashSet<TValue>> instances)
        {
            _instances = instances;
            Reset();
        }

        #region IEnumerator<ISupportIfcParser> Members

        public object Current
        {
            get { return _current; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        #region IEnumerator Members

        public bool MoveNext()
        {
            if (_instanceEnumerator != null && _instanceEnumerator.MoveNext()) //can we get an instance
            {
                _current = _instanceEnumerator.Current;
                return true;
            }

            while (_keyEnumerator.MoveNext()) //we can get a  collection and see if it has any instances
            {
                _instanceEnumerator = _keyEnumerator.Current.Value.GetEnumerator();
                if (_instanceEnumerator != null && _instanceEnumerator.MoveNext()) //can we get an instance
                {
                    _current = _instanceEnumerator.Current;
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            _current = default(TValue);
            _keyEnumerator = _instances.GetEnumerator();
            _instanceEnumerator = null;
        }

        #endregion

        #region IEnumerator<ISupportIfcParser> Members

        TValue IEnumerator<TValue>.Current
        {
            get { return _current; }
        }

        #endregion
    }

    /*
	 * The DelegateFactory was pulled from this great post by Nate Kohari
	 * http://kohari.org/2009/03/06/fast-late-bound-invocation-with-expression-trees/
	 */

    internal delegate object LateBoundMethod(object target, object[] arguments);

    internal static class DelegateFactory
    {
        public static LateBoundMethod Create(MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof (object), "target");
            ParameterExpression argumentsParameter = Expression.Parameter(typeof (object[]), "arguments");

            MethodCallExpression call = Expression.Call(
                Expression.Convert(instanceParameter, method.DeclaringType),
                method,
                CreateParameterExpressions(method, argumentsParameter));

            Expression<LateBoundMethod> lambda = Expression.Lambda<LateBoundMethod>(
                Expression.Convert(call, typeof (object)),
                instanceParameter,
                argumentsParameter);

            return lambda.Compile();
        }

        private static Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter)
        {
            return method.GetParameters().Select((parameter, index) =>
                                                 Expression.Convert(
                                                     Expression.ArrayIndex(argumentsParameter,
                                                                           Expression.Constant(index)),
                                                     parameter.ParameterType)).ToArray();
        }
    }

    #endregion

    internal static class ExpressionHelper
    {
        public static string GetMemberName<T, TProperty>(this Expression<Func<T, TProperty>> propertyExpression)
        {
            return ((MemberExpression) (((propertyExpression)).Body)).Member.Name;
        }
    }
}
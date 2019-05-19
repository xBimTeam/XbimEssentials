using System.Collections.Concurrent;
using System.Collections.Generic;
using Xbim.Common;

namespace Xbim.Presentation
{
    public class XbimIPersistEntityCollection<TType> : ICollection<TType> where TType : class, IPersistEntity 
    {
        /// <summary>
        /// private dictionary changed from hashset to concurrent dictionay to resolve multi-threaded use of class.
        /// Since there's no concurrentHashSet, we have used a dummy byte value that is never used.
        /// This solution seems the most upvoted in https://stackoverflow.com/questions/18922985/concurrent-hashsett-in-net-framework
        /// but there is a controversy.
        /// The code might change in the future but the dictionary being private should not affect users.
        /// </summary>
        private readonly Dictionary<IModel, ConcurrentDictionary<TType, byte>> _dictionary = new Dictionary<IModel, ConcurrentDictionary<TType, byte>>();

        public void Add(TType item)
        {
            ConcurrentDictionary<TType, byte> found;
            var fnd = _dictionary.TryGetValue(item.Model, out found);
            if (!fnd)
            {
                var dic = new ConcurrentDictionary<TType, byte>();
                dic.TryAdd(item, 0);
                _dictionary.Add(item.Model, dic);
            }
            else
            {
                found.TryAdd(item, 0);
            }
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(TType item)
        {
            ConcurrentDictionary<TType, byte> found;
            return _dictionary.TryGetValue(item.Model, out found) && found.ContainsKey(item);
        }

        public void CopyTo(TType[] array, int arrayIndex) 
        {
            foreach (var dir in _dictionary)
            {
                dir.Value.Keys.CopyTo(array, arrayIndex);
                arrayIndex += dir.Value.Count;
            }
        }

        public int Count
        {
            get
            {
                int tally = 0;
                foreach (var dir in _dictionary.Values)
                {
                    tally += dir.Count;
                }
                return tally;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(TType item)
        {
            ConcurrentDictionary<TType, byte> found;
            var fnd = _dictionary.TryGetValue(item.Model, out found);
            if (!fnd)
                return false;
            byte dummyValue;
            return found.TryRemove(item, out dummyValue);
        }

        public IEnumerator<TType> GetEnumerator()
        {
            return new XbimIPersistIfcEntityCollectionEnumerator(_dictionary);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new XbimIPersistIfcEntityCollectionEnumerator(_dictionary);
        }

        private class XbimIPersistIfcEntityCollectionEnumerator : IEnumerator<TType>
        {
            private readonly List<IEnumerator<TType>> _enumerators;
            public XbimIPersistIfcEntityCollectionEnumerator(Dictionary<IModel, ConcurrentDictionary<TType, byte>> collection)
            {
                _enumerators = new List<IEnumerator<TType>>();
                foreach (var vl in collection.Values)
                {
                    _enumerators.Add(vl.Keys.GetEnumerator());
                }
            }

            public TType Current
            {
                get
                {
                    if (_index == -1 || _index >= _enumerators.Count)
                        return null;
                    return _enumerators[_index].Current;
                }
            }

            public void Dispose()
            {
                foreach (var enumerator in _enumerators)
                {
                   enumerator.Dispose();
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    if (_index == -1 || _index >= _enumerators.Count)
                        return null;
                    return _enumerators[_index].Current;
                }
            }


            private int _index = -1;
            public bool MoveNext()
            {
                if (_index == -1)
                    _index = 0;
                while (_index < _enumerators.Count)
                {
                    var cret = _enumerators[_index].MoveNext();
                    if (cret)
                        return true;
                    _index++;
                }
                return false;
            }

            public void Reset()
            {
                foreach (var en in _enumerators)
                {
                    en.Reset();
                }
                _index = -1;
            }
        }
    }
}

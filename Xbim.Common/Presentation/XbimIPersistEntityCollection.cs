using System.Collections.Generic;
using Xbim.Common;

namespace Xbim.Presentation
{
    public class XbimIPersistEntityCollection<TType> : ICollection<TType> where TType : class, IPersistEntity 
    {
        private readonly Dictionary<IModel, HashSet<TType>> _dictionary = new Dictionary<IModel, HashSet<TType>>();

        public void Add(TType item)
        {
            HashSet<TType> found;
            var fnd = _dictionary.TryGetValue(item.Model, out found);
            if (!fnd)
            {
                _dictionary.Add(item.Model, new HashSet<TType> { item });
            }
            else
            {
                found.Add(item);
            }
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(TType item)
        {
            HashSet<TType> found;
            return _dictionary.TryGetValue(item.Model, out found) && found.Contains(item);
        }

        public void CopyTo(TType[] array, int arrayIndex) 
        {
            foreach (var dir in _dictionary)
            {
                dir.Value.CopyTo(array, arrayIndex);
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
            HashSet<TType> found;
            var fnd = _dictionary.TryGetValue(item.Model, out found);
            if (!fnd)
                return false;
            return found.Remove(item);
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
            public XbimIPersistIfcEntityCollectionEnumerator(Dictionary<IModel, HashSet<TType>> collection)
            {
                _enumerators = new List<IEnumerator<TType>>();
                foreach (var vl in collection.Values)
                {
                    _enumerators.Add(vl.GetEnumerator());
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

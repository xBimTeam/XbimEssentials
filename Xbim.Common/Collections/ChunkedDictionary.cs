using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xbim.Common.Collections
{
    public class ChunkedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private int _currentChunk;
        private readonly List<Dictionary<TKey, TValue>> _chunks = new List<Dictionary<TKey, TValue>>();
        private readonly List<int> _chunkSizes;
        private readonly int _preferredChunkSize;


        public ChunkedDictionary(int chunkSize)
        {
            if (chunkSize <= 0)
                throw new ArgumentException("Chunk size must be greater than 0.", nameof(chunkSize));

            _preferredChunkSize = chunkSize;

            //get list of chunk sizes
            _chunkSizes = new List<int>()
            {
                3
            };
            _currentChunk = 0;
            _chunks.Add(new Dictionary<TKey, TValue>(_chunkSizes[_currentChunk]));
        }

        public ChunkedDictionary(int totalSize, int chunkSize)
        {
            if (totalSize <= 0)
                throw new ArgumentException("Total size must be greater than 0.", nameof(totalSize));

            if (chunkSize <= 0)
                throw new ArgumentException("Chunk size must be greater than 0.", nameof(chunkSize));

            if (chunkSize > totalSize)
                throw new ArgumentException("Chunk size must be less than or equal to the total size.",
                    nameof(chunkSize));

            _preferredChunkSize = chunkSize;

            //get list of actual chunk sizes
            _chunkSizes = new List<int>();
            while (totalSize > 0)
            {
                if (totalSize >= chunkSize)
                {
                    _chunkSizes.Add(chunkSize);
                    totalSize -= chunkSize;
                }
                else
                {
                    _chunkSizes.Add(totalSize);
                    totalSize = 0;
                }
            }

            _currentChunk = 0;
            _chunks.Add(new Dictionary<TKey, TValue>(_chunkSizes[_currentChunk]));
        }

        public TValue this[TKey key]
        {
            get => FindChunkContainingKey(key)[key];
            set
            {
                var chunk = FindChunkContainingKey(key, true);
                chunk[key] = value;
            }
        }

        public ICollection<TKey> Keys => _chunks.SelectMany(chunk => chunk.Keys).ToList();

        public ICollection<TValue> Values => _chunks.SelectMany(chunk => chunk.Values).ToList();

        public int Count => _chunks.Sum(chunk => chunk.Count);

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            var chunk = _chunks[_currentChunk].Count >= _chunkSizes[_currentChunk]
                ? AddNewChunk()
                : _chunks[_currentChunk];
            chunk.Add(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _chunks.Clear();
            _currentChunk = 0;
            _chunks.Add(new Dictionary<TKey, TValue>(_chunkSizes[_currentChunk]));
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _chunks.Any(chunk => chunk.Contains(item));
        }

        public bool ContainsKey(TKey key)
        {
            return _chunks.Any(chunk => chunk.ContainsKey(key));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var chunk in _chunks)
            {
                foreach (var pair in chunk)
                {
                    if (arrayIndex >= array.Length) return;
                    array[arrayIndex++] = pair;
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _chunks.SelectMany(chunk => chunk).GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            var chunk = FindChunkContainingKey(key);
            return chunk != null && chunk.Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var chunk = FindChunkContainingKey(item.Key);
            return chunk != null && chunk.Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var chunk = FindChunkContainingKey(key);
            if (chunk != null)
                return chunk.TryGetValue(key, out value);

            value = default;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Dictionary<TKey, TValue> FindChunkContainingKey(TKey key, bool createIfNotExists = false)
        {
            foreach (var chunk in _chunks)
            {
                if (chunk.ContainsKey(key))
                    return chunk;
            }

            if (createIfNotExists)
                return AddNewChunk();

            throw new KeyNotFoundException($"The given key was not present in the dictionary.");
        }

        private Dictionary<TKey, TValue> AddNewChunk()
        {
            _currentChunk++;
            if (_chunkSizes.Count <= _currentChunk)
            {
                // grow the chunk sizes list with a chunk size equal to the preferred chunk size
                _chunkSizes.Add(_preferredChunkSize);
            }

            var newChunk = new Dictionary<TKey, TValue>(_chunkSizes[_currentChunk]);
            _chunks.Add(newChunk);
            return newChunk;
        }

        internal int GetChunkCount()
        {
            return _chunks.Count;
        }
    }
}
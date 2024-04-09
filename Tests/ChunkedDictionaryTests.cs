using System;
using System.Collections.Generic;
using Xbim.Common.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class ChunkedDictionaryTests
    {
        
        [TestMethod]
        public void Init_ChunkSizeCanBeTotalSize()
        {
            var dictionary = new ChunkedDictionary<int, string>(2, 2);
            var chunks = dictionary.GetChunkCount();
            Assert.AreEqual(1, chunks, "Chunk count should be 1 when chunk size equals total size.");
        }
        
        [TestMethod]
        public void Init_OnlyOneChunkInitializedAtTheBeginning()
        {
            var dictionary = new ChunkedDictionary<int, string>(20, 2);
            var chunks = dictionary.GetChunkCount();
            Assert.AreEqual(1, chunks, "Only one chunk should be initialized at the beginning.");
        }
        
        [TestMethod]
        public void Add_SingleItem_ItemExists()
        {
            var dictionary = new ChunkedDictionary<int, string>(10);
            dictionary.Add(1, "One");

            Assert.IsTrue(dictionary.ContainsKey(1), "Key should exist after adding.");
            Assert.AreEqual("One", dictionary[1], "Value should match the added value.");
        }
        
        [TestMethod]
        public void Remove_ByKey_ItemRemoved()
        {
            var dictionary = new ChunkedDictionary<int, string>(10);
            dictionary.Add(1, "One");
            var removed = dictionary.Remove(1);

            Assert.IsTrue(removed, "Item should be removed successfully.");
            Assert.IsFalse(dictionary.ContainsKey(1), "Key should not exist after removal.");
        }

        [TestMethod]
        public void Remove_ByKeyValuePair_ItemRemoved()
        {
            var dictionary = new ChunkedDictionary<int, string>(10);
            dictionary.Add(1, "One");
            var removed = dictionary.Remove(new KeyValuePair<int, string>(1, "One"));

            Assert.IsTrue(removed, "Item should be removed successfully.");
            Assert.IsFalse(dictionary.ContainsKey(1), "Key should not exist after removal.");
        }

        [TestMethod]
        public void TryGetValue_ExistingItem_ReturnsTrueAndCorrectValue()
        {
            var dictionary = new ChunkedDictionary<int, string>(10);
            dictionary.Add(1, "One");

            string value;
            var result = dictionary.TryGetValue(1, out value);

            Assert.IsTrue(result, "TryGetValue should return true for existing key.");
            Assert.AreEqual("One", value, "Value should match the added value.");
        }

        [TestMethod]
        public void Add_MultipleItems_ExceedChunkSize_DistributesAcrossChunks()
        {
            var chunkSize = 5;
            var dictionary = new ChunkedDictionary<int, string>(chunkSize);
            for (int i = 0; i < chunkSize * 2; i++)
            {
                dictionary.Add(i, $"Item{i}");
            }

            Assert.AreEqual(chunkSize * 2, dictionary.Count, "Total items should match items added.");
            Assert.IsTrue(dictionary.Keys.Max() >= chunkSize, "Keys should be distributed across chunks.");
        }
        
        [TestMethod]
        public void Add_MultipleItems_WithTotalSize_DistributesAcrossChunks()
        {
            var chunkSize = 5;
            var total = (chunkSize * 2) + 2; // +2 to ensure we need more than 2 chunks

            var dictionary = new ChunkedDictionary<int, string>(total, chunkSize);
            for (int i = 0; i < total; i++)
            {
                dictionary.Add(i, $"Item{i}");
            }

            Assert.AreEqual(total, dictionary.Count, "Total items should match items added.");
            Assert.IsTrue(dictionary.GetChunkCount() == 3, "We have 3 chunks.");
        }
        
        [TestMethod]
        public void Add_MultipleItems_WithTotalSize_GrowsWhenAddingMoreItems()
        {
            var chunkSize = 5;
            var total = (chunkSize * 2) + 2;

            var dictionary = new ChunkedDictionary<int, string>(total, chunkSize);
            for (int i = 0; i < total; i++)
            {
                dictionary.Add(i, $"Item{i}");
            }
            
            dictionary.Add(total, $"Item{total}"); // Add one more item creating a new chunk

            Assert.AreEqual(total + 1, dictionary.Count, "Total items should match items added.");
            Assert.IsTrue(dictionary.GetChunkCount() == 4, "We have 4 chunks.");

            // Add more items to ensure we can grow further
            for (int i = 1; i < 6; i++)
            {
                dictionary.Add(total+i, $"Item{total+i}");
            }
            
            Assert.AreEqual(total + 6, dictionary.Count, "Total items should match items added.");
            Assert.IsTrue(dictionary.GetChunkCount() == 5, "We have 5 chunks by now.");
        }
        
        [TestMethod]
        public void KeysAndValues_AfterMutations_AccuratelyReflected()
        {
            var dictionary = new ChunkedDictionary<int, string>(10);
            dictionary.Add(1, "One");
            dictionary.Add(2, "Two");
            dictionary.Remove(1);

            var keys = dictionary.Keys;
            var values = dictionary.Values;

            Assert.IsTrue(keys.Contains(2) && keys.Count == 1, "Keys collection should accurately reflect current keys.");
            Assert.IsTrue(values.Contains("Two") && values.Count == 1, "Values collection should accurately reflect current values.");
        }
        
        [TestMethod]
        public void Enumeration_YieldsAllItemsExactlyOnce()
        {
            var testData = new Dictionary<int, string>
            {
                { 1, "One" }, { 2, "Two" }, { 3, "Three" },
                { 4, "Four" }, { 5, "Five" }, { 6, "Six" }
            };
            var dictionary = new ChunkedDictionary<int, string>(testData.Count, 3);

            foreach (var kvp in testData)
            {
                dictionary.Add(kvp.Key, kvp.Value);
            }

            var enumeratedItems = new Dictionary<int, string>();
            foreach (var kvp in dictionary)
            {
                // Verify no duplicates during enumeration
                Assert.IsFalse(enumeratedItems.ContainsKey(kvp.Key), "Duplicate key found during enumeration.");
                enumeratedItems.Add(kvp.Key, kvp.Value);
            }

            // Verify all items were enumerated
            Assert.AreEqual(testData.Count, enumeratedItems.Count, "Not all items were enumerated.");
            foreach (var kvp in testData)
            {
                Assert.IsTrue(enumeratedItems.ContainsKey(kvp.Key) && enumeratedItems[kvp.Key] == kvp.Value,
                    "Missing or incorrect item in enumeration.");
            }
        }

        [TestMethod]
        public void Add_ItemsExceedSingleChunkCapacity_ItemsDistributedAcrossMultipleChunks()
        {
            var chunkSize = 3;
            var itemCount = chunkSize * 2; // Ensure we need at least two chunks
            var dictionary = new ChunkedDictionary<int, string>(itemCount, chunkSize);

            for (int i = 0; i < itemCount; i++)
            {
                dictionary.Add(i, $"Item{i}");
            }

            var chunkCount = dictionary.GetChunkCount(); 
            Assert.IsTrue(chunkCount > 1, "Items should be distributed across more than one chunk.");
        }
        
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Indexer_GetMissingKey_ThrowsKeyNotFoundException()
        {
            var dictionary = new ChunkedDictionary<int, string>(10);
            var value = dictionary[999]; // This should throw
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_DuplicateKey_ThrowsArgumentException()
        {
            var dictionary = new ChunkedDictionary<int, string>(10);
            dictionary.Add(1, "One");
            dictionary.Add(1, "One again"); // This should throw
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Init_InvalidChunkSize_ThrowsArgumentException()
        {
            var dictionary = new ChunkedDictionary<int, string>(10, 11);
        }
        
    }
}
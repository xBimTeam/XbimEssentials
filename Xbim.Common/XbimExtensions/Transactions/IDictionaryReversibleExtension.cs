#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IDictionaryReversibleExtension.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;

#endregion

namespace Xbim.XbimExtensions.Transactions
{
    /// <summary>
    ///   Contains reversible extension methods for type-safe IDictionaries
    /// </summary>
    public static class IDictionaryReversibleExtension
    {
        /// <summary>
        ///   Adds an item reversible to a dictionary
        /// </summary>
        /// <typeparam name = "TKey">Type of the key</typeparam>
        /// <typeparam name = "TValue">Type of the item</typeparam>
        /// <param name = "dictionary"></param>
        /// <param name = "key">The key of the item</param>
        /// <param name = "itemToAdd">The item associated with the key</param>
        public static void Add_Reversible<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
                                                        TValue itemToAdd)
        {
            dictionary.Add(key, itemToAdd);
            Transaction txn = Transaction.Current;
            if (txn != null)
            {
                txn.AddEdit(new IDictionaryEdit<TKey, TValue>(IDictionaryEditAction.AddItem, dictionary, key, itemToAdd));
            }
        }

        /// <summary>
        ///   Removes an item with the specified key from the dictionary
        /// </summary>
        /// <typeparam name = "TKey"></typeparam>
        /// <typeparam name = "TValue"></typeparam>
        /// <param name = "dictionary"></param>
        /// <param name = "key"></param>
        /// <returns></returns>
        public static bool Remove_Reversible<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue item;
            bool retValue = dictionary.TryGetValue(key, out item);
            retValue = dictionary.Remove(key);
            Transaction txn = Transaction.Current;
            if (txn != null)
            {
                txn.AddEdit(new IDictionaryEdit<TKey, TValue>(IDictionaryEditAction.RemoveItem, dictionary, key, item));
            }
            return retValue;
        }

        /// <summary>
        ///   Clears a dictionary reversibly
        /// </summary>
        /// <typeparam name = "TKey"></typeparam>
        /// <typeparam name = "TValue"></typeparam>
        /// <param name = "dictionary"></param>
        public static void Clear_Reversible<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            Transaction txn = Transaction.Current;
            if (txn != null)
            {
                txn.AddEdit(new IDictionaryClearEdit<TKey, TValue>(dictionary));
            }
            dictionary.Clear();
        }

        private enum IDictionaryEditAction
        {
            AddItem,
            RemoveItem
        }

        private class IDictionaryEdit<TKey, TValue> : Edit
        {
            private IDictionaryEditAction action;
            private readonly IDictionary<TKey, TValue> dictionary;
            private readonly TValue item;
            private readonly TKey key;

            public IDictionaryEdit(IDictionaryEditAction action, IDictionary<TKey, TValue> dictionary, TKey key,
                                   TValue item)
            {
                this.action = action;
                this.dictionary = dictionary;
                this.item = item;
                this.key = key;
            }

            public override Edit Reverse()
            {
                switch (action)
                {
                    case IDictionaryEditAction.AddItem:
                        dictionary.Remove(key);
                        action = IDictionaryEditAction.RemoveItem;
                        break;
                    case IDictionaryEditAction.RemoveItem:
                        dictionary.Add(key, item);
                        action = IDictionaryEditAction.AddItem;
                        break;
                }
                return this;
            }
        }

        private class IDictionaryClearEdit<TKey, TValue> : Edit
        {
            private readonly IDictionary<TKey, TValue> dictionary;
            private readonly Dictionary<TKey, TValue> backupItems;

            public IDictionaryClearEdit(IDictionary<TKey, TValue> dictionary)
            {
                this.dictionary = dictionary;
                backupItems = new Dictionary<TKey, TValue>(dictionary);
            }

            public override Edit Reverse()
            {
                if (dictionary.Count > 0)
                {
                    dictionary.Clear();
                }
                else
                {
                    foreach (KeyValuePair<TKey, TValue> item in backupItems)
                    {
                        dictionary.Add(item);
                    }
                }
                return this;
            }
        }
    }
}
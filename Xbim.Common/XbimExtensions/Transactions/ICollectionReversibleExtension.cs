#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    ICollectionReversibleExtension.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;

#endregion

namespace Xbim.XbimExtensions.Transactions.Extensions
{
    /// <summary>
    ///   Contains reversible extension methods for type-safe ICollections
    /// </summary>
    public static class ICollectionReversibleExtension
    {
        /// <summary>
        ///   Adds an item reversible
        /// </summary>
        /// <typeparam name = "T">Type of item to add</typeparam>
        /// <param name = "collection">Collection to add item to</param>
        /// <param name = "itemToAdd">Item to add</param>
        public static void Add_Reversible<T>(this ICollection<T> collection, T itemToAdd)
        {
            collection.Add(itemToAdd);
            Transaction txn = Transaction.Current;
            if (txn != null)
            {
                txn.AddEdit(new ICollectionEdit<T>(ICollectionEditAction.AddItem, collection, itemToAdd));
            }
        }


        /// <summary>
        ///   Removes an item reversibly.
        /// </summary>
        /// <typeparam name = "T">Type of item to add</typeparam>
        /// <param name = "collection">Collection to remove from</param>
        /// <param name = "itemToRemove">Item to remove</param>
        public static bool Remove_Reversible<T>(this ICollection<T> collection, T itemToRemove)
        {
            bool retValue = collection.Remove(itemToRemove);
            Transaction txn = Transaction.Current;
            if (txn != null)
            {
                txn.AddEdit(new ICollectionEdit<T>(ICollectionEditAction.RemoveItem, collection, itemToRemove));
            }
            return retValue;
        }

        /// <summary>
        ///   Clears an items reversibly.
        /// </summary>
        /// <typeparam name = "T">Type of ICollection</typeparam>
        /// <param name = "collection">Collection to clear</param>
        public static void Clear_Reversible<T>(this ICollection<T> collection)
        {
            Transaction txn = Transaction.Current;
            if (txn != null)
            {
                txn.AddEdit(new ICollectionClearEdit<T>(collection));
            }
            collection.Clear();
        }

        /// <summary>
        ///   Creates a reversible version of a given collection.
        /// </summary>
        /// <typeparam name = "T">Type of collection</typeparam>
        /// <param name = "collection">collection to wrap</param>
        /// <returns></returns>
        public static ReversibleCollection<T> AsReversible<T>(this ICollection<T> collection)
        {
            return new ReversibleCollection<T>(collection);
        }

        private enum ICollectionEditAction
        {
            AddItem,
            RemoveItem
        }

        private class ICollectionEdit<T> : Edit
        {
            private ICollectionEditAction action;
            private readonly ICollection<T> collection;
            private readonly T item;

            public ICollectionEdit(ICollectionEditAction action, ICollection<T> collection, T item)
            {
                this.action = action;
                this.collection = collection;
                this.item = item;
            }

            public override Edit Reverse()
            {
                switch (action)
                {
                    case ICollectionEditAction.AddItem:
                        collection.Remove(item);
                        action = ICollectionEditAction.RemoveItem;
                        break;
                    case ICollectionEditAction.RemoveItem:
                        collection.Add(item);
                        action = ICollectionEditAction.AddItem;
                        break;
                }
                return this;
            }
        }

        private class ICollectionClearEdit<T> : Edit
        {
            private readonly ICollection<T> collection;
            private readonly List<T> backupItems;

            public ICollectionClearEdit(ICollection<T> collection)
            {
                this.collection = collection;
                backupItems = new List<T>(collection);
            }

            public override Edit Reverse()
            {
                if (collection.Count > 0)
                {
                    collection.Clear();
                }
                else
                {
                    foreach (T item in backupItems)
                    {
                        collection.Add(item);
                    }
                }
                return this;
            }
        }
    }
}
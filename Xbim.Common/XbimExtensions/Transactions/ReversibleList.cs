#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    ReversibleList.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xbim.XbimExtensions.Transactions.Extensions;

#endregion

namespace Xbim.XbimExtensions.Transactions
{
    /// <summary>
    ///   Enumerates possible actions in an ListEdit and IListEdit.
    /// </summary>
    public enum ListEditAction
    {
        ChangeItem = 1,
        AddItem,
        RemoveItem
    }

    /// <summary>
    ///   Represents a reversible change in a range of items in a list.
    /// </summary>
    /// <typeparam name = "T">Type of list</typeparam>
    public class ListRangeEdit<T> : Edit
    {
        private ListEditAction action;
        private readonly List<T> list;
        private readonly List<T> items;
        private readonly int startIndex;

        public ListRangeEdit(ListEditAction action, List<T> list, List<T> items, int startIndex)
        {
            this.action = action;
            this.list = list;
            this.items = items;
            this.startIndex = startIndex;
        }

        public override Edit Reverse()
        {
            switch (action)
            {
                case ListEditAction.ChangeItem:
                    for (int i = 0; i < items.Count; i++)
                    {
                        T backup = items[i];
                        items[i] = list[i + startIndex];
                        list[i + startIndex] = backup;
                    }
                    break;
                case ListEditAction.AddItem:
                    list.RemoveRange(startIndex, items.Count);
                    action = ListEditAction.RemoveItem;
                    break;
                case ListEditAction.RemoveItem:
                    list.InsertRange(startIndex, items);
                    action = ListEditAction.AddItem;
                    break;
            }
            return this;
        }
    }

    /// <summary>
    ///   A reversible version of the generic List class.
    /// </summary>
    /// <typeparam name = "T">Type of items in the list</typeparam>
    public class ReversibleList<T> : IList<T>, IList
    {
        private readonly List<T> list;

        public T[] ToArray()
        {
            return list.ToArray();
        }

        /// <summary>
        ///   Creates a new ReversibleList instance.
        /// </summary>
        public ReversibleList()
        {
            list = new List<T>();
        }

        /// <summary>
        ///   Creates a new ReversibleList instance with a given initial capacity
        /// </summary>
        /// <param name = "capacity">Number of items to allocate room for.</param>
        public ReversibleList(int capacity)
        {
            list = new List<T>(capacity);
        }

        /// <summary>
        ///   Creates a new ReversibleList instance with elements from a collection.
        /// </summary>
        /// <param name = "collection">Collection to copy items from.</param>
        public ReversibleList(IEnumerable<T> collection)
        {
            list = new List<T>(collection);
        }

        /// <summary>
        ///   Creates a new ReversibleList instance that wraps a given non-reversible list.
        /// </summary>
        /// <param name = "listToWrap"></param>
        private ReversibleList(List<T> listToWrap)
        {
            list = listToWrap;
        }

        /// <summary>
        ///   Creates a ReversibleList that wraps another (non-reversible) IList to make the original reversible.
        /// </summary>
        /// <param name = "listToWrap">List to wrap.</param>
        /// <returns>A ReversibleList instance that wraps the given list</returns>
        public static ReversibleList<T> CreateReversibleWrapper(List<T> listToWrap)
        {
            return new ReversibleList<T>(listToWrap);
        }

        /// <summary>
        ///   Gets or sets the current capacity of the list.
        /// </summary>
        public int Capacity
        {
            get { return list.Capacity; }
            set { list.Capacity = value; }
        }

        /// <summary>
        ///   Gets a range of items
        /// </summary>
        /// <param name = "index">First item index of range</param>
        /// <param name = "count">Number of items to include in range</param>
        /// <returns>A non-reversible list with the items in the specified range</returns>
        public List<T> GetRange(int index, int count)
        {
            return list.GetRange(index, count);
        }

        /// <summary>
        ///   Adds a set of items to the end of the list (reversibly).
        /// </summary>
        /// <param name = "collection">A set of items to add</param>
        public void AddRange(IEnumerable<T> collection)
        {
            InsertRange(list.Count, collection);
        }

        /// <summary>
        ///   Inserts a set of items at a given position in a list (reversibly).
        /// </summary>
        /// <param name = "index"></param>
        /// <param name = "collection"></param>
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            Transaction txn = Transaction.Current;
            if (txn != null)
            {
                List<T> itemsToInsert = new List<T>(collection);
                list.InsertRange(index, itemsToInsert);
                txn.AddEdit(new ListRangeEdit<T>(ListEditAction.AddItem, list, itemsToInsert, index));
            }
            else
            {
                list.InsertRange(index, collection);
            }
        }

        /// <summary>
        ///   Removes a range of items in a list (reversibly).
        /// </summary>
        /// <param name = "index">Index of first item to remove</param>
        /// <param name = "count">Count of items to remove.</param>
        public void RemoveRange(int index, int count)
        {
            Transaction txn = Transaction.Current;
            if (txn != null)
            {
                List<T> removedItems = list.GetRange(index, count);
                list.RemoveRange(index, count);
                txn.AddEdit(new ListRangeEdit<T>(ListEditAction.RemoveItem, list, removedItems, index));
            }
            else
            {
                list.RemoveRange(index, count);
            }
        }

        /// <summary>
        ///   Reverses the order of all items in the list (reversible).
        /// </summary>
        public void Reverse()
        {
            Reverse(0, list.Count);
        }

        /// <summary>
        ///   Sorts the list in default order (reversible).
        /// </summary>
        public void Sort()
        {
            Sort(0, list.Count, Comparer<T>.Default);
        }

        /// <summary>
        ///   Sorts the list using a given comparer (reversible).
        /// </summary>
        /// <param name = "comparer">Comparer to use when sorting items.</param>
        public void Sort(IComparer<T> comparer)
        {
            Sort(0, list.Count, comparer);
        }

        /// <summary>
        ///   Sorts the list using a given Comparison (reversible)
        /// </summary>
        /// <param name = "comparison">Comparision delegate to use.</param>
        public void Sort(Comparison<T> comparison)
        {
            Transaction txn = Transaction.Current;
            if (txn != null)
            {
                List<T> changedItems = list.GetRange(0, list.Count);
                list.Sort(comparison);
                txn.AddEdit(new ListRangeEdit<T>(ListEditAction.ChangeItem, list, changedItems, 0));
            }
            else
            {
                list.Sort(comparison);
            }
        }

        /// <summary>
        ///   Sorts a subset of the list (reversible).
        /// </summary>
        /// <param name = "index">Index of first item to sort</param>
        /// <param name = "count">Count of items to sort</param>
        /// <param name = "comparer">Comparer to use to sort.</param>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            Transaction txn = Transaction.Current;
            if (txn != null)
            {
                List<T> changedItems = list.GetRange(index, count);
                list.Sort(index, count, comparer);
                txn.AddEdit(new ListRangeEdit<T>(ListEditAction.ChangeItem, list, changedItems, index));
            }
            else
            {
                list.Sort(index, count, comparer);
            }
        }

        /// <summary>
        ///   Reverses the order of a list subset (reversible).
        /// </summary>
        /// <param name = "index">Index of first item to reverse.</param>
        /// <param name = "count">Number of items to reverse.</param>
        public void Reverse(int index, int count)
        {
            Transaction txn = Transaction.Current;
            if (txn != null)
            {
                List<T> changedItems = list.GetRange(index, count);
                list.Reverse(index, count);
                txn.AddEdit(new ListRangeEdit<T>(ListEditAction.ChangeItem, list, changedItems, index));
            }
            else
            {
                list.Reverse(index, count);
            }
        }

        /// <summary>
        ///   Finds the first item that match a given Predicate.
        /// </summary>
        /// <param name = "match">A Predicate to use for matching</param>
        /// <returns>The item found.</returns>
        public T Find(Predicate<T> match)
        {
            return list.Find(match);
        }

        /// <summary>
        ///   Performs an action on all items in list.
        /// </summary>
        /// <param name = "action">Action to perform</param>
        public void ForEach(Action<T> action)
        {
            list.ForEach(action);
        }

        /// <summary>
        ///   Returns a read-only version of this list.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<T> AsReadOnly()
        {
            return list.AsReadOnly();
        }

        /// <summary>
        ///   Converts all elements to another type.
        /// </summary>
        /// <typeparam name = "TOutput">Type to convert to</typeparam>
        /// <param name = "converter">Converter to use</param>
        /// <returns>A non-reversible List of the given type</returns>
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            return list.ConvertAll(converter);
        }

        #region IList<T> Members

        /// <summary>
        ///   Gets the first index of a given item.
        /// </summary>
        /// <param name = "item">Item to search for.</param>
        /// <returns>The index of the item (or -1 if not found)</returns>
        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        /// <summary>
        ///   Insert an item at a given position (reversibly).
        /// </summary>
        /// <param name = "index">List index to insert at</param>
        /// <param name = "item">Item to insert</param>
        public void Insert(int index, T item)
        {
            list.Insert_Reversible(index, item);
        }

        /// <summary>
        ///   Removes an item at a given position (reversibly).
        /// </summary>
        /// <param name = "index">Index of item to remove</param>
        public void RemoveAt(int index)
        {
            list.RemoveAt_Reversible(index);
        }

        /// <summary>
        ///   Gets or sets item at a given index (reversibly).
        /// </summary>
        /// <param name = "index">Index of list item to get or set</param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return list[index]; }
            set { list.Item_SetReversible(index, value); }
        }

        #endregion

        #region ICollection<T> Members

        /// <summary>
        ///   Adds an item to end of list (reversibly).
        /// </summary>
        /// <param name = "item"></param>
        public void Add(T item)
        {
            list.Add_Reversible(item);
        }

        /// <summary>
        ///   Clears a list (reversibly).
        /// </summary>
        public void Clear()
        {
            Transaction txn = Transaction.Current;
            if (txn != null)
            {
                List<T> itemsToClear = list.GetRange(0, list.Count);
                list.Clear();
                txn.AddEdit(new ListRangeEdit<T>(ListEditAction.RemoveItem, list, itemsToClear, 0));
            }
            else
            {
                list.Clear();
            }
        }

        /// <summary>
        ///   Determines whether an element is in the list.
        /// </summary>
        /// <param name = "item">Item to look for</param>
        /// <returns>True if item found, else False</returns>
        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        /// <summary>
        ///   Copies the element of the list to an array.
        /// </summary>
        /// <param name = "array">Array to copy to</param>
        /// <param name = "arrayIndex">Array index to start from.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///   Gets number of items in list.
        /// </summary>
        public int Count
        {
            get { return list.Count; }
        }

        /// <summary>
        ///   Determines whether the list is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///   Removes the first occurence of the given item (reversibly).
        /// </summary>
        /// <param name = "item">Item to remove</param>
        /// <returns>True if item was found and removed.</returns>
        public bool Remove(T item)
        {
            return list.Remove_Reversible(item);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            Add((T) value);
            return list.Count - 1;
        }

        void IList.Clear()
        {
            Clear();
        }

        bool IList.Contains(object value)
        {
            return Contains((T) value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T) value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T) value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            Remove((T) value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveAt(index);
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (T) value; }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection) list).CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return list.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return ((IList) list).SyncRoot; }
        }

        #endregion
    }
}
#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    ListReversibleExtension.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;

#endregion

namespace Xbim.XbimExtensions.Transactions.Extensions
{
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

    public static class ListReversibleExtension
    {
        /// <summary>
        ///   Adds a set of items to the end of the list (reversibly).
        /// </summary>
        /// <param name = "collection">A set of items to add</param>
        public static void AddRange_Reversible<T>(this List<T> list, IEnumerable<T> collection)
        {
            InsertRange_Reversible(list, list.Count, collection);
        }

        /// <summary>
        ///   Inserts a set of items at a given position in a list (reversibly).
        /// </summary>
        /// <param name = "index"></param>
        /// <param name = "collection"></param>
        public static void InsertRange_Reversible<T>(this List<T> list, int index, IEnumerable<T> collection)
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
        public static void RemoveRange_Reversible<T>(this List<T> list, int index, int count)
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
        public static void Reverse_Reversible<T>(this List<T> list)
        {
            Reverse_Reversible(list, 0, list.Count);
        }

        /// <summary>
        ///   Reverses the order of a list subset (reversible).
        /// </summary>
        /// <param name = "index">Index of first item to reverse.</param>
        /// <param name = "count">Number of items to reverse.</param>
        public static void Reverse_Reversible<T>(this List<T> list, int index, int count)
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
        ///   Sorts the list in default order (reversible).
        /// </summary>
        public static void Sort_Reversible<T>(this List<T> list)
        {
            Sort_Reversible(list, 0, list.Count, Comparer<T>.Default);
        }

        /// <summary>
        ///   Sorts the list using a given comparer (reversible).
        /// </summary>
        /// <param name = "comparer">Comparer to use when sorting items.</param>
        public static void Sort_Reversible<T>(this List<T> list, IComparer<T> comparer)
        {
            Sort_Reversible(list, 0, list.Count, comparer);
        }

        /// <summary>
        ///   Sorts the list using a given Comparison (reversible)
        /// </summary>
        /// <param name = "comparison">Comparision delegate to use.</param>
        public static void Sort_Reversible<T>(this List<T> list, Comparison<T> comparison)
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
        public static void Sort_Reversible<T>(this List<T> list, int index, int count, IComparer<T> comparer)
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
        ///   Makes a given list reversible by wrapping it into a ReversibleList instance.
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "list"></param>
        /// <returns></returns>
        public static ReversibleList<T> AsReversible<T>(this List<T> list)
        {
            return ReversibleList<T>.CreateReversibleWrapper(list);
        }
    }
}
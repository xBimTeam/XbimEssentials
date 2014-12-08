#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    ReversibleCollection.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections;
using System.Collections.Generic;
using Xbim.XbimExtensions.Transactions.Extensions;

#endregion

namespace Xbim.XbimExtensions.Transactions
{
    /// <summary>
    ///   Represents a generic reversible collection.
    /// </summary>
    /// <typeparam name = "T">Type of items in the collection</typeparam>
    public class ReversibleCollection<T> : ICollection<T>
    {
        private readonly ICollection<T> collection;

        /// <summary>
        ///   Creates a new empty reversible collection
        /// </summary>
        public ReversibleCollection()
        {
            collection = new List<T>();
        }

        /// <summary>
        ///   Creates a reversible collection that wraps a non-reversible collection.
        /// </summary>
        /// <param name = "collectionToWrap"></param>
        public ReversibleCollection(ICollection<T> collectionToWrap)
        {
            this.collection = collectionToWrap;
        }

        #region ICollection<T> Members

        public void Add(T item)
        {
            collection.Add_Reversible(item);
        }

        public void Clear()
        {
            collection.Clear_Reversible();
        }

        public bool Contains(T item)
        {
            return collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return collection.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            return collection.Remove_Reversible(item);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        #endregion
    }
}
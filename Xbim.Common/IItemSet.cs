using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Xbim.Common
{
    public interface IItemSet
    {
        IPersistEntity OwningEntity { get; }
        
    }
    public interface IItemSet<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged, IExpressEnumerable, IItemSet
    {
        T GetAt(int index);

        /// <summary>
        /// Convenient feature taken from List implementation which allows to add more items in one go.
        /// </summary>
        /// <param name="values">Values to be added</param>
        void AddRange(IEnumerable<T> values);

        /// <summary>
        /// Function which mimics IEnumerable feature hidden by the templated version otherwise
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns> Firts or default item</returns>
        T FirstOrDefault(Func<T, bool> predicate);

        /// <summary>
        /// Function which mimics IQueryable feature of getting first or default 
        /// item of defined type. Additional type safety security is provided by type constrain.
        /// </summary>
        /// <typeparam name="TF">Type of requested item</typeparam>
        /// <param name="predicate">Logical predicate</param>
        /// <returns>First item satisfying both type and logical condition</returns>
        TF FirstOrDefault<TF>(Func<TF, bool> predicate) where TF: T;

        /// <summary>
        /// Function which mimics IQueryable feature of combining both type and logical condition.
        /// item of defined type. Additional type safety security is provided by type constrain.
        /// </summary>
        /// <typeparam name="TW">Requested type</typeparam>
        /// <param name="predicate"></param>
        /// <returns>List of items satisfying both type and logical condition</returns>
        IEnumerable<TW> Where<TW>(Func<TW, bool> predicate) where TW: T;
    }
}

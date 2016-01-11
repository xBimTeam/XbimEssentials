using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Xbim.Common
{
    public interface IItemSet<T> : IList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged, IExpressEnumerable
    {
		IPersistEntity OwningEntity { get; }
        void AddRange(IEnumerable<T> values);
        T First { get; }
        T FirstOrDefault();
        T FirstOrDefault(Func<T, bool> predicate);
        TF FirstOrDefault<TF>(Func<TF, bool> predicate);
        IEnumerable<TW> Where<TW>(Func<TW, bool> predicate);
        IEnumerable<TO> OfType<TO>();
    }
}

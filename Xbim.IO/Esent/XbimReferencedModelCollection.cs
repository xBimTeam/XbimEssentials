using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Xbim.Common.Exceptions;
using Xbim.Common.Federation;

namespace Xbim.IO.Esent
{
    public class XbimReferencedModelCollection : KeyedCollection<string, IReferencedModel>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        protected override string GetKeyForItem(IReferencedModel item)
        {
            return item.Identifier;
        }

        public string NextIdentifer()
        {
            for (short i = 1; i < short.MaxValue; i++)
            {
                if (!this.Contains(i.ToString()))
                    return i.ToString();
            }
            throw new XbimException("Too many Reference Models added");
        }

        private event NotifyCollectionChangedEventHandler _collectionChanged;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _collectionChanged += value; }
            remove { _collectionChanged -= value; }
        }


        protected override void InsertItem(int index, IReferencedModel item)
        {
            IReferencedModel removed = null;
            if (index < Count)
                removed = this[index];
            base.InsertItem(index, item);
            var collChanged = _collectionChanged;
            if (collChanged != null)
            {
                if (index == Count)
                    collChanged(this,
                                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, removed, index));
                else
                {
                    collChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
                    NotifyCountChanged(Count - 1);
                }
            }
        }

        protected override void RemoveItem(int index)
        {
            var oldCount = Count;
            var removed = this[index];
            base.RemoveItem(index);
            var collChanged = _collectionChanged;
            if (collChanged != null)
                collChanged(this,
                            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, index));
            NotifyCountChanged(oldCount);
        }

        protected override void ClearItems()
        {
            var oldCount = Count;
            base.ClearItems();
            var collChanged = _collectionChanged;
            if (collChanged != null)
                collChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            NotifyCountChanged(oldCount);
        }

        protected override void SetItem(int index, IReferencedModel item)
        {
            IReferencedModel removed = null;
            if (index < Count)
                removed = this[index];
            base.SetItem(index, item);
            var collChanged = _collectionChanged;
            if (collChanged != null)
                collChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, removed, index));
        }

        private static PropertyChangedEventArgs countPropChangedEventArgs = new PropertyChangedEventArgs("Count");

        private void NotifyCountChanged(int oldValue)
        {

            if (PropertyChanged != null && oldValue != Count)
                PropertyChanged(this, countPropChangedEventArgs);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

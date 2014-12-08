using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Xbim.Common.Exceptions;
using System.Collections.Specialized;
using System.ComponentModel;
using Xbim.Ifc2x3.MeasureResource;
namespace Xbim.IO
{
    public class XbimReferencedModelCollection : KeyedCollection<string, XbimReferencedModel>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        protected override string GetKeyForItem(XbimReferencedModel item)
        {
            return item.Identifier;
        }

        internal IfcIdentifier NextIdentifer()
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


        protected override void InsertItem(int index, XbimReferencedModel item)
        {
            XbimReferencedModel removed = null;
            if (index < Count)
                removed = this[index];
            base.InsertItem(index, item);
            NotifyCollectionChangedEventHandler collChanged = _collectionChanged;
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
            int oldCount = Count;
            XbimReferencedModel removed = this[index];
            base.RemoveItem(index);
            NotifyCollectionChangedEventHandler collChanged = _collectionChanged;
            if (collChanged != null)
                collChanged(this,
                            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removed, index));
            NotifyCountChanged(oldCount);
        }

        protected override void ClearItems()
        {
            int oldCount = Count;
            base.ClearItems();
            NotifyCollectionChangedEventHandler collChanged = _collectionChanged;
            if (collChanged != null)
                collChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            NotifyCountChanged(oldCount);
        }

        protected override void SetItem(int index, XbimReferencedModel item)
        {
            XbimReferencedModel removed = null;
            if (index < Count)
                removed = this[index];
            base.SetItem(index, item);
            NotifyCollectionChangedEventHandler collChanged = _collectionChanged;
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

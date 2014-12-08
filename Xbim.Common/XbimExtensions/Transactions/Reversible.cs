#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    Reversible.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.XbimExtensions.Transactions
{
    /// <summary>
    ///   Wraps a field to make it reversible.
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    public struct Reversible<T>
    {
        private T localValue;
        private ReversibleStorage<T> storage;

        public Reversible(T initialValue)
        {
            localValue = initialValue;
            storage = null;
        }

        public T Value
        {
            get
            {
                if (storage == null)
                {
                    return localValue;
                }
                else
                {
                    return storage.CurrentValue;
                }
            }
            set
            {
                Transaction txn = Transaction.Current;
                if (storage == null)
                {
                    if (txn != null)
                    {
                        storage = new ReversibleStorage<T>(value, localValue);
                        txn.AddEdit(storage);
                    }
                    else
                    {
                        localValue = value;
                    }
                }
                else
                {
                    if (txn != null)
                    {
                        ReversibleStorageEdit<T> storageEdit = new ReversibleStorageEdit<T>(storage,
                                                                                            storage.CurrentValue);
                        storage.CurrentValue = value;
                        txn.AddEdit(storageEdit);
                    }
                    else
                    {
                        storage.CurrentValue = value;
                    }
                }
            }
        }

        public override int GetHashCode()
        {
            object valueObj = Value;
            if (valueObj == null)
            {
                return 0;
            }
            else
            {
                return valueObj.GetHashCode();
            }
        }

        public override string ToString()
        {
            object valueObj = Value;
            if (valueObj == null)
            {
                return null;
            }
            else
            {
                return valueObj.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            object valueObj = Value;
            if (valueObj == null)
            {
                return false;
            }
            else
            {
                return valueObj.Equals(obj);
            }
        }
    }
}
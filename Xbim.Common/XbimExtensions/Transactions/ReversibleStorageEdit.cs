#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    ReversibleStorageEdit.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.XbimExtensions.Transactions
{
    internal sealed class ReversibleStorageEdit<T> : Edit
    {
        private readonly ReversibleStorage<T> storage;
        private T backupValue;

        public ReversibleStorageEdit(ReversibleStorage<T> storage, T backup)
        {
            this.storage = storage;
            backupValue = backup;
        }

        public override Edit Reverse()
        {
            T lastValue = storage.CurrentValue;
            storage.CurrentValue = backupValue;
            backupValue = lastValue;
            return this;
        }
    }
}
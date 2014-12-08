#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    ReversibleStorage.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.XbimExtensions.Transactions
{
    public sealed class ReversibleStorage<T> : Edit
    {
        public T CurrentValue;

        private T backupValue;

        public ReversibleStorage(T current, T backup)
        {
            CurrentValue = current;
            backupValue = backup;
        }

        public override Edit Reverse()
        {
            T backup = backupValue;
            backupValue = CurrentValue;
            CurrentValue = backup;
            return this;
        }
    }
}
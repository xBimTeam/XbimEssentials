using System;

namespace Xbim.Common
{
    /// <summary>
    /// A class of transaction that has no impact and does nothing used when another transaction is normally running
    /// </summary>
    public class PlaceboTransaction:ITransaction
    {
        public string Name
        {
            get { return "Placebo"; }
        }

        public void Commit()
        {
            //do nothing
        }

        public void RollBack()
        {
           //do nothing
        }

        public void AddReversibleAction(Action doAction, Action undoAction, IPersistEntity entity, ChangeType changeType, int property)
        {
            //do nothing
        }

        public void Dispose()
        {
           //do nothing
        }
    }
}

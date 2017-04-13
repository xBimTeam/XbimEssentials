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

        public void DoReversibleAction(Action doAction, Action undoAction, IPersistEntity entity, ChangeType changeType, int property)
        {
            //do nothing
        }

        public event EntityChangedHandler EntityChanged;
        public event EntityChangingHandler EntityChanging;

        public void Dispose()
        {
           //do nothing
        }

        protected virtual void OnEntityChanged(IPersistEntity entity, ChangeType change, int property)
        {
            var handler = EntityChanged;
            if (handler != null) handler(entity, change, property);
        }

        protected virtual void OnEntityChanging(IPersistEntity entity, ChangeType change, int property)
        {
            var handler = EntityChanging;
            if (handler != null) handler(entity, change, property);
        }
    }
}

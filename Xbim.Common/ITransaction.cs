using System;

namespace Xbim.Common
{
	public interface ITransaction: IDisposable
	{
		string Name { get; }
		void Commit();
		void RollBack();
		void DoReversibleAction(Action doAction, Action undoAction, IPersistEntity entity, ChangeType changeType, int property);

        /// <summary>
        /// This event should be fired after entity is changed
        /// </summary>
        event EntityChangedHandler EntityChanged;
        /// <summary>
        /// This event should be fired before entity is changed.
        /// </summary>
        event EntityChangingHandler EntityChanging;
	}

    public delegate void EntityChangingHandler(IPersistEntity entity, ChangeType change, int property);
    public delegate void EntityChangedHandler(IPersistEntity entity, ChangeType change, int property);

	public enum ChangeType
	{
		New,
		Deleted,
		Modified,
	}
}
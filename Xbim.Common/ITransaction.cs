using System;

namespace Xbim.Common
{
	public interface ITransaction: IDisposable
	{
		string Name { get; }
		void Commit();
		void RollBack();
		void AddReversibleAction(Action doAction, Action undoAction, IPersistEntity entity, ChangeType changeType, byte property);
	}

	public enum ChangeType
	{
		New,
		Deleted,
		Modified,
	}
}
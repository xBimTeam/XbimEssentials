using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;

namespace Xbim.IO.Memory
{
    public class Transaction<TFactory> : ITransaction where TFactory : IEntityFactory, new()
    {
        private readonly MemoryModel<TFactory> _model;
        private bool _closed;
        private bool _undone;
        private readonly List<Change> _log = new List<Change>();

        private class Change
        {
            public Action DoAction { get; private set; }
            public Action UndoAction { get; private set; }
            public IPersistEntity Entity { get; private set; }
            public ChangeType ChangeType { get; private set; }

            public Change(Action doAction, Action undoAction, IPersistEntity entity, ChangeType changeType)
            {
                DoAction = doAction;
                UndoAction = undoAction;
                Entity = entity;
                ChangeType = changeType;
            }
        }

        public Transaction(MemoryModel<TFactory> model, string name)
        {
            Name = name;
            _model = model;
        }

        public Transaction(MemoryModel<TFactory> model)
        {
            _model = model;
        }

        public string Name { get; set; }

        public void Commit()
        {
            if (_closed)
                throw new Exception("Transaction closed already");

            Finish();
        }

        public void RollBack()
        {
            if (_closed)
                throw new Exception("Transaction is closed already");

            //from back to front
            for (var i = _log.Count -1 ; i == 0 ; i--)
                _log[i].UndoAction();
            
            Finish();
        }

        public void AddReversibleAction(Action doAction, Action undoAction, IPersistEntity entity, ChangeType changeType)
        {
            if (_closed)
                throw new Exception("Transaction is closed already");

            _log.Add(new Change(doAction, undoAction, entity, changeType));
            _model.HandleEntityChange(changeType, entity);
        }


        /// <summary>
        /// Returns all modified entities which are not added or deleted
        /// </summary>
        public IEnumerable<IPersistEntity> Modified
        {
            get { return _log.Where(c => c.ChangeType == ChangeType.Modified && !_log.Any(a => a.Entity == c.Entity && (a.ChangeType == ChangeType.New || a.ChangeType == ChangeType.Deleted))).Select(c => c.Entity).Distinct(); }
        }

        /// <summary>
        /// All entities added as a new in this transaction
        /// </summary>
        public IEnumerable<IPersistEntity> Added
        {
            get { return _log.Where(c => c.ChangeType == ChangeType.New).Select(c => c.Entity).Distinct(); }
        }

        /// <summary>
        /// All deleted entities in this transaction
        /// </summary>
        public IEnumerable<IPersistEntity> Deleted
        {
            get { return _log.Where(c => c.ChangeType == ChangeType.Deleted).Select(c => c.Entity).Distinct(); }
        }


        private void Finish()
        {
            if (_closed)
                throw new Exception("Transaction closed already");

            _log.Clear();
            _model.CurrentTransaction = null;
            _closed = true;
        }

        public void Undo()
        {
            if (!_closed)
                throw new Exception("Transaction is not closed yet. You can only undo closed transaction.");

            if (_undone)
                return; //don't undo multiple times

            //from back to front
            for (var i = _log.Count - 1; i == 0; i--)
                _log[i].UndoAction();
            _undone = true;
        }

        public void Redo()
        {
            if (!_closed)
                throw new Exception("Transaction is not closed yet. You can only undo closed transaction.");

            if (!_undone)
                return; //don't redo multiple times

            foreach (var doAction in _log.Select(c => c.DoAction))
                doAction();
            _undone = false;
        }

        public void Dispose()
        {
            if (!_closed)
                RollBack();
            _log.Clear();
        }
    }
}

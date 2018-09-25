using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Xbim.Common;

namespace Xbim.Presentation
{
    /// <summary>
    /// Provides a container for entity selections capable of undo/redo operations and notification of changes.
    /// </summary>
    public class EntitySelection : INotifyCollectionChanged, IEnumerable<IPersistEntity>
    {
        private readonly List<SelectionEvent> _selectionLog;
        private readonly XbimIPersistEntityCollection<IPersistEntity> _selection = new XbimIPersistEntityCollection<IPersistEntity>();
        private int _position = -1;

        /// <summary>
        /// Initialises an empty selection;
        /// </summary>
        /// <param name="keepLogging">Set to True to enable activity logging for undo/redo operations.</param>
        public EntitySelection(bool keepLogging = false)
        {
            if (keepLogging)
                _selectionLog = new List<SelectionEvent>();
        }

        public void Undo()
        {
            if (_selectionLog == null)
                return;
            if (_position >= 0)
            {
                RollBack(_selectionLog[_position]);
                _position--;
            }
        }

        public void Redo()
        {
            if (!(_position < _selectionLog?.Count - 1))
                return;
            _position++;
            RollForward(_selectionLog[_position]);
        }

        private void RollBack(SelectionEvent e) 
        {
            switch (e.Action)
            {
                case Action.Add:
                    RemoveRange(e.Entities);
                    break;
                case Action.Remove:
                    AddRange(e.Entities);
                    break;
            }
        }

        private void RollForward(SelectionEvent e)
        {
            switch (e.Action)
            {
                case Action.Add:
                    AddRange(e.Entities);
                    break;
                case Action.Remove:
                    RemoveRange(e.Entities);
                    break;
            }
        }

        public IEnumerable<IPersistEntity> SetRange(IEnumerable<IPersistEntity> entities)
        {
            Clear();
            return AddRange(entities);
        }

        //add without logging
        public IEnumerable<IPersistEntity> AddRange(IEnumerable<IPersistEntity> entities)
        {
            List<IPersistEntity> check = new List<IPersistEntity>();
            foreach (var item in entities) //check all for redundancy
            {
                if (!_selection.Contains(item))
                {
                    _selection.Add(item);
                    check.Add(item);
                }
            }
            OnCollectionChanged(NotifyCollectionChangedAction.Add, check);
            return check;
        }

        //remove without logging
        private IEnumerable<IPersistEntity> RemoveRange(IEnumerable<IPersistEntity> entities)
        {
            List<IPersistEntity> check = new List<IPersistEntity>();

            foreach (var item in entities) //check all for existance
            {
                if (_selection.Contains(item))
                {
                    check.Add(item);
                    _selection.Remove(item);
                }
            }
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, check);
            return check;
        }

        public void Add(IPersistEntity entity)
        {
            if (entity == null)
                return;
            Add(new[] { entity });
        }

        public void Add(IEnumerable<IPersistEntity> entity)
        {
            IEnumerable<IPersistEntity> check = AddRange(entity);
            if (_selectionLog == null)
                return;
            _selectionLog.Add(new SelectionEvent { Action = Action.Add, Entities = check });
            ResetLog();
        }

        public void Remove(IPersistEntity entity)
        {
            Remove(new[] { entity });
        }

        public void Remove(IEnumerable<IPersistEntity> entity)
        {
            if (entity == null)
                return;
            IEnumerable<IPersistEntity> check = RemoveRange(entity);
            if (_selectionLog == null)
                return;
            _selectionLog.Add(new SelectionEvent { Action = Action.Remove, Entities = check });
            ResetLog();
        }

        private void ResetLog()
        {
            if (_position == _selectionLog.Count - 2) 
                _position = _selectionLog.Count - 1; //normal transaction
            if (_position < _selectionLog.Count - 2) //there were undo/redo operations and action inbetween must be discarded
            {
                _selectionLog.RemoveRange(_position + 1, _selectionLog.Count - 2);
                _position = _selectionLog.Count - 1;
            }
        }

        private enum Action
        {
            Add,
            Remove
        }

        private struct SelectionEvent
        {
            public Action Action;
            public IEnumerable<IPersistEntity> Entities;
        }


        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedAction action, IList entities)
        {
            if (action != NotifyCollectionChangedAction.Add && action != NotifyCollectionChangedAction.Remove)
                throw new ArgumentException("Only Add and Remove operations are supported");
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, entities));
        }

        public IEnumerator<IPersistEntity> GetEnumerator()
        {
            return _selection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Toggles the selection state of an IPersistEntity
        /// </summary>
        /// <param name="item">the IPersistEntity to add or remove from the selection</param>
        /// <returns>true if added; false if removed</returns>
        public bool Toggle(IPersistEntity item)
        {
            if (_selection.Contains(item))
            {
                Remove(item);
                return false;
            }
            Add(item);
            return true;
        }

        public void Clear()
        {
            // to preserve undo capability
            //
            IPersistEntity[] t = new IPersistEntity[_selection.Count];
            _selection.CopyTo(t, 0);
            RemoveRange(t);
        }
    }
}

#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    UndoRedoSession.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.ComponentModel;

#endregion

namespace Xbim.XbimExtensions.Transactions
{
    /// <summary>
    ///   Represents a session that keeps track of reversible operations making it possible to undo and redo them.
    /// </summary>
    public class UndoRedoSession : Transaction, INotifyPropertyChanged
    {
        /// <summary>
        ///   Creates a new undo redo session without making it active.
        /// </summary>
        public UndoRedoSession()
            : base(null, null)
        {
           
        }

        /// <summary>
        ///   Creates a new undo redo session without making it active.
        /// </summary>
        /// <param name = "sessionName">A name to assign to the session</param>
        public UndoRedoSession(string sessionName)
            : base(null, sessionName)
        {
           
        }

        public Transaction Begin(string operationName, bool useExisting)
        {
            Current = this;
            return Transaction.Begin(operationName);
        }


        /// <summary>
        ///   Makes this UndoRedoSession active and initiates a new transaction in it
        /// </summary>
        /// <param name = "operationName">Name of new Transaction.</param>
        /// <returns>A new Transaction</returns>
        /// <remarks>
        ///   When the returned transaction is commited or rollbacked,
        ///   this UndoSession will still the active Transaction.
        /// </remarks>
        public new Transaction Begin(string operationName)
        {
           
            Current = this;
            return Transaction.Begin(operationName);
        }

        /// <summary>
        ///   Makes this UndoRedoSession active and initiates a new transaction in it
        /// </summary>
        /// <returns></returns>
        public new Transaction Begin()
        {
            return this.Begin(null);
        }

        /// <summary>
        ///   Add an edit operation to this transaction
        /// </summary>
        /// <param name = "edit">Edit to add</param>
        public override void AddEdit(Edit edit)
        {
#if DEBUG
            if (!(edit is Transaction))
                throw new InvalidOperationException("Only transaction edits can be added to an UndoRedoSession");
#endif
            base.AddEdit(edit);
            RaiseHistoryChanged();
        }

        /// <summary>
        ///   Raised whenever the undo history changes.
        /// </summary>
        public event EventHandler HistoryChanged;

        private void RaiseHistoryChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("CanUndo"));
                PropertyChanged(this, new PropertyChangedEventArgs("CanRedo"));
            }

            EventHandler eh = HistoryChanged;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        /// <summary>
        ///   Called when this transaction is wholy or partly reversed. Raises the Reversed and the HistoryChanged events.
        /// </summary>
        protected override void OnReversed()
        {
            base.OnReversed();

            RaiseHistoryChanged();
        }

       
        /// <summary>
        ///   Reverses the last (not previously undone) operation.
        /// </summary>
        public void Undo()
        {
            Undo(1);
        }

        /// <summary>
        ///   Undos a given number of operations
        /// </summary>
        /// <param name = "count">Number of operations to undo.</param>
        public void Undo(int count)
        {
            if (currentIndex > 0)
            {
                // Clears the current transaction during undo operation
                Transaction currentTransaction = Current;
                
                Current = null;

                while (count > 0 && currentIndex > 0)
                {
                    currentIndex--;
                    int i = currentIndex;
                    edits[i] = edits[i].Reverse();
                    count--;
                }

                OnReversed();

                // Restores the current transaction.
               
                Current = currentTransaction;
            }
        }

        /// <summary>
        ///   Returns true if there is anything to undo.
        /// </summary>
        /// <returns></returns>
        public bool CanUndo
        {
            get { return (currentIndex > 0); }
        }

        /// <summary>
        ///   Redos the last undone operation.
        /// </summary>
        public void Redo()
        {
            Redo(1);
        }

        /// <summary>
        ///   Redos a given number of operations.
        /// </summary>
        /// <param name = "count">Number of operations to redo.</param>
        public void Redo(int count)
        {
            if (currentIndex < edits.Count)
            {
                // Clears the current transaction during redo operation.
                Transaction currentTransaction = Current;
               
                Current = null;

                while (count > 0 && currentIndex < edits.Count)
                {
                    int i = currentIndex;
                    edits[i] = edits[i].Reverse();

                    currentIndex++;
                    count--;
                }

                OnReversed();

                // Restores the current operation.
               
                Current = currentTransaction;
            }
        }

        /// <summary>
        ///   Returns true if there is anything that can be redone.
        /// </summary>
        /// <returns></returns>
        public bool CanRedo
        {
            get { return currentIndex < edits.Count; }
        }

        /// <summary>
        ///   Gets the name of the last performed operation
        /// </summary>
        /// <returns>Name of last operation performed (not undone), or null if no operation performed</returns>
        public string GetUndoText()
        {
            if (currentIndex > 0)
            {
                return this.edits[currentIndex - 1].Name;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///   Gets the name of the last undone operation
        /// </summary>
        /// <returns>Name of last operation undone), or null if no operation is undone</returns>
        public string GetRedoText()
        {
            if (currentIndex < edits.Count)
            {
                return this.edits[currentIndex].Name;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        ///   Gets the names for all operations that can be undone starting with the most recently added.
        /// </summary>
        /// <returns>An array of strings</returns>
        public string[] GetUndoTextList
        {
            get
            {
                string[] texts = new string[currentIndex];
                for (int i = 0; i < currentIndex; i++)
                {
                    texts[i] = edits[currentIndex - i - 1].Name;
                }
                return texts;
            }
        }

        /// <summary>
        ///   Gets the names for all operations that can be redone starting with the most recently undone.
        /// </summary>
        /// <returns>An array of strings</returns>
        public string[] GetRedoTextList
        {
            get
            {
                string[] texts = new string[edits.Count - currentIndex];
                for (int i = 0; i < edits.Count - currentIndex; i++)
                {
                    texts[i] = edits[currentIndex + i].Name;
                }
                return texts;
            }
        }


      

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
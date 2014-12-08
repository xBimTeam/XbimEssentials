#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    Transaction.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;

#endregion

namespace Xbim.XbimExtensions.Transactions
{
    public delegate void TransactionReversedNotification();

    public delegate void TransactionFinaliseNotification();

    public delegate void TransactionValidationNotification();

    /// <summary>
    ///   Represents a transaction enabling rollbacks, undoing and redoing of operations in an application.
    /// </summary>
    public class Transaction : Edit, IDisposable
    {
        private static Transaction _current;
        private static bool _rollingBack;


        /// <summary>
        ///   Gets the current transaction
        /// </summary>
        public static Transaction Current
        {
            get { return _rollingBack ? null : _current; }
            protected set { _current = value; }
        }

        public static bool IsRollingBack
        {
            get { return _rollingBack; }
        }

        /// <summary>
        ///   Begins a new transaction (nested in current)
        /// </summary>
        /// <returns></returns>
        public static Transaction Begin()
        {
            return Begin(null);
        }

        /// <summary>
        ///   Begins a new named transaction (nested in current)
        /// </summary>
        /// <param name = "transactionName">Name of transaction/operation</param>
        /// <returns></returns>
        public static Transaction Begin(string transactionName)
        {
            Transaction newTransaction = new Transaction(_current, transactionName);
            if (_current != null)
            {
                _current._currentChild = newTransaction;
            }
            _current = newTransaction;
            return newTransaction;
        }


        /// <summary>
        ///   The parent transaction that this transaction will be added to when commited.
        /// </summary>
        private readonly Transaction parent;

        /// <summary>
        ///   The list of edits added to the transaction
        /// </summary>
        protected readonly List<Edit> edits = new List<Edit>();


        /// <summary>
        ///   The index in edits list to add next edit to.
        /// </summary>
        protected int currentIndex;

        /// <summary>
        ///   The currently active child transaction.
        /// </summary>
        private Transaction _currentChild;

        public Transaction CurrentChild
        {
            get { return _currentChild; }
            set { _currentChild = value; }
        }

        /*
        /// <summary>
        /// A list of post operations to be performed after undoing or redoing.
        /// </summary>
        protected List<PostOperation> postOperations = null;
        */

        /// <summary>
        ///   Fired when some or all operations in this transaction are reversed (undone, redone or rollbacked).
        /// </summary>
        private event TransactionReversedNotification reversed;

        private event TransactionFinaliseNotification finalise;
        private event TransactionValidationNotification validated;

        /// <summary>
        ///   Actions that are invoked when a transaction is closed/finalised
        /// </summary>
        public event TransactionFinaliseNotification Finalised
        {
            add { finalise += value; }
            remove { finalise -= value; }
        }

        public event TransactionReversedNotification Reversed
        {
            add { reversed += value; }
            remove { reversed -= value; }
        }

        public event TransactionValidationNotification Validated
        {
            add { validated += value; }
            remove { validated -= value; }
        }


        /// <summary>
        ///   Creates a new transaction instance without making it active.
        /// </summary>
        /// <param name = "parentTrans">Parent transaction (or null if it is a root transaction)</param>
        /// <param name = "transName">Name of transaction</param>
        protected Transaction(Transaction parentTrans, string transName)
        {
            parent = parentTrans;
            this.transName = transName;
        }

        private readonly string transName;

        /// <summary>
        ///   Gets the name of the transaction
        /// </summary>
        public override string Name
        {
            get { return transName; }
        }

        /// <summary>
        ///   Gets a value indicating whether this transaction has been finished (committed or rollbacked).
        /// </summary>
        public bool IsFinished { get; private set; }

        #region IDisposable Members

        /// <summary>
        ///   Disposes this transaction. If it has not been commited it will be rollbacked.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (!IsFinished)
            {
                Rollback();
            }
        }

        #endregion

        /// <summary>
        ///   Adds a reversible operation to this transaction
        /// </summary>
        /// <param name = "edit">An edit representing the reversible operation to be added.</param>
        public virtual void AddEdit(Edit edit)
        {
            if (IsFinished)
            {
                throw new InvalidOperationException("Cannot add edits to a finished transaction");
            }

            // Remove all from currentIndex, i.e. redone edits 
            edits.RemoveRange(currentIndex, edits.Count - currentIndex);

            edits.Add(edit);
            currentIndex++;
        }

        


       

        /// <summary>
        ///   Reverses all reversible operations performed during this transaction and ends this transaction (making any parent
        ///   transaction current again).
        /// </summary>
        public virtual void Rollback()
        {
            if (IsFinished)
            {
                throw new InvalidOperationException(
                    "Rollback cannot be performed on an finished transaction. Ensure that Rollback is only called once and not after a Commit has been made.");
            }

            IsFinished = true;

            // Clears the current transaction during rollback.
            _current = null;
            _rollingBack = true;

            // Rollback any active child first.
            if (_currentChild != null)
            {
                _currentChild.Rollback();
            }

            // Reverses all operations in this transaction.
            if (currentIndex > 0)
            {
                Reverse();
            }

            // Clears parents current child.
            if (parent != null)
            {
                parent._currentChild = null;
            }

            // Makes the parent transaction the active one.
            _current = parent;
            _rollingBack = false;
            OnReversed();
        }

        /// <summary>
        ///   Mark this transaction as successfully completed. This transaction will be added as a reversible operation in the parent 
        ///   transaction (if any) which also will be the new current transaction.
        /// </summary>
        /// <remarks>
        ///   Empty transactions (i.e. with no edits) will be discarded directly and not added to parent transaction.
        /// </remarks>
        public virtual void Commit()
        {
            if (IsFinished)
            {
                throw new InvalidOperationException(
                    "Commit cannot be performed on a finished transaction. Ensure that Commit is only called once and not after a Rollback has been made.");
            }

            //OnValidated();
            //clear these actions

            validated = null;
            IsFinished = true;

            // Commit any unfinished child transaction.
            if (_currentChild != null)
            {
                _currentChild.Commit();
            }

            if (parent != null)
            {
                parent._currentChild = null;

                if (edits.Count > 0)
                {
                    parent.AddEdit(this);
                }
            }

            // Make the parent transaction the current transaction.
            _current = parent;
            OnFinalised();
        }

        /// <summary>
        ///   Makes this transaction the current one (or its current child transaction).
        /// </summary>
        public void Enter()
        {
            Transaction leafTransaction = this;
            while (leafTransaction._currentChild != null)
            {
                leafTransaction = leafTransaction._currentChild;
            }
            _current = leafTransaction;
        }

        /// <summary>
        ///   Leaves this transaction making none transaction active if this transaction (or one of its children) is currently active.
        /// </summary>
        /// <remarks>
        ///   Call Enter() to switch back to the transaction.
        /// </remarks>
        public void Exit()
        {
            Transaction txn = _current;
            while (txn != null)
            {
                if (txn == this)
                {
                    _current = null;
                    return;
                }
                txn = txn.parent;
            }
        }

        /// <summary>
        ///   Reverses all operation in this transaction (either undos or redos all operation depending on state).
        /// </summary>
        /// <returns>Edit that represents the reverse operation</returns>
        public override Edit Reverse()
        {
            if (currentIndex > 0)
            {
                for (int i = currentIndex - 1; i >= 0; i--)
                {
                    edits[i] = edits[i].Reverse();
                }
                currentIndex = 0;
            }
            else
            {
                for (int i = 0; i < edits.Count; i++)
                {
                    edits[i] = edits[i].Reverse();
                }
                currentIndex = edits.Count;
            }
            OnReversed();
            return this;
        }

        /// <summary>
        ///   Called when this transaction is wholy or partly reversed. Raised the Reversed event.
        /// </summary>
        protected virtual void OnReversed()
        {
            if (reversed != null)
            {
                reversed();
                reversed = null;
            }
        }

        /// <summary>
        ///   Called when this transaction is about to be committed .
        /// </summary>
        protected virtual void OnFinalised()
        {
            if (finalise != null)
            {
                finalise();
                finalise = null;
            }
        }

        /// <summary>
        ///   Called when this transaction is committed to execution any transaction wide validation routines.
        /// </summary>
        protected virtual void OnValidated()
        {
            if (validated != null)
            {
                validated();
            }
        }

        /// <summary>
        ///   Adds a reversible property change to the current transaction (if any).
        /// </summary>
        /// <typeparam name = "TOwner">Type that owns the property</typeparam>
        /// <typeparam name = "TProperty">Type of property</typeparam>
        /// <param name = "setter">A method that sets the property</param>
        /// <param name = "instance">The instance whose property is changed</param>
        /// <param name = "oldValue">The present value of the property</param>
        /// <param name = "newValue">The new value to be assigned to the property</param>
        public static void AddPropertyChange<TOwner, TProperty>(ReversiblePropertySetter<TOwner, TProperty> setter,
                                                                TOwner instance, TProperty oldValue, TProperty newValue)
            where TOwner : class
        {
            Transaction txn = _current;
            if (txn != null)
            {
                txn.AddEdit(new ReversiblePropertyEdit<TOwner, TProperty>(setter, instance, oldValue, newValue));
            }
        }


        /// <summary>
        ///   Adds a reversible property change to the current transaction (if any).
        /// </summary>
        /// <param name = "setter">A instance method (delegate) that sets the property of to a given value</param>
        /// <typeparam name = "TProperty">Type of property</typeparam>
        /// <param name = "oldValue">The present value of the property</param>
        /// <param name = "newValue">The new value to be assigned to the property</param>
        public static void AddPropertyChange<TProperty>(ReversibleInstancePropertySetter<TProperty> setter,
                                                        TProperty oldValue, TProperty newValue)
        {
            Transaction txn = _current;
            if (txn != null)
            {
                txn.AddEdit(new ReversibleInstancePropertyEdit<TProperty>(setter, oldValue, newValue));
            }
        }


        public static void AddTransactionReversedHandler(TransactionReversedNotification handler)
        {
            Transaction txn = _current;
            if (txn != null)
            {
                txn.Reversed += handler;
            }
        }

        /// <summary>
        ///   Provide a pair of operations that when both are applied leave no change to the intended target
        /// </summary>
        /// <typeparam name = "TClass"></typeparam>
        /// <param name = "op1"></param>
        /// <param name = "op2"></param>
        public static void AddReversibleOperation<TClass>(ReversibleOperation<TClass> op1,
                                                          ReversibleOperation<TClass> op2)
        {
            Transaction txn = _current;
            if (txn != null)
            {
                txn.AddEdit(new ReversibleOperationEdit<TClass>(op1, op2));
            }
        }

        /// <summary>
        ///   Call this handler to add any functions that are called before a transaction is called, typically to tidy up any outstanding operations.
        ///   Note these functions will be called both prior to Rollback and prior to Commit
        /// </summary>
        /// <param name = "handler"></param>
        public static void AddTransactionFinaliseHandler(TransactionFinaliseNotification handler)
        {
            Transaction txn = _current;
            if (txn != null)
            {
                txn.Finalised += handler;
            }
        }

        public static void AddTransactionValidationHandler(TransactionValidationNotification handler)
        {
            Transaction txn = _current;
            if (txn != null)
            {
                txn.Validated += handler;
            }
        }
    }
}
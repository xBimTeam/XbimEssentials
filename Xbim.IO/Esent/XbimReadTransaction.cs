using System;
using Xbim.Common;

namespace Xbim.IO.Esent
{
    /// <summary>
    /// A transaction allowing read only operations on a model
    /// </summary>
    public class XbimReadTransaction : ITransaction
    {
        /// <summary>
        /// to detect redundant calls
        /// </summary>
        private bool _disposed; 
        /// <summary>
        /// True if we are in a transaction.
        /// </summary>
        protected bool InTransaction;
        protected EsentModel Model;
        private EsentReadOnlyTransaction _readTransaction;

        protected XbimReadTransaction()
        {

        }

        internal XbimReadTransaction(EsentModel theModel, EsentReadOnlyTransaction txn)
        {
            Model = theModel;
            _readTransaction = txn;
            InTransaction = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                try
                {
                    if (InTransaction) _readTransaction.Dispose();
                }
                finally
                {
                    Model.EndTransaction();
                }                     
                    
            }
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
        }

        string ITransaction.Name
        {
            get { return ""; }
        }

        void ITransaction.Commit()
        {
            throw new Exception("This is a read-only transaction so you can't commit anything.");
        }

        void ITransaction.RollBack()
        {
            throw new Exception("This is a read-only transaction so you can't commit anything.");
        }

        void ITransaction.DoReversibleAction(Action doAction, Action undoAction, IPersistEntity entity, ChangeType changeType, int property)
        {
            throw new Exception("This is a read-only transaction so you can't commit anything.");
        }

        public event EntityChangedHandler EntityChanged;
        public event EntityChangingHandler EntityChanging;

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

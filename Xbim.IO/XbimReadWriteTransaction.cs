using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.IO
{
    /// <summary>
    /// A transaction allowing read and write operations on a model
    /// </summary>
    public class XbimReadWriteTransaction : XbimReadTransaction
    {
        private XbimLazyDBTransaction readWriteTransaction;
        private int _pulseCount;
        private int _transactionBatchSize;

        /// <summary>
        /// The maximum number of pulse actions before a transaction is automatically commited, default is 100 
        /// </summary>
        public int TransactionBatchSize
        {
            get { return _transactionBatchSize; }
            set { _transactionBatchSize = value; }
        }

        internal XbimReadWriteTransaction(XbimModel theModel, XbimLazyDBTransaction txn)
        {
            model = theModel;
            readWriteTransaction = txn;
            inTransaction = true;
            _pulseCount = 0;
            _transactionBatchSize = 100;
        }

        /// <summary>
        /// Increments the pulse count for the transaction, if the pulse count exceed the Transaction batch size the transaction is committed and restarted
        /// </summary>
        /// <returns></returns>
        public int Pulse()
        {
            Interlocked.Increment(ref _pulseCount);
            long remainder = _pulseCount % _transactionBatchSize;
            if (remainder == _transactionBatchSize - 1)
            {
                this.Commit();
                this.Begin();
            }
            return _pulseCount;
        }
        public void Commit()
        {
            try
            {
                model.Flush();
                readWriteTransaction.Commit();
            }
            finally
            {
                inTransaction = false;
            }
        }

        public void Begin()
        {
            try
            {
                readWriteTransaction.Begin();
            }
            finally
            {
                inTransaction = true;
            }
        }


        protected override void Dispose(bool disposing)
        {
            try
            {
                if (inTransaction) readWriteTransaction.Dispose();
            }
            finally
            {
                inTransaction = false;
                base.Dispose(disposing);
            }
        }

        public IEnumerable<IPersistIfcEntity> Modified()
        {
            return model.Cache.Modified();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.IO
{
    /// <summary>
    /// A transaction allowing read only operations on a model
    /// </summary>
    public class XbimReadTransaction : IDisposable
    {
        /// <summary>
        /// to detect redundant calls
        /// </summary>
        private bool disposed = false; 
        /// <summary>
        /// True if we are in a transaction.
        /// </summary>
        protected bool inTransaction = false;
        protected XbimModel model;
        private XbimReadOnlyDBTransaction readTransaction;

        protected XbimReadTransaction()
        {

        }

        internal XbimReadTransaction(XbimModel theModel, XbimReadOnlyDBTransaction txn)
        {
            model = theModel;
            readTransaction = txn;
            inTransaction = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    try
                    {
                        if (inTransaction) readTransaction.Dispose();
                    }
                    finally
                    {
                        model.EndTransaction();
                    }                     
                    
                }
                disposed = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
    }
}

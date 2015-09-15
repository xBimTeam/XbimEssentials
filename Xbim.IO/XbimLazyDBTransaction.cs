using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Isam.Esent.Interop;
using Xbim.Common.Logging;

namespace Xbim.IO
{
    /// <summary>
    /// Used for wrapping a Database Lazy Transaction, if commit is not called the Dispose function rolls back the transaction
    /// </summary>
    public struct  XbimLazyDBTransaction : IDisposable
    {
            /// <summary>
            /// The session that has the transaction.
            /// </summary>
            private readonly JET_SESID sesid;

            /// <summary>
            /// True if we are in a transaction.
            /// </summary>
            private bool inTransaction;

            /// <summary>
            /// Initializes a new instance of the <see cref="XbimLazyDBTransaction"/> struct.
            /// </summary>
            /// <param name="sesid">
            /// The sesid.
            /// </param>
            public XbimLazyDBTransaction(JET_SESID sesid)
            {
                this.sesid = sesid;
                Api.JetBeginTransaction(this.sesid);
                this.inTransaction = true;
                LoggerFactory.GetLogger().DebugFormat("New XbimTransaction with session {0}", sesid);
            }

            /// <summary>
            /// Commit the transaction.
            /// </summary>
            public void Commit()
            {
                LoggerFactory.GetLogger().DebugFormat("Committing XbimTransaction with session {0}", sesid);
                Api.JetCommitTransaction(this.sesid, CommitTransactionGrbit.LazyFlush);
                this.inTransaction = false;
            }

            public void Begin()
            {
                Api.JetBeginTransaction(this.sesid);
                this.inTransaction = true;
            }


            /// <summary>
            /// Rollback the transaction if not already committed.
            /// </summary>
            public void Dispose()
            {
                if (this.inTransaction)
                {
                    LoggerFactory.GetLogger().DebugFormat("Rolling back XbimTransaction with session {0}", sesid);
                    Api.JetRollback(this.sesid, RollbackTransactionGrbit.None);
                }
            }

        
    }
}

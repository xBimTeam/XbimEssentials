using System;
using Microsoft.Isam.Esent.Interop;

namespace Xbim.IO.Esent
{
    /// <summary>
    /// Used for wrapping a Database Lazy Transaction, if commit is not called the Dispose function rolls back the transaction
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public struct  EsentLazyDBTransaction : IDisposable
    {
            /// <summary>
            /// The session that has the transaction.
            /// </summary>
            private readonly JET_SESID _sesid;

            /// <summary>
            /// True if we are in a transaction.
            /// </summary>
            private bool _inTransaction;

            /// <summary>
            /// Initializes a new instance of the <see cref="EsentLazyDBTransaction"/> struct.
            /// </summary>
            /// <param name="sesid">
            /// The sesid.
            /// </param>
            public EsentLazyDBTransaction(JET_SESID sesid)
            {
                _sesid = sesid;
                Api.JetBeginTransaction(_sesid);
                _inTransaction = true;
            }

            /// <summary>
            /// Commit the transaction.
            /// </summary>
            public void Commit()
            {
                Api.JetCommitTransaction(_sesid, CommitTransactionGrbit.LazyFlush);
                _inTransaction = false;
            }

            /// <summary>
            /// Commit the transaction.
            /// </summary>
            public void RollBack()
            {
                Api.JetRollback(_sesid, RollbackTransactionGrbit.None);
                _inTransaction = false;
            }

            public void Begin()
            {
                Api.JetBeginTransaction(_sesid);
                _inTransaction = true;
            }


            /// <summary>
            /// Rollback the transaction if not already committed.
            /// </summary>
            public void Dispose()
            {
                if (_inTransaction)
                {
                    Api.JetRollback(_sesid, RollbackTransactionGrbit.None);
                }
            }

        
    }
}

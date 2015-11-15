using System;
using Microsoft.Isam.Esent.Interop;

namespace Xbim.IO.Esent
{
    public struct EsentReadOnlyTransaction: IDisposable
    {
         /// <summary>
        /// The session that has the transaction.
        /// </summary>
        private readonly JET_SESID _sesid;

        /// <summary>
        /// Initializes a new instance of the <see cref="EsentReadOnlyTransaction"/> struct.
        /// </summary>
        /// <param name="sesid">
        /// The sesid.
        /// </param>
        public EsentReadOnlyTransaction(JET_SESID sesid)
        {
            _sesid = sesid;
            Api.JetBeginTransaction2(_sesid, BeginTransactionGrbit.ReadOnly);
        }

        /// <summary>
        /// Rollback the transaction if not already committed.
        /// </summary>
        public void Dispose()
        {
            Api.JetCommitTransaction(_sesid, CommitTransactionGrbit.LazyFlush);
        }
    }
}

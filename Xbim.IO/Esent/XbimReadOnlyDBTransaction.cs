using System;
using Microsoft.Isam.Esent.Interop;

namespace Xbim.IO.Esent
{
    public struct XbimReadOnlyDBTransaction: IDisposable
    {
         /// <summary>
        /// The session that has the transaction.
        /// </summary>
        private readonly JET_SESID sesid;

        /// <summary>
        /// Initializes a new instance of the <see cref="XbimReadOnlyDBTransaction"/> struct.
        /// </summary>
        /// <param name="sesid">
        /// The sesid.
        /// </param>
        public XbimReadOnlyDBTransaction(JET_SESID sesid)
        {
            this.sesid = sesid;
            Api.JetBeginTransaction2(this.sesid, BeginTransactionGrbit.ReadOnly);
        }

        /// <summary>
        /// Rollback the transaction if not already committed.
        /// </summary>
        public void Dispose()
        {
            Api.JetCommitTransaction(this.sesid, CommitTransactionGrbit.LazyFlush);
        }
    }
}

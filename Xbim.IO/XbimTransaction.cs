using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions.Transactions;
using Microsoft.Isam.Esent.Interop;
using System.Diagnostics;
namespace Xbim.IO
{
    public class XbimTransaction : IDisposable
    {
        private UndoRedoSession _undoRedoSession;
        private Microsoft.Isam.Esent.Interop.Transaction _dbTransaction;
        private Session _session;
        private object lockObject;
        private XbimEntityCursor[] _entityTables;
        private const int MaxCachedEntityTables = 32;

        public XbimTransaction()
        {
            _session = new Session(IfcPersistedInstanceCache.JetInstance);
            _dbTransaction = new Microsoft.Isam.Esent.Interop.Transaction(_session);
            _entityTables = new XbimEntityCursor[MaxCachedEntityTables];
        }
        
        public void Dispose()
        {
            //_undoRedoSession.Rollback();
            _dbTransaction.Dispose();
            _session.Dispose();
        }

        public void Begin()
        {
            //_undoRedoSession = new UndoRedoSession();
            _dbTransaction.Begin();
        }

        public void Rollback()
        {
            //_undoRedoSession.Rollback();
            _dbTransaction.Rollback();
        }

        public void Commit()
        {
            //_undoRedoSession.Commit();
            _dbTransaction.Commit(CommitTransactionGrbit.None);
        }

       

       
    }
}

﻿using System;
using System.Text;
using Microsoft.Isam.Esent.Interop;

namespace Xbim.IO.Esent
{
    public abstract class EsentCursor : IDisposable
    {
        protected const int TransactionBatchSize = 100;
        /// <summary>
        /// The ESENT instance the cursor is opened against.
        /// </summary>
        protected readonly JET_INSTANCE Instance;
        /// <summary> 
        /// The ESENT session the cursor is using.
        /// </summary>
        protected readonly Session Sesid;
        /// <summary>
        /// ID of the opened database.
        /// </summary>
        protected readonly JET_DBID DbId;

        /// <summary>
        /// ID of the opened data table.
        /// </summary>
        protected JET_TABLEID Table;

        protected EsentModel Model;
        /// <summary>
        /// Global Table
        /// </summary>
        protected JET_TABLEID GlobalsTable;
        protected static string GlobalsTableName = "MetaData";
        protected static string EntityCountColumnName = "EntityCount";
        protected static string GeometryCountColumnName = "GeometryCount";
        protected static string FlushColumnName = "FlushColumn";
        protected JET_COLUMNID EntityCountColumn;
        protected JET_COLUMNID GeometryCountColumn;
        protected JET_COLUMNID FlushColumn;
        protected JET_COLUMNID IfcHeaderColumn;
        protected static string VersionColumnName = "Version";
        protected static string Version = "2.4.1";


        public Session Session { get { return Sesid; } }
        protected readonly object LockObject;

       
        private static string ifcHeaderColumnName = "IfcHeader";
        private bool disposedValue;

        public bool ReadOnly { get; set; }

        protected EsentCursor(EsentModel model, string database,  OpenDatabaseGrbit mode)
        {
            try
            {
                LockObject = new Object();
                Model = model;
                Instance = model.Cache.JetInstance;
                Sesid = new Session(Instance);
                Api.JetOpenDatabase(Sesid, database, String.Empty, out DbId, mode);
                Api.JetOpenTable(Sesid, DbId, GlobalsTableName, null, 0, mode == OpenDatabaseGrbit.ReadOnly ? OpenTableGrbit.ReadOnly :
                                                                                    mode == OpenDatabaseGrbit.Exclusive ? OpenTableGrbit.DenyWrite : OpenTableGrbit.None,
                                                                                    out GlobalsTable);
                EntityCountColumn = Api.GetTableColumnid(Sesid, GlobalsTable, EntityCountColumnName);
                GeometryCountColumn = Api.GetTableColumnid(Sesid, GlobalsTable, GeometryCountColumnName);
                FlushColumn = Api.GetTableColumnid(Sesid, GlobalsTable, FlushColumnName);
                IfcHeaderColumn = Api.GetTableColumnid(Sesid, GlobalsTable, ifcHeaderColumnName);
                ReadOnly = (mode == OpenDatabaseGrbit.ReadOnly);
            }
            catch
            {
                if (Sesid != null)
                {
                    if (Table != JET_TABLEID.Nil && !Table.IsInvalid)
                        Api.JetCloseTable(Sesid, Table);
                    if (GlobalsTable != JET_TABLEID.Nil && !GlobalsTable.IsInvalid)
                        Api.JetCloseTable(Sesid, GlobalsTable);
                    if (DbId != JET_DBID.Nil && DbId != default(JET_DBID))
                        Api.JetCloseDatabase(Sesid, DbId, CloseDatabaseGrbit.None);
                    Api.JetEndSession(Sesid, EndSessionGrbit.None);
                }
                throw;
            }
        }

        internal abstract int RetrieveCount();

       
       
        protected abstract void UpdateCount(int delta);

        /// <summary>
        /// Create the globals table.
        /// </summary>
        /// <param name="sesid">The session to use.</param>
        /// <param name="dbid">The database to create the table in.</param>
        public static void CreateGlobalsTable(JET_SESID sesid, JET_DBID dbid)
        {
            using (var transaction = new Transaction(sesid))
            {
                JET_TABLEID tableid;
                Api.JetCreateTable(sesid, dbid, GlobalsTableName, 1, 100, out tableid);
                JET_COLUMNID versionColumnid;
                Api.JetAddColumn(
                    sesid,
                    tableid,
                    VersionColumnName,
                    new JET_COLUMNDEF { coltyp = JET_coltyp.LongText },
                    null,
                    0,
                    out versionColumnid);

                var defaultValue = BitConverter.GetBytes(0);

                JET_COLUMNID countColumnid;
                Api.JetAddColumn(
                    sesid,
                    tableid,
                    EntityCountColumnName,
                    new JET_COLUMNDEF { coltyp = JET_coltyp.Long, grbit = ColumndefGrbit.ColumnEscrowUpdate },
                    defaultValue,
                    defaultValue.Length,
                    out countColumnid);

                Api.JetAddColumn(
                    sesid,
                    tableid,
                    GeometryCountColumnName,
                    new JET_COLUMNDEF { coltyp = JET_coltyp.Long, grbit = ColumndefGrbit.ColumnEscrowUpdate },
                    defaultValue,
                    defaultValue.Length,
                    out countColumnid);

                Api.JetAddColumn(
                    sesid,
                    tableid,
                    FlushColumnName,
                    new JET_COLUMNDEF { coltyp = JET_coltyp.Long, grbit = ColumndefGrbit.ColumnEscrowUpdate },
                    defaultValue,
                    defaultValue.Length,
                    out countColumnid);

                Api.JetAddColumn(
                   sesid,
                   tableid,
                   ifcHeaderColumnName,
                   new JET_COLUMNDEF { coltyp = JET_coltyp.LongBinary },
                   null,
                   0,
                   out countColumnid);

                using (var update = new Update(sesid, tableid, JET_prep.Insert))
                {
                    Api.SetColumn(sesid, tableid, versionColumnid, Version, Encoding.Unicode);
                    update.Save();
                }

                Api.JetCloseTable(sesid, tableid);
                transaction.Commit(CommitTransactionGrbit.LazyFlush);
            }
        }

       

        public EsentLazyDBTransaction BeginLazyTransaction()
        {
            return new EsentLazyDBTransaction(Sesid);
        }

        /// <summary>
        /// Begin a new transaction for this cursor.
        /// </summary>
        /// <returns>The new transaction.</returns>
        internal Transaction BeginTransaction()
        {
            return new Transaction(Sesid);
        }
        /// <summary>
        /// Begin a new transaction for this cursor. This is the cheapest
        /// transaction type because it returns a struct and no separate
        /// commit call has to be made.
        /// </summary>
        /// <returns>The new transaction.</returns>
        public EsentReadOnlyTransaction BeginReadOnlyTransaction()
        {
            return new EsentReadOnlyTransaction(Sesid);
        }

        /// <summary>
        /// Generate a null database update that we can wrap in a non-lazy transaction.
        /// </summary>
        internal void Flush()
        {
            using (var transaction = BeginTransaction())
            {
                Api.EscrowUpdate(Sesid, GlobalsTable, FlushColumn, 1);
                transaction.Commit(CommitTransactionGrbit.WaitLastLevel0Commit);
            }
        }

        internal bool TryMoveNext()
        {
            return Api.TryMoveNext(Sesid, Table);
        }

        internal virtual bool TryMoveFirst()
        {
            return Api.TryMoveFirst(Sesid, Table);
        }

        internal bool TryMoveLast()
        {
            return Api.TryMoveLast(Sesid, Table);
        }

        internal void SetCurrentIndex(string indexName)
        {
            Api.JetSetCurrentIndex(Sesid, Table, indexName);
        }

        internal void MoveBeforeFirst()
        {
            Api.MoveBeforeFirst(Sesid, Table); 
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    try
                    {
                        Api.JetCloseTable(Sesid, Table);
                        Api.JetCloseTable(Sesid, GlobalsTable);
                        Api.JetCloseDatabase(Sesid, DbId, CloseDatabaseGrbit.None);
                        Api.JetEndSession(Sesid, EndSessionGrbit.None);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~EsentCursor()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

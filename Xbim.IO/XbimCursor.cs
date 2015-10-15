using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Isam.Esent.Interop;
using Xbim.Common.Logging;

namespace Xbim.IO
{
    public abstract class XbimCursor : IDisposable
    {
        protected const int transactionBatchSize = 100;
        /// <summary>
        /// The ESENT instance the cursor is opened against.
        /// </summary>
        protected readonly JET_INSTANCE instance;
        /// <summary> 
        /// The ESENT session the cursor is using.
        /// </summary>
        protected readonly Session sesid;
        /// <summary>
        /// ID of the opened database.
        /// </summary>
        protected readonly JET_DBID dbId;

        /// <summary>
        /// ID of the opened data table.
        /// </summary>
        protected JET_TABLEID table;

        protected XbimModel model;
        /// <summary>
        /// Global Table
        /// </summary>
        protected JET_TABLEID globalsTable;
        protected static string globalsTableName = "MetaData";
        protected static string entityCountColumnName = "EntityCount";
        protected static string geometryCountColumnName = "GeometryCount";
        protected static string flushColumnName = "FlushColumn";
        protected JET_COLUMNID entityCountColumn;
        protected JET_COLUMNID geometryCountColumn;
        protected JET_COLUMNID flushColumn;
        protected JET_COLUMNID ifcHeaderColumn;
        protected static string versionColumnName = "Version";
        protected static string version = "2.4.1";



        protected readonly object lockObject;

       
        private static string ifcHeaderColumnName = "IfcHeader";

        private readonly ILogger Logger = LoggerFactory.GetLogger();



        public bool ReadOnly { get; set; }
        public XbimCursor(XbimModel model, string database,  OpenDatabaseGrbit mode)
        {
            // TODO: [APW] Should we be using this lock?
            this.lockObject = new Object();
            this.model = model;
            this.instance = model.Cache.JetInstance;
            sesid = new Session(instance);
            Api.JetOpenDatabase(sesid, database, String.Empty, out this.dbId, mode);
            Api.JetOpenTable(this.sesid, this.dbId, globalsTableName, null, 0,  mode == OpenDatabaseGrbit.ReadOnly ? OpenTableGrbit.ReadOnly : 
                                                                                mode == OpenDatabaseGrbit.Exclusive ? OpenTableGrbit.DenyWrite : OpenTableGrbit.None, 
                                                                                out this.globalsTable);
            this.entityCountColumn = Api.GetTableColumnid(this.sesid, this.globalsTable, entityCountColumnName);
            this.geometryCountColumn = Api.GetTableColumnid(this.sesid, this.globalsTable, geometryCountColumnName);
            this.flushColumn = Api.GetTableColumnid(this.sesid, this.globalsTable, flushColumnName);
            this.ifcHeaderColumn = Api.GetTableColumnid(this.sesid, this.globalsTable, ifcHeaderColumnName);
            ReadOnly = (mode == OpenDatabaseGrbit.ReadOnly);

            Logger.DebugFormat("New XbimCursor with session {0}", sesid);
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
            JET_TABLEID tableid;
            JET_COLUMNID versionColumnid;
            JET_COLUMNID countColumnid;
            using (var transaction = new Microsoft.Isam.Esent.Interop.Transaction(sesid))
            {
                Api.JetCreateTable(sesid, dbid, globalsTableName, 1, 100, out tableid);
                Api.JetAddColumn(
                    sesid,
                    tableid,
                    versionColumnName,
                    new JET_COLUMNDEF { coltyp = JET_coltyp.LongText },
                    null,
                    0,
                    out versionColumnid);

                byte[] defaultValue = BitConverter.GetBytes(0);

                Api.JetAddColumn(
                    sesid,
                    tableid,
                    entityCountColumnName,
                    new JET_COLUMNDEF { coltyp = JET_coltyp.Long, grbit = ColumndefGrbit.ColumnEscrowUpdate },
                    defaultValue,
                    defaultValue.Length,
                    out countColumnid);

                Api.JetAddColumn(
                    sesid,
                    tableid,
                    geometryCountColumnName,
                    new JET_COLUMNDEF { coltyp = JET_coltyp.Long, grbit = ColumndefGrbit.ColumnEscrowUpdate },
                    defaultValue,
                    defaultValue.Length,
                    out countColumnid);

                Api.JetAddColumn(
                    sesid,
                    tableid,
                    flushColumnName,
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
                    Api.SetColumn(sesid, tableid, versionColumnid, version, Encoding.Unicode);
                    update.Save();
                }

                Api.JetCloseTable(sesid, tableid);
                transaction.Commit(CommitTransactionGrbit.LazyFlush);
            }
        }

       

        public XbimLazyDBTransaction BeginLazyTransaction()
        {
            return new XbimLazyDBTransaction(this.sesid);
        }

        /// <summary>
        /// Begin a new transaction for this cursor.
        /// </summary>
        /// <returns>The new transaction.</returns>
        internal Transaction BeginTransaction()
        {
            return new Transaction(this.sesid);
        }

        /// <summary>
        /// Begin a new transaction for this cursor. This is the cheapest
        /// transaction type because it returns a struct and no separate
        /// commit call has to be made.
        /// </summary>
        /// <returns>The new transaction.</returns>
        public XbimReadOnlyDBTransaction BeginReadOnlyTransaction()
        {
            return new XbimReadOnlyDBTransaction(this.sesid);
        }

        /// <summary>
        /// Generate a null database update that we can wrap in a non-lazy transaction.
        /// </summary>
        internal void Flush()
        {
            using (var transaction = this.BeginTransaction())
            {
                Api.EscrowUpdate(this.sesid, this.globalsTable, this.flushColumn, 1);
                transaction.Commit(CommitTransactionGrbit.WaitLastLevel0Commit);
            }
        }

        internal bool TryMoveNext()
        {
            return Api.TryMoveNext(this.sesid, this.table);
        }

        internal virtual bool TryMoveFirst()
        {
            return Api.TryMoveFirst(this.sesid, this.table);
        }

        internal bool TryMoveLast()
        {
            return Api.TryMoveLast(this.sesid, this.table);
        }

        internal void SetCurrentIndex(string indexName)
        {
            Api.JetSetCurrentIndex(this.sesid, this.table, indexName);
        }

        internal void MoveBeforeFirst()
        {
            Api.MoveBeforeFirst(sesid, table); 
        }


        virtual public void Dispose()
        {
            try
            {
                Api.JetCloseTable(sesid, table);
                Api.JetCloseTable(sesid, globalsTable);
                Api.JetCloseDatabase(this.sesid, this.dbId, CloseDatabaseGrbit.None);
                Api.JetEndSession(this.sesid, EndSessionGrbit.None);
            }
            catch (Exception e)
            {
                Logger.Warn("Error disposing XbimCursor", e);
            }
            
        }
    }
}

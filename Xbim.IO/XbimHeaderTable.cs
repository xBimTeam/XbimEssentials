using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Isam.Esent.Interop;
using System.IO;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.IO
{
    public class XbimHeaderTable : IDisposable
    {

        Session _jetSession;
        JET_DBID _jetDatabaseId;
        private const string XbimDatabaseVersion = "1.0.0";
        private const string ifcHeaderTableName = "IfcHeader";

        private const string _colNameHeaderData = "HeaderData";
        private const string _colNameEntityCount = "EntityCount";
        private const string _colNameFileVersion = "FileVersion";
        private const string _colNameHeaderId = "HeaderId";

        private Table _jetCursor;

        private JET_COLUMNID _colIdHeaderId;
        private JET_COLUMNID _colIdEntityCount;
        private JET_COLUMNID _colIdFileVersion;
        private JET_COLUMNID _colIdHeaderData;

        Int32ColumnValue _colValHeaderId;
        Int64ColumnValue _colValEntityCount;
        StringColumnValue _colValFileVersion;
        BytesColumnValue _colValHeaderData;

        /// <summary>
        /// Constructs and opens a header table
        /// </summary>
        /// <param name="jetSession"></param>
        /// <param name="jetDatabaseId"></param>
        /// <param name="mode"></param>
        public XbimHeaderTable(Session jetSession, JET_DBID jetDatabaseId, OpenTableGrbit mode)
        {
            _jetSession = jetSession;
            _jetDatabaseId = jetDatabaseId;
            Open(mode);
        }
        /// <summary>
        /// Constructs but does not open a header table
        /// </summary>
        /// <param name="jetSession"></param>
        /// <param name="jetDatabaseId"></param>
        public XbimHeaderTable(Session jetSession, JET_DBID jetDatabaseId)
        {
            _jetSession = jetSession;
            _jetDatabaseId = jetDatabaseId;
        }
        internal static void CreateTable(Session session, JET_DBID dbid)
        {
            JET_TABLEID tableid;
            Api.JetCreateTable(session, dbid, ifcHeaderTableName, 1, 100, out tableid);

            using (var transaction = new Microsoft.Isam.Esent.Interop.Transaction(session))
            {
                JET_COLUMNID columnid;
                
                var columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.Long,
                    grbit = ColumndefGrbit.ColumnAutoincrement
                };
                Api.JetAddColumn(session, tableid, _colNameHeaderId, columndef, null, 0, out columnid);
                columndef.coltyp = JET_coltyp.Currency;
                columndef.grbit = ColumndefGrbit.ColumnNotNULL;
                Api.JetAddColumn(session, tableid, _colNameEntityCount, columndef, null, 0, out columnid);
                
                columndef.coltyp = JET_coltyp.LongBinary;
            
                columndef.grbit = ColumndefGrbit.ColumnNotNULL;
                Api.JetAddColumn(session, tableid, _colNameHeaderData, columndef, null, 0, out columnid);
                columndef.coltyp = JET_coltyp.Text;
                columndef.grbit = ColumndefGrbit.ColumnNotNULL;
                columndef.cbMax = 32;

                Api.JetAddColumn(session, tableid, _colNameFileVersion, columndef, null, 0, out columnid);
                transaction.Commit(CommitTransactionGrbit.LazyFlush);
            }
        }

        public void Open(OpenTableGrbit mode)
        {
            _jetCursor = new Table(_jetSession, _jetDatabaseId, ifcHeaderTableName, mode);
            InitColumns();
        }

        private void InitColumns()
        {
           // IDictionary<string, JET_COLUMNID> columnids = Api.GetColumnDictionary(_jetSession, _jetCursor);
            _colIdHeaderId = Api.GetTableColumnid(_jetSession, _jetCursor, _colNameHeaderId);
            _colIdEntityCount = Api.GetTableColumnid(_jetSession, _jetCursor, _colNameEntityCount);
            _colIdFileVersion = Api.GetTableColumnid(_jetSession, _jetCursor, _colNameFileVersion);
            _colIdHeaderData = Api.GetTableColumnid(_jetSession, _jetCursor, _colNameHeaderData);
            
            _colValHeaderId = new Int32ColumnValue { Columnid = _colIdHeaderId };
            _colValEntityCount = new Int64ColumnValue { Columnid = _colIdEntityCount };
            _colValFileVersion = new StringColumnValue { Columnid = _colIdFileVersion };
            _colValHeaderData = new BytesColumnValue { Columnid = _colIdHeaderData };
           

        }
        public void Close()
        {
            Api.JetCloseTable(_jetSession, _jetCursor);
            _jetCursor = null;
        }

        public void Dispose()
        {
            if(_jetCursor!=null) Close();
        }

        public IfcFileHeader IfcFileHeader
        {
            get
            {
                Api.JetSetCurrentIndex(_jetSession, _jetCursor, null);
                Api.TryMoveFirst(_jetSession, _jetCursor); //this should never fail
                byte[] hd = Api.RetrieveColumn(_jetSession, _jetCursor, _colIdHeaderData);
                BinaryReader br = new BinaryReader(new MemoryStream(hd));
                IfcFileHeader hdr = new IfcFileHeader();
                hdr.Read(br);
                return hdr;
            }
        }

        public long EntityCount
        {
            get
            {
                Api.JetSetCurrentIndex(_jetSession, _jetCursor, null);
                Api.TryMoveFirst(_jetSession, _jetCursor); //this should never fail
                long? cnt = Api.RetrieveColumnAsInt64(_jetSession, _jetCursor, _colIdEntityCount);
                if (cnt.HasValue) 
                    return cnt.Value;
                else
                    return -1;
            }

        }

        public string DatabaseVersion
        {
            get
            {
                Api.JetSetCurrentIndex(_jetSession, _jetCursor, null);
                Api.TryMoveFirst(_jetSession, _jetCursor); //this should never fail
                return  Api.RetrieveColumnAsString(_jetSession, _jetCursor, _colIdFileVersion);
                
            }

        }

        internal void UpdateHeader(IIfcFileHeader ifcFileHeader, long entityCount)
        {
            MemoryStream ms = new MemoryStream(4096);
            BinaryWriter bw = new BinaryWriter(ms);
            ifcFileHeader.Write(bw);
            if (!Api.TryMoveFirst(_jetSession, _jetCursor)) //there is nothing in
            {
                using (var update = new Update(_jetSession, _jetCursor, JET_prep.Insert))
                {
                    Api.SetColumn(_jetSession, _jetCursor, _colIdHeaderData, ms.ToArray());
                    Api.SetColumn(_jetSession, _jetCursor, _colIdEntityCount, entityCount);
                    Api.SetColumn(_jetSession, _jetCursor, _colIdFileVersion, XbimDatabaseVersion, Encoding.ASCII);
                    update.Save();
                }
            }
            else
            {
                using (var update = new Update(_jetSession, _jetCursor, JET_prep.Replace))
                {
                    Api.SetColumn(_jetSession, _jetCursor, _colIdHeaderData, ms.ToArray());
                    Api.SetColumn(_jetSession, _jetCursor, _colIdEntityCount, entityCount);
                    Api.SetColumn(_jetSession, _jetCursor, _colIdFileVersion, XbimDatabaseVersion, Encoding.ASCII);
                    update.Save();
                }
            }
        }
    }

}

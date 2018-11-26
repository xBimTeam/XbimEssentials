using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Isam.Esent.Interop;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Common.Metadata;
using Xbim.Common.Step21;

namespace Xbim.IO.Esent
{
    public class EsentEntityCursor : EsentCursor
    {
        private ILogger _logger;

        private const string ifcEntityTableName = "IfcEntities";
        private const string entityTableTypeLabelIndex = "EntByTypeLabel";

        private const string entityTableLabelIndex = "EntByLabel";
        private const string colNameEntityLabel = "EntityLabel";
        private const string colNameIfcType = "IfcType";
        private const string colNameEntityData = "EntityData";
        private const string colNameIsIndexedClass = "IsIndexedClass";


        private JET_COLUMNID _colIdEntityLabel;

        private JET_COLUMNID _colIdIfcType;
        private JET_COLUMNID _colIdEntityData;
        private JET_COLUMNID _colIdIsIndexedClass;
        Int32ColumnValue _colValEntityLabel;
        Int16ColumnValue _colValTypeId;
        BytesColumnValue _colValData;
        BoolColumnValue _colValIsIndexedClass;
        ColumnValue[] _colValues;

        //Index Table
        private static string ifcEntityIndexTableName = "IfcEntitiesIndex";
        private const string colNameSecondaryKey = "SecondaryKey";
        private JET_TABLEID _indexTable;
        private JET_COLUMNID _colIdIdxIfcType;
        private JET_COLUMNID _colIdIdxKey;
        private JET_COLUMNID _colIdIdxEntityLabel;
        Int16ColumnValue _colValIdxIfcType;
        Int32ColumnValue _colValIdxKey;
        Int32ColumnValue _colValIdxEntityLabel;
        ColumnValue[] _colIdxValues;


        public ColumnValue[] ColumnValues
        {
            get
            {
                return _colValues;
            }
        }

        public static implicit operator JET_TABLEID(EsentEntityCursor table)
        {
            return table;
        }

        public override void Dispose()
        {
            try
            {
                Api.JetCloseTable(Sesid, _indexTable);
            }
            catch (Exception)
            {
            }
            base.Dispose();
        }

        internal static void CreateTable(JET_SESID sesid, JET_DBID dbid)
        {
            JET_TABLEID tableid;


            using (var transaction = new Transaction(sesid))
            {
                Api.JetCreateTable(sesid, dbid, ifcEntityTableName, 8, 100, out tableid);

                JET_COLUMNID columnid;
                //Add the primary key, Entity Label
                var columndef = new JET_COLUMNDEF { coltyp = JET_coltyp.Long, grbit = ColumndefGrbit.ColumnNotNULL };
                Api.JetAddColumn(sesid, tableid, colNameEntityLabel, columndef, null, 0, out columnid);

                // Identity of the type of the object : 16-bit integer looked up in IfcType Table
                columndef = new JET_COLUMNDEF { coltyp = JET_coltyp.Short, grbit = ColumndefGrbit.ColumnMaybeNull };
                Api.JetAddColumn(sesid, tableid, colNameIfcType, columndef, null, 0, out columnid);


                //The properties of the entity
                columndef = new JET_COLUMNDEF { coltyp = JET_coltyp.LongBinary, grbit = ColumndefGrbit.ColumnMaybeNull };
                //if(EsentVersion.SupportsWindows7Features) columndef.grbit |= Windows7Grbits.ColumnCompressed;
                Api.JetAddColumn(sesid, tableid, colNameEntityData, columndef, null, 0, out columnid);

                //Flag to say if this class is to be indexed by type
                columndef = new JET_COLUMNDEF { coltyp = JET_coltyp.Bit, grbit = ColumndefGrbit.None };
                Api.JetAddColumn(sesid, tableid, colNameIsIndexedClass, columndef, null, 0, out columnid);

                //Primary Key index
                var labelIndexDef = string.Format("+{0}\0\0", colNameEntityLabel);
                Api.JetCreateIndex(sesid, tableid, entityTableLabelIndex, CreateIndexGrbit.IndexPrimary, labelIndexDef, labelIndexDef.Length, 100);
                Api.JetCloseTable(sesid, tableid);
                transaction.Commit(CommitTransactionGrbit.LazyFlush);
            }

            ////Now add the conditional index for retrieval by type
            //using (var transaction = new Transaction(sesid))
            //{
            //    Api.JetOpenTable(sesid, dbid, ifcEntityTableName, null, 0, OpenTableGrbit.DenyRead, out tableid);
            //    string typeIndexDef = string.Format("+{0}\0{1}\0\0", colNameIfcType, colNameEntityLabel);

            //    JET_INDEXCREATE[] indexes = new[]
            //    {
            //        new JET_INDEXCREATE
            //        {
            //            szIndexName = entityTableTypeLabelIndex,
            //            szKey = typeIndexDef,
            //            cbKey = typeIndexDef.Length,
            //            rgconditionalcolumn = new[]
            //            {
            //                new JET_CONDITIONALCOLUMN
            //                {
            //                    szColumnName = colNameIsIndexedClass,
            //                    grbit = ConditionalColumnGrbit.ColumnMustBeNonNull
            //                }
            //            },
            //            cConditionalColumn = 1,
            //            ulDensity=100,
            //            grbit = CreateIndexGrbit.IndexUnique
            //        }
            //    };

            //    Api.JetCreateIndex2(sesid, tableid, indexes, indexes.Length);
            //    Api.JetCloseTable(sesid, tableid);
            //    transaction.Commit(CommitTransactionGrbit.LazyFlush);
            //}

            //Now create a table for the indexed properties
            using (var transaction = new Transaction(sesid))
            {
                Api.JetCreateTable(sesid, dbid, ifcEntityIndexTableName, 8, 100, out tableid);
                JET_COLUMNID columnid;
                // Identity of the type of the object : 16-bit integer looked up in IfcType Table
                var columndef = new JET_COLUMNDEF { coltyp = JET_coltyp.Short, grbit = ColumndefGrbit.ColumnNotNULL };
                Api.JetAddColumn(sesid, tableid, colNameIfcType, columndef, null, 0, out columnid);
                // Name of the secondary key : for lookup by a property value of the object that is a foreign object
                columndef = new JET_COLUMNDEF { coltyp = JET_coltyp.Long, grbit = ColumndefGrbit.ColumnNotNULL };
                Api.JetAddColumn(sesid, tableid, colNameSecondaryKey, columndef, null, 0, out columnid);
                //Add the entity Label
                Api.JetAddColumn(sesid, tableid, colNameEntityLabel, columndef, null, 0, out columnid);

                //Add the primary key, Entity Type, Index label and Entity Label 
                var labelIndexDef = string.Format("+{0}\0{1}\0{2}\0\0", colNameIfcType, colNameSecondaryKey, colNameEntityLabel);
                Api.JetCreateIndex(sesid, tableid, entityTableLabelIndex, CreateIndexGrbit.IndexPrimary, labelIndexDef, labelIndexDef.Length, 100);
                Api.JetCloseTable(sesid, tableid);
                transaction.Commit(CommitTransactionGrbit.LazyFlush);
            }
        }

        private void InitColumns()
        {

            // The main entity Table
            _colIdEntityLabel = Api.GetTableColumnid(Sesid, Table, colNameEntityLabel);

            _colIdIfcType = Api.GetTableColumnid(Sesid, Table, colNameIfcType);
            _colIdEntityData = Api.GetTableColumnid(Sesid, Table, colNameEntityData);
            _colIdIsIndexedClass = Api.GetTableColumnid(Sesid, Table, colNameIsIndexedClass);
            _colValEntityLabel = new Int32ColumnValue { Columnid = _colIdEntityLabel };
            _colValTypeId = new Int16ColumnValue { Columnid = _colIdIfcType };

            _colValData = new BytesColumnValue { Columnid = _colIdEntityData };
            _colValIsIndexedClass = new BoolColumnValue { Columnid = _colIdIsIndexedClass };
            _colValues = new ColumnValue[] { _colValEntityLabel, _colValTypeId, _colValData, _colValIsIndexedClass };

            //The index table
            _colIdIdxIfcType = Api.GetTableColumnid(Sesid, _indexTable, colNameIfcType);
            _colValIdxIfcType = new Int16ColumnValue { Columnid = _colIdIdxIfcType };
            _colIdIdxKey = Api.GetTableColumnid(Sesid, _indexTable, colNameSecondaryKey);
            _colValIdxKey = new Int32ColumnValue { Columnid = _colIdIdxKey };
            _colIdIdxEntityLabel = Api.GetTableColumnid(Sesid, _indexTable, colNameEntityLabel);
            _colValIdxEntityLabel = new Int32ColumnValue { Columnid = _colIdIdxEntityLabel };
            _colIdxValues = new ColumnValue[] { _colValIdxIfcType, _colValIdxKey, _colValIdxEntityLabel };

        }

        public EsentEntityCursor(EsentModel model, string database)
            : this(model, database, OpenDatabaseGrbit.None)
        {
        }
        /// <summary>
        /// Constructs a table and opens it
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="database"></param>
        public EsentEntityCursor(EsentModel model, string database, OpenDatabaseGrbit mode)
            : base(model, database, mode)
        {
            _logger = XbimLogging.CreateLogger<EsentEntityCursor>(); ;
            Api.JetOpenTable(Sesid, DbId, ifcEntityTableName, null, 0,
                mode == OpenDatabaseGrbit.ReadOnly ? OpenTableGrbit.ReadOnly :
                mode == OpenDatabaseGrbit.Exclusive ? OpenTableGrbit.DenyWrite : OpenTableGrbit.None, out Table);
            Api.JetOpenTable(Sesid, DbId, ifcEntityIndexTableName, null, 0,
                mode == OpenDatabaseGrbit.ReadOnly ? OpenTableGrbit.ReadOnly :
                mode == OpenDatabaseGrbit.Exclusive ? OpenTableGrbit.DenyWrite : OpenTableGrbit.None, out _indexTable);
            InitColumns();
        }


        /// <summary>
        /// Sets the values of the fields, no update is performed
        /// </summary>
        /// <param name="primaryKey">The label of the entity</param>
        /// <param name="type">The index of the type of the entity</param>
        /// <param name="data">The property data</param>
        internal void SetEntityRowValues(int primaryKey, short type, byte[] data, bool? index)
        {
            //SRL need to upgrade store to uint
            _colValEntityLabel.Value = primaryKey;
            _colValTypeId.Value = type;
            _colValData.Value = data;
            _colValIsIndexedClass.Value = index;
        }

        /// <summary>
        /// Sets the values prior to update to write to the entity index table 
        /// </summary>
        /// <param name="primaryKey">The entity label</param>
        /// <param name="type">The Ifc Type ID</param>
        /// <param name="indexKey">The secondary key to index by</param>
        internal void SetEntityIndexRowValues(short type, int indexKey, int primaryKey)
        {
            //SRL we need to change the underlying store to UINT for large models
            _colValIdxEntityLabel.Value = primaryKey;
            _colValIdxIfcType.Value = type;
            _colValIdxKey.Value = indexKey;
        }


        public string PrimaryIndex { get { return entityTableTypeLabelIndex; } }

        public JET_COLUMNID ColIdEntityLabel { get { return _colIdEntityLabel; } }

        public JET_COLUMNID ColIdIfcType { get { return _colIdIfcType; } }



        public JET_COLUMNID ColIdEntityData { get { return _colIdEntityData; } }

        internal void WriteHeader(IStepFileHeader ifcFileHeader)
        {
            var ms = new MemoryStream(4096);
            var bw = new BinaryWriter(ms);
            ifcFileHeader.Write(bw);
            if (Api.TryMoveFirst(Sesid, GlobalsTable))
            {
                using (var update = new Update(Sesid, GlobalsTable, JET_prep.Replace))
                {
                    Api.SetColumn(Sesid, GlobalsTable, IfcHeaderColumn, ms.ToArray());
                    update.Save();
                }
            }

        }

        internal IStepFileHeader ReadHeader()
        {

            if (Api.TryMoveFirst(Sesid, GlobalsTable))
            {
                var hd = Api.RetrieveColumn(Sesid, GlobalsTable, IfcHeaderColumn);
                if (hd == null) return null;//there is nothing in at the moment
                var br = new BinaryReader(new MemoryStream(hd));
                var hdr = new StepFileHeader(StepFileHeader.HeaderCreationMode.LeaveEmpty, Model);
                hdr.Read(br);
                return hdr;
            }
            else
                return null;

        }

        /// <summary>
        /// Updates an entity, assumes a valid transaction is running
        /// </summary>
        /// <param name="toWrite"></param>
        internal void UpdateEntity(IPersistEntity toWrite)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            toWrite.WriteEntity(bw, Model.Metadata);
            var expressType = Model.Metadata.ExpressType(toWrite);
            UpdateEntity(toWrite.EntityLabel, expressType.TypeId, expressType.GetIndexedValues(toWrite), ms.ToArray(), expressType.IndexedClass);
        }

        /// <summary>
        /// Updates an entity, assumes a valid transaction is running
        /// </summary>
        /// <param name="currentLabel">Primary key/label</param>
        /// <param name="typeId">Type identifer</param>
        /// <param name="indexKeys">Search keys to use specifiy null if no indices</param>
        /// <param name="data">property data</param>
        internal void UpdateEntity(int currentLabel, short typeId, IEnumerable<int> indexKeys, byte[] data, bool? indexed)
        {
            try
            {
                if (indexed.HasValue && indexed.Value == false) indexed = null;

                if (!TrySeekEntityLabel(currentLabel)) throw new XbimException("Attempt to update an entity that does not exist in the model");

                using (var update = new Update(Sesid, Table, JET_prep.Replace))
                {
                    //first put a record in with a null type key
                    SetEntityRowValues(currentLabel, typeId, data, indexed);
                    Api.SetColumns(Sesid, Table, _colValues);
                    update.Save();
                }

                if (indexed.HasValue && indexed.Value == true && !TrySeekEntityType(typeId, -1, currentLabel)) //this should be there
                    throw new XbimException("It is illegal to change an entities type, please delete and insert the entity as a new type");

                //TODO delete any existing keys, not strictly necessary as the search engine checks the compliance of every entity with the search criteria after retrieval
                //Only need to implement delete to tidy the database up, may be better just to reindex and compact

                //now add in any search keys
                if (indexKeys != null && indexKeys.Any())
                {
                    //set the main variables of label and type just ones
                    SetEntityIndexRowValues(typeId, -1, currentLabel);
                    var uniqueKeys = indexKeys.Distinct();
                    //SRL need to make keys uint on the store
                    foreach (var key in uniqueKeys.Cast<int>())
                    {
                        if (!TrySeekEntityType(typeId, key, currentLabel)) //already got it just skip
                        {
                            using (var update = new Update(Sesid, _indexTable, JET_prep.Insert))
                            {
                                _colValIdxKey.Value = key;
                                Api.SetColumns(Sesid, _indexTable, _colIdxValues);
                                update.Save();
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {

                throw new XbimException("Error updating an entity in the database", e);
            }

        }

        /// <summary>
        /// Adds an entity, assumes a valid transaction is running
        /// </summary>
        /// <param name="toWrite"></param>
        internal void AddEntity(IPersistEntity toWrite)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            toWrite.WriteEntity(bw, Model.Metadata);
            var expressType = Model.Metadata.ExpressType(toWrite);
            AddEntity(toWrite.EntityLabel, expressType.TypeId, expressType.GetIndexedValues(toWrite), ms.ToArray(), expressType.IndexedClass);
        }


        /// <summary>
        /// Adds an entity, assumes a valid transaction is running
        /// </summary>
        /// <param name="currentLabel">Primary key/label</param>
        /// <param name="typeId">Type identifer</param>
        /// <param name="indexKeys">Search keys to use specifiy null if no indices</param>
        /// <param name="data">property data</param>
        internal void AddEntity(int currentLabel, short typeId, IEnumerable<int> indexKeys, byte[] data, bool? indexed, EsentLazyDBTransaction? trans = null)
        {
            //Debug.WriteLine(currentLabel);
            try
            {
                if (indexed.HasValue && indexed.Value == false)
                    indexed = null;
                using (var update = new Update(Sesid, Table, JET_prep.Insert))
                {
                    
                    try
                    {
                        // this populates the _colValues array 
                        SetEntityRowValues(currentLabel, typeId, data, indexed);
                        Api.SetColumns(Sesid, Table, _colValues);
                        update.Save();
                        UpdateCount(1);
                    }
                    catch (Exception ex)
                    {
                        update.Cancel();
                        var msg = string.Format("Failed to add (probably clashing) entity #{0} to the database", currentLabel);
                        _logger?.LogError(msg, ex);
                        return;
                    }
                }
                //set the main variables of label and type just ones
                SetEntityIndexRowValues(typeId, -1, currentLabel);
                //add -1 record to allow retrieval by type id alone
                if (indexed.HasValue && indexed.Value == true)
                {
                    using (var update = new Update(Sesid, _indexTable, JET_prep.Insert))
                    {
                        Api.SetColumns(Sesid, _indexTable, _colIdxValues);
                        update.Save();
                    }
                }

                //now add in any extra index keys
                if (indexKeys == null) return;

                var transactionCounter = 0;
                //SRL need to upgrade store to uint
                foreach (var key in indexKeys.Distinct())
                {
                    using (var update = new Update(Sesid, _indexTable, JET_prep.Insert))
                    {
                        _colValIdxKey.Value = key;
                        Api.SetColumns(Sesid, _indexTable, _colIdxValues);
                        update.Save();
                        transactionCounter++;
                        if (!trans.HasValue || transactionCounter % 100 != 0) continue;

                        trans.Value.Commit();
                        trans.Value.Begin();
                    }
                }
            }
            catch (Exception e)
            {
                var msg = string.Format("Failed to add entity #{0} to the database", currentLabel);
                throw new XbimException(msg, e);
            }
        }

        /// <summary>
        /// Create a new entity of the specified type, the entity will be blank, all properties with default values
        /// </summary>
        /// <param name="type">Type of entity to create, this must support IPersistEntity</param>
        /// <returns>A handle to the entity</returns>
        internal XbimInstanceHandle AddEntity(Type type)
        {
            //System.Diagnostics.Debug.Assert(typeof(IPersistEntity).IsAssignableFrom(type));
            var highest = RetrieveHighestLabel();
            var expressType = Model.Metadata.ExpressType(type);
            var h = new XbimInstanceHandle(Model, highest + 1, expressType.TypeId);
            AddEntity(h.EntityLabel, h.EntityTypeId, null, null, expressType.IndexedClass);
            return h;
        }

        /// <summary>
        /// Create a new entity of the specified type, the entity will be blank, all properties with default values
        /// The entity label will be as specified, an exception will be raised if the label is already in use
        /// </summary>
        /// <param name="type">Type of entity to create, this must support IPersistIfcEntity</param>
        /// <returns>A handle to the entity</returns>
        internal XbimInstanceHandle AddEntity(Type type, int entityLabel)
        {
            var entityType = Model.Metadata.ExpressType(type);
            var h = new XbimInstanceHandle(Model, entityLabel, entityType.TypeId);
            AddEntity(h.EntityLabel, h.EntityTypeId, null, null, entityType.IndexedClass);
            return h;
        }

        /// <summary>
        /// Returns true if the specified entity label is present in the table, assumes the current index has been set to by primary key (SetPrimaryIndex)
        /// </summary>
        /// <param name="key">The entity label to lookup</param>
        /// <returns></returns>
        public bool TrySeekEntityLabel(int key)
        {

            Api.MakeKey(Sesid, Table, key, MakeKeyGrbit.NewKey);
            return Api.TrySeek(Sesid, Table, SeekGrbit.SeekEQ);
        }
        /// <summary>
        /// Trys to move to the first entity of the specified type, assumes the current index has been set to order by type (SetOrderByType)
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public bool TrySeekEntityType(short typeId, out XbimInstanceHandle ih)
        {
            return TrySeekEntityType(typeId, out ih, -1);

        }

        /// <summary>
        /// Looks up an entity index to see if the secondary key exists
        /// </summary>
        /// <param name="typeId">The Ifc Type</param>
        /// <param name="key">The secondary key</param>
        /// <param name="currentLabel">The indexed entity label</param>
        /// <returns></returns>
        private bool TrySeekEntityType(short typeId, int key, int currentLabel)
        {
            Api.MakeKey(Sesid, _indexTable, typeId, MakeKeyGrbit.NewKey);
            Api.MakeKey(Sesid, _indexTable, key, MakeKeyGrbit.None);
            Api.MakeKey(Sesid, _indexTable, currentLabel, MakeKeyGrbit.None);
            return Api.TrySeek(Sesid, _indexTable, SeekGrbit.SeekEQ);
        }

        /// <summary>
        /// Trys to move to the first entity of the specified type, assumes the current index has been set to order by type (SetOrderByType)
        /// Secondary keys are specific to the type and defined as IfcAttributes in the class declaration
        /// </summary>
        /// <param name="typeId">the type of entity to look up</param>
        /// <param name="lookupKey">Secondary key on the search</param>
        /// <returns>Returns an instance handle to the first or an empty handle if not found</returns>
        public bool TrySeekEntityType(short typeId, out XbimInstanceHandle ih, int lookupKey)
        {
            Api.MakeKey(Sesid, _indexTable, typeId, MakeKeyGrbit.NewKey);
            Api.MakeKey(Sesid, _indexTable, lookupKey, MakeKeyGrbit.None);
            if (Api.TrySeek(Sesid, _indexTable, SeekGrbit.SeekGE))
            {
                Api.MakeKey(Sesid, _indexTable, typeId, MakeKeyGrbit.NewKey);
                Api.MakeKey(Sesid, _indexTable, lookupKey, MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(Sesid, _indexTable, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    ih = new XbimInstanceHandle(Model, Api.RetrieveColumnAsInt32(Sesid, _indexTable, _colIdIdxEntityLabel, RetrieveColumnGrbit.RetrieveFromIndex), Api.RetrieveColumnAsInt16(Sesid, _indexTable, _colIdIdxIfcType, RetrieveColumnGrbit.RetrieveFromIndex));

                    return true;
                }
            }
            ih = new XbimInstanceHandle();
            return false;
        }

        public bool TrySeekEntityType(short typeId)
        {
            Api.MakeKey(Sesid, _indexTable, typeId, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(Sesid, _indexTable, SeekGrbit.SeekGE))
            {
                Api.MakeKey(Sesid, _indexTable, typeId, MakeKeyGrbit.NewKey);
                return Api.TrySetIndexRange(Sesid, _indexTable, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive);
            }
            return false;
        }


        /// <summary>
        /// returns the instance handle for the object at the current cursor position. Assumes the index has been set to the correct position
        /// and the current index is SetOrderByType
        /// </summary>
        /// <returns></returns>
        internal XbimInstanceHandle GetInstanceHandle()
        {
            var label = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdEntityLabel);
            var typeId = Api.RetrieveColumnAsInt16(Sesid, Table, _colIdIfcType);
            return new XbimInstanceHandle(Model, label.Value, typeId.Value);

        }
        /// <summary>
        /// Gets the property values of the entity from the current record
        /// </summary>
        /// <returns>byte array of the property data in binary ifc format</returns>
        internal byte[] GetProperties()
        {
            return Api.RetrieveColumn(Sesid, Table, _colIdEntityData);

        }


        /// <summary>
        /// Retrieve the count of entity items in the database from the globals table.
        /// </summary>
        /// <returns>The number of items in the database.</returns>
        override internal int RetrieveCount()
        {
            return (int)Api.RetrieveColumnAsInt32(Sesid, GlobalsTable, EntityCountColumn);
        }

        /// <summary>
        /// Update the count of entity in the globals table. This is done with EscrowUpdate
        /// so that there won't be any write conflicts.
        /// </summary>
        /// <param name="delta">The delta to apply to the count.</param>
        override protected void UpdateCount(int delta)
        {
            Api.EscrowUpdate(Sesid, GlobalsTable, EntityCountColumn, delta);
        }

        internal int RetrieveHighestLabel()
        {
            if (TryMoveLast())
            {
                var val = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdEntityLabel, RetrieveColumnGrbit.RetrieveFromIndex);
                if (val.HasValue) return val.Value;
            }
            return 0;
        }


        /// <summary>
        /// Returns the id of the current ifc type
        /// </summary>
        /// <returns></returns>
        public short GetIfcType()
        {
            var typeId = Api.RetrieveColumnAsInt16(Sesid, Table, _colIdIfcType);
            if (typeId.HasValue)
                return typeId.Value;
            else
                return 0;
        }

        /// <summary>
        /// Returns the current enity label from the curos of the main entity table
        /// </summary>
        /// <returns></returns>
        public int GetLabel()
        {
            var label = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdEntityLabel);
            if (label.HasValue)
                return label.Value;
            else
                return 0;
        }


        /// <summary>
        /// For use only on the index table, accesses data from the index only
        /// </summary>
        /// <param name="ih"></param>
        /// <returns></returns>
        public bool TryMoveNextEntityType(out XbimInstanceHandle ih)
        {
            if (Api.TryMoveNext(Sesid, _indexTable))
            {
                ih = new XbimInstanceHandle(Model, Api.RetrieveColumnAsInt32(Sesid, _indexTable, _colIdIdxEntityLabel, RetrieveColumnGrbit.RetrieveFromIndex), Api.RetrieveColumnAsInt16(Sesid, _indexTable, _colIdIdxIfcType, RetrieveColumnGrbit.RetrieveFromIndex));
                return true;
            }
            else
            {
                ih = new XbimInstanceHandle();
                return false;
            }
        }
        /// <summary>
        /// Iterates over the main entity table, access data from the index only
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        internal bool TryMoveFirstLabel(out int label)
        {
            if (TryMoveFirst())
            {
                label = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdEntityLabel, RetrieveColumnGrbit.RetrieveFromIndex).Value;
                return true;
            }
            else
            {
                label = 0;
                return false;
            }
        }
        /// <summary>
        /// Iterates over the main entity table, access data from the index only
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        internal bool TryMoveNextLabel(out int label)
        {
            if (TryMoveNext())
            {
                //SRL we need to remove storage dependency on int
                label = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdEntityLabel, RetrieveColumnGrbit.RetrieveFromIndex).Value;
                return true;
            }
            else
            {
                label = 0;
                return false;
            }
        }



    }

    internal class XbimInstancesLabelEnumerator : IEnumerator<int>, IEnumerator
    {
        private PersistedEntityInstanceCache cache;
        private EsentEntityCursor cursor;
        private int current;

        public XbimInstancesLabelEnumerator(PersistedEntityInstanceCache cache)
        {
            this.cache = cache;
            cursor = cache.GetEntityTable();
            Reset();
        }
        public int Current
        {
            get { return current; }
        }


        public void Reset()
        {
            cursor.MoveBeforeFirst();
            current = 0;
        }



        object IEnumerator.Current
        {
            get { return current; }
        }

        bool IEnumerator.MoveNext()
        {
            int label;
            if (cursor.TryMoveNextLabel(out label))
            {
                current = label;
                return true;
            }
            else
                return false;
        }


        public void Dispose()
        {
            cache.FreeTable(cursor);
        }
    }

    internal class XbimInstancesEntityEnumerator : IEnumerator<IPersistEntity>, IEnumerator
    {
        private PersistedEntityInstanceCache cache;
        private EsentEntityCursor cursor;
        private int current;

        public XbimInstancesEntityEnumerator(PersistedEntityInstanceCache cache)
        {
            this.cache = cache;
            cursor = cache.GetEntityTable();
            Reset();
        }
        public IPersistEntity Current
        {
            get { return cache.GetInstance(current); }
        }


        public void Reset()
        {
            cursor.MoveBeforeFirst();
            current = 0;
        }


        object IEnumerator.Current
        {
            get { return cache.GetInstance(current); }
        }

        bool IEnumerator.MoveNext()
        {
            int label;
            if (!cursor.TryMoveNextLabel(out label)) return false;

            current = label;
            return true;
        }


        public void Dispose()
        {
            cache.FreeTable(cursor);
        }
    }
}
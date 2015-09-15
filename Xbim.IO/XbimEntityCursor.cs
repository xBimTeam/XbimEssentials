using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Isam.Esent.Interop;
using Xbim.XbimExtensions.Interfaces;

using System.IO;
using Microsoft.Isam.Esent.Interop.Windows7;
using Xbim.Common.Exceptions;
using System.Collections;

namespace Xbim.IO
{
    public class XbimEntityCursor : XbimCursor
    {

        
        private  const string ifcEntityTableName = "IfcEntities";
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

        public static implicit operator JET_TABLEID (XbimEntityCursor table)
        {
            return table;
        }

        public override void Dispose()
        {
            try
            {
                Api.JetCloseTable(sesid, this._indexTable);
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
                var columndef = new JET_COLUMNDEF {coltyp = JET_coltyp.Long, grbit = ColumndefGrbit.ColumnNotNULL};
                Api.JetAddColumn(sesid, tableid, colNameEntityLabel, columndef, null, 0, out columnid);
               
                // Identity of the type of the object : 16-bit integer looked up in IfcType Table
                columndef = new JET_COLUMNDEF{coltyp = JET_coltyp.Short,grbit = ColumndefGrbit.ColumnMaybeNull};
                Api.JetAddColumn(sesid, tableid, colNameIfcType, columndef, null, 0, out columnid);


                //The properties of the entity
                columndef = new JET_COLUMNDEF {coltyp = JET_coltyp.LongBinary,grbit = ColumndefGrbit.ColumnMaybeNull};
                //if(EsentVersion.SupportsWindows7Features) columndef.grbit |= Windows7Grbits.ColumnCompressed;
                Api.JetAddColumn(sesid, tableid, colNameEntityData, columndef, null, 0, out columnid);
                
                //Flag to say if this class is to be indexed by type
                columndef = new JET_COLUMNDEF{coltyp = JET_coltyp.Bit,grbit = ColumndefGrbit.None};
                Api.JetAddColumn(sesid, tableid, colNameIsIndexedClass, columndef, null, 0, out columnid);
                
                //Primary Key index
                string labelIndexDef = string.Format("+{0}\0\0", colNameEntityLabel);
                Api.JetCreateIndex(sesid, tableid, entityTableLabelIndex, CreateIndexGrbit.IndexPrimary, labelIndexDef, labelIndexDef.Length,100);
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
                var columndef = new JET_COLUMNDEF{coltyp = JET_coltyp.Short,grbit = ColumndefGrbit.ColumnNotNULL};  
                Api.JetAddColumn(sesid, tableid, colNameIfcType, columndef, null, 0, out columnid);
                // Name of the secondary key : for lookup by a property value of the object that is a foreign object
                columndef = new JET_COLUMNDEF { coltyp = JET_coltyp.Long, grbit = ColumndefGrbit.ColumnNotNULL }; 
                Api.JetAddColumn(sesid, tableid, colNameSecondaryKey, columndef, null, 0, out columnid);
                //Add the entity Label
                Api.JetAddColumn(sesid, tableid, colNameEntityLabel, columndef, null, 0, out columnid);

                //Add the primary key, Entity Type, Index label and Entity Label 
                string labelIndexDef = string.Format("+{0}\0{1}\0{2}\0\0", colNameIfcType, colNameSecondaryKey, colNameEntityLabel);
                Api.JetCreateIndex(sesid, tableid, entityTableLabelIndex, CreateIndexGrbit.IndexPrimary, labelIndexDef, labelIndexDef.Length, 100);
                Api.JetCloseTable(sesid, tableid);
                transaction.Commit(CommitTransactionGrbit.LazyFlush);
            }
        }
        
        private void InitColumns()
        {

           // The main entity Table
            _colIdEntityLabel = Api.GetTableColumnid(sesid, table, colNameEntityLabel);
            
            _colIdIfcType = Api.GetTableColumnid(sesid, table, colNameIfcType);
            _colIdEntityData = Api.GetTableColumnid(sesid, table, colNameEntityData);
            _colIdIsIndexedClass = Api.GetTableColumnid(sesid, table, colNameIsIndexedClass);
            _colValEntityLabel = new Int32ColumnValue { Columnid = _colIdEntityLabel };
            _colValTypeId = new Int16ColumnValue { Columnid = _colIdIfcType };
            
            _colValData = new BytesColumnValue { Columnid = _colIdEntityData };
            _colValIsIndexedClass = new BoolColumnValue { Columnid = _colIdIsIndexedClass };
            _colValues = new ColumnValue[] { _colValEntityLabel, _colValTypeId, _colValData, _colValIsIndexedClass };

            //The index table
            _colIdIdxIfcType = Api.GetTableColumnid(sesid, _indexTable, colNameIfcType);
            _colValIdxIfcType = new Int16ColumnValue { Columnid = _colIdIdxIfcType };
            _colIdIdxKey = Api.GetTableColumnid(sesid, _indexTable, colNameSecondaryKey);
            _colValIdxKey = new Int32ColumnValue { Columnid = _colIdIdxKey };
            _colIdIdxEntityLabel = Api.GetTableColumnid(sesid, _indexTable, colNameEntityLabel);
            _colValIdxEntityLabel = new Int32ColumnValue { Columnid = _colIdIdxEntityLabel };
            _colIdxValues = new ColumnValue[] { _colValIdxIfcType, _colValIdxKey, _colValIdxEntityLabel };

        }

        public XbimEntityCursor(XbimModel model, string database)
            : this(model, database, OpenDatabaseGrbit.None)
        {
        }
        /// <summary>
        /// Constructs a table and opens it
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="database"></param>
        public XbimEntityCursor(XbimModel model, string database, OpenDatabaseGrbit mode)
            : base(model, database, mode)
        {
            Api.JetOpenTable(this.sesid, this.dbId, ifcEntityTableName, null, 0, 
                mode == OpenDatabaseGrbit.ReadOnly ? OpenTableGrbit.ReadOnly :
                mode == OpenDatabaseGrbit.Exclusive ? OpenTableGrbit.DenyWrite : OpenTableGrbit.None, out this.table);
            Api.JetOpenTable(this.sesid, this.dbId, ifcEntityIndexTableName, null, 0,
                mode == OpenDatabaseGrbit.ReadOnly ? OpenTableGrbit.ReadOnly :
                mode == OpenDatabaseGrbit.Exclusive ? OpenTableGrbit.DenyWrite : OpenTableGrbit.None, out this._indexTable);    
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

        internal void WriteHeader(IIfcFileHeader ifcFileHeader)
        {
            MemoryStream ms = new MemoryStream(4096);
            BinaryWriter bw = new BinaryWriter(ms);
            ifcFileHeader.Write(bw);    
            if (Api.TryMoveFirst(sesid, globalsTable)) 
            {
                using (var update = new Update(sesid, globalsTable, JET_prep.Replace))
                {
                    Api.SetColumn(sesid, globalsTable, ifcHeaderColumn, ms.ToArray());
                    update.Save(); 
                }
            }
           
        }

        internal IIfcFileHeader ReadHeader()
        {
            
            if (Api.TryMoveFirst(sesid, globalsTable)) 
            {
                byte[] hd = Api.RetrieveColumn(sesid, globalsTable, ifcHeaderColumn);
                if (hd == null) return null;//there is nothing in at the moment
                BinaryReader br = new BinaryReader(new MemoryStream(hd));
                IfcFileHeader hdr = new IfcFileHeader(IfcFileHeader.HeaderCreationMode.LeaveEmpty);
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
        internal void UpdateEntity(IPersistIfcEntity toWrite)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            toWrite.WriteEntity(bw);
            IfcType ifcType = IfcMetaData.IfcType(toWrite);
            UpdateEntity(toWrite.EntityLabel, ifcType.TypeId, ifcType.GetIndexedValues(toWrite), ms.ToArray(), ifcType.IndexedClass);
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
                
                using (var update = new Update(sesid, table, JET_prep.Replace))
                {
                    //first put a record in with a null type key
                    SetEntityRowValues(currentLabel, typeId, data, indexed);
                    Api.SetColumns(sesid, table, _colValues);
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
                    IEnumerable<int> uniqueKeys = indexKeys.Distinct();
                    //SRL need to make keys uint on the store
                    foreach (var key in uniqueKeys.Cast<int>())
                    {
                        if (!TrySeekEntityType(typeId, key, currentLabel)) //already got it just skip
                        {
                            using (var update = new Update(sesid, _indexTable, JET_prep.Insert))
                            {
                                _colValIdxKey.Value = key;
                                Api.SetColumns(sesid, _indexTable, _colIdxValues);
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
        internal void AddEntity(IPersistIfcEntity toWrite)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            toWrite.WriteEntity(bw);
            IfcType ifcType = IfcMetaData.IfcType(toWrite);
            AddEntity(toWrite.EntityLabel, ifcType.TypeId, ifcType.GetIndexedValues(toWrite), ms.ToArray(), ifcType.IndexedClass);
        }
        

        /// <summary>
        /// Adds an entity, assumes a valid transaction is running
        /// </summary>
        /// <param name="currentLabel">Primary key/label</param>
        /// <param name="typeId">Type identifer</param>
        /// <param name="indexKeys">Search keys to use specifiy null if no indices</param>
        /// <param name="data">property data</param>
        internal void AddEntity(int currentLabel, short typeId, IEnumerable<int> indexKeys, byte[] data, bool? indexed, XbimLazyDBTransaction? trans = null)
        {
            try
            {
                if (indexed.HasValue && indexed.Value == false) indexed = null;
                using (var update = new Update(sesid, table, JET_prep.Insert))
                {
                    //first put a record in with a null type key
                    SetEntityRowValues(currentLabel, typeId, data, indexed);
                    Api.SetColumns(sesid, table, _colValues);
                    update.Save();
                    UpdateCount(1);
                }
                //set the main variables of label and type just ones
                SetEntityIndexRowValues(typeId, -1, currentLabel);
                //add -1 record to allow retrieval by type id alone
                if (indexed.HasValue && indexed.Value == true) 
                {
                    using (var update = new Update(sesid, _indexTable, JET_prep.Insert))
                    {
                        Api.SetColumns(sesid, _indexTable, _colIdxValues);
                        update.Save();
                    }
                }

                //now add in any extra index keys
                if (indexKeys != null && indexKeys.Any())
                {
                    int transactionCounter = 0;
                    //SRL need to upgrade store to uint
                    foreach (var key in indexKeys.Distinct())
                    {
                        using (var update = new Update(sesid, _indexTable, JET_prep.Insert))
                        {
                            _colValIdxKey.Value = (int)key;
                            Api.SetColumns(sesid, _indexTable, _colIdxValues);
                            update.Save();
                            transactionCounter++;
                            if (trans.HasValue && transactionCounter % 100 == 0)
                            {
                                trans.Value.Commit();
                                trans.Value.Begin();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

                throw new XbimException("Failed to add entity to the database", e);
            }

        }

        /// <summary>
        /// Create a new entity of the specified type, the entity will be blank, all properties with default values
        /// </summary>
        /// <param name="type">Type of entity to create, this must support IPersistIfcEntity</param>
        /// <returns>A handle to the entity</returns>
        internal XbimInstanceHandle AddEntity(Type type)
        {
            //System.Diagnostics.Debug.Assert(typeof(IPersistIfcEntity).IsAssignableFrom(type));
            int highest = RetrieveHighestLabel();
            IfcType ifcType = IfcMetaData.IfcType(type);
            XbimInstanceHandle h = new XbimInstanceHandle(this.model, highest + 1, ifcType.TypeId);
            AddEntity(h.EntityLabel, h.EntityTypeId, null, null, ifcType.IndexedClass);
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
            IfcType ifcType = IfcMetaData.IfcType(type);
            XbimInstanceHandle h = new XbimInstanceHandle(this.model, entityLabel, ifcType.TypeId);
            AddEntity(h.EntityLabel, h.EntityTypeId, null, null, ifcType.IndexedClass);
            return h;
        }


        /// <summary>
        /// Returns true if the specified entity label is present in the table, assumes the current index has been set to by primary key (SetPrimaryIndex)
        /// </summary>
        /// <param name="key">The entity label to lookup</param>
        /// <returns></returns>
        public bool TrySeekEntityLabel(int key)
        {
           
            Api.MakeKey(sesid, table, key, MakeKeyGrbit.NewKey);
            return Api.TrySeek(this.sesid, this.table, SeekGrbit.SeekEQ);
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
            Api.MakeKey(sesid, _indexTable, typeId, MakeKeyGrbit.NewKey);
            Api.MakeKey(sesid, _indexTable, key, MakeKeyGrbit.None);
            Api.MakeKey(sesid, _indexTable, currentLabel, MakeKeyGrbit.None);
            return Api.TrySeek(sesid, _indexTable, SeekGrbit.SeekEQ);
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
            Api.MakeKey(sesid, _indexTable, typeId, MakeKeyGrbit.NewKey);
            Api.MakeKey(sesid, _indexTable, lookupKey, MakeKeyGrbit.None);
            if (Api.TrySeek(sesid, _indexTable, SeekGrbit.SeekGE))
            {
                Api.MakeKey(sesid, _indexTable, typeId, MakeKeyGrbit.NewKey);
                Api.MakeKey(sesid, _indexTable, lookupKey, MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(sesid, _indexTable, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    ih = new XbimInstanceHandle(this.model, Api.RetrieveColumnAsInt32(sesid, _indexTable, _colIdIdxEntityLabel, RetrieveColumnGrbit.RetrieveFromIndex), Api.RetrieveColumnAsInt16(sesid, _indexTable, _colIdIdxIfcType, RetrieveColumnGrbit.RetrieveFromIndex));
                    
                    return true;
                }
            }
            ih = new XbimInstanceHandle();
            return false;
        }

        public bool TrySeekEntityType(short typeId)
        {
            Api.MakeKey(sesid, _indexTable, typeId, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(sesid, _indexTable, SeekGrbit.SeekGE))
            {
                Api.MakeKey(sesid, _indexTable, typeId, MakeKeyGrbit.NewKey);
                return Api.TrySetIndexRange(sesid, _indexTable, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive);
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
            int? label = Api.RetrieveColumnAsInt32(sesid, table, _colIdEntityLabel);
            short? typeId = Api.RetrieveColumnAsInt16(sesid, table, _colIdIfcType);
            return new XbimInstanceHandle(this.model, label.Value, typeId.Value);
            
        }
        /// <summary>
        /// Gets the property values of the entity from the current record
        /// </summary>
        /// <returns>byte array of the property data in binary ifc format</returns>
        internal byte[] GetProperties()
        {
            return Api.RetrieveColumn(sesid, table, _colIdEntityData);
           
        }

        
        /// <summary>
        /// Retrieve the count of entity items in the database from the globals table.
        /// </summary>
        /// <returns>The number of items in the database.</returns>
        override internal int RetrieveCount()
        {
            return (int)Api.RetrieveColumnAsInt32(this.sesid, this.globalsTable, this.entityCountColumn);
        }

        /// <summary>
        /// Update the count of entity in the globals table. This is done with EscrowUpdate
        /// so that there won't be any write conflicts.
        /// </summary>
        /// <param name="delta">The delta to apply to the count.</param>
        override protected void UpdateCount(int delta)
        {
            Api.EscrowUpdate(this.sesid, this.globalsTable, this.entityCountColumn, delta);
        }

        internal int RetrieveHighestLabel()
        { 
            if (TryMoveLast())
            {
                int? val = Api.RetrieveColumnAsInt32(sesid, table, _colIdEntityLabel, RetrieveColumnGrbit.RetrieveFromIndex);
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
            short? typeId = Api.RetrieveColumnAsInt16(sesid, table, _colIdIfcType);
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
            int? label = Api.RetrieveColumnAsInt32(sesid, table, _colIdEntityLabel);
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
            if (Api.TryMoveNext(this.sesid, this._indexTable))
            {
                ih = new XbimInstanceHandle(this.model, Api.RetrieveColumnAsInt32(sesid, _indexTable, _colIdIdxEntityLabel, RetrieveColumnGrbit.RetrieveFromIndex), Api.RetrieveColumnAsInt16(sesid, _indexTable, _colIdIdxIfcType, RetrieveColumnGrbit.RetrieveFromIndex));
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
                label = Api.RetrieveColumnAsInt32(sesid, table, _colIdEntityLabel, RetrieveColumnGrbit.RetrieveFromIndex).Value;
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
                label = Api.RetrieveColumnAsInt32(sesid, table, _colIdEntityLabel, RetrieveColumnGrbit.RetrieveFromIndex).Value;
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
        private IfcPersistedInstanceCache cache;
        private XbimEntityCursor cursor;
        private int current;

        public XbimInstancesLabelEnumerator(IfcPersistedInstanceCache cache)
        {
            this.cache = cache;
            this.cursor = cache.GetEntityTable();
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

    internal class XbimInstancesEntityEnumerator : IEnumerator<IPersistIfcEntity>, IEnumerator
    {
        private IfcPersistedInstanceCache cache;
        private XbimEntityCursor cursor;
        private int current;

        public XbimInstancesEntityEnumerator(IfcPersistedInstanceCache cache)
        {
            this.cache = cache;
            this.cursor = cache.GetEntityTable();
            Reset();
        }
        public IPersistIfcEntity Current
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
}
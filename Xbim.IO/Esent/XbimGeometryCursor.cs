using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Isam.Esent.Interop;
using Xbim.Common.Exceptions;
using Xbim.Common.Geometry;

#if CREATEGEOMHASH
using System.Security.Cryptography;
#endif

namespace Xbim.IO.Esent
{
    public class XbimGeometryCursor : EsentCursor
    {
       
    
        //geometry fields
        public static string GeometryTableName = "Geometry";

        const string GeometryTablePrimaryIndex = "GeomPrimaryIndex";
        const string GeometryTableGeomTypeIndex= "GeomTypeIndex";
        const string GeometryTableStyleIndex = "GeomStyleIndex";
        const string GeometryTableHashIndex = "GeomHashIndex";

        const string ColNameGeometryLabel = "GeometryLabel";
        const string ColNameProductLabel = "GeomProductLabel";
        const string ColNameGeomType = "GeomType";
        const string ColNameProductIfcTypeId = "GeomIfcType";
        const string ColNameSubPart = "GeomSubPart";     
        const string ColNameTransformMatrix = "GeomTransformMatrix";
        const string ColNameShapeData = "GeomShapeData";
        const string ColNameGeometryHash = "GeomGeometryHash";
        const string ColNameStyleLabel = "GeomRepStyleLabel";
     
        private JET_COLUMNID _colIdProductLabel;
        private JET_COLUMNID _colIdGeometryLabel;
        private JET_COLUMNID _colIdGeomType;
        private JET_COLUMNID _colIdProductIfcTypeId;
        private JET_COLUMNID _colIdGeometryHash;
        private JET_COLUMNID _colIdShapeData;
        private JET_COLUMNID _colIdSubPart;
        private JET_COLUMNID _colIdTransformMatrix;
        private JET_COLUMNID _colIdStyleLabel;
        Int32ColumnValue _colValGeometryLabel;
        Int32ColumnValue _colValProductLabel;
        ByteColumnValue _colValGeomType;
        Int16ColumnValue _colValProductIfcTypeId;
        Int16ColumnValue _colValSubPart;
        BytesColumnValue _colValTransformMatrix;
        BytesColumnValue _colValShapeData;
        Int32ColumnValue _colValGeometryHash;
        Int32ColumnValue _colValStyleLabel;
        ColumnValue[] _colValues;

#if CREATEGEOMHASH
        //Create a hash provider
        private static SHA1 Sha = new SHA1CryptoServiceProvider();
#endif
        

        internal static void CreateTable(JET_SESID sesid, JET_DBID dbid)
        {
            JET_TABLEID tableid;
            Api.JetCreateTable(sesid, dbid, GeometryTableName, 8, 80, out tableid);

            using (var transaction = new Transaction(sesid))
            {
                JET_COLUMNID columnid;

                var columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.Long,
                    grbit = ColumndefGrbit.ColumnAutoincrement
                };

                Api.JetAddColumn(sesid, tableid, ColNameGeometryLabel, columndef, null, 0, out columnid);

                columndef.grbit = ColumndefGrbit.ColumnNotNULL;

                Api.JetAddColumn(sesid, tableid, ColNameProductLabel, columndef, null, 0, out columnid);

                columndef.coltyp = JET_coltyp.UnsignedByte;
                Api.JetAddColumn(sesid, tableid, ColNameGeomType, columndef, null, 0, out columnid);
                
                columndef.coltyp = JET_coltyp.Short;
                Api.JetAddColumn(sesid, tableid, ColNameProductIfcTypeId, columndef, null, 0, out columnid);
                Api.JetAddColumn(sesid, tableid, ColNameSubPart, columndef, null, 0, out columnid);
               

                columndef.coltyp = JET_coltyp.Binary;
                columndef.grbit = ColumndefGrbit.ColumnMaybeNull;
                Api.JetAddColumn(sesid, tableid, ColNameTransformMatrix, columndef, null, 0, out columnid);
               
                columndef.coltyp = JET_coltyp.LongBinary;
                //if (EsentVersion.SupportsWindows7Features)
                //    columndef.grbit |= Windows7Grbits.ColumnCompressed;
                Api.JetAddColumn(sesid, tableid, ColNameShapeData, columndef, null, 0, out columnid);

                columndef.coltyp = JET_coltyp.Long;
                columndef.grbit = ColumndefGrbit.ColumnNotNULL;
                Api.JetAddColumn(sesid, tableid, ColNameGeometryHash, columndef, null, 0, out columnid);
                columndef.grbit = ColumndefGrbit.ColumnNotNULL;
                Api.JetAddColumn(sesid, tableid, ColNameStyleLabel, columndef, null, 0, out columnid);
                // The primary index is the type and the entity label.
                var indexDef = string.Format("+{0}\0\0", ColNameGeometryLabel);
                Api.JetCreateIndex(sesid, tableid, GeometryTablePrimaryIndex, CreateIndexGrbit.IndexPrimary, indexDef, indexDef.Length, 100);
                //create index by geometry hashes    
                indexDef = string.Format("+{0}\0\0", ColNameGeometryHash);
                Api.JetCreateIndex(sesid, tableid, GeometryTableHashIndex, CreateIndexGrbit.IndexDisallowNull, indexDef, indexDef.Length, 100);
                //Create index by product
                indexDef = string.Format("+{0}\0{1}\0{2}\0{3}\0{4}\0\0", ColNameGeomType, ColNameProductIfcTypeId, ColNameProductLabel, ColNameSubPart, ColNameStyleLabel);
                Api.JetCreateIndex(sesid, tableid, GeometryTableGeomTypeIndex, CreateIndexGrbit.IndexUnique, indexDef, indexDef.Length, 100);
                //create index by style
                indexDef = string.Format("+{0}\0{1}\0{2}\0{3}\0{4}\0\0", ColNameGeomType, ColNameStyleLabel, ColNameProductIfcTypeId, ColNameProductLabel, ColNameGeometryLabel);
                Api.JetCreateIndex(sesid, tableid, GeometryTableStyleIndex, CreateIndexGrbit.None, indexDef, indexDef.Length, 100);
                Api.JetCloseTable(sesid, tableid);
                transaction.Commit(CommitTransactionGrbit.LazyFlush);
            }
           
        }
        public string PrimaryIndex { get { return GeometryTablePrimaryIndex; } }

        private void InitColumns()
        {

            _colIdGeometryLabel = Api.GetTableColumnid(Sesid, Table, ColNameGeometryLabel);
            _colIdGeomType = Api.GetTableColumnid(Sesid, Table, ColNameGeomType);
            _colIdProductIfcTypeId = Api.GetTableColumnid(Sesid, Table, ColNameProductIfcTypeId);
            _colIdProductLabel = Api.GetTableColumnid(Sesid, Table, ColNameProductLabel);
            _colIdSubPart = Api.GetTableColumnid(Sesid, Table, ColNameSubPart);
            _colIdTransformMatrix = Api.GetTableColumnid(Sesid, Table, ColNameTransformMatrix);
            _colIdShapeData = Api.GetTableColumnid(Sesid, Table, ColNameShapeData);
            _colIdGeometryHash = Api.GetTableColumnid(Sesid, Table, ColNameGeometryHash);
            _colIdStyleLabel = Api.GetTableColumnid(Sesid, Table, ColNameStyleLabel);

            _colValGeometryLabel = new Int32ColumnValue { Columnid = _colIdGeometryLabel };
            _colValGeomType = new ByteColumnValue { Columnid = _colIdGeomType };
            _colValProductIfcTypeId = new Int16ColumnValue { Columnid = _colIdProductIfcTypeId };
            _colValProductLabel = new Int32ColumnValue { Columnid = _colIdProductLabel };
            _colValSubPart = new Int16ColumnValue { Columnid = _colIdSubPart };
            _colValTransformMatrix = new BytesColumnValue { Columnid = _colIdTransformMatrix };
            _colValShapeData = new BytesColumnValue { Columnid = _colIdShapeData };
            _colValGeometryHash = new Int32ColumnValue { Columnid = _colIdGeometryHash };
            _colValStyleLabel = new Int32ColumnValue { Columnid = _colIdStyleLabel };
            _colValues = new ColumnValue[] { _colValGeomType, _colValProductLabel, _colValProductIfcTypeId, _colValSubPart, _colValTransformMatrix, _colValShapeData, _colValGeometryHash , _colValStyleLabel};

        }
        public XbimGeometryCursor(EsentModel model, string database)
            : this(model, database, OpenDatabaseGrbit.None)
        {
        }
        public XbimGeometryCursor(EsentModel model, string database, OpenDatabaseGrbit mode)
            : base(model, database, mode)
        {
            Api.JetOpenTable(Sesid, DbId, GeometryTableName, null, 0, mode == OpenDatabaseGrbit.ReadOnly ? OpenTableGrbit.ReadOnly :
                                                                                mode == OpenDatabaseGrbit.Exclusive ? OpenTableGrbit.DenyWrite : OpenTableGrbit.None,
                                                                                out Table);
            InitColumns();
        }

       

        /// <summary>
        /// Adds a geometry record and returns the hash of the geometry data
        /// </summary>
        /// <param name="prodLabel"></param>
        /// <param name="type"></param>
        /// <param name="expressType"></param>
        /// <param name="transform"></param>
        /// <param name="shapeData"></param>
        /// <param name="subPart"></param>
        /// <param name="styleLabel"></param>
        /// <param name="geometryHash"></param>
        /// <returns></returns>
        public int AddGeometry(int prodLabel, XbimGeometryType type, short expressType, byte[] transform, byte[] shapeData, short subPart = 0, int styleLabel = 0, int? geometryHash = null)
        {

            var mainId = -1;
            using (var update = new Update(Sesid, Table, JET_prep.Insert))
            {

                _colValProductLabel.Value = prodLabel;
                _colValGeomType.Value = (Byte)type;
                _colValProductIfcTypeId.Value = expressType;
                _colValSubPart.Value = subPart;
                _colValTransformMatrix.Value = transform;
                _colValShapeData.Value = shapeData;
                _colValGeometryHash.Value = geometryHash ?? 0;// geomHash;
                // if (styleLabel > 0)
                _colValStyleLabel.Value = styleLabel;
                // else
                //     _colValStyleLabel.Value = -expressType; //use the negative type id as a style for object that have no render material
                Api.SetColumns(Sesid, Table, _colValues);

                try
                {
                    var columnAsInt32 = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdGeometryLabel, RetrieveColumnGrbit.RetrieveCopy);
                    if (
                        columnAsInt32 !=
                        null)
                        mainId = columnAsInt32.Value;
                    update.Save();
                    UpdateCount(1);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error saving geometry for: " + prodLabel + "Error is " + ex.Message);
                }

            }
#if CREATEGEOMHASH
            if (type == XbimGeometryType.TriangulatedMesh && shapeData.Length > 0)
            {
                int hashId;
                Api.JetSetCurrentIndex(sesid, table, geometryTablePrimaryIndex);
                Api.MakeKey(sesid, table, mainId, MakeKeyGrbit.NewKey);
                Api.TrySeek(sesid, table, SeekGrbit.SeekEQ);

                //Create a hash provider
                var Sha = new SHA1CryptoServiceProvider();
                Byte[] hashdata = Sha.ComputeHash(shapeData);
                using (var update = new Update(sesid, table, JET_prep.InsertCopy))
                {
                    Api.SetColumn(sesid, table, _colIdShapeData, hashdata); //change the shapeData variable
                    Api.SetColumn(sesid, table, _colIdGeomType, (Byte)XbimGeometryType.TriangulatedMeshHash);
                    Api.SetColumn(sesid, table, _colIdGeometryHash, mainId);// id of main geom this is a hash of;
                    UpdateCount(1);
                    hashId = Api.RetrieveColumnAsInt32(sesid, table, _colIdGeometryLabel, RetrieveColumnGrbit.RetrieveCopy).Value;
                    update.Save();
                }

                Api.MakeKey(sesid, table, mainId, MakeKeyGrbit.NewKey);

                if (Api.TrySeek(sesid, table, SeekGrbit.SeekEQ)) //find the main geometry
                {
                    using (var update = new Update(sesid, table, JET_prep.Replace))
                    {
                        Api.SetColumn(sesid, table, _colIdGeometryHash, hashId);// id of main geom this is a hash of;
                        update.Save();
                    }

                }
            }
#endif
            return mainId;
        }


        public int AddMapGeometry(int geomId, int prodLabel, short expressType, byte[] transform, int styleLabel = 0)
        {
            Api.JetSetCurrentIndex(Sesid, Table, GeometryTablePrimaryIndex);
            Api.MakeKey(Sesid, Table, geomId, MakeKeyGrbit.NewKey);
            int mainId;
            int mapId;
            XbimGeometryType geomType;
            if (Api.TrySeek(Sesid, Table, SeekGrbit.SeekEQ)) //find map
            {
                using (var update = new Update(Sesid, Table, JET_prep.InsertCopy))
                {

                    Api.SetColumn(Sesid, Table, _colIdProductLabel, prodLabel);
                    Api.SetColumn(Sesid, Table, _colIdProductIfcTypeId, expressType);
                    Api.SetColumn(Sesid, Table, _colIdTransformMatrix, transform);
                    if (styleLabel > 0)
                        Api.SetColumn(Sesid, Table, _colIdStyleLabel, styleLabel);
                    else
                        Api.SetColumn(Sesid, Table, _colIdStyleLabel, -expressType); //use the negative type id as a style for object that have no render material
                    UpdateCount(1);
                    var mapGeomId = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdGeometryLabel, RetrieveColumnGrbit.RetrieveCopy);
                    update.Save();
                    mapId = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdGeometryHash, RetrieveColumnGrbit.RetrieveCopy).Value;
                    geomType = (XbimGeometryType)Api.RetrieveColumnAsByte(Sesid, Table, _colIdGeomType, RetrieveColumnGrbit.RetrieveCopy).Value;
                    mainId = mapGeomId.Value;
                }
#if CREATEGEOMHASH
                if (geomType == XbimGeometryType.TriangulatedMesh)
                {
                    Api.MakeKey(sesid, table, mapId, MakeKeyGrbit.NewKey); //find the hash record and make a copy of the hash
                    if (Api.TrySeek(sesid, table, SeekGrbit.SeekEQ)) //find map
                    {
                        using (var update = new Update(sesid, table, JET_prep.InsertCopy))
                        {
                            Api.SetColumn(sesid, table, _colIdProductLabel, prodLabel);
                            Api.SetColumn(sesid, table, _colIdProductIfcTypeId, expressType);
                            Api.SetColumn(sesid, table, _colIdTransformMatrix, transform);
                            if (styleLabel > 0)
                                Api.SetColumn(sesid, table, _colIdStyleLabel, styleLabel);
                            else
                                Api.SetColumn(sesid, table, _colIdStyleLabel, -expressType); //use the negative type id as a style for object that have no render material
                            UpdateCount(1);
                            update.Save();
                        }
                    }
                }
#endif
            }
            else
                throw new XbimException("Mapped geometry not found = #" + geomId);

            return mainId;

        }

        internal IEnumerable<XbimGeometryData> GeometryData(short typeId, int productLabel, XbimGeometryType geomType)
        {
            
            Api.JetSetCurrentIndex(Sesid, Table, GeometryTableGeomTypeIndex);
            Api.MakeKey(Sesid, Table, (byte)geomType, MakeKeyGrbit.NewKey);
            Api.MakeKey(Sesid, Table, typeId, MakeKeyGrbit.None);
            Api.MakeKey(Sesid, Table, productLabel, MakeKeyGrbit.None);
            if (Api.TrySeek(Sesid, Table, SeekGrbit.SeekGE))
            {
                Api.MakeKey(Sesid, Table, (byte)geomType, MakeKeyGrbit.NewKey);
                Api.MakeKey(Sesid, Table, typeId, MakeKeyGrbit.None);
                Api.MakeKey(Sesid, Table, productLabel, MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(Sesid, Table, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    do
                    {

                        Api.RetrieveColumns(Sesid, Table, _colValues);
                       // System.Diagnostics.Debug.Assert((byte)geomType == _colValGeomType.Value);
                        _colValGeometryLabel.Value = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdGeometryLabel);
                        //SRL note this method needs to be modified to support UINT rather than int, the casting below should be removed
                        yield return new XbimGeometryData(_colValGeometryLabel.Value.Value, productLabel, (XbimGeometryType)_colValGeomType.Value, _colValProductIfcTypeId.Value.Value, _colValShapeData.Value, _colValTransformMatrix.Value, _colValGeometryHash.Value.Value, _colValStyleLabel.Value.HasValue ? _colValStyleLabel.Value.Value : 0, _colValSubPart.Value.HasValue ? _colValSubPart.Value.Value : 0);

                    } while (Api.TryMoveNext(Sesid, Table));
                }
            }
        }

        internal IEnumerable<XbimGeometryData> GetGeometryData(XbimGeometryType ofType)
        {

            Api.JetSetCurrentIndex(Sesid, Table, GeometryTableGeomTypeIndex);
            Api.MakeKey(Sesid, Table, (byte)ofType, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(Sesid, Table, SeekGrbit.SeekGE))
            {
                Api.MakeKey(Sesid, Table, (byte)ofType,  MakeKeyGrbit.NewKey| MakeKeyGrbit.FullColumnEndLimit);

                if (Api.TrySetIndexRange(Sesid, Table, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    do
                    {
                        Api.RetrieveColumns(Sesid, Table, _colValues);
                        _colValGeometryLabel.Value = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdGeometryLabel);
                        yield return new XbimGeometryData(_colValGeometryLabel.Value.Value, _colValProductLabel.Value.Value, (XbimGeometryType)_colValGeomType.Value, _colValProductIfcTypeId.Value.Value, _colValShapeData.Value, _colValTransformMatrix.Value, _colValGeometryHash.Value.Value, _colValStyleLabel.Value.HasValue ? _colValStyleLabel.Value.Value : 0, _colValSubPart.Value.HasValue ? _colValSubPart.Value.Value : 0);
                    } while (Api.TryMoveNext(Sesid, Table));
                }
            }
        }

        /// <summary>
        /// Retrieve the count of geometry items in the database from the globals table.
        /// </summary>
        /// <returns>The number of items in the database.</returns>
        override internal int RetrieveCount()
        {
            return (int)Api.RetrieveColumnAsInt32(Sesid, GlobalsTable, GeometryCountColumn);
        }

        /// <summary>
        /// Update the count of geometry entities in the globals table. This is done with EscrowUpdate
        /// so that there won't be any write conflicts.
        /// </summary>
        /// <param name="delta">The delta to apply to the count.</param>
        override protected void UpdateCount(int delta)
        {
            Api.EscrowUpdate(Sesid, GlobalsTable, GeometryCountColumn, delta);
        }



      

        internal XbimGeometryHandleCollection GetGeometryHandles(XbimGeometryType geomType, XbimGeometrySort sortOrder)
        {
            switch (sortOrder)
            {
                case XbimGeometrySort.OrderByIfcSurfaceStyleThenIfcType:
                    return GetGeometryHandlesBySurfaceStyle(geomType);          
                case XbimGeometrySort.OrderByIfcTypeThenIfcProduct:
                    return GetGeometryHandlesByIfcType(geomType);
                case XbimGeometrySort.OrderByGeometryID:
                    return GetGeometryHandlesById(geomType);
                default:
                    throw new XbimException("Illegal geometry sort order");
            }
        }

        private XbimGeometryHandleCollection GetGeometryHandlesById(XbimGeometryType geomType)
        {
            var result = new XbimGeometryHandleCollection();
            Api.JetSetCurrentIndex(Sesid, Table, GeometryTablePrimaryIndex);

            Api.MakeKey(Sesid, Table, (byte)geomType, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(Sesid, Table, SeekGrbit.SeekGE))
            {
                Api.MakeKey(Sesid, Table, (byte)geomType, MakeKeyGrbit.NewKey | MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(Sesid, Table, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    do
                    {
                        var style = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdStyleLabel);
                        var expressType = Api.RetrieveColumnAsInt16(Sesid, Table, _colIdProductIfcTypeId);
                        var product = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdProductLabel);
                        var geomId = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdGeometryLabel);
                        var hashId = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdGeometryHash);
                        //srl note casting to UINT, needs to be resolved at database level
                        result.Add(new XbimGeometryHandle(geomId.Value, geomType, product.Value, expressType.Value, style.Value, hashId.Value));
                    } while (Api.TryMoveNext(Sesid, Table));
                }

            }
            return result;
        }

        private XbimGeometryHandleCollection GetGeometryHandlesByIfcType(XbimGeometryType geomType)
        {
            var result = new XbimGeometryHandleCollection();
            Api.JetSetCurrentIndex(Sesid, Table, GeometryTableGeomTypeIndex);

            Api.MakeKey(Sesid, Table, (byte)geomType, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(Sesid, Table, SeekGrbit.SeekGE))
            {
                Api.MakeKey(Sesid, Table, (byte)geomType, MakeKeyGrbit.NewKey | MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(Sesid, Table, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    do
                    {
                        var style = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdStyleLabel, RetrieveColumnGrbit.RetrieveFromIndex);
                        var expressType = Api.RetrieveColumnAsInt16(Sesid, Table, _colIdProductIfcTypeId, RetrieveColumnGrbit.RetrieveFromIndex);
                        var product = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdProductLabel, RetrieveColumnGrbit.RetrieveFromIndex);
                        var geomId = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdGeometryLabel, RetrieveColumnGrbit.RetrieveFromIndex);
                        //srl note casting to UINT, needs to be resolved at database level
                        result.Add(new XbimGeometryHandle(geomId.Value, geomType, product.Value, expressType.Value, style.Value, geomId.Value));
                    } while (Api.TryMoveNext(Sesid, Table));
                }

            }
            return result;
        }

        private XbimGeometryHandleCollection GetGeometryHandlesBySurfaceStyle(XbimGeometryType geomType)
        {
            var result = new XbimGeometryHandleCollection();
            Api.JetSetCurrentIndex(Sesid, Table, GeometryTableStyleIndex);
            Api.MakeKey(Sesid, Table, (byte)geomType, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(Sesid, Table, SeekGrbit.SeekGE))
            {
                Api.MakeKey(Sesid, Table, (byte)geomType, MakeKeyGrbit.NewKey | MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(Sesid, Table, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    do
                    {
                        var style = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdStyleLabel, RetrieveColumnGrbit.RetrieveFromIndex);
                        var expressType = Api.RetrieveColumnAsInt16(Sesid, Table, _colIdProductIfcTypeId, RetrieveColumnGrbit.RetrieveFromIndex);
                        var product = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdProductLabel, RetrieveColumnGrbit.RetrieveFromIndex);
                        var geomId = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdGeometryLabel, RetrieveColumnGrbit.RetrieveFromIndex);
                        result.Add(new XbimGeometryHandle(geomId.Value, geomType, product.Value, expressType.Value, style.Value));
                    } while (Api.TryMoveNext(Sesid, Table));
                }

            }
            return result;
        }

        internal XbimGeometryHandle GetGeometryHandle(int geometryLabel)
        {
            // Api.JetSetCurrentIndex(sesid, table, geometryTablePrimaryIndex);
            Api.JetSetCurrentIndex(Sesid, Table, GeometryTablePrimaryIndex );


            Api.MakeKey(Sesid, Table, geometryLabel, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(Sesid, Table, SeekGrbit.SeekEQ))
            {
                Api.RetrieveColumns(Sesid, Table, _colValues);
                var style = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdStyleLabel);
                var expressType = Api.RetrieveColumnAsInt16(Sesid, Table, _colIdProductIfcTypeId);
                var product = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdProductLabel);
                var geomType = Api.RetrieveColumnAsByte(Sesid, Table, _colIdGeomType);
                var geomHash = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdGeometryHash);
               
                return new XbimGeometryHandle(geometryLabel, (XbimGeometryType)geomType.Value, product.Value, expressType.Value, style.Value, geomHash.Value);
            }
            return new XbimGeometryHandle();
        }

        internal XbimGeometryData GetGeometryData(XbimGeometryHandle handle)
        {
            
            Api.JetSetCurrentIndex(Sesid, Table, GeometryTablePrimaryIndex);
            Api.MakeKey(Sesid, Table, handle.GeometryLabel, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(Sesid, Table, SeekGrbit.SeekEQ))
            {
                Api.RetrieveColumns(Sesid, Table, _colValues);
                _colValGeometryLabel.Value = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdGeometryLabel);
                return new XbimGeometryData(_colValGeometryLabel.Value.Value, _colValProductLabel.Value.Value, (XbimGeometryType)_colValGeomType.Value, _colValProductIfcTypeId.Value.Value, _colValShapeData.Value, _colValTransformMatrix.Value, _colValGeometryHash.Value.Value, _colValStyleLabel.Value.HasValue ? _colValStyleLabel.Value.Value : 0, _colValSubPart.Value.HasValue ? _colValSubPart.Value.Value : 0);

            }
            else
                return null;
        }

        internal IEnumerable<XbimGeometryData> GetGeometryData(IEnumerable<XbimGeometryHandle> handles)
        {
            Api.JetSetCurrentIndex(Sesid, Table, GeometryTablePrimaryIndex);
            foreach (var handle in handles)
            {
                Api.MakeKey(Sesid, Table, handle.GeometryLabel, MakeKeyGrbit.NewKey);
                if (Api.TrySeek(Sesid, Table, SeekGrbit.SeekEQ))
                {
                    Api.RetrieveColumns(Sesid, Table, _colValues);
                    _colValGeometryLabel.Value = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdGeometryLabel);
                    yield return new XbimGeometryData(_colValGeometryLabel.Value.Value, _colValProductLabel.Value.Value, (XbimGeometryType)_colValGeomType.Value, _colValProductIfcTypeId.Value.Value, _colValShapeData.Value, _colValTransformMatrix.Value, _colValGeometryHash.Value.Value, _colValStyleLabel.Value.HasValue ? _colValStyleLabel.Value.Value : 0, _colValSubPart.Value.HasValue ? _colValSubPart.Value.Value : 0);
                }
            }
        }




        public XbimGeometryData GetGeometryData(int geomLabel)
        {
            Api.JetSetCurrentIndex(Sesid, Table, GeometryTablePrimaryIndex);
            Api.MakeKey(Sesid, Table, geomLabel, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(Sesid, Table, SeekGrbit.SeekEQ))
            {
                Api.RetrieveColumns(Sesid, Table, _colValues);
              //  _colValGeometryLabel.Value = Api.RetrieveColumnAsInt32(sesid, table, _colIdGeometryLabel); //unnecessary call
                return new XbimGeometryData(geomLabel, _colValProductLabel.Value.Value, (XbimGeometryType)_colValGeomType.Value, _colValProductIfcTypeId.Value.Value, _colValShapeData.Value, _colValTransformMatrix.Value, _colValGeometryHash.Value.Value, _colValStyleLabel.Value.HasValue ? _colValStyleLabel.Value.Value : 0, _colValSubPart.Value.HasValue ? _colValSubPart.Value.Value : 0);

            }
            else
                return null;
        }

        /// <summary>
        /// Updates the number of references to a geomentry, a value of 1 indicates 1 reference in addition to one original use.
        /// A value of 0 means there is only the single use and no other references to this geometry
        /// </summary>
        /// <param name="geomLabel"></param>
        /// <param name="refCount"></param>
        /// <returns></returns>
        public int UpdateReferenceCount(int geomLabel, int refCount)
        {
            if (refCount < 1) return 0;
            Api.JetSetCurrentIndex(Sesid, Table, GeometryTablePrimaryIndex);
            Api.MakeKey(Sesid, Table, geomLabel, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(Sesid, Table, SeekGrbit.SeekEQ))
            {
                var size = Api.RetrieveColumnAsInt16(Sesid, Table, _colIdSubPart, RetrieveColumnGrbit.RetrieveCopy).Value;
                var count = (short)Math.Min(refCount,short.MaxValue);
                using (var update = new Update(Sesid, Table, JET_prep.Replace))
                {
                    
                    Api.SetColumn(Sesid, Table, _colIdSubPart, count); //change the order variable to hold the number of references to this object
                    update.Save();
                }
                return (size+1)*refCount;
            }
            return 0;
        }

        /// <summary>
        /// Returns the records for all geometries of the specified type
        /// </summary>
        /// <param name="xbimGeometryType"></param>
        /// <returns></returns>
        public IEnumerable<XbimGeometryData>  GeometryData(XbimGeometryType xbimGeometryType)
        {
            
            Api.JetSetCurrentIndex(Sesid, Table, GeometryTableGeomTypeIndex);
            Api.MakeKey(Sesid, Table, (byte)xbimGeometryType, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(Sesid, Table, SeekGrbit.SeekGE))
            {
                Api.MakeKey(Sesid, Table, (byte)xbimGeometryType, MakeKeyGrbit.NewKey | MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(Sesid, Table, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    do
                    {

                        Api.RetrieveColumns(Sesid, Table, _colValues);
                        Debug.Assert((byte)xbimGeometryType == _colValGeomType.Value);
                        _colValGeometryLabel.Value = Api.RetrieveColumnAsInt32(Sesid, Table, _colIdGeometryLabel);
                        yield return new XbimGeometryData(_colValGeometryLabel.Value.Value, _colValProductLabel.Value.Value, (XbimGeometryType)_colValGeomType.Value, _colValProductIfcTypeId.Value.Value, _colValShapeData.Value, _colValTransformMatrix.Value, _colValGeometryHash.Value.Value, _colValStyleLabel.Value.HasValue ? _colValStyleLabel.Value.Value : 0, _colValSubPart.Value.HasValue ? _colValSubPart.Value.Value : 0);

                    } while (Api.TryMoveNext(Sesid, Table));
                }
            }
            
        }

    }
}

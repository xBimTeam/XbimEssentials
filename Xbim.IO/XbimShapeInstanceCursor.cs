using Microsoft.Isam.Esent.Interop;
using Xbim.Common.Geometry;

namespace Xbim.IO
{
    /// <summary>
    /// Provides a table of instantiations of shapes at specific tranformations
    /// </summary>
    public class XbimShapeInstanceCursor : XbimCursor
    {

        const int SizeOfTransformation = 16 * sizeof(float); //the 16 floats that make a transformation
        #region Field Definition

        /// <summary>
        /// The unique label of this shape instance
        /// </summary>
        Int32ColumnValue _colValInstanceLabel;
        /// <summary>
        /// The IFC type of the product this instance represents
        /// </summary>
        Int16ColumnValue _colValIfcTypeId;
        /// <summary>
        /// The label of the IFC Product object that  this instance fully or partly defines
        /// </summary>
        Int32ColumnValue _colValIfcProductLabel;
        /// <summary>
        /// The style that this shape is presented in when it overrides the shape style
        /// </summary>
        Int32ColumnValue _colValStyleLabel;
        /// <summary>
        /// The id of the shape  that this is an instance of
        /// </summary>
        Int32ColumnValue _colValShapeLabel;
        /// <summary>
        /// The label of the IFC representation context of this instance
        /// </summary>
        Int32ColumnValue _colValRepresentationContext;
        /// <summary>
        /// What type of representation, typically this is how the shape has been generated, i.e. openings have been applied or not applied
        /// </summary>
        ByteColumnValue _colValRepType;
        /// <summary>
        /// The transformation to be applied to shape to place it in the world coordinates
        /// </summary>
        BytesColumnValue _colValTransformation;
        /// <summary>
        /// The bounding box of this instance, requires tranformation to place in world coordinates
        /// </summary>
        BytesColumnValue _colValBoundingBox;

        #endregion


        #region Constructors
        public XbimShapeInstanceCursor(XbimModel model, string database)
            : this(model, database, OpenDatabaseGrbit.None)
        {
        }
        public XbimShapeInstanceCursor(XbimModel model, string database, OpenDatabaseGrbit mode)
            : base(model, database, mode)
        {
            Api.JetOpenTable(this.sesid, this.dbId, InstanceTableName, null, 0, mode == OpenDatabaseGrbit.ReadOnly ? OpenTableGrbit.ReadOnly :
                                                                                mode == OpenDatabaseGrbit.Exclusive ? OpenTableGrbit.DenyWrite : OpenTableGrbit.None,
                                                                                out this.table);
            InitColumns();
        }
        #endregion
        

        #region Table definition

        /// <summary>
        /// shape geometry table name
        /// </summary>
        public static string InstanceTableName = "ShapeInstances";
        /// <summary>
        /// Index on the context, style, ifc type then instance label
        /// </summary>
        const string instanceTablePrimaryIndex = "ShapeInstancePrimaryIndex";
        /// <summary>
        /// Index on theifc type id
        /// </summary>
        const string productTypeIndex = "ProductTypeIndex";
        /// <summary>
        /// index on the  product label
        /// </summary>
        const string productIndex = "ProductIndex";

        /// <summary>
        /// Index on the shape of the object
        /// </summary>
        const string geometryShapeIndex = "GeometryShapeIndex";

        /// <summary>
        /// The unique label of this shape instance
        /// </summary>
        const string colNameInstanceLabel = "InstanceLabel";
        private JET_COLUMNID _colIdInstanceLabel;

        /// <summary>
        /// The type ID of the IFC Product object that defines this shape
        /// </summary>
        const string colNameIfcTypeId = "IfcTypeId";
        private JET_COLUMNID _colIdIfcTypeId;

        /// <summary>
        /// The label of the IFC Product object that defines this shape
        /// </summary>
        const string colNameIfcProductLabel = "IfcProductLabel";
        private JET_COLUMNID _colIdIfcProductLabel;

        /// <summary>
        /// The style that this shape is presented in when it overrides the shape style
        /// </summary>
        const string colNameStyleLabel = "StyleLabel";
        private JET_COLUMNID _colIdStyleLabel;

        /// <summary>
        /// The id of the shape  that this is an instance of
        /// </summary>
        const string colNameShapeLabel = "ShapeLabel";
        private JET_COLUMNID _colIdShapeLabel;

        /// <summary>
        /// The label of the IFC representation context of this instance
        /// </summary>
        const string colNameRepresentationContext = "RepresentationContext";
        private JET_COLUMNID _colIdRepresentationContext;

        /// <summary>
        /// What type of representation, typically this is how the shape has been generated, i.e. openings have been applied or not applied
        /// </summary>
        const string colNameRepType = "ShapeGeomRepType";
        private JET_COLUMNID _colIdRepType;

        /// <summary>
        /// The transformation to be applied to shape to place it in the world coordinates
        /// </summary>
        const string colNameTransformation = "Transformation";
        private JET_COLUMNID _colIdTransformation;

        /// <summary>
        /// The bounding box of this instance, requires tranformation to place in world coordinates
        /// </summary>
        const string colNameBoundingBox = "BoundingBox";

        private JET_COLUMNID _colIdBoundingBox;
        /// <summary>
        /// Holds all the table row values
        /// </summary>
        ColumnValue[] _colValues;

        private void InitColumns()
        {

            _colIdInstanceLabel = Api.GetTableColumnid(sesid, table, colNameInstanceLabel);
            _colIdIfcTypeId = Api.GetTableColumnid(sesid, table, colNameIfcTypeId);
            _colIdIfcProductLabel = Api.GetTableColumnid(sesid, table, colNameIfcProductLabel);     
            _colIdStyleLabel = Api.GetTableColumnid(sesid, table, colNameStyleLabel);
            _colIdShapeLabel = Api.GetTableColumnid(sesid, table, colNameShapeLabel);
            _colIdRepresentationContext = Api.GetTableColumnid(sesid, table, colNameRepresentationContext);
            _colIdRepType = Api.GetTableColumnid(sesid, table, colNameRepType);
            _colIdTransformation = Api.GetTableColumnid(sesid, table, colNameTransformation);
            _colIdBoundingBox = Api.GetTableColumnid(sesid, table, colNameBoundingBox);

            _colValInstanceLabel = new Int32ColumnValue { Columnid = _colIdInstanceLabel };
            _colValIfcTypeId = new Int16ColumnValue { Columnid = _colIdIfcTypeId };
            _colValIfcProductLabel = new Int32ColumnValue { Columnid = _colIdIfcProductLabel };
            _colValStyleLabel = new Int32ColumnValue { Columnid = _colIdStyleLabel };
            _colValShapeLabel = new Int32ColumnValue { Columnid = _colIdShapeLabel };
            _colValRepresentationContext = new Int32ColumnValue { Columnid = _colIdRepresentationContext };
            _colValRepType = new ByteColumnValue { Columnid = _colIdRepType };
            _colValTransformation = new BytesColumnValue { Columnid = _colIdTransformation };
            _colValBoundingBox = new BytesColumnValue { Columnid = _colIdBoundingBox };


            _colValues = new ColumnValue[] { _colValIfcTypeId, _colValIfcProductLabel, _colValStyleLabel, _colValShapeLabel, _colValRepresentationContext, _colValRepType, _colValTransformation,_colValBoundingBox, };



        }
        #endregion

        #region Table Creation

        internal static void CreateTable(JET_SESID sesid, JET_DBID dbid)
        {
            JET_TABLEID tableid;
            Api.JetCreateTable(sesid, dbid, InstanceTableName, 8, 80, out tableid);

            using (var transaction = new Microsoft.Isam.Esent.Interop.Transaction(sesid))
            {
                JET_COLUMNID columnid;

                //Unique instance label
                var columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.Long,
                    grbit = ColumndefGrbit.ColumnAutoincrement |ColumndefGrbit.ColumnNotNULL
                };
                Api.JetAddColumn(sesid, tableid, colNameInstanceLabel, columndef, null, 0, out columnid);
              
                //IFC type ID
                columndef.coltyp = JET_coltyp.Short;
                columndef.grbit = ColumndefGrbit.ColumnNotNULL;
                Api.JetAddColumn(sesid, tableid, colNameIfcTypeId, columndef, null, 0, out columnid);

                //ifc Product label
                columndef.coltyp = JET_coltyp.Long;
                columndef.grbit = ColumndefGrbit.ColumnNotNULL;
                Api.JetAddColumn(sesid, tableid, colNameIfcProductLabel, columndef, null, 0, out columnid);
                
                //style label
                columndef.coltyp = JET_coltyp.Long;
                columndef.grbit = ColumndefGrbit.ColumnNotNULL;
                Api.JetAddColumn(sesid, tableid, colNameStyleLabel, columndef, null, 0, out columnid);
             
                //shape label
                columndef.coltyp = JET_coltyp.Long;
                columndef.grbit = ColumndefGrbit.ColumnNotNULL;
                Api.JetAddColumn(sesid, tableid, colNameShapeLabel, columndef, null, 0, out columnid);
              
                //Representation Context
                columndef.coltyp = JET_coltyp.Long;
                columndef.grbit = ColumndefGrbit.ColumnNotNULL;
                Api.JetAddColumn(sesid, tableid, colNameRepresentationContext, columndef, null, 0, out columnid);

                //Representation Context
                columndef.coltyp = JET_coltyp.UnsignedByte;
                columndef.grbit = ColumndefGrbit.ColumnNotNULL;
                Api.JetAddColumn(sesid, tableid, colNameRepType, columndef, null, 0, out columnid);

                //Transformation data
                columndef.coltyp = JET_coltyp.Binary;
                columndef.grbit = ColumndefGrbit.ColumnNotNULL;
                columndef.cbMax = SizeOfTransformation;
                Api.JetAddColumn(sesid, tableid, colNameTransformation, columndef, null, 0, out columnid);

                //Bounding Box data
                columndef.coltyp = JET_coltyp.Binary;
                columndef.grbit = ColumndefGrbit.ColumnNotNULL;
                Api.JetAddColumn(sesid, tableid, colNameBoundingBox, columndef, null, 0, out columnid);

                string indexDef;
                // The  index on the shape geometry label.
                indexDef = string.Format("+{0}\0\0", colNameShapeLabel);
                Api.JetCreateIndex(sesid, tableid, geometryShapeIndex, CreateIndexGrbit.IndexDisallowNull, indexDef, indexDef.Length, 100);

                //create index by ifc product label..  ..
                indexDef = string.Format("+{0}\0\0", colNameIfcProductLabel);
                Api.JetCreateIndex(sesid, tableid, productIndex, CreateIndexGrbit.IndexDisallowNull, indexDef, indexDef.Length, 100);

                //create index by ifc product type label..  ..
                indexDef = string.Format("+{0}\0\0", colNameIfcTypeId);
                Api.JetCreateIndex(sesid, tableid, productTypeIndex, CreateIndexGrbit.IndexDisallowNull, indexDef, indexDef.Length, 100);

                //create by context,then ifc style...  
                indexDef = string.Format("+{0}\0{1}\0{2}\0{3}\0\0", colNameRepresentationContext, colNameStyleLabel, colNameIfcTypeId,  colNameInstanceLabel);
                Api.JetCreateIndex(sesid, tableid, instanceTablePrimaryIndex, CreateIndexGrbit.IndexPrimary, indexDef, indexDef.Length, 100);

                Api.JetCloseTable(sesid, tableid);

                transaction.Commit(CommitTransactionGrbit.LazyFlush);
            }

        }
        #endregion

        #region Table operations
        /// <summary>
        /// Retrieve the count of geometry items in the database from the globals table.
        /// </summary>
        /// <returns>The number of items in the database.</returns>
        override internal int RetrieveCount()
        {
            return (int)Api.RetrieveColumnAsInt32(this.sesid, this.globalsTable, this.geometryCountColumn);
        }

        /// <summary>
        /// Update the count of geometry entities in the globals table. This is done with EscrowUpdate
        /// so that there won't be any write conflicts.
        /// </summary>
        /// <param name="delta">The delta to apply to the count.</param>
        override protected void UpdateCount(int delta)
        {
            Api.EscrowUpdate(this.sesid, this.globalsTable, this.geometryCountColumn, delta);
        }
        #endregion

        public int AddInstance(IXbimShapeInstanceData instance)
        {
            using (var update = new Update(sesid, table, JET_prep.Insert))
            {
                _colValRepresentationContext.Value = instance.RepresentationContext;
                _colValIfcProductLabel.Value = instance.IfcProductLabel;
                _colValIfcTypeId.Value = instance.IfcTypeId;
                _colValShapeLabel.Value = instance.ShapeGeometryLabel;
                _colValStyleLabel.Value = instance.StyleLabel;
                _colValRepType.Value = instance.RepresentationType;
                _colValTransformation.Value = instance.Transformation;
                _colValBoundingBox.Value = instance.BoundingBox;
                Api.SetColumns(sesid, table, _colValues);
                instance.InstanceLabel = Api.RetrieveColumnAsInt32(sesid, table, _colIdInstanceLabel, RetrieveColumnGrbit.RetrieveCopy).Value;
                update.Save();
                UpdateCount(1);              
               
            }
            return instance.InstanceLabel;
        }

        /// <summary>
        /// Adds a shape instance to the database table
        /// </summary>
        /// <param name="ctxtId"></param>
        /// <param name="shapeLabel"></param>
        /// <param name="typeId"></param>
        /// <param name="productLabel"></param>
        /// <param name="repType"></param>
        /// <param name="bounds"></param>
        /// <param name="transform"></param>
        public int AddInstance(int ctxtId, int shapeLabel, int styleLabel, short typeId, int productLabel, XbimGeometryRepresentationType repType, byte[] transform)
        {
            int id = -1;
            using (var update = new Update(sesid, table, JET_prep.Insert))
            {
                _colValRepresentationContext.Value = ctxtId;
                _colValIfcProductLabel.Value = productLabel;
                _colValIfcTypeId.Value = typeId;
                _colValShapeLabel.Value = shapeLabel;
                _colValStyleLabel.Value = styleLabel;
                _colValRepType.Value = (byte) repType;
                _colValTransformation.Value = transform;
                Api.SetColumns(sesid, table, _colValues);
                id = Api.RetrieveColumnAsInt32(sesid, table, _colIdInstanceLabel, RetrieveColumnGrbit.RetrieveCopy).Value;
                update.Save();
                UpdateCount(1);

            }
            return id; ;
        }

        private void GetShapeInstanceData(IXbimShapeInstanceData si)
        {
            Api.RetrieveColumns(sesid, table, _colValues);
            si.RepresentationContext = _colValRepresentationContext.Value.Value;
            si.InstanceLabel = Api.RetrieveColumnAsInt32(sesid, table, _colIdInstanceLabel).Value;
            si.IfcTypeId = _colValIfcTypeId.Value.Value;
            si.IfcProductLabel = _colValIfcProductLabel.Value.Value;
            si.StyleLabel = _colValStyleLabel.Value.Value;
            si.ShapeGeometryLabel = _colValShapeLabel.Value.Value;
            si.RepresentationType = _colValRepType.Value.Value;
            si.Transformation = _colValTransformation.Value;
            si.BoundingBox = _colValBoundingBox.Value;
        }

       /// <summary>
        /// xbimShapeInstanceData will contain the first shape instance in the specified context
       /// </summary>
       /// <param name="context"></param>
       /// <param name="si"></param>
       /// <param name="retrieveAll">if false only retrieve the key index data for speed, if true all data is returned</param>
       /// <returns></returns>
        public bool TrySeekShapeInstance(int context, ref IXbimShapeInstanceData si)
        {
            Api.JetSetCurrentIndex(sesid, table, instanceTablePrimaryIndex);
            Api.MakeKey(sesid, table, context, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(sesid, table, SeekGrbit.SeekGE))
            {
                Api.MakeKey(sesid, table, context, MakeKeyGrbit.NewKey | MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(sesid, table, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    GetShapeInstanceData(si);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Moves the cursor to the next shape instance that meets the criteria of the previous TrySeek call
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        public bool TryMoveNextShapeInstance(ref IXbimShapeInstanceData si)
        {
            if (Api.TryMoveNext(this.sesid, this.table))
            {
                GetShapeInstanceData(si);
                return true;
            }
            else 
                return false;
        }

        /// <summary>
        /// xbimShapeInstanceData will contain the first shape instance of the specified product label
        /// </summary>
        /// <param name="product"></param>
        /// <param name="si"></param>
        /// <returns></returns>
        public bool TrySeekShapeInstanceOfProduct(int product, ref IXbimShapeInstanceData si)
        {
            Api.JetSetCurrentIndex(sesid, table, productIndex);
            Api.MakeKey(sesid, table, product, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(sesid, table, SeekGrbit.SeekGE))
            {
                Api.MakeKey(sesid, table, product, MakeKeyGrbit.NewKey | MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(sesid, table, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    GetShapeInstanceData(si);
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Return whether the product has any instances
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public bool TrySeekShapeInstanceOfProduct(int product)
        {
            Api.JetSetCurrentIndex(sesid, table, productIndex);
            Api.MakeKey(sesid, table, product, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(sesid, table, SeekGrbit.SeekGE))
            {
                Api.MakeKey(sesid, table, product, MakeKeyGrbit.NewKey | MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(sesid, table, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    return true;
                }
            }
            return false;
        }

      

        /// <summary>
        /// xbimShapeInstanceData will contain the first shape instance of the specified product label
        /// </summary>
        /// <param name="context"></param>
        /// <param name="si"></param>
        /// <returns></returns>
        public bool TrySeekShapeInstanceOfGeometry(int shapeGeometryLabel, ref IXbimShapeInstanceData si)
        {
            Api.JetSetCurrentIndex(sesid, table, geometryShapeIndex);
            Api.MakeKey(sesid, table, shapeGeometryLabel, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(sesid, table, SeekGrbit.SeekGE))
            {
                Api.MakeKey(sesid, table, shapeGeometryLabel, MakeKeyGrbit.NewKey | MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(sesid, table, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    GetShapeInstanceData(si);
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Returns the first surface style in the specified context, -1 if no styles exists
        /// </summary>
        /// <param name="p"></param>
        /// <param name="surfaceStyle"></param>
        /// <returns></returns>
        public bool TryMoveFirstSurfaceStyle(int context, out int surfaceStyle, out short productType)
        {
            Api.JetSetCurrentIndex(sesid, table, instanceTablePrimaryIndex);
            Api.MakeKey(sesid, table, context, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(sesid, table, SeekGrbit.SeekGE))
            {
                Api.MakeKey(sesid, table, context, MakeKeyGrbit.NewKey | MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(sesid, table, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    surfaceStyle = Api.RetrieveColumnAsInt32(sesid, table, _colIdStyleLabel,RetrieveColumnGrbit.RetrieveFromIndex).Value;
                    productType = Api.RetrieveColumnAsInt16(sesid, table, _colIdIfcTypeId, RetrieveColumnGrbit.RetrieveFromIndex).Value; 
                    return true;
                }
            }
            surfaceStyle = -1;
            productType = 0;
            return false;
        }

        /// <summary>
        /// Returns the next surface style in the specified context, assumes TryMoveFirstSurfaceStyle was the last call on this cursor
        /// </summary>
        /// <param name="surfaceStyle"></param>
        /// <returns></returns>
        public bool TryMoveNextSurfaceStyle(out int surfaceStyle, out short productType)
        {
            if (Api.TryMoveNext(this.sesid, this.table))
            {
                surfaceStyle = Api.RetrieveColumnAsInt32(sesid, table, _colIdStyleLabel, RetrieveColumnGrbit.RetrieveFromIndex).Value;
                productType = Api.RetrieveColumnAsInt16(sesid, table, _colIdIfcTypeId, RetrieveColumnGrbit.RetrieveFromIndex).Value; 
                return true;
            }
            surfaceStyle = -1;
            productType = 0;
            return false;
        }
        /// <summary>
        /// Skips all instances of skipstlye and returns in the next SurfaceStyle 
        /// </summary>
        /// <param name="skipStyle"></param>
        /// <param name="surfaceStyle"></param>
        public int SkipSurfaceStyes( int skipStyle)
        {
           
            int? nextStyle;
            //skip over the rest with this style
            do
            {
                if (Api.TryMoveNext(sesid, table))
                    nextStyle = Api.RetrieveColumnAsInt32(sesid, table, _colIdStyleLabel, RetrieveColumnGrbit.RetrieveFromIndex);     
                else
                    nextStyle = null;
            }
            while (nextStyle.HasValue && nextStyle.Value == skipStyle);
            return nextStyle ?? -1;
        }

        /// <summary>
        /// Returns the first product type in the specified context
        /// </summary>
        /// <param name="p"></param>
        /// <param name="surfaceStyle"></param>
        /// <returns></returns>
        public bool TryMoveFirstProductType(int context, out short productType)
        {
            Api.JetSetCurrentIndex(sesid, table, instanceTablePrimaryIndex);
            Api.MakeKey(sesid, table, context, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(sesid, table, SeekGrbit.SeekGE))
            {
                Api.MakeKey(sesid, table, context, MakeKeyGrbit.NewKey | MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(sesid, table, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    productType = Api.RetrieveColumnAsInt16(sesid, table, _colIdIfcTypeId, RetrieveColumnGrbit.RetrieveFromIndex).Value;
                    short? nextProductType;
                    //skip over the rest with this style
                    do
                    {
                        if (Api.TryMoveNext(sesid, table))
                            nextProductType = Api.RetrieveColumnAsInt16(sesid, table, _colIdStyleLabel, RetrieveColumnGrbit.RetrieveFromIndex);
                        else
                            nextProductType = null;
                    }
                    while (nextProductType.HasValue && nextProductType.Value == productType);
                    Api.TryMovePrevious(sesid, table); //go back to last valid index
                    return true;
                }
            }
           
            productType = 0;
            return false;
        }

        /// <summary>
        /// Returns the next product type in the specified context, assumes TryMoveFirstSurfaceStyle was the last call on this cursor
        /// </summary>
        /// <param name="surfaceStyle"></param>
        /// <returns></returns>
        public bool TryMoveNextProductType(out short productType)
        {
            if (Api.TryMoveNext(this.sesid, this.table))
            {
                
                productType = Api.RetrieveColumnAsInt16(sesid, table, _colIdIfcTypeId, RetrieveColumnGrbit.RetrieveFromIndex).Value;
                short? nextProductType;
                //skip over the rest with this style
                do
                {
                    if (Api.TryMoveNext(sesid, table))
                        nextProductType = Api.RetrieveColumnAsInt16(sesid, table, _colIdStyleLabel, RetrieveColumnGrbit.RetrieveFromIndex);
                    else
                        nextProductType = null;
                }
                while (nextProductType.HasValue && nextProductType.Value == productType);
                Api.TryMovePrevious(sesid, table); //go back to last valid index
                return true;
            }
            productType = 0;
            return false;
        }



        public bool TrySeekProductType(short productType, ref IXbimShapeInstanceData shapeInstance)
        {
            Api.JetSetCurrentIndex(sesid, table, productTypeIndex);
            Api.MakeKey(sesid, table, productType, MakeKeyGrbit.NewKey);
            if (Api.TrySeek(sesid, table, SeekGrbit.SeekGE))
            {
                Api.MakeKey(sesid, table, productType, MakeKeyGrbit.NewKey | MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(sesid, table, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    GetShapeInstanceData(shapeInstance);
                    return true;
                }
            }
            return false;
        }


        public bool TrySeekSurfaceStyle(int context, int surfaceStyle, ref IXbimShapeInstanceData shapeInstance)
        {
            Api.JetSetCurrentIndex(sesid, table, instanceTablePrimaryIndex);
            Api.MakeKey(sesid, table, context, MakeKeyGrbit.NewKey);
            Api.MakeKey(sesid, table, surfaceStyle, MakeKeyGrbit.None);
            if (Api.TrySeek(sesid, table, SeekGrbit.SeekGE))
            {
                Api.MakeKey(sesid, table, context, MakeKeyGrbit.NewKey );
                Api.MakeKey(sesid, table, surfaceStyle, MakeKeyGrbit.FullColumnEndLimit);
                if (Api.TrySetIndexRange(sesid, table, SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                {
                    GetShapeInstanceData(shapeInstance);
                    return true;
                }
            }
            return false;
        }

       
    }
}

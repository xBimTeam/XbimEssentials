using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.PresentationAppearanceResource
{
    /// <summary>
    /// The fill area style tiles defines a two dimensional tile to be used for the filling of annotation fill areas 
    /// or other closed regions. The content of a tile is defined by the tile set, and the placement of each tile 
    /// determined by the filling pattern which indicates how to place tiles next to each other. Tiles or parts of 
    /// tiles outside of the annotation fill area or closed region shall be clipped at the boundaries of the area or region.
    /// </summary>
    [IfcPersistedEntity]
    public class IfcFillAreaStyleTiles : IfcGeometricRepresentationItem
    {

        public IfcFillAreaStyleTiles()
        {
            _Tiles = new XbimSet<IfcFillAreaStyleTileShapeSelect>(this);
        }
        private IfcOneDirectionRepeatFactor _TilingPattern;

        /// <summary>
        /// A two direction repeat factor defining the shape and relative positioning of the tiles. 
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcOneDirectionRepeatFactor TilingPattern
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _TilingPattern;
            }
            set { this.SetModelValue(this, ref _TilingPattern, value, v => TilingPattern = v, "TilingPattern"); }
        }

        private XbimSet<IfcFillAreaStyleTileShapeSelect> _Tiles;

        /// <summary>
        ///  	A set of constituents of the tile. 
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcFillAreaStyleTileShapeSelect> Tiles
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _Tiles;
            }
            set { this.SetModelValue(this, ref _Tiles, value, v => Tiles = v, "Tiles"); }
        }

        private IfcPositiveRatioMeasure _TilingScale;

        /// <summary>
        /// The scale factor applied to each tile as it is placed in the annotation fill area. 
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcPositiveRatioMeasure TilingScale
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _TilingScale;
            }
            set { this.SetModelValue(this, ref _TilingScale, value, v => TilingScale = v, "TilingScale"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _TilingPattern = (IfcOneDirectionRepeatFactor)value.EntityVal;
                    break;
                case 1:
                    _Tiles.Add((IfcFillAreaStyleTileShapeSelect)value.EntityVal);
                    break;
                case 2:
                    _TilingScale = value.RealVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }    
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}

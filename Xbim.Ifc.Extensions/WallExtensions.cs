#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    WallExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using Xbim.Ifc2x3.MaterialResource;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.QuantityResource;
using Xbim.XbimExtensions.SelectTypes;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class WallExtensions
    {
        /// <summary>
        ///   Set Material set usage with typical values and creates it if it doesn't exist.
        ///   LayerSetDirection = IfcLayerSetDirectionEnum.AXIS1
        ///   DirectionSense = IfcDirectionSenseEnum.POSITIVE
        ///   OffsetFromReferenceLine = 0
        /// </summary>
        /// <param name = "forLayerSet">Material layer set for the usage</param>
        public static void SetTypicalMaterialLayerSetUsage(this IfcWall wall, IfcMaterialLayerSet forLayerSet)
        {
            wall.SetMaterialLayerSetUsage(forLayerSet, IfcLayerSetDirectionEnum.AXIS1, IfcDirectionSenseEnum.POSITIVE, 0);
        }

       

        /// <summary>
        /// Gets the area of the wall in elevation
        /// </summary>
        /// <param name="wall"></param>
        /// <returns></returns>
        public static IfcAreaMeasure? GetWallSideArea(this IfcWall wall)
        {
            IfcQuantityArea qArea = wall.GetQuantity<IfcQuantityArea>("BaseQuantities", "GrossSideArea");
            if (qArea == null) qArea = wall.GetQuantity<IfcQuantityArea>("GrossSideArea"); //just look for any area
            if (qArea != null) return qArea.AreaValue;
            return null;
        }

        /// <summary>
        /// True if the wall is external
        /// </summary>
        /// <param name="wall"></param>
        /// <returns></returns>
        public static IfcBoolean? GetIsExternal(this IfcWall wall)
        {
            IfcValue val = wall.GetPropertySingleNominalValue("Pset_WallCommon", "IsExternal");
            if (val != null && val is IfcBoolean)
                return (IfcBoolean)val;
            else
                return null;

        }
    }
}
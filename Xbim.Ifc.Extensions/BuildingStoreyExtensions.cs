using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.QuantityResource;
using Xbim.Ifc2x3.Kernel;

namespace Xbim.Ifc2x3.Extensions
{
    public static class BuildingStoreyExtensions
    {
        /// <summary>
        /// Returns the Gross Floor Area, if the element base quantity GrossFloorArea is defined
        /// </summary>
        /// <param name="buildingStorey"></param>
        /// <returns></returns>
        public static IfcAreaMeasure? GetGrossFloorArea(this IfcBuildingStorey buildingStorey)
        {
            IfcQuantityArea qArea = buildingStorey.GetQuantity<IfcQuantityArea>("BaseQuantities", "GrossFloorArea");
            if (qArea == null) qArea = buildingStorey.GetQuantity<IfcQuantityArea>("GrossFloorArea"); //just look for any area
            if (qArea != null) return qArea.AreaValue;
            return null;
        }

        public static IfcLengthMeasure? GetTotalHeight(this IfcBuildingStorey buildingStorey)
        {
            IfcQuantityLength qLen = buildingStorey.GetQuantity<IfcQuantityLength>("BaseQuantities", "TotalHeight");
            if (qLen == null) qLen = buildingStorey.GetQuantity<IfcQuantityLength>("TotalHeight"); //just look for any height
            if (qLen != null) return qLen.LengthValue;
            return null;
        }

        /// <summary>
        /// Returns all spaces that are sub-spaces of this storey
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IfcSpace> GetSpaces(this IfcBuildingStorey buildingStorey)
        {
            IEnumerable<IfcRelDecomposes> decomp = buildingStorey.IsDecomposedBy;
            IEnumerable<IfcObjectDefinition> objs = decomp.SelectMany(s => s.RelatedObjects);
            return objs.OfType<IfcSpace>();

        }

        public static int CompareStoreysByElevation(IfcBuildingStorey x, IfcBuildingStorey y)
        {
            double a = x.Elevation ?? 0;
            double b = y.Elevation ?? 0;
            return a.CompareTo(b);
        }
        public static IfcQuantityLength GetTotalHeightProperty(this IfcBuildingStorey buildingStorey)
        {
            var qLen = buildingStorey.GetQuantity<IfcQuantityLength>("BaseQuantities", "TotalHeight");
            if (qLen == null) qLen = buildingStorey.GetQuantity<IfcQuantityLength>("TotalHeight"); //just look for any height
            return qLen;
        }

        public static IEnumerable<IfcBuildingStorey> GetBuildingStoreys(this IfcBuildingStorey buildingStorey, bool sortByElevation = false)
        {
            IEnumerable<IfcRelDecomposes> decomp = buildingStorey.IsDecomposedBy;
            IEnumerable<IfcObjectDefinition> objs = decomp.SelectMany(s => s.RelatedObjects);
             if (sortByElevation)
            {
                List<IfcBuildingStorey> storeys = objs.OfType<IfcBuildingStorey>().ToList();
                storeys.Sort(CompareStoreysByElevation);
                return storeys;
            }
            else
                 return objs.OfType<IfcBuildingStorey>();
        }
    }
}

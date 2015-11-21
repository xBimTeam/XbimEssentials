using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.QuantityResource;

namespace Xbim.Ifc2x3.ProductExtension
{
    public partial class IfcBuildingStorey
    {
        internal static int CompareStoreysByElevation(IfcBuildingStorey x, IfcBuildingStorey y)
        {
            double a = x.Elevation ?? 0;
            double b = y.Elevation ?? 0;
            return a.CompareTo(b);
        }
        /// <summary>
        /// Returns the Gross Floor Area, if the element base quantity GrossFloorArea is defined
        /// </summary>
        /// <returns></returns>
        public  IfcAreaMeasure? GrossFloorArea
        {
            get
            {
                var qArea = GetQuantity<IfcQuantityArea>("BaseQuantities", "GrossFloorArea") ??
                            GetQuantity<IfcQuantityArea>("GrossFloorArea");
                return qArea != null ? (IfcAreaMeasure?) qArea.AreaValue : null;
            }
        }

        public IfcLengthMeasure? TotalHeight
        {
            get
            {
                var qLen = GetQuantity<IfcQuantityLength>("BaseQuantities", "TotalHeight") ??
                           GetQuantity<IfcQuantityLength>("TotalHeight");
                return qLen != null ? (IfcLengthMeasure?) qLen.LengthValue : null;
            }
        }

        /// <summary>
        /// Returns all spaces that are sub-spaces of this storey
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IfcSpace> Spaces
        {
            get
            {              
                return IsDecomposedBy.SelectMany(s => s.RelatedObjects).OfType<IfcSpace>();
            }
        }


        public IEnumerable<IfcBuildingStorey> BuildingStoreys
        {
            get
            {
                var storeys = IsDecomposedBy.SelectMany(s => s.RelatedObjects).OfType<IfcBuildingStorey>().ToList();         
                storeys.Sort(CompareStoreysByElevation);
                return storeys;
            }
        }
    }
}

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
        public  Ifc4.MeasureResource.IfcAreaMeasure? GrossFloorArea
        {
            get
            {
                var qArea = GetQuantity<IfcQuantityArea>("BaseQuantities", "GrossFloorArea") ??
                            GetQuantity<IfcQuantityArea>("GrossFloorArea");
                if (qArea != null) 
                    return new Ifc4.MeasureResource.IfcAreaMeasure(qArea.AreaValue); 
                else return null;
            }
        }

        public Ifc4.MeasureResource.IfcLengthMeasure? TotalHeight
        {
            get
            {
                var qLen = GetQuantity<IfcQuantityLength>("BaseQuantities", "TotalHeight") ??
                           GetQuantity<IfcQuantityLength>("TotalHeight");
                if (qLen != null)
                    return new Ifc4.MeasureResource.IfcLengthMeasure(qLen.LengthValue);
                else
                    return null;
            }
        }

        /// <summary>
        /// Returns all spaces that are sub-spaces of this storey
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Xbim.Ifc4.Interfaces.IIfcSpace> Spaces
        {
            get
            {              
                return IsDecomposedBy.SelectMany(s => s.RelatedObjects).OfType<IfcSpace>();
            }
        }


        public IEnumerable<Xbim.Ifc4.Interfaces.IIfcBuildingStorey> BuildingStoreys
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

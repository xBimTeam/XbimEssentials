using Xbim.Ifc2x3.QuantityResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.SharedBldgElements;

namespace Xbim.Ifc2x3.Extensions
{
    public static class SlabExtensions
    {
        /// <summary>
        /// Returns the Gross Footprint Area, if the element base quantity GrossFloorArea is defined
        /// </summary>
        /// <returns></returns>
        public static IfcAreaMeasure GrossFootprintArea(this IfcSlab slab)
        {
            IfcQuantityArea qArea = slab.GetQuantity<IfcQuantityArea>("BaseQuantities", "GrossFootprintArea");
            if (qArea == null) qArea = slab.GetQuantity<IfcQuantityArea>("GrossFootprintArea"); //just look for any area
            if (qArea == null) qArea = slab.GetQuantity<IfcQuantityArea>("CrossArea"); //just look for any area if revit has done it
            if (qArea != null) return qArea.AreaValue;
            //try none schema defined properties

            return new IfcAreaMeasure();
        }
    }
}

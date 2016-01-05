using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.QuantityResource;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc2x3.ProductExtension
{
    public partial class IfcSpace
    {
        /// <summary>
        /// Returns the Gross Floor Area, if the element base quantity GrossFloorArea is defined
        /// </summary>
        /// <returns></returns>
        public IfcAreaMeasure? GrossFloorArea
        {
            get
            {
                var qArea = GetQuantity<IfcQuantityArea>("BaseQuantities", "GrossFloorArea") ??
                            GetQuantity<IfcQuantityArea>("GrossFloorArea");
                return qArea != null ? (IfcAreaMeasure?) qArea.AreaValue : null;
                //try none schema defined properties
            }
        }


        /// <summary>
        /// Returns the Net Floor Area, if the element base quantity GrossFloorArea is defined
        /// Will use GSA Space Areas if the Ifc common property NetFloorArea is not defined
        /// </summary>
        /// <returns></returns>
        public IfcAreaMeasure? NetFloorArea
        {
            get
            {
                var qArea = GetQuantity<IfcQuantityArea>("BaseQuantities", "NetFloorArea") ??
                            GetQuantity<IfcQuantityArea>("NetFloorArea");
                if (qArea != null) return qArea.AreaValue;
                //try none schema defined properties
                qArea = GetQuantity<IfcQuantityArea>("GSA Space Areas", "GSA BIM Area");
                if (qArea != null) return qArea.AreaValue;
                return null;
            }
        }
        /// <summary>
        /// Returns the Height, if the element base quantity Height is defined
        /// </summary>
        /// <returns></returns>
        public IfcLengthMeasure? Height
        {
            get
            {
                var qLength = GetQuantity<IfcQuantityLength>("BaseQuantities", "Height") ??
                              GetQuantity<IfcQuantityLength>("Height");
                return qLength != null ? (IfcLengthMeasure?) qLength.LengthValue : null;
                //try none schema defined properties
            }
        }


        /// <summary>
        /// Returns the Perimeter, if the element base quantity GrossPerimeter is defined
        /// 
        /// </summary>
        /// <returns></returns>
        public IfcLengthMeasure? GrossPerimeter
        {
            get
            {
                var qLength = GetQuantity<IfcQuantityLength>("BaseQuantities", "GrossPerimeter") ??
                              GetQuantity<IfcQuantityLength>("GrossPerimeter");
                if (qLength != null) return qLength.LengthValue;
                //try none schema defined properties
                return null;
            }
        }

    }
}

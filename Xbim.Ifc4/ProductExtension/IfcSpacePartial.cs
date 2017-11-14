using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.QuantityResource;

namespace Xbim.Ifc4.Interfaces
{
    /// <summary>
    /// Readonly interface for IfcSpace
    /// </summary>
    // ReSharper disable once PartialTypeWithSinglePart
    public partial interface @IIfcSpace
    {
        IEnumerable<IIfcSpace> Spaces { get; }
    }
}

namespace Xbim.Ifc4.ProductExtension
{
    public partial class IfcSpace
    {
        #region Properties
        /// <summary>
        /// Returns the Gross Floor Area, if the element base quantity GrossFloorArea is defined
        /// </summary>
        /// <returns></returns>
        public IfcAreaMeasure? GrossFloorArea
        {
            get
            {
                var qArea = GetQuantity<IIfcQuantityArea>("BaseQuantities", "GrossFloorArea") ??
                            GetQuantity<IIfcQuantityArea>("GrossFloorArea");
                return qArea != null ? (IfcAreaMeasure?)qArea.AreaValue : null;
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
                var qArea = GetQuantity<IIfcQuantityArea>("BaseQuantities", "NetFloorArea") ??
                            GetQuantity<IIfcQuantityArea>("NetFloorArea");
                if (qArea != null) return qArea.AreaValue;
                //try none schema defined properties
                qArea = GetQuantity<IIfcQuantityArea>("GSA Space Areas", "GSA BIM Area");
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
                var qLength = GetQuantity<IIfcQuantityLength>("BaseQuantities", "Height") ??
                              GetQuantity<IIfcQuantityLength>("Height");
                return qLength != null ? (IfcLengthMeasure?)qLength.LengthValue : null;
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
                var qLength = GetQuantity<IIfcQuantityLength>("BaseQuantities", "GrossPerimeter") ??
                              GetQuantity<IIfcQuantityLength>("GrossPerimeter");
                if (qLength != null) return qLength.LengthValue;
                //try none schema defined properties
                return null;
            }
        } 
        #endregion

        /// <summary>
        /// Returns all spaces that are sub-spaces of this space
        /// </summary>
        /// <returns></returns>
        public  IEnumerable<IIfcSpace> Spaces
        {
            get
            {
                return IsDecomposedBy.SelectMany(s => s.RelatedObjects).OfType<IIfcSpace>();
            }
        }
    }
}

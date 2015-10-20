#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    SpaceExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.QuantityResource;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class SpaceExtensions
    {
        #region Representation methods

        public static IfcShapeRepresentation GetFootPrintRepresentation(this IfcSpace space)
        {
            if (space.Representation != null)
                return
                    space.Representation.Representations.OfType<IfcShapeRepresentation>().FirstOrDefault(
                        r => string.Compare(r.RepresentationIdentifier.GetValueOrDefault(), "FootPrint", true) == 0);
            return null;
        }

        #endregion

        #region Generator methods

        /// <summary>
        ///   If the space has a footprint represenation this will generate a set of walls conforming to that footprint, otherwise returns null
        /// </summary>
        /// <param name = "space"></param>
        /// <param name = "model"></param>
        /// <returns></returns>
        public static List<IfcWall> GenerateWalls(this IfcSpace space, IModel model)
        {
            var fp = GetFootPrintRepresentation(space);
            if (fp != null)
            {
                var rep = fp.Items.FirstOrDefault();
                if (rep != null && rep is IfcGeometricCurveSet) //we have a set of curves and inner boundaries
                {
                }
                else if (rep != null)
                {
                }
            }
            return null;
        }

        #endregion

        /// <summary>
        /// Returns the Gross Floor Area, if the element base quantity GrossFloorArea is defined
        /// </summary>
        /// <returns></returns>
        public static IfcAreaMeasure? GetGrossFloorArea(this IfcSpace space)
        {
            var qArea = space.GetQuantity<IfcQuantityArea>("BaseQuantities", "GrossFloorArea");
            if (qArea == null) qArea = space.GetQuantity<IfcQuantityArea>("GrossFloorArea"); //just look for any area
            if (qArea != null) return qArea.AreaValue;
            //try none schema defined properties

            return null;
        }


        /// <summary>
        /// Returns the Net Floor Area, if the element base quantity GrossFloorArea is defined
        /// Will use GSA Space Areas if the Ifc common property NetFloorArea is not defined
        /// </summary>
        /// <returns></returns>
        public static IfcAreaMeasure? GetNetFloorArea(this IfcSpace space)
        {
            var qArea = space.GetQuantity<IfcQuantityArea>("BaseQuantities", "NetFloorArea");
            if (qArea == null) qArea = space.GetQuantity<IfcQuantityArea>("NetFloorArea"); //just look for any area
            if (qArea != null) return qArea.AreaValue;
            //try none schema defined properties
            qArea = space.GetQuantity<IfcQuantityArea>("GSA Space Areas", "GSA BIM Area");
            if (qArea != null) return qArea.AreaValue;
            return null;
        }
        /// <summary>
        /// Returns the Height, if the element base quantity Height is defined
        /// </summary>
        /// <returns></returns>
        public static IfcLengthMeasure? GetHeight(this IfcSpace space)
        {
            var qLength = space.GetQuantity<IfcQuantityLength>("BaseQuantities", "Height");
            if (qLength == null) qLength = space.GetQuantity<IfcQuantityLength>("Height"); //just look for any area
            if (qLength != null) return qLength.LengthValue;
            //try none schema defined properties
            return null;
        }


        /// <summary>
        /// Returns the Perimeter, if the element base quantity GrossPerimeter is defined
        /// 
        /// </summary>
        /// <returns></returns>
        public static IfcLengthMeasure? GetGrossPerimeter(this IfcSpace space)
        {
            var qLength = space.GetQuantity<IfcQuantityLength>("BaseQuantities", "GrossPerimeter") ??
                          space.GetQuantity<IfcQuantityLength>("GrossPerimeter");
            if (qLength != null) return qLength.LengthValue;
            //try none schema defined properties
            return null;
        }
        /// <summary>
        /// Returns all spaces that are sub-spaces of this space
        /// </summary>
        /// <param name="space"></param>
        /// <returns></returns>
        public static IEnumerable<IfcSpace> GetSpaces(this IfcSpace space)
        {
            var decomp = space.IsDecomposedBy;
            var objs = decomp.SelectMany(s => s.RelatedObjects);
            return objs.OfType<IfcSpace>();

        }

        public static void AddBoundingElement(this IfcSpace space, IModel model, IfcElement element,
                                              IfcPhysicalOrVirtualEnum physicalOrVirtualBoundary,
                                              IfcInternalOrExternalEnum internalOrExternalBoundary)
        {
            //avoid adding element which is already defined as bounding element
            if (space.HasBoundingElement(model, element)) return;

            model.Instances.New<IfcRelSpaceBoundary>(rel =>
                                                                              {
                                                                                  rel.RelatingSpace = space;
                                                                                  rel.InternalOrExternalBoundary =
                                                                                      internalOrExternalBoundary;
                                                                                  rel.RelatedBuildingElement = element;
                                                                                  rel.PhysicalOrVirtualBoundary =
                                                                                      physicalOrVirtualBoundary;
                                                                              });
        }
        /// <summary>
        /// Returns the IfcSpaceType of this space, null if one is not defined
        /// </summary>
        /// <param name="space"></param>
        /// <returns></returns>
        public static IfcSpaceType GetSpaceType(this IfcSpace space)
        {
            var sType =  space.GetDefiningType();
            return sType as IfcSpaceType;
        }

        public static bool HasBoundingElement(this IfcSpace space, IModel model, IfcElement element)
        {
            var relation =
                model.Instances.Where<IfcRelSpaceBoundary>(
                    rel => rel.RelatingSpace == space && rel.RelatedBuildingElement == element).FirstOrDefault();
            return relation != null;
        }
    }
}
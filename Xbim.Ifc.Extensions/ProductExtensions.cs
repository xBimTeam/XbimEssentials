#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    ProductExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class ProductExtensions
    {
        public static IfcShapeRepresentation GetAxisRepresentation(this IfcProduct prod)
        {
            if (prod.Representation != null)
                return
                    prod.Representation.Representations.OfType<IfcShapeRepresentation>().FirstOrDefault(
                        r => string.Compare(r.RepresentationIdentifier.GetValueOrDefault(), "Axis", true) == 0);
            return null;
        }
        /// <summary>
        /// Returns the spatial structural elements that this product is in
        /// </summary>
        /// <param name="prod"></param>
        /// <returns></returns>
        public static IEnumerable<IfcSpatialStructureElement> IsContainedIn(this IfcProduct prod)
        {
            return prod.Model.Instances.Where<IfcRelContainedInSpatialStructure>(r => r.RelatedElements.Contains(prod)).Select(s=>s.RelatingStructure);
        }
        /// <summary>
        ///   Returns the first Body(Solid) Representation, null if none exists
        /// </summary>
        /// <returns></returns>
        public static IfcShapeRepresentation GetBodyRepresentation(this IfcProduct prod)
        {
            if (prod.Representation != null)
                return
                    prod.Representation.Representations.OfType<IfcShapeRepresentation>().Where(
                        r => string.Compare(r.RepresentationIdentifier.GetValueOrDefault(), "Body", true) == 0).FirstOrDefault();
            return null;
        }

        /// <summary>
        ///   Sets new object placement as LocalPlacement with defined coordinates. If any placement exists it is overwritten;
        /// </summary>
        /// <param name = "placementX">X coordinate of placement</param>
        /// <param name = "placementY">Y coordinate of placement</param>
        /// <param name = "placementZ">Z coordinate of placement</param>
        public static void SetNewObjectLocalPlacement(this IfcProduct prod, double placementX, double placementY,
                                                      double placementZ)
        {
            var model = prod.Model;

            prod.ObjectPlacement = model.Instances.New<IfcLocalPlacement>();
            var localPlacement = prod.ObjectPlacement as IfcLocalPlacement;

            if (localPlacement.RelativePlacement == null)
                localPlacement.RelativePlacement = model.Instances.New<IfcAxis2Placement3D>();
            var placement = localPlacement.RelativePlacement as IfcAxis2Placement3D;
            placement.SetNewLocation(placementX, placementY, placementZ);
        }

        /// <summary>
        ///   Sets X axis direction of the existing local placement. If it does not exist, exception raises.
        /// </summary>
        /// <param name = "xAxisDirectionX">X coordinate of the X asis direction</param>
        /// <param name = "xAxisDirectionY">Y coordinate of the X asis direction</param>
        /// <param name = "xAxisDirectionZ">Z coordinate of the X asis direction</param>
        public static void SetObjectLocalPlacement_XZdirection(this IfcProduct prod, double xAxisDirectionX,
                                                               double xAxisDirectionY, double xAxisDirectionZ,
                                                               double zAxisDirectionX, double zAxisDirectionY,
                                                               double Z_axisDirection_Z)
        {
            var localPlacement = prod.ObjectPlacement as IfcLocalPlacement;
            if (localPlacement == null) throw new Exception("ProductExtensions: Local placement is not defined.");

            var placement = localPlacement.RelativePlacement as IfcAxis2Placement3D;
            if (placement == null) throw new Exception("ProductExtensions: Local placement is not defined.");

            placement.SetNewDirectionOf_XZ(xAxisDirectionX, xAxisDirectionY, xAxisDirectionZ, zAxisDirectionX,
                                           zAxisDirectionY, Z_axisDirection_Z);
        }

        /// <summary>
        ///   Returns first set of IFC representation items or null;
        /// </summary>
        public static IfcShapeRepresentation GetFirstShapeRepresentation(this IfcProduct prod)
        {
            var definitionShape = prod.Representation as IfcProductDefinitionShape;
            if (definitionShape == null)
            {
                return null;
            }

            var shapeRepresentation = definitionShape.Representations.FirstOrDefault() as IfcShapeRepresentation;
            return shapeRepresentation;
        }

        /// <summary>
        ///   Returns set of IFC representation items from the specified context or null;
        /// </summary>
        public static ItemSet<IfcRepresentationItem> GetShapeRepresentationItems(this IfcProduct prod,
                                                                                 IfcRepresentationContext context)
        {
            var definitionShape = prod.Representation as IfcProductDefinitionShape;
            if (definitionShape == null)
            {
                return null;
            }

            var shapeRepresentation =
                definitionShape.Representations.FirstOrDefault<IfcShapeRepresentation>(rep => rep.ContextOfItems == context);
            return shapeRepresentation == null ? null : shapeRepresentation.Items;
        }


        /// <summary>
        ///   Creates new body representation it as "Body", "Brep".
        /// </summary>
        /// <param name = "context">Geometry context</param>
        /// <returns>New empty set of representation items</returns>
        public static IfcShapeRepresentation GetNewBrepShapeRepresentation(this IfcProduct prod,
                                                                                        IfcRepresentationContext context)
        {
            var model = (prod as IPersistEntity).Model;
            if (model == null) model = prod.Model;
            var definitionShape = prod.Representation as IfcProductDefinitionShape;
            if (definitionShape == null)
            {
                definitionShape = model.Instances.New<IfcProductDefinitionShape>();
                prod.Representation = definitionShape;
            }

            var shapeRepresentation = model.Instances.New<IfcShapeRepresentation>();
            shapeRepresentation.ContextOfItems = context; // model.IfcProject.ModelContext();
            shapeRepresentation.RepresentationIdentifier = "Body";
            shapeRepresentation.RepresentationType = "Brep";
            definitionShape.Representations.Add(shapeRepresentation);
            return shapeRepresentation;
        }

        /// <summary>
        ///   Creates new body representation it as "Body", "Brep".
        /// </summary>
        /// <param name = "context">Geometry context</param>
        /// <returns>New empty set of representation items</returns>
        public static IfcShapeRepresentation GetNewSweptSolidShapeRepresentation(this IfcProduct prod,
                                                                                              IfcRepresentationContext
                                                                                                  context)
        {
            var model = (prod as IPersistEntity).Model;
            if (model == null) model = prod.Model;
            var definitionShape = prod.Representation as IfcProductDefinitionShape;
            if (definitionShape == null)
            {
                definitionShape = model.Instances.New<IfcProductDefinitionShape>();
                prod.Representation = definitionShape;
            }

            var shapeRepresentation = model.Instances.New<IfcShapeRepresentation>();
            shapeRepresentation.ContextOfItems = context; // model.IfcProject.ModelContext();
            shapeRepresentation.RepresentationIdentifier = "Body";
            shapeRepresentation.RepresentationType = "SweptSolid";
            definitionShape.Representations.Add(shapeRepresentation);
            return shapeRepresentation;
        }

        public static IfcShapeRepresentation GetSweptSolidShapeRepresentation(this IfcProduct prod)
        {
            if (prod.Representation != null && prod.Representation.Representations!=null)
                return
                    prod.Representation.Representations.OfType<IfcShapeRepresentation>().FirstOrDefault(
                        r => string.Compare(r.RepresentationIdentifier.GetValueOrDefault(), "Body", true) == 0 &&
                             string.Compare(r.RepresentationType.GetValueOrDefault(), "SweptSolid", true) == 0);
            return null;
        }

        public static IfcShapeRepresentation GetOrCreateSweptSolidShapeRepresentation(this IfcProduct prod, IfcRepresentationContext context)
        {
            return GetSweptSolidShapeRepresentation(prod) ?? GetNewSweptSolidShapeRepresentation(prod, context);
        }
    }
}
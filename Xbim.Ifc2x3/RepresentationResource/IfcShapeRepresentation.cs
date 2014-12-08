#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcShapeRepresentation.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.PresentationDefinitionResource;
using Xbim.Ifc2x3.TopologyResource;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.RepresentationResource
{
    /// <summary>
    ///   The shape representation is a specific kind of representation that represents a shape.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: The shape representation is a specific kind of representation that represents a shape. 
    ///   Definition from IAI: The IfcShapeRepresentation represents the concept of a particular geometric representation of a product or a product component within a specific geometric representation context. The inherited attribute RepresentationType is used to define the geometric model used for the shape representation, the inherited attribute RepresentationIdentifier is used to denote the part of the representation captured by the IfcShapeRepresentation (e.g. Axis, Body, etc.). 
    ///   Several representation types for shape representation are included as predefined types: 
    ///   Curve2D 2 dimensional curves  
    ///   GeometricSet  points, curves, surfaces (2 or 3 dimensional) 
    ///   GeometricCurveSet points, curves (2 or 3 dimensional) 
    ///   SurfaceModel  face based and shell based surface model 
    ///   SolidModel  including swept solid, Boolean results and Brep bodies
    ///   more specific types are: 
    ///   SweptSolid swept area solids, by extrusion and revolution 
    ///   Brep faceted Brep's with and without voids 
    ///   CSG Boolean results of operations between solid models, half spaces and Boolean results 
    ///   Clipping Boolean differences between swept area solids, half spaces and Boolean results 
    ///   AdvancedSweptSolid swept area solids created by sweeping a profile along a directrix 
    ///   additional types  some additional representation types are given: 
    ///   BoundingBox simplistic 3D representation by a bounding box 
    ///   SectionedSpine cross section based representation of a spine curve and planar cross sections. It can represent a surface or a solid and the interpolations of the between the cross sections is not defined 
    ///   MappedRepresentation representation based on mapped item(s), referring to a representation map. Note: it can be seen as an inserted block reference. The shape representation of the mapped item has a representation type declaring the type of its representation items. 
    ///   Table 1: string values for the inherited attribute 'RepresentationType'.
    ///   NOTE: The definition of this entity relates to the STEP entity shape_representation. Please refer to ISO/IS 10303-41:1994 for the final definition of the formal standard. 
    ///   HISTORY: New entity in IFC Release 1.5.
    ///   Formal Propositions:
    ///   WR21   :   The context to which the IfcShapeRepresentation is assign, shall be of type IfcGeometricRepresentationContext.  
    ///   WR22   :   No topological representation item shall be directly used for shape representations, with the exception of IfcVertexPoint, IfcEdgeCurve, IfcFaceSurface.  
    ///   WR23   :   A representation type should be given to the shape representation.  
    ///   WR24   :   Checks the proper use of Items according to the RepresentationType.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcShapeRepresentation : IfcShapeModel
    {
        public override string WhereRule()
        {
            string typeErr;
            string baseErr = base.WhereRule();
            if (!(ContextOfItems is IfcGeometricRepresentationContext))
                baseErr +=
                    "WR21 ShapeRepresentation: The context to which the IfcShapeRepresentation is assign, shall be of type IfcGeometricRepresentationContext.\n";
            if (
                Items.OfType<IfcTopologicalRepresentationItem>().Where(
                    tr => !(tr is IfcVertexPoint) && !(tr is IfcEdgeCurve) && !(tr is IfcFaceSurface)).Count() != 0)
                baseErr +=
                    "WR22 ShapeRepresentation: No topological representation item shall be directly used for shape representations, with the exception of IfcVertexPoint, IfcEdgeCurve, IfcFaceSurface.\n";
            if (string.IsNullOrEmpty(RepresentationType.GetValueOrDefault()))
                baseErr +=
                    "WR23 ShapeRepresentation: A representation type should be given to the shape representation.\n";
            if (!ShapeRepresentationTypesComply(out typeErr))
            {
                baseErr += "WR24 ShapeRepresentation: Incorrect use of Items according to the RepresentationType\n";
                baseErr += typeErr;
                baseErr += '\n';
            }
            return baseErr;
        }

        #region Ifc Schema Validation Methods

        private bool ShapeRepresentationTypesComply(
            out string errStr)
        {
            string repType = RepresentationType.GetValueOrDefault();
            string msg = "";
            bool found = true;
            XbimSet<IfcRepresentationItem> items = Items;
            int count = 0;
            //split case statements to overcome bug in db4o, which cannot traverse more than 5 deep in a case statement
            switch (repType)
            {
                case "Curve2D":
                    count = items.OfType<IfcCurve>().Where(i => i.Dim == 2).Count();
                    msg = "RepresentationType = Curve2D, but Items contains shapes that are not Curve";
                    break;
                case "Annotation2D":
                    count =
                        items.Where(
                            i =>
                            i is IfcPoint || i is IfcCurve || i is IfcGeometricCurveSet || i is IfcAnnotationFillArea ||
                            i is IfcDefinedSymbol || i is IfcTextLiteral || i is IfcDraughtingCallOut).Count();
                    msg =
                        "RepresentationType = Annotation2D, but Items contains shapes that are not GeometricCurveSet, Curve, or AnnotationFillArea, DefinedSymbol, TextLiteral or DraughtingCallOut";
                    break;
                case "GeometricSet":
                    count =
                        items.Where(i => i is IfcGeometricSet || i is IfcPoint || i is IfcCurve || i is IfcSurface).
                            Count();
                    msg =
                        "RepresentationType = GeometricSet, but Items contains shapes that are not Point, Curve, or Surface";
                    break;
                case "GeometricCurveSet":
                    count =
                        items.Where(
                            i => i is IfcGeometricCurveSet || i is IfcPoint || i is IfcCurve || i is IfcGeometricSet).
                            Count();
                    msg =
                        "RepresentationType = GeometricCurveSet, but Items contains shapes that are not Point, Curve, or GeometricSet";
                    foreach (IfcRepresentationItem item in items)
                    {
                        IfcGeometricSet gs = item as IfcGeometricSet;
                        if (gs != null)
                        {
                            if (gs.Elements.OfType<IfcSurface>().Count() > 0)
                                count--;
                        }
                    }
                    break;
                case "SurfaceModel":
                    count =
                        items.Where(
                            i =>
                            i is IfcShellBasedSurfaceModel || i is IfcFaceBasedSurfaceModel || i is IfcFacetedBrep ||
                            i is IfcFacetedBrepWithVoids).Count();
                    msg =
                        "RepresentationType = SurfaceModel, but Items contains shapes that are not ShellBasedSurfaceModel, FaceBasedSurfaceModel, FacetedBrepWithVoids or FacetedBrep";
                    break;
                default:
                    found = false;
                    break;
            }
            if (!found)
            {
                found = true;
                switch (repType)
                {
                    case "SolidModel":
                        count = items.OfType<IfcSolidModel>().Count();
                        msg = "RepresentationType = SolidModel, but Items contains shapes that are not SolidModel";
                        break;
                    case "SweptSolid":
                        count = items.OfType<IfcSweptAreaSolid>().Count();
                        msg = "RepresentationType = SweptSolid, but Items contains shapes that are not SweptAreaSolid";
                        break;
                    case "CSG":
                        count = items.OfType<IfcBooleanResult>().Count();
                        msg = "RepresentationType = CSG, but Items contains shapes that are not BooleanResult";
                        break;
                    case "Clipping":
                        count = items.OfType<IfcBooleanClippingResult>().Count();
                        msg =
                            "RepresentationType = Clipping, but Items contains shapes that are not BooleanClippingResult";
                        break;
                    case "AdvancedSweptSolid":
                        count = items.Where(i => i is IfcSurfaceCurveSweptAreaSolid || i is IfcSweptDiskSolid).Count();
                        msg =
                            "RepresentationType = AdvancedSweptSolid, but Items contains shapes that are not SurfaceCurveSweptAreaSolid or SweptDiskSolid";
                        break;
                    default:
                        found = false;
                        break;
                }
            }
            if (!found)
            {
                found = true;
                switch (repType)
                {
                    case "Brep":
                        count = items.Where(i => i is IfcFacetedBrep || i is IfcFacetedBrepWithVoids).Count();
                        msg =
                            "RepresentationType = Brep, but Items contains shapes that are not FacetedBrep or FacetedBrepWithVoids";
                        break;
                    case "BoundingBox":
                        count = items.OfType<IfcBoundingBox>().Count();
                        msg = "RepresentationType = BoundingBox, but Items contains shapes that are not BoundingBox";
                        if (items.Count > 1) count = 0;
                        break;
                    case "SectionedSpine":
                        count = items.OfType<IfcSectionedSpine>().Count();
                        msg =
                            "RepresentationType = SectionedSpine, but Items contains shapes that are not SectionedSpine";
                        break;
                    case "MappedRepresentation":
                        count = items.OfType<IfcMappedItem>().Count();
                        msg =
                            "RepresentationType = MappedRepresentation, but Items contains shapes that are not MappedItem";
                        break;
                    default:
                        found = false;
                        break;
                }
            }
            if (!found)
            {
                errStr = string.Format("Illegal or unknown Representation Identifier, {0}", repType);
                return false;
            }
            if (count != items.Count)
            {
                errStr = msg;
                return false;
            }
            else
            {
                errStr = "";
                return true;
            }
        }

        #endregion
    }
}
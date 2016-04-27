﻿using System.IO;
using Xbim.Common.Geometry;
using Xbim.Common.Logging;

// ReSharper disable once CheckNamespace
namespace Xbim.Ifc4.Interfaces
{
    public interface IXbimGeometryEngine
    {
        ILogger Logger { get; }

        IXbimGeometryObject Create(IIfcGeometricRepresentationItem ifcRepresentation);


        IXbimGeometryObject Create(IIfcGeometricRepresentationItem ifcRepresentation, IIfcAxis2Placement3D objectLocation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryObject"></param>
        /// <param name="precision">the distance at which two points are considered to be the same</param>
        /// <param name="deflection">the max distance between the chord of a curve and the line segment of a faceted edge </param>
        /// <param name="angle">Defaults to 0.5</param>
        /// <param name="storageType">Defaults to Polyhedron in compressed text format</param>
        /// <returns></returns>
        XbimShapeGeometry CreateShapeGeometry(IXbimGeometryObject geometryObject, double precision, double deflection, double angle, XbimGeometryType storageType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryObject"></param>
        /// <param name="precision">the distance at which two points are considered to be the same</param>
        /// <param name="deflection">the max distance between the chord of a curve and the line segment of a faceted edge </param>
        /// <returns></returns>
        XbimShapeGeometry CreateShapeGeometry(IXbimGeometryObject geometryObject, double precision, double deflection /*, double angle=0.5, XbimGeometryType storageType = XbimGeometryType::Polyhedron*/);
        IXbimGeometryObjectSet CreateGeometryObjectSet();
        IXbimSolid CreateSolid(IIfcSweptAreaSolid ifcSolid);
        IXbimSolid CreateSolid(IIfcExtrudedAreaSolid ifcSolid);
        IXbimSolid CreateSolid(IIfcRevolvedAreaSolid ifcSolid);
        IXbimSolid CreateSolid(IIfcSweptDiskSolid ifcSolid);
        IXbimSolid CreateSolid(IIfcBoundingBox ifcSolid);
        IXbimSolid CreateSolid(IIfcSurfaceCurveSweptAreaSolid ifcSolid);

        IXbimSolid CreateSolid(IIfcBooleanClippingResult ifcSolid);
        IXbimSolid CreateSolid(IIfcBooleanOperand ifcSolid);
        IXbimSolid CreateSolid(IIfcHalfSpaceSolid ifcSolid);
        IXbimSolid CreateSolid(IIfcPolygonalBoundedHalfSpace ifcSolid);
        IXbimSolid CreateSolid(IIfcBoxedHalfSpace ifcSolid);

        IXbimSolidSet CreateSolidSet(IIfcManifoldSolidBrep ifcSolid);
        IXbimSolidSet CreateSolidSet(IIfcFacetedBrep ifcSolid);
        IXbimSolidSet CreateSolidSet(IIfcFacetedBrepWithVoids ifcSolid);
        IXbimSolidSet CreateSolidSet(IIfcClosedShell ifcSolid);

        IXbimSolid CreateSolid(IIfcCsgPrimitive3D ifcSolid);
        IXbimSolid CreateSolid(IIfcCsgSolid ifcSolid);
        IXbimSolid CreateSolid(IIfcSphere ifcSolid);
        IXbimSolid CreateSolid(IIfcBlock ifcSolid);
        IXbimSolid CreateSolid(IIfcRightCircularCylinder ifcSolid);
        IXbimSolid CreateSolid(IIfcRightCircularCone ifcSolid);
        IXbimSolid CreateSolid(IIfcRectangularPyramid ifcSolid);
        //Ifc4 specific classes
        IXbimSolid CreateSolid(IIfcSweptDiskSolidPolygonal ifcSolid);
        IXbimSolid CreateSolid(IIfcRevolvedAreaSolidTapered ifcSolid);
        IXbimSolid CreateSolid(IIfcFixedReferenceSweptAreaSolid ifcSolid);
        IXbimSolid CreateSolid(IIfcAdvancedBrep ifcSolid);
        IXbimSolid CreateSolid(IIfcAdvancedBrepWithVoids ifcSolid);
        IXbimSolid CreateSolid(IIfcSectionedSpine ifcSolid);

        //Shells creation
        IXbimShell CreateShell(IIfcOpenShell shell);
        IXbimShell CreateShell(IIfcConnectedFaceSet shell);
        IXbimShell CreateShell(IIfcSurfaceOfLinearExtrusion shell);
        //Ifc4 specific classes
        //IXbimShell CreateShell(IfcTessellatedItem shell);
        //IXbimShell CreateShell(IIfcTessellatedFaceSet shell);
        IXbimGeometryObjectSet CreateSurfaceModel(IIfcTriangulatedFaceSet shell);

        //Surface Models containing one or more faces, shells or solids
        IXbimGeometryObjectSet CreateSurfaceModel(IIfcShellBasedSurfaceModel ifcSurface);
        IXbimGeometryObjectSet CreateSurfaceModel(IIfcFaceBasedSurfaceModel ifcSurface);

        //Faces

        IXbimFace CreateFace(IIfcProfileDef profileDef);
        IXbimFace CreateFace(IIfcCompositeCurve cCurve);
        IXbimFace CreateFace(IIfcPolyline pline);
        IXbimFace CreateFace(IIfcPolyLoop loop);
        IXbimFace CreateFace(IIfcSurface surface);
        IXbimFace CreateFace(IIfcPlane plane);
        IXbimFace CreateFace(IXbimWire wire);

        //Create Wire
        IXbimWire CreateWire(IIfcCurve curve);
        IXbimWire CreateWire(IIfcCompositeCurveSegment compCurveSeg);

        IXbimCurve CreateCurve(IIfcCurve curve);
        IXbimCurve CreateCurve(IIfcPolyline curve);
        IXbimCurve CreateCurve(IIfcCircle curve);
        IXbimCurve CreateCurve(IIfcEllipse curve);
        IXbimCurve CreateCurve(IIfcLine curve);
        IXbimCurve CreateCurve(IIfcTrimmedCurve curve);
        IXbimCurve CreateCurve(IIfcRationalBSplineCurveWithKnots curve);
        IXbimCurve CreateCurve(IIfcBSplineCurveWithKnots curve);
        IXbimCurve CreateCurve(IIfcOffsetCurve3D curve);
        IXbimCurve CreateCurve(IIfcOffsetCurve2D curve);
        IXbimPoint CreatePoint(double x, double y, double z, double tolerance);
        IXbimPoint CreatePoint(IIfcCartesianPoint p);
        IXbimPoint CreatePoint(XbimPoint3D p, double tolerance);
        IXbimPoint CreatePoint(IIfcPoint pt);
        IXbimPoint CreatePoint(IIfcPointOnCurve p);
        IXbimPoint CreatePoint(IIfcPointOnSurface p);

        //Vertex Creation
        IXbimVertex CreateVertexPoint(XbimPoint3D point, double precision);

        //IIfcFacetedBrep CreateFacetedBrep(Xbim.Common.IModel model, IXbimSolid solid);
        //Creates collections of objects
        IXbimSolidSet CreateSolidSet();
        IXbimSolidSet CreateSolidSet(IIfcBooleanResult boolOp);
        IXbimSolidSet CreateGrid(IIfcGrid grid);
        //converts an object placement to a matrix transform in the WCS
        XbimMatrix3D ToMatrix3D(IIfcObjectPlacement objPlacement);
        //Read and write functions
        void WriteTriangulation(TextWriter tw, IXbimGeometryObject shape, double tolerance, double deflection, double angle);
        void WriteTriangulation(BinaryWriter bw, IXbimGeometryObject shape, double tolerance, double deflection, double angle);
        void Mesh(IXbimMeshReceiver receiver, IXbimGeometryObject geometryObject, double precision, double deflection, double angle=0.5);
        

        //Transforms an object geometrically and returns a new copy of the object, geometryObject passed is unchanged.
        IXbimGeometryObject Transformed(IXbimGeometryObject geometryObject, IIfcCartesianTransformationOperator transformation);
        //Moves an object to the required placement, the original geometry is unchanged a copy is returned. Tags are preserved
        IXbimGeometryObject Moved(IXbimGeometryObject geometryObject, IIfcPlacement placement);
        //Moves an object to the required placement, the original geometry is unchanged a copy is returned. Tags are preserved
        IXbimGeometryObject Moved(IXbimGeometryObject geometryObject, IIfcObjectPlacement objectPlacement);
        //Moves an object to the required placement, the original geometry is unchanged a copy is returned. Tags are preserved
        IXbimGeometryObject Moved(IXbimGeometryObject geometryObject, IIfcAxis2Placement3D placement);
        IXbimGeometryObject Moved(IXbimGeometryObject geometryObject, IIfcAxis2Placement2D placement);


    }
}

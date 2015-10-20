using System.IO;
using Xbim.Common.Geometry;
using Xbim.Common.Logging;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.TopologyResource;
using Xbim.IO;
using XbimGeometry.Interfaces;

namespace Xbim.Ifc2x3.IO
{
    public interface IXbimGeometryCreator
    {
        ILogger Logger { get; }

        IXbimGeometryObject Create(IfcGeometricRepresentationItem ifcRepresentation);


        IXbimGeometryObject Create(IfcGeometricRepresentationItem ifcRepresentation, IfcAxis2Placement3D objectLocation);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryObject"></param>
        /// <param name="precision">the distance at which two points are considered to be the same</param>
        /// <param name="deflection">the max distance between the chord of a curve and the line segment of a faceted edge </param>
        /// <param name="angle">Defaults to 0.5</param>
        /// <param name="storageType">Defaults to Polyhedron in compressed text format</param>
        /// <returns></returns>
        IXbimShapeGeometryData CreateShapeGeometry(IXbimGeometryObject geometryObject, double precision, double deflection, double angle, XbimGeometryType storageType);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometryObject"></param>
        /// <param name="precision">the distance at which two points are considered to be the same</param>
        /// <param name="deflection">the max distance between the chord of a curve and the line segment of a faceted edge </param>
        /// <returns></returns>
        IXbimShapeGeometryData CreateShapeGeometry(IXbimGeometryObject geometryObject, double precision, double deflection /*, double angle=0.5, XbimGeometryType storageType = XbimGeometryType::Polyhedron*/);
        IXbimGeometryObjectSet CreateGeometryObjectSet();
        IXbimSolid CreateSolid(IfcSweptAreaSolid ifcSolid);
        IXbimSolid CreateSolid(IfcExtrudedAreaSolid ifcSolid);
        IXbimSolid CreateSolid(IfcRevolvedAreaSolid ifcSolid);
        IXbimSolid CreateSolid(IfcSweptDiskSolid ifcSolid);
        IXbimSolid CreateSolid(IfcBoundingBox ifcSolid);
        IXbimSolid CreateSolid(IfcSurfaceCurveSweptAreaSolid ifcSolid);

        IXbimSolid CreateSolid(IfcBooleanClippingResult ifcSolid);
        IXbimSolid CreateSolid(IfcBooleanOperand ifcSolid);
        IXbimSolid CreateSolid(IfcHalfSpaceSolid ifcSolid);
        IXbimSolid CreateSolid(IfcPolygonalBoundedHalfSpace ifcSolid);
        IXbimSolid CreateSolid(IfcBoxedHalfSpace ifcSolid);

        IXbimSolidSet CreateSolidSet(IfcManifoldSolidBrep ifcSolid);
        IXbimSolidSet CreateSolidSet(IfcFacetedBrep ifcSolid);
        IXbimSolidSet CreateSolidSet(IfcFacetedBrepWithVoids ifcSolid);
        IXbimSolidSet CreateSolidSet(IfcClosedShell ifcSolid);

        IXbimSolid CreateSolid(IfcCsgPrimitive3D ifcSolid);
        IXbimSolid CreateSolid(IfcCsgSolid ifcSolid);
        IXbimSolid CreateSolid(IfcSphere ifcSolid);
        IXbimSolid CreateSolid(IfcBlock ifcSolid);
        IXbimSolid CreateSolid(IfcRightCircularCylinder ifcSolid);
        IXbimSolid CreateSolid(IfcRightCircularCone ifcSolid);
        IXbimSolid CreateSolid(IfcRectangularPyramid ifcSolid);

        //Shells creation
        IXbimShell CreateShell(IfcOpenShell shell);
        IXbimShell CreateShell(IfcConnectedFaceSet shell);
        IXbimShell CreateShell(IfcSurfaceOfLinearExtrusion linExt);


        //Surface Models containing one or more faces, shells or solids
        IXbimGeometryObjectSet CreateSurfaceModel(IfcShellBasedSurfaceModel ifcSurface);
        IXbimGeometryObjectSet CreateSurfaceModel(IfcFaceBasedSurfaceModel ifcSurface);

        //Faces

        IXbimFace CreateFace(IfcProfileDef profileDef);
        IXbimFace CreateFace(IfcCompositeCurve cCurve);
        IXbimFace CreateFace(IfcPolyline pline);
        IXbimFace CreateFace(IfcPolyLoop loop);
        IXbimFace CreateFace(IfcSurface surface);
        IXbimFace CreateFace(IfcPlane plane);
        IXbimFace CreateFace(IXbimWire wire);

        //Create Wire
        IXbimWire CreateWire(IfcCurve curve);
        IXbimWire CreateWire(IfcCompositeCurveSegment compCurveSeg);

        IXbimPoint CreatePoint(double x, double y, double z, double tolerance);
        IXbimPoint CreatePoint(IfcCartesianPoint p);
        IXbimPoint CreatePoint(XbimPoint3D p, double tolerance);
        IXbimPoint CreatePoint(IfcPoint pt);
        IXbimPoint CreatePoint(IfcPointOnCurve p);
        IXbimPoint CreatePoint(IfcPointOnSurface p);

        //Vertex Creation
        IXbimVertex CreateVertexPoint(XbimPoint3D point, double precision);

        IfcFacetedBrep CreateFacetedBrep(Xbim.Ifc2x3.IO.XbimModel model, IXbimSolid solid);
        //Creates collections of objects
        IXbimSolidSet CreateSolidSet();
        IXbimSolidSet CreateSolidSet(IfcBooleanResult boolOp);

        //Read and write functions
        void WriteTriangulation(TextWriter tw, IXbimGeometryObject shape, double tolerance, double deflection, double angle);
        void WriteTriangulation(BinaryWriter bw, IXbimGeometryObject shape, double tolerance, double deflection, double angle);
    }
}

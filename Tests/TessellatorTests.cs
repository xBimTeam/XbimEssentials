using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xbim.Common.Geometry;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Tessellator;

namespace Xbim.IO.Tests
{
    [TestClass]
    public class TessellatorTests
    {
        [TestMethod]
        public void Test_Large_Coordinates_Reduction()
        {
            XbimGeometryType tp = Xbim.Common.Geometry.XbimGeometryType.PolyhedronBinary;
            using (var model = IfcStore.Open("TestFiles\\LargeTriangulatedCoordinates.ifc"))
            {
                var xbimTessellator = new XbimTessellator(model, tp);
                var representation = model.Instances.FirstOrDefault<IIfcFacetedBrep>();
                var shape = xbimTessellator.Mesh(representation);

                // geometry should have a local displacement
                Assert.IsTrue(shape.LocalShapeDisplacement.HasValue);

                // it should be more than 6 200 000
                var distance = shape.LocalShapeDisplacement.Value.Length;
                Assert.IsTrue(distance > 6200000);

                var ms = new MemoryStream(((IXbimShapeGeometryData)shape).ShapeData);
                var br = new BinaryReader(ms);
                var geometry = br.ReadShapeTriangulation();

                // vertex geometry should be small
                var vertex = geometry.Vertices.First();
                Assert.IsTrue(vertex.X < 1000);
                Assert.IsTrue(vertex.Y < 1000);
                Assert.IsTrue(vertex.Z < 1000);

                // bounding box should be at [0,0,0] position
                var bb = shape.BoundingBox;
                var pos = bb.Location;
                var test = Math.Abs(pos.X + pos.Y + pos.Z);
                Assert.IsTrue(test < 0.1)
;            }
        }

        [TestMethod]
        public void Test_PolygonalFaceSet_Tessellation()
        {
            XbimGeometryType tp = Xbim.Common.Geometry.XbimGeometryType.PolyhedronBinary;
            using (var model = IfcStore.Open("TestFiles\\polygonal-face-tessellation.ifc"))
            {
                var xbimTessellator = new XbimTessellator(model, tp);
                XbimShapeGeometry shapeGeom = null;

                var shape = model.Instances.FirstOrDefault<IIfcPolygonalFaceSet>();
                shapeGeom = xbimTessellator.Mesh(shape);
                Assert.AreEqual(8000000000000, shapeGeom.BoundingBox.Volume);
            }
        }

        [TestMethod]
        public void TestBoundingBoxSize()
        {
            XbimGeometryType tp = Xbim.Common.Geometry.XbimGeometryType.PolyhedronBinary;
            using (var model = IfcStore.Open("TestFiles\\Roof-01_BCAD.ifc"))
            {
                var xbimTessellator = new XbimTessellator(model, tp);
                XbimShapeGeometry shapeGeom = null;
                
                var shape = model.Instances[1192] as IIfcGeometricRepresentationItem;
                shapeGeom = xbimTessellator.Mesh(shape);
                Debug.WriteLine(shapeGeom.BoundingBox);
            }
        }

        [TestMethod]
        public void TestPnSize_Add2_Support()
        {
            XbimGeometryType tp = Xbim.Common.Geometry.XbimGeometryType.PolyhedronBinary;
            using (var model = IfcStore.Open("TestFiles\\IfcTriangulatedFaceSet.ifc"))
            {
                var xbimTessellator = new XbimTessellator(model, tp);
                XbimShapeGeometry shapeGeom = null;

                var shape = model.Instances[48] as IIfcGeometricRepresentationItem;
                shapeGeom = xbimTessellator.Mesh(shape);
                Debug.WriteLine(shapeGeom.BoundingBox);
            }
        }
    }
}

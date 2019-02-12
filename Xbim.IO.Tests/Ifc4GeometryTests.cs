using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Tessellator;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using System.IO;
using Xbim.Common.XbimExtensions;
using System;
using System.Collections.Generic;

namespace  Xbim.MemoryModel.Tests
{
    
    [TestClass]
    public class Ifc4GeometryTests
    {
        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void IfcTriangulatedFaceSetTest()
        {
            using (var store = IfcStore.Open("BasinTessellation.ifc"))
            {
                var basinTess = store.Instances[501] as IIfcTriangulatedFaceSet;
                
                var tessellator = new XbimTessellator(store, XbimGeometryType.PolyhedronBinary);
                Assert.IsNotNull(basinTess);
                Assert.IsTrue(tessellator.CanMesh(basinTess));
                var geom = tessellator.Mesh(basinTess);
                int calculatedVolume = Convert.ToInt32(geom.BoundingBox.Volume);
                Assert.AreEqual(23913892, calculatedVolume);
            }
        }


        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void IfcTriangulatedFaceSetWithNormalsTest()
        {
            using (var store = IfcStore.Open("column-straight-rectangle-tessellation.ifc"))
            {
                var columnTess = store.Instances[288] as IIfcTriangulatedFaceSet;
               
                var tessellator = new XbimTessellator(store, XbimGeometryType.PolyhedronBinary);
                Assert.IsNotNull(columnTess);
                Assert.IsTrue(tessellator.CanMesh(columnTess));
                var geom = tessellator.Mesh(columnTess);
                Assert.IsTrue((int)(geom.BoundingBox.Volume) == 7680);
                
            }
        }
        
        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void IfcTriangulatedFaceSetWithColoursTest()
        {
            using (var store = IfcStore.Open("tessellation-with-individual-colors.ifc"))
            {
                var triangulatedFaceSet = store.Instances.OfType<IfcTriangulatedFaceSet>().FirstOrDefault();
               
                var tessellator = new XbimTessellator(store, XbimGeometryType.PolyhedronBinary);
                Assert.IsNotNull(triangulatedFaceSet);
                Assert.IsTrue(tessellator.CanMesh(triangulatedFaceSet));
                var geom = tessellator.Mesh(triangulatedFaceSet);
                Assert.IsTrue((int)(geom.BoundingBox.Volume) == 2000000000);
                
            }
        }


        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void IfcTriangulatedFaceSetSimpleBinaryTest()
        {
            using (var store = IfcStore.Open("BasinTessellation.ifc"))
            {
                var triangulatedFaceSet = store.Instances.OfType<IfcTriangulatedFaceSet>().FirstOrDefault();

                var tessellator = new XbimTessellator(store, XbimGeometryType.PolyhedronBinary);
                Assert.IsNotNull(triangulatedFaceSet);
                Assert.IsTrue(tessellator.CanMesh(triangulatedFaceSet));
                var geom = tessellator.Mesh(triangulatedFaceSet);
                using (var ms = new MemoryStream(((IXbimShapeGeometryData)geom).ShapeData))
                {
                    using (var br = new BinaryReader(ms))
                    {
                        XbimShapeTriangulation myShapeTriangulation = br.ReadShapeTriangulation();
                        Assert.IsTrue(myShapeTriangulation.Faces.Sum(t => t.TriangleCount) == triangulatedFaceSet.NumberOfTriangles);
                    }
                }
            }
        }

        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void OneTesselatedSquareGeometryIsOk()
        {
            using (var store = IfcStore.Open("OneTesselatedSquare.ifc"))
            {
                var triangulatedFaceSet = store.Instances.OfType<IIfcConnectedFaceSet>().FirstOrDefault();

                var tessellator = new XbimTessellator(store, XbimGeometryType.PolyhedronBinary);
                Assert.IsNotNull(triangulatedFaceSet);
                Assert.IsTrue(tessellator.CanMesh(triangulatedFaceSet));
                var geom = tessellator.Mesh(triangulatedFaceSet);
                using (var ms = new MemoryStream(((IXbimShapeGeometryData)geom).ShapeData))
                {
                    using (var br = new BinaryReader(ms))
                    {
                        XbimShapeTriangulation myShapeTriangulation = br.ReadShapeTriangulation();
                        Assert.AreEqual(2, myShapeTriangulation.Faces.Sum(t => t.TriangleCount));
                    }
                }
            }
        }

        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void IfcPolygonalFaceSetGeometryIsOk()
        {
            using (var store = IfcStore.Open("IfcPolygonalFaceSet.ifc"))
            {
                var geometries = store.Instances.OfType<IIfcPolygonalFaceSet>();
                Dictionary<int, int> expectedvalues = new Dictionary<int, int>();
                expectedvalues.Add(144, 4);
                expectedvalues.Add(148, 2);
                var tested = 0;
                foreach (var IfcPolygonalFaceSet in geometries)
                {
                    if (!expectedvalues.Keys.Contains(IfcPolygonalFaceSet.EntityLabel))
                        continue;
                    var returned = GetTriangululatedCount(store, IfcPolygonalFaceSet); 
                    Assert.AreEqual(expectedvalues[IfcPolygonalFaceSet.EntityLabel], returned);
                    tested++;
                }
                Assert.AreEqual(2, tested);
            }
        }

        private static int GetTriangululatedCount(IfcStore store, IIfcPolygonalFaceSet IfcPolygonalFaceSet)
        {
            var tessellator = new XbimTessellator(store, XbimGeometryType.PolyhedronBinary);
            Assert.IsNotNull(IfcPolygonalFaceSet);
            Assert.IsTrue(tessellator.CanMesh(IfcPolygonalFaceSet));
            var geom = tessellator.Mesh(IfcPolygonalFaceSet);
            using (var ms = new MemoryStream(((IXbimShapeGeometryData)geom).ShapeData))
            using (var br = new BinaryReader(ms))
            {
                XbimShapeTriangulation myShapeTriangulation = br.ReadShapeTriangulation();
                return myShapeTriangulation.Faces.Sum(t => t.TriangleCount);
            }
        }

        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void IfcTriangulatedFaceSetComplexBinaryTest()
        {
            using (var store = IfcStore.Open("IFC4TessellationComplex.ifc"))
            {
                var triangulatedFaceSet = store.Instances[4373] as IIfcTriangulatedFaceSet;

                var tessellator = new XbimTessellator(store, XbimGeometryType.PolyhedronBinary);
                Assert.IsNotNull(triangulatedFaceSet);
                Assert.IsTrue(tessellator.CanMesh(triangulatedFaceSet));
                var geom = tessellator.Mesh(triangulatedFaceSet);
                using (var ms = new MemoryStream(((IXbimShapeGeometryData)geom).ShapeData))
                {
                    using (var br = new BinaryReader(ms))
                    {
                        XbimShapeTriangulation myShapeTriangulation = br.ReadShapeTriangulation();
                        Assert.IsTrue(myShapeTriangulation.Faces.Sum(t => t.TriangleCount) == triangulatedFaceSet.NumberOfTriangles);
                    }
                }
            }
        }
    }
}

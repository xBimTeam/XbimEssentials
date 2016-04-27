﻿using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Tessellator;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Interfaces;

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
                Assert.IsTrue((int)(geom.BoundingBox.Volume) == 23913892);
                
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
    }
}

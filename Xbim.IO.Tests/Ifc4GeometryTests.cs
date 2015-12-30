using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Tessellator;
using Xbim.Common.Geometry;
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
                var geomStore = store.GeometryStore;
                var tessellator = new XbimTessellator(store, XbimGeometryType.PolyhedronBinary);
                Assert.IsNotNull(basinTess);
                Assert.IsTrue(tessellator.CanMesh(basinTess));
                var geom = tessellator.Mesh(basinTess);
                Assert.IsTrue(geom.BoundingBox.Volume > 23913892);
                Assert.IsTrue(geom.BoundingBox.Volume < 23913893);
            }
        }
    }
}

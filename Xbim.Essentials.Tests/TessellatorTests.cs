using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Tessellator;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class TessellatorTests
    {
        private object debug;

        [TestMethod]
        [DeploymentItem("TestSourceFiles\\HighDisplacement.ifc")]
        public void TessallatorDisplacement()
        {
            using (IfcStore s = IfcStore.Open("HighDisplacement.ifc"))
            {
                var xbimTessellator = new XbimTessellator(s, Common.Geometry.XbimGeometryType.PolyhedronBinary);
                xbimTessellator.MoveMinToOrigin = true;
                foreach (var ent in s.Instances.OfType<IIfcGeometricRepresentationItem>().Where(x=>xbimTessellator.CanMesh(x)))
                {
                    var meshed = xbimTessellator.Mesh(ent);
                }
            }
        }

        [TestMethod]
        [DeploymentItem("TestSourceFiles\\PositionWithMappedItems.ifc")]
        public void TessallatorBoundingBox()
        {
            using (IfcStore s = IfcStore.Open("PositionWithMappedItems.ifc"))
            {
                var xbimTessellator = new XbimTessellator(s, Common.Geometry.XbimGeometryType.PolyhedronBinary);
                xbimTessellator.MoveMinToOrigin = true;               
                foreach (var ent in s.Instances.OfType<IIfcGeometricRepresentationItem>().Where(x => xbimTessellator.CanMesh(x)))
                {
                    var meshed = xbimTessellator.Mesh(ent);
                    Debug.WriteLine(meshed.BoundingBox);
                }
            }
        }

        [TestMethod]
        [DeploymentItem("TestSourceFiles\\IFCPOLYGONALFACESET_Subtraction2.ifc")]
        public void TestIIfcPolygonalFaceSetProfile()
        {
            using (IfcStore s = IfcStore.Open("IFCPOLYGONALFACESET_Subtraction2.ifc"))
            {
                IIfcPolygonalFaceSet ent = s.Instances[1655739] as IIfcPolygonalFaceSet;
                var shape = XbimTessellator.GetContourPoints(ent);
                if (false)
                {
                    int iFace = 0;
                    foreach (var face in shape)
                    {
                        Debug.WriteLine($"Face: {iFace++}");
                        int iProfile = 0;
                        foreach (var profile in face)
                        {
                            Debug.WriteLine($"  Profile: {iProfile++}");
                            foreach (var point in profile)
                            {
                                Debug.WriteLine($"    {point.X}, {point.Y}, {point.Z}");
                            }
                        }
                    }
                }
                Assert.AreEqual(9, shape[0].Count, "There should be 9 profiles in face 0");
                Assert.AreEqual(9, shape[37].Count, "There should be 9 profiles in face 37");
                for (int i = 1; i < 37; i++)
                {
                    Assert.AreEqual(1, shape[i].Count, "There should be one profile in other faces");
                }
            }
        }
    }
}

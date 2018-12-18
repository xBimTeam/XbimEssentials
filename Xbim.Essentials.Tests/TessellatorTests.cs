using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
    }
}

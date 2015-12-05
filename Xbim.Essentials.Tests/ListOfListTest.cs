using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;

namespace Xbim.Essentials.Tests
{

    [DeploymentItem(@"TestSourceFiles\")]
    [TestClass]
    public class ListOfListTest
    {
        [TestMethod]
        public void ReadListOfListFromIfcFile()
        {
            using (var model = IfcStore.Open(@"IfcBSplineSurfaceWithKnots.ifc"))
            {
                var surface = model.Instances.OfType<IfcBSplineSurfaceWithKnots>().FirstOrDefault();
                Assert.IsTrue(surface.ControlPointsList.Count==4);
                foreach (var cpList in surface.ControlPointsList)
                {
                    Assert.IsTrue(cpList.Count == 7);
                }
                Assert.IsTrue(surface.ControlPoints.Count == 4);
                foreach (var cpList in surface.ControlPoints)
                {
                    Assert.IsTrue(cpList.Count == 7);
                }
            }
        }
    }
}

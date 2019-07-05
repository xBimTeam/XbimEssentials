using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc4.GeometryResource;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{

    [DeploymentItem(@"TestSourceFiles\")]
    [TestClass]
    public class ListOfListTest
    {
        [TestMethod]
        public void ReadListOfListFromIfcFile()
        {
            using (var model = MemoryModel.OpenRead(@"IfcBSplineSurfaceWithKnots.ifc"))
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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc4;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Esent;
using Xbim.IO.Memory;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    [DeploymentItem("TestFiles")]
    public class ReadIfc4InterfacesTest
    {
        [TestMethod]
        public void LoadIfc4IntoMemoryTest()
        {
            var model = new MemoryModel<EntityFactory>();
            model.Open("SampleHouse4.ifc");

            var project = model.Instances.FirstOrDefault<IIfcProject>();
            Assert.IsNotNull(project);
            Assert.IsNotNull(project.Name);

            var walls = model.Instances.OfType<IIfcWall>();
            var doors = model.Instances.OfType<IIfcDoor>();
            Assert.IsTrue(walls.Any());
            Assert.IsTrue(doors.Any());
        }

        [TestMethod]
        public void LoadIfc4IntoDbTest()
        {
            var model = new EsentModel(new EntityFactory());
            model.CreateFrom("SampleHouse4.ifc", null, null, true, true);

            var project = model.Instances.FirstOrDefault<IIfcProject>();
            Assert.IsNotNull(project);
            Assert.IsNotNull(project.Name);

            var walls = model.Instances.OfType<IIfcWall>();
            var doors = model.Instances.OfType<IIfcDoor>();
            Assert.IsTrue(walls.Any());
            Assert.IsTrue(doors.Any());
        }
    }
}

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Ifc4;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Esent;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem("TestFiles")]
    public class ReadIfc4InterfacesTest
    {
        private static readonly IEntityFactory ef4 = new EntityFactoryIfc4();

        [TestMethod]
        public void LoadIfc4IntoMemoryTest()
        {
            var model = new IO.Memory.MemoryModel(ef4);
            model.LoadStep21("SampleHouse4.ifc");

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
            using (var model = new IO.Esent.EsentModel(ef4))
            {
                model.CreateFrom("SampleHouse4.ifc", null, null, true, true);
                var project = model.Instances.FirstOrDefault<IIfcProject>();
                Assert.IsNotNull(project);
                Assert.IsNotNull(project.Name);

                var walls = model.Instances.OfType<IIfcWall>();
                var doors = model.Instances.OfType<IIfcDoor>();

                var pset = model.Instances.FirstOrDefault<IIfcPropertySet>();
                var singleProperties = pset.HasProperties.OfType<IIfcPropertySingleValue>();

                Assert.IsNotNull(singleProperties);
                Assert.IsTrue(walls.Any());
                Assert.IsTrue(doors.Any());
            }
            
        }


    }
}

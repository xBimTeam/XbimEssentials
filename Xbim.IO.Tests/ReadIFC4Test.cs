using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc4;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.IO.Memory;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    [DeploymentItem("TestFiles")]
    public class ReadIfc4Test
    {
        [TestMethod]
        public void LoadIfc4Test()
        {
            var model = new MemoryModel<EntityFactory>();
            model.Open("SampleHouse4.ifc");

            var project = model.Instances.FirstOrDefault<IfcProject>();
            Assert.IsNotNull(project);
            Assert.IsNotNull(project.Name);

            var walls = model.Instances.OfType<IfcWall>();
            var doors = model.Instances.OfType<IfcDoor>();
            Assert.IsTrue(walls.Any());
            Assert.IsTrue(doors.Any());
        }

        [TestMethod]
        public void ReadingOfNestedLists()
        {
            var model = new MemoryModel<EntityFactory>();
            model.Open("IfcCartesianPointList3D.ifc");
            var pl = model.Instances.FirstOrDefault<IfcCartesianPointList3D>();
            Assert.IsNotNull(pl);
            Assert.AreEqual(3, pl.CoordList.Count);
            Assert.AreEqual(9, pl.CoordList.SelectMany(c => c).Count());

            //write new file
            model.Save("..\\..\\SerializedNestedList.ifc");
        }

        [TestMethod]
        public void CreationOfNestedListsFromAPI()
        {
            var model = new MemoryModel<EntityFactory>();
            using (var txn = model.BeginTransaction("Test"))
            {
                var pl = model.Instances.New<IfcCartesianPointList3D>();
                var nested = pl.CoordList.GetAt(0);
                Assert.IsNotNull(nested);
                txn.RollBack();
            }
            
        }
    }
}

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.SharedBldgElements;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class ActivationTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles\\4walls1floorSite.ifc")]
        public void ActivationTest()
        {
            var model = new IO.Esent.EsentModel(new EntityFactoryIfc2x3());
            model.CreateFrom("4walls1floorSite.ifc", null, null, true);
            TestActivation(model);
        }

        private void TestActivation(IModel model)
        {
            var wall = model.Instances.FirstOrDefault<IfcWall>();
            var instance = (IPersistEntity) wall;
            Assert.IsFalse(instance.Activated);

            var name = wall.Name;
            Assert.IsTrue(instance.Activated);

            var rel = model.Instances.FirstOrDefault<IfcRelDefinesByProperties>();
            instance = rel;
            Assert.IsFalse(instance.Activated);

            var relObj = rel.RelatedObjects;
            Assert.IsTrue(instance.Activated);

            var tRel = model.Instances.FirstOrDefault<IfcRelDefinesByType>();
            instance = tRel;
            Assert.IsFalse(instance.Activated);

            var numObj = tRel.RelatedObjects.Count;
            Assert.IsTrue(instance.Activated);

            //activation for write shouldn't load the data again
            Assert.AreEqual(numObj, tRel.RelatedObjects.Count);
        }
    }
}

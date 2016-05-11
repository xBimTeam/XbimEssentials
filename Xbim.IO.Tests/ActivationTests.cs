﻿using System;
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
            var model = new IO.Esent.EsentModel(new EntityFactory());
            model.CreateFrom("4walls1floorSite.ifc", null, null, true);
            TestActivation(model);
        }

        private void TestActivation(IModel model)
        {
            var wall = model.Instances.FirstOrDefault<IfcWall>();
            var instance = (IPersistEntity) wall;
            Assert.IsTrue(instance.ActivationStatus == ActivationStatus.NotActivated);

            var name = wall.Name;
            Assert.IsTrue(instance.ActivationStatus == ActivationStatus.ActivatedRead);

            using (model.BeginTransaction("Test"))
            {
                wall.Name = "New Name";
                Assert.IsTrue(instance.ActivationStatus == ActivationStatus.ActivatedReadWrite);
            }

            var rel = model.Instances.FirstOrDefault<IfcRelDefinesByProperties>();
            instance = rel;
            Assert.IsTrue(instance.ActivationStatus == ActivationStatus.NotActivated);

            var relObj = rel.RelatedObjects;
            Assert.IsTrue(instance.ActivationStatus == ActivationStatus.ActivatedRead);

            using (model.BeginTransaction("Test"))
            {
                relObj.Clear();
                Assert.IsTrue(instance.ActivationStatus == ActivationStatus.ActivatedReadWrite);
            }

            var tRel = model.Instances.FirstOrDefault<IfcRelDefinesByType>();
            instance = tRel;
            Assert.IsTrue(instance.ActivationStatus == ActivationStatus.NotActivated);

            var numObj = tRel.RelatedObjects.Count;
            Assert.IsTrue(instance.ActivationStatus == ActivationStatus.ActivatedRead);

            using (model.BeginTransaction("Test"))
            {
                tRel.Name = "New name";
                Assert.IsTrue(instance.ActivationStatus == ActivationStatus.ActivatedReadWrite);
            }

            //activation for write shouldn't load the data again
            Assert.AreEqual(numObj, tRel.RelatedObjects.Count);
        }
    }
}

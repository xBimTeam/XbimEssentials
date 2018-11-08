using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO.Memory;
using Xbim.Ifc;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class EqualityTests
    {

        private static readonly IEntityFactory f = new EntityFactoryIfc2x3();

        [TestMethod]
        public void EqualityTest()
        {
            var model = new MemoryModel(f);
            var model2 = new MemoryModel(f);

            //use entity factory to create objects outside of transaction or anything

            var wall1 = f.New<IfcWall>(model, 1, true);
            var wall2 = f.New<IfcWall>(model, 1, true);
            var wall3 = f.New<IfcWall>(model2, 1, true);
            var wall4 = f.New<IfcWall>(model2, 2, true);
            IfcWall wall0 = null;

            Assert.IsTrue(wall0 == null);
            Assert.IsTrue(null == wall0);

            //different objects with the same label and the same model should be treated as being the same
            Assert.IsFalse(ReferenceEquals(wall1, wall2));
            Assert.IsTrue(wall1 == wall2);
            Assert.IsTrue(wall2 == wall1);
            Assert.IsTrue(wall1.Equals(wall2));
            Assert.IsTrue(((IEquatable<IfcWall>)wall1).Equals(wall2));
            Assert.AreEqual(wall2, wall1);

            //different model
            Assert.IsFalse(wall2 == wall3);
            Assert.IsFalse(((IEquatable<IfcWall>)wall2).Equals(wall3));

            //different label
            Assert.IsFalse(wall3 == wall4);
            Assert.IsFalse(wall3.Equals(wall4));

            //second wall should be excluded from distinct list
            var walls = new List<IfcWall>{wall1, wall2, wall3, wall4};
            var distinct = walls.Distinct().ToList();
            Assert.AreEqual(3, distinct.Count);
            //but it "contains" the object as far as the list is concerned
            Assert.IsTrue(distinct.Contains(wall1));
            Assert.IsTrue(distinct.Contains(wall2));

            //it should also work on interface list
            var entities = new List<IPersistEntity> { wall1, wall2, wall3, wall4 };
            var distinctEntities = entities.Distinct().ToList();
            Assert.AreEqual(3, distinctEntities.Count);
            //but it "contains" the object as far as the list is concerned
            Assert.IsTrue(distinctEntities.Contains(wall1));
            Assert.IsTrue(distinctEntities.Contains(wall2));
        }


        /// <summary>
        /// This method tests the opening and retrival of an esent model from a dictionary
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"TestSourceFiles\P1.xbim")]
        public void StoreEqualityTest()
        {
            Dictionary<IModel, int> d = new Dictionary<IModel, int>();
            var model = IfcStore.Open(@"P1.xbim");
            d.Add(model, 1);

            int restored;
            var restoredOk = d.TryGetValue(model, out restored);

            Assert.IsTrue(restoredOk, "The store could not be found in the dictionary");
            Assert.AreEqual(1, restored);
        }

        /// <summary>
        /// This method tests the opening and retrival of an XbimInstanceHandle from a dictionary
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"TestSourceFiles\P1.xbim")]
        public void XbimInstanceHandleSupportsDictionary()
        {
            Dictionary<XbimInstanceHandle, int> d = new Dictionary<XbimInstanceHandle, int>();
            using (var model = IfcStore.Open(@"P1.xbim"))
            {
                var ent = model.Instances.FirstOrDefault();

                var handlefromPointers = new XbimInstanceHandle(model, ent.EntityLabel, ent.ExpressType.TypeId);
                d.Add(handlefromPointers, 1);

                int restored;
                var handlefromEntity = new XbimInstanceHandle(ent);
                var restoredOk = d.TryGetValue(handlefromEntity, out restored);

                Assert.IsTrue(restoredOk, "The XbimInstanceHandle could not be found in the dictionary");
                Assert.AreEqual(1, restored);
            }
        }

        /// <summary>
        /// Similarly to StoreEqualityTest above 
        /// this method tests the opening and retrival of an memory model from a dictionary
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"TestSourceFiles\email.ifc")]
        public void IfcStoreEqualityTest()
        {
            Dictionary<IModel, int> d = new Dictionary<IModel, int>();
            var model = IfcStore.Open(@"email.ifc");
            d.Add(model, 1);

            var hasInstances = model.Instances.Any();
            Assert.IsTrue(hasInstances);

            int restored;
            var restoredOk = d.TryGetValue(model, out restored);

            Assert.IsTrue(restoredOk, "The store could not be found in the dictionary");
            Assert.AreEqual(1, restored);
        }
    }
}

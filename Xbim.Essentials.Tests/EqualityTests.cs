using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class EqualityTests
    {
        [TestMethod]
        public void EqualityTest()
        {
            var model = new MemoryModel<EntityFactory>();
            var model2 = new MemoryModel<EntityFactory>();

            //use entity factory to create objects outside of transaction or anything
            var f = new EntityFactory();

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


        }
    }
}

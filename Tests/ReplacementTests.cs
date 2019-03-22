using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Model;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.SharedBldgElements;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class ReplacementTests
    {
        [TestMethod]
        public void ReferenceDeletion()
        {
            using (var model = new StepModel(new Xbim.Ifc4.EntityFactoryIfc4()))
            {
                IfcWall wall;
                IfcPropertySet pset;
                IfcRelDefinesByProperties rel;
                var i = model.Instances;

                using (var txn = model.BeginTransaction("Init"))
                {
                    wall = i.New<IfcWall>();
                    pset = i.New<IfcPropertySet>();
                    rel = i.New<IfcRelDefinesByProperties>(r => {
                        r.RelatedObjects.Add(wall);
                        r.RelatingPropertyDefinition = pset;
                    });
                    txn.Commit();
                }

                Assert.AreEqual(3, i.Count);
                using (var txn = model.BeginTransaction("Delete"))
                {
                    model.Delete(wall);
                    model.Delete(pset);
                    txn.Commit();
                }
                Assert.AreEqual(1, i.Count);
                Assert.IsFalse(rel.RelatedObjects.Any());
                Assert.IsNull(rel.RelatingPropertyDefinition);
            }
        }

        [TestMethod]
        public void ReferenceReplacement()
        {
            using (var model = new StepModel(new Xbim.Ifc4.EntityFactoryIfc4()))
            {
                IfcWall wall;
                IfcPropertySet pset;
                IfcRelDefinesByProperties rel;
                var i = model.Instances;

                using (var txn = model.BeginTransaction("Init"))
                {
                    wall = i.New<IfcWall>();
                    pset = i.New<IfcPropertySet>();
                    rel = i.New<IfcRelDefinesByProperties>(r => {
                        r.RelatedObjects.Add(wall);
                        r.RelatingPropertyDefinition = pset;
                    });
                    txn.Commit();
                }
                Assert.AreEqual(3, i.Count);

                IfcDoor door;
                IfcElementQuantity quant;
                using (var txn = model.BeginTransaction("Replace"))
                {
                    door = model.Replace<IfcWall, IfcDoor>(new[] { wall }).Values.FirstOrDefault();
                    quant = model.Replace<IfcPropertySet, IfcElementQuantity>(new[] { pset }).Values.FirstOrDefault();
                    txn.Commit();
                }
                Assert.AreEqual(3, i.Count);
                Assert.IsNotNull(door);
                Assert.IsNotNull(quant);
                Assert.AreEqual(1, rel.RelatedObjects.Count);
                Assert.IsTrue(rel.RelatedObjects.Contains(door));
                Assert.IsTrue(rel.RelatingPropertyDefinition == quant);
                Assert.IsNull(wall.Model);
                Assert.IsNull(pset.Model);
            }
        }
    }
}

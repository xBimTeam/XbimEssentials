using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Delta;
using Xbim.Ifc;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Step21;
using IfcWallTypeEnum = Xbim.Ifc4.Interfaces.IfcWallTypeEnum;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class TransactionLogTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles\\4walls1floorSite.ifc")]
        public void LogTest()
        {
            using (var model = new IO.Memory.MemoryModel(new EntityFactoryIfc2x3()))
            {
                model.LoadStep21("4walls1floorSite.ifc");
                using (var txn = model.BeginTransaction("Log test"))
                {
                    using (var log = new TransactionLog(txn))
                    {
                        var wall = model.Instances.FirstOrDefault<IIfcWall>();
                        var name = wall.Name.ToString();
                        Assert.IsNotNull(wall);
                        Assert.IsFalse(log.Changes.Any());

                        wall.Name = "New name";
                        var changes = log.Changes.ToList();
                        Assert.IsTrue(changes.Count == 1);
                        var change = changes.FirstOrDefault();
                        Assert.IsNotNull(change);
                        Assert.IsTrue(change.ChangeType == ChangeType.Modified);
                        Assert.IsTrue(change.CurrentEntity != change.OriginalEntity);
                        var propChanges = change.ChangedProperties.ToList();
                        Assert.IsTrue(propChanges.Count == 1);
                        var pChange = propChanges.FirstOrDefault();
                        Assert.IsNotNull(pChange);
                        Assert.IsTrue(pChange.Name == "Name");
                        Assert.IsTrue(pChange.CurrentValue == "'New name'");
                        Assert.IsTrue(pChange.OriginalValue == "'" + name + "'");

                        model.Delete(wall);
                        Assert.IsTrue(change.ChangeType == ChangeType.Deleted);
                        Assert.IsTrue(!change.ChangedProperties.Any());
                        Assert.IsTrue(change.CurrentEntity == "");

                        wall = model.Instances.New<IfcWall>();
                        change = log.Changes.FirstOrDefault(c => c.Entity.Equals(wall));
                        Assert.IsNotNull(change);

                        wall.Name = "Some name";
                        wall.PredefinedType = IfcWallTypeEnum.STANDARD;
                        wall.GlobalId = Guid.NewGuid().ToPart21();
                        Assert.IsTrue(change.ChangeType == ChangeType.New);
                        Assert.IsTrue(change.OriginalEntity == "");
                    }
                }
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles\\4walls1floorSite.ifc")]
        public void LogCreation()
        {
            using (var model = IfcStore.Open("4walls1floorSite.ifc"))
            {
                using (var txn = model.BeginTransaction("Modification"))
                {
                    Console.WriteLine(@"Changing existing entity:");
                    Console.WriteLine(@"=========================");
                    using (var log = new TransactionLog(txn))
                    {
                        //change to existing wall
                        var wall = model.Instances.FirstOrDefault<IfcWall>();
                        wall.Name = "Unexpected name";
                        wall.GlobalId = Guid.NewGuid().ToPart21();
                        wall.Description = "New and more descriptive description";

                        //print all changes caused by this
                        PrintChanges(log);
                        txn.Commit();
                    }
                    Console.WriteLine();
                }

                using (var txn = model.BeginTransaction("New"))
                {
                    Console.WriteLine(@"Creating new entity:");
                    Console.WriteLine(@"====================");
                    using (var log = new TransactionLog(txn))
                    {
                        model.Instances.New<IfcWall>(w =>
                        {
                            w.Name = "Beautiful wall";
                            w.Description = "One of the best walls in the world";
                            w.GlobalId = Guid.NewGuid().ToPart21();
                        });

                        //print all changes caused by this
                        PrintChanges(log);
                        txn.Commit();
                    }
                    Console.WriteLine();
                }

                using (var txn = model.BeginTransaction("Delete"))
                {
                    Console.WriteLine(@"Deleting existing entity:");
                    Console.WriteLine(@"=========================");
                    using (var log = new TransactionLog(txn))
                    {
                        var wall = model.Instances.FirstOrDefault<IfcWall>(w => w.Representation != null);
                        model.Delete(wall);

                        //print all changes caused by this
                        PrintChanges(log);
                        txn.Commit();
                    }
                    Console.WriteLine();
                }
            }
        }

        private static void PrintChanges(TransactionLog log)
        {
            foreach (var change in log.Changes)
            {
                Console.WriteLine(@"Entity: #{0}={1}, Change: {2}", change.Entity.EntityLabel, change.Entity.GetType().Name, change.ChangeType);
                Console.WriteLine(@"    Original entity: {0}", change.OriginalEntity);
                Console.WriteLine(@"    Current entity: {0}", change.CurrentEntity);
                if (change.ChangeType != ChangeType.Modified) continue;

                foreach (var prop in change.ChangedProperties)
                    Console.WriteLine(@"        Property '{0}' changed from {1} to {2}", prop.Name, prop.OriginalValue, prop.CurrentValue);
            }
        }
    }
}

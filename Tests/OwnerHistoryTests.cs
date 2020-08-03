using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class OwnerHistoryTests
    {
        [TestMethod]
        public void NonEmptyEditorCredentials()
        {
            using (var model = IfcStore.Create(XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel))
            {
                using (var txn = model.BeginTransaction())
                {
                    var create = new Create(model);
                    var wall = create.Wall(w => w.Name = "New wall");
                    Assert.IsNotNull(wall.OwnerHistory);
                    Assert.IsTrue(model.Instances.Count > 1);
                }
            }
        }

        [TestMethod]
        public void EmptyEditorCredentials()
        {
            using (var model = IfcStore.Create(XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel))
            {
                model.ManageOwnerHistory = false;
                using (var txn = model.BeginTransaction())
                {
                    var create = new Create(model);
                    var wall = create.Wall(w => w.Name = "New wall");
                    Assert.IsNull(wall.OwnerHistory);
                    Assert.AreEqual(1, model.Instances.Count);
                }
            }
        }

        [TestMethod]
        public void NoOwnerHistoryForInsertCopy()
        {
            using (var source = IfcStore.Create(XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel))
            {
                using (var txn = source.BeginTransaction())
                {
                    var create = new Create(source);
                    source.ManageOwnerHistory = false;
                    create.Wall(w => w.Name = "New wall #1");
                    create.Wall(w => w.Name = "New wall #2");
                    create.Wall(w => w.Name = "New wall #3");
                    source.ManageOwnerHistory = true;
                    txn.Commit();
                }

                using (var target = IfcStore.Create(source.SchemaVersion, XbimStoreType.InMemoryModel))
                {
                    using (var txn = target.BeginTransaction())
                    {
                        var map = new XbimInstanceHandleMap(source, target);
                        target.InsertCopy(source.Instances.OfType<IIfcProduct>(), true, true, map);
                        txn.Commit();
                    }

                    Assert.AreEqual(source.Instances.Count, target.Instances.Count);
                }
            }
        }
    }
}

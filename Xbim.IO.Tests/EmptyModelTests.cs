using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Common.Step21;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.CobieExpress.IO;
using Xbim.CobieExpress;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class EmptyModelTests
    {
        [TestMethod]
        public void EsentInIfcStoreTest()
        {
            using (var model = IfcStore.Create(IfcSchemaVersion.Ifc2X3, XbimStoreType.EsentDatabase))
            {
                using (var txn = model.BeginTransaction())
                {
                    var wall = model.Instances.New<IfcWall>(w => w.Name = "Wall A");
                    txn.Commit();
                }
            }
        }

        [TestMethod]
        public void EsentInCobieModelTest()
        {
            using (var model = new CobieModel(true))
            {
                using (var txn = model.BeginTransaction("Creation"))
                {
                    var wall = model.Instances.New<CobieComponent>(w => w.Name = "Wall A");
                    txn.Commit();
                }
            }
        }

        [TestMethod]
        public void MemoryToEsentCobieModelTest()
        {
            //creating as in-memory model
            using (var model = new CobieModel())
            {
                using (var txn = model.BeginTransaction("Creation"))
                {
                    var wall = model.Instances.New<CobieComponent>(w => w.Name = "Wall A");
                    txn.Commit();
                }
                //saving to Esent (will change extention to *.xbim even if you define something else)
                model.SaveAsEsent("test.xbim");
            }

            //check it worked
            using (var model = CobieModel.OpenEsent("test.xbim"))
            {
                var wall = model.Instances.FirstOrDefault<CobieComponent>();
                Assert.IsNotNull(wall);
                Assert.IsTrue(wall.Name == "Wall A");
            }

            //creating esent model in outer scope for better control
            using (var esent = IO.Esent.EsentModel.CreateModel(new EntityFactory(), "test2.xbim"))
            {
                using (var model = new CobieModel(esent))
                {
                    using (var txn = model.BeginTransaction("Creation"))
                    {
                        var wall = model.Instances.New<CobieComponent>(w => w.Name = "Wall A");
                        txn.Commit();
                    }
                }

                //we can close Esent model or do anything we need with it
                esent.Close();
            }

            using (var model = CobieModel.OpenEsent("test2.xbim"))
            {
                var wall = model.Instances.FirstOrDefault<CobieComponent>();
                Assert.IsNotNull(wall);
                Assert.IsTrue(wall.Name == "Wall A");
            }
        }
    }
}

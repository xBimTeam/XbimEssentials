using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.IO.Memory;

namespace Xbim.IfcCore.UnitTests
{
    [TestClass]
    public class ModelReadingTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles/SmallModelIfc2x3.ifc")]
        public void OpenReadIfc2x3StepFormatTest()
        {
            using (var mm = MemoryModel.OpenRead("TestFiles/SmallModelIfc2x3.ifc"))
            {
                Assert.IsTrue(mm.Instances.Count == 579);
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles/SmallModelIfc4.ifc")]
        public void OpenReadIfc4StepFormatTest()
        {
            using (var mm = MemoryModel.OpenRead("TestFiles/SmallModelIfc4.ifc"))
            {
                Assert.IsTrue(mm.Instances.Count == 52);
            }
        }
        [TestMethod]
        [DeploymentItem("TestFiles/SmallModelIfc2x3.ifczip")]
        public void OpenReadIfc2x3StepZipFormatTest()
        {
            using (var mm = MemoryModel.OpenRead("TestFiles/SmallModelIfc2x3.ifczip"))
            {
                Assert.IsTrue(mm.Instances.Count == 579);
            }
        }
        [TestMethod]
        [DeploymentItem("TestFiles/SmallModelIfc4.ifczip")]
        public void OpenReadIfc4StepZipFormatTest()
        {
            using (var mm = MemoryModel.OpenRead("TestFiles/SmallModelIfc4.ifczip"))
            {
                Assert.IsTrue(mm.Instances.Count == 52);
            }
        }
        [TestMethod]
        [DeploymentItem("TestFiles/SmallModelIfc4.ifc")]
        public void OpenReadSchemaVersionTest()
        {
            using (var mm = MemoryModel.OpenRead("TestFiles/SmallModelIfc4.ifc") )
            {
                var im = mm as IModel;
                Assert.IsTrue(im.SchemaVersion==mm.SchemaVersion);
            }
        }

        [TestMethod]
        public void CreateNew()
        {
            var ef2x3 = new Ifc2x3.EntityFactory();
            using (var mm = new MemoryModel(ef2x3))
            {
                using (var txn = mm.BeginTransaction("Simple"))
                {
                    mm.Instances.New<Ifc2x3.ActorResource.IfcPerson>();
                    txn.Commit();
                    Assert.IsTrue(mm.Instances.Count == 1);
                }
                
            }
            var ef4 = new Ifc4.EntityFactory();
            using (var mm = new MemoryModel(ef4))
            {
                using (var txn = mm.BeginTransaction("Simple"))
                {
                    mm.Instances.New<Ifc4.ActorResource.IfcPerson>();
                    txn.Commit();
                    Assert.IsTrue(mm.Instances.Count == 1);
                }

            }
        }
    }
}

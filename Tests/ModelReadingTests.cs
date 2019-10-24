using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Xbim.Common;
using Xbim.IO.Memory;

namespace Xbim.IfcCore.UnitTests
{
    [TestClass]
    public class ModelReadingTests
    {

        [TestMethod]
        public void RadianValuesOverPITest()
        {
            using (var mm = MemoryModel.OpenRead("TestFiles\\RadianValuesOverPI.ifc"))
            {
                Assert.IsTrue(mm.ModelFactors.AngleToRadiansConversionFactor == 1);
            }
        }
        [TestMethod]
        public void OpenReadModelFactorPrecisionTest()
        {
            using (var mm = MemoryModel.OpenRead("TestFiles\\Axis2PlacementError.ifc"))
            {
                Assert.IsTrue(mm.ModelFactors.Precision == 0.01);
            }
        }

        [TestMethod]
        public void OpenReadIfc2x3StepFormatTest()
        {
            using (var mm = MemoryModel.OpenRead("TestFiles\\SmallModelIfc2x3.ifc"))
            {
                Assert.IsTrue(mm.Instances.Count == 579);
            }
        }

        [TestMethod]
        public void OpenReadStepFormatTest()
        {
            using (var file = File.OpenRead("TestFiles\\SmallModelIfc2x3.ifc"))
            using (var mm = MemoryModel.OpenReadStep21(file))
            {
                Assert.IsTrue(mm.Instances.Count == 579);
            }

        }

        [TestMethod]
        public void OpenReadIfc4StepFormatTest()
        {
            using (var mm = MemoryModel.OpenRead(@"TestFiles\\SmallModelIfc4.ifc"))
            {
                Assert.IsTrue(mm.Instances.Count == 52);
            }
        }
        [TestMethod]
        public void OpenReadIfc2x3StepZipFormatTest()
        {
            using (var mm = MemoryModel.OpenRead("TestFiles\\SmallModelIfc2x3.ifczip"))
            {
                Assert.IsTrue(mm.Instances.Count == 579);
            }
        }
        [TestMethod]
        public void OpenReadIfc4StepZipFormatTest()
        {
            using (var mm = MemoryModel.OpenRead("TestFiles\\SmallModelIfc4.ifczip"))
            {
                Assert.IsTrue(mm.Instances.Count == 52);
            }
        }

        [TestMethod]
        public void OpenReadIfc4XmlZipFormatTest()
        {
            using (var mm = MemoryModel.OpenRead("TestFiles\\HelloWallXml.ifczip"))
            {
                Assert.AreEqual(163, mm.Instances.Count);
            }
        }

        [TestMethod]
        public void OpenReadIfc2x3XmlFormatTest()
        {
            using (var mm = MemoryModel.OpenRead("TestFiles\\4walls1floorSite.ifcxml"))
            {
                Assert.IsTrue(mm.Instances.Count == 579);
            }
        }

        [TestMethod]
        public void OpenReadSchemaVersionTest()
        {
            using (var mm = MemoryModel.OpenRead("TestFiles\\SmallModelIfc4.ifc") )
            {
                var im = mm as IModel;
                Assert.IsTrue(im.SchemaVersion==mm.SchemaVersion);
            }
        }
 
        [TestMethod]
        public void CreateNew()
        {
            var ef2x3 = new Ifc2x3.EntityFactoryIfc2x3();
            using (var mm = new MemoryModel(ef2x3))
            {
                using (var txn = mm.BeginTransaction("Simple"))
                {
                    mm.Instances.New<Ifc2x3.ActorResource.IfcPerson>();
                    txn.Commit();
                    Assert.IsTrue(mm.Instances.Count == 1);
                }
                
            }
            var ef4 = new Ifc4.EntityFactoryIfc4();
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

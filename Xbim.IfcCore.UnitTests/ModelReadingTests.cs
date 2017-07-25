using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}

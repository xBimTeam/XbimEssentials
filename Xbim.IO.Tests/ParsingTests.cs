using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.IO.Memory;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class ParsingTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void ReadHeaderOnlyTest()
        {
            var model = new MemoryModel<Xbim.Ifc4.EntityFactory>();
            model.Open("SampleHouse4.ifc",true);

            var project = model.Instances.FirstOrDefault<Xbim.Ifc4.Interfaces.IIfcProject>();
            Assert.IsNull(project,"There should be no project created");
            Assert.IsNotNull(model.Header.CreatingApplication);
            Assert.IsTrue(model.Header.SchemaVersion == "IFC4", "Incorrect schema version");
        }
    }
}

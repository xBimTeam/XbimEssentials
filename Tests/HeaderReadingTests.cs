using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Model;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class HeaderReadingTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles\\4walls1floorSite.ifc")]
        public void Step21HeaderTest()
        {
            using (var s = File.OpenRead("4walls1floorSite.ifc"))
            {
                var header = StepModel.LoadStep21Header(s);
                var schema = header.FileSchema.Schemas.FirstOrDefault();
                Assert.IsTrue(header.FileSchema.Schemas.Count == 1);
                Assert.AreEqual("IFC2X3", schema);
                var name = header.FileName.Name;
                Assert.AreEqual("Project Number", name);
                var mvd = header.FileDescription.Description.FirstOrDefault();
                Assert.AreEqual("ViewDefinition [CoordinationView]", mvd);
            }
        }
    }
}

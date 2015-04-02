using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class IfcHeaderTests
    {
        [TestMethod]
        public void EscapeHeaderTests()
        {
            const string path = @"x:\path1\path2\filename.ifc";
            const string umlaut = "name with umlaut ü";
            using (var model = XbimModel.CreateTemporaryModel())
            {

                model.Initialise("Creating Author", " Creating Organisation", "This Application", "This Developer", "v1.1");
                using (var txn = model.BeginTransaction())
                {
                    model.IfcProject.Name = "Project Name";
                    txn.Commit();
                }

                model.Header.FileName.Name = path;
                model.Header.FileName.Organization.Add(umlaut); 
                model.SaveAs("testOutput.ifc");
            }
            using (var model = new XbimModel())
            {
                model.CreateFrom("testOutput.ifc", null, null, true);
                Assert.IsTrue(model.Header.FileName.Name == path);
                Assert.IsTrue(model.Header.FileName.Organization.FirstOrDefault() == umlaut);
                model.Close();
            }
        }
    }
}

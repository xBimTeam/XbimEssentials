using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Geometry;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
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
            using (var model = Xbim.Ifc2x3.IO.XbimModel.CreateTemporaryModel())
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
            using (var model = new Xbim.Ifc2x3.IO.XbimModel())
            {
                model.CreateFrom("testOutput.ifc", null, null, true);
                Assert.IsTrue(model.Header.FileName.Name == path);
                Assert.IsTrue(model.Header.FileName.Organization.FirstOrDefault() == umlaut);
                model.Close();
            }
        }

        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void ReadPreProcessorTest()
        {
            string revitPattern = @"- Exporter\s(\d*.\d*.\d*.\d*)";
            string[] files = new[] { "4walls1floorSite.ifc", "SampleHouse4.ifc" };
            var surfaceOfLinearExtrusionVersion = new Version(17, 0, 416);
          
            foreach (var file in files)
            {
                using (var store = IfcStore.Open(file))
                {
                    var matches = Regex.Matches(store.Header.FileName.OriginatingSystem, revitPattern,
                        RegexOptions.IgnoreCase);
                    Assert.IsTrue(matches.Count > 0, "No match found");
                    Assert.IsTrue(matches[0].Groups.Count == 2, "Should only be two matches");
                    Version modelVersion;
                    if (Version.TryParse(matches[0].Groups[1].Value, out modelVersion))
                    {
                        var shouldHaveWorkAround = (modelVersion <= surfaceOfLinearExtrusionVersion);
                        Assert.IsTrue(!shouldHaveWorkAround || shouldHaveWorkAround &&
                                      store.ModelFactors.ApplyWorkAround("#SurfaceOfLinearExtrusion"),
                            "Work aroud not detected");

                    }
                }
            }
        }
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Geometry;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.IO;
using Xbim.IO.Memory;

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

            var testFile = "EscapeHeaderTests.ifc";

            using (var model = new MemoryModel(new Ifc2x3.EntityFactoryIfc2x3()))
            {
                model.Header.FileName.Name = path;
                model.Header.FileName.Organization.Add(umlaut);
                using (var file = File.Create(testFile))
                {
                model.SaveAsStep21(file);
                    file.Close();
                }
            }
            using (var model = MemoryModel.OpenRead(testFile))
            {
                Assert.IsTrue(model.Header.FileName.Name == path);
                Assert.IsTrue(model.Header.FileName.Organization.FirstOrDefault() == umlaut);
            }
        }

        [DeploymentItem("TestSourceFiles")]
        [TestMethod]
        public void ReadPreProcessorTest()
        {
            string revitPattern = @"- Exporter\s(\d*.\d*.\d*.\d*)";
            string[] files = new[] { @"Axis2PlacementError.ifc", "4walls1floorSite.ifc", "SampleHouse4.ifc" };
            var surfaceOfLinearExtrusionVersion = new Version(17, 0, 416, 0);
          
            foreach (var file in files)
            {
                using (var store = MemoryModel.OpenRead(file))
                {
                    var matches = Regex.Matches(store.Header.FileName.OriginatingSystem, revitPattern,
                        RegexOptions.IgnoreCase);
                    Assert.IsTrue(matches.Count > 0, "No match found");
                    Assert.IsTrue(matches[0].Groups.Count == 2, "Should only be two matches");
                    if (Version.TryParse(matches[0].Groups[1].Value, out Version modelVersion))
                    {
                        var shouldHaveWorkAround = (modelVersion <= surfaceOfLinearExtrusionVersion);

                        if (shouldHaveWorkAround)
                            Assert.IsTrue(store.ModelFactors.ApplyWorkAround("#SurfaceOfLinearExtrusion"), "Work around should be implemented");
                        else
                            Assert.IsFalse(store.ModelFactors.ApplyWorkAround("#SurfaceOfLinearExtrusion"), "Work around should not be implemented");
                    }
                }
            }
        }
    }
}

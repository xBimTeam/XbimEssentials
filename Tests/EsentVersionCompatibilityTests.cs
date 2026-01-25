using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Configuration;
using Xbim.Ifc;
using Xbim.Ifc2x3;
using Xbim.Ifc4.Interfaces;
using Xbim.IO;
using System.Diagnostics;
using Xbim.IO.Esent;
using Xunit;
using Xunit.Abstractions;

namespace Xbim.Essentials.Tests
{
    [Collection(nameof(xUnitBootstrap))]
    public class EsentVersionCompatibilityTests
    {
        private static readonly IEntityFactory ef2x3 = new Ifc2x3.EntityFactoryIfc2x3();
        private readonly ITestOutputHelper output;

        public EsentVersionCompatibilityTests(ITestOutputHelper output)
        {
            this.output = output;
            // Clear the singleton collection each test
            SuT = XbimServices.CreateInstanceInternal();
        }

        private XbimServices SuT;

        // xUnit captures Console.WriteLine and shows it in test output; no test output helper required here.

        [Fact]
        public void EsentModel_DefaultEngineFormatVersion_IsNot9060()
        {
            // set the Esent model as the default for this test
            SuT.ConfigureServices(s => s.AddXbimToolkit(opt => opt.AddEsentModel()));
            PersistedEntityInstanceCache.ForceEngineFormatVersion9060.Should().BeFalse("Esent model should be configured to default engine format for this test.");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanWriteWithLimitedDatabase(bool limit)
        {
            SuT.ConfigureServices(s => s.AddXbimToolkit(opt => opt.AddEsentModel(limit))); // we are limiting the esent version here
            string transientXbimFile;
            string savedXBimFile = Path.ChangeExtension(Guid.NewGuid().ToString(), ".xbim");

            // Load with Esent/File-based store - creates a temp xbim file in %TEMP%
            using (var ifcStore = IfcStore.Open("TestFiles\\4walls1floorSite.ifc", null, 0))
            {
                transientXbimFile = ifcStore.Location;

                ifcStore.SaveAs(savedXBimFile);
                ifcStore.Close();
            }
            Assert.True(File.Exists(savedXBimFile));
            Assert.False(File.Exists(transientXbimFile));

            // Run EsentUtl to show the header metadata for the saved xbim file and capture its output
            string esentOutput = null;

            var psi = new ProcessStartInfo
            {
                FileName = "esentutl.exe",
                Arguments = $"/mh \"{savedXBimFile}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var p = Process.Start(psi))
            {
                // read both streams to the end
                var stdOut = p.StandardOutput.ReadToEnd();
                var stdErr = p.StandardError.ReadToEnd();
                p.WaitForExit(60000); // wait up to 60s
                esentOutput = stdOut + (string.IsNullOrEmpty(stdErr) ? string.Empty : (Environment.NewLine + "ERROR:" + Environment.NewLine + stdErr));
                var limitedVersionString = esentOutput.Contains("Format ulVersion: 0x620,60,140");
                limit.Should().Be(limitedVersionString, $"Esent model saved file should {(limit ? "" : "not ")}be limited to version 9060.");
                output.WriteLine(esentOutput);
            }


            // write captured output to test output for inspection
            output.WriteLine(esentOutput ?? "(no output)");

            File.Delete(savedXBimFile);
        }

        [Fact]
        public void OpenAllEsentVersionFilesAndCountEntities()
        {
            // set the Esent model as the default for this test, and configure it to limit db version.
            // Create a logger factory that writes to the xUnit test output
            var loggerFactory = new Microsoft.Extensions.Logging.LoggerFactory();
            loggerFactory.AddProvider(new TestOutputLoggerProvider(output));
            SuT.ConfigureServices(s => s.AddXbimToolkit(opt => opt.AddLoggerFactory(loggerFactory).AddEsentModel(true)));

            PersistedEntityInstanceCache.ForceEngineFormatVersion9060.Should().BeTrue("Esent model should be configured to force engine format version 9060 for this test.");

            var testDir = Path.Combine("TestFiles", "EsentVersionFiles");
            Assert.True(Directory.Exists(testDir), $"Test directory not found: {testDir}");

            // Hardcoded list of test files with a boolean indicating whether
            // the file should be skipped on Windows 10 installations.
            var files = new[]
            {
                Tuple.Create("win10-generatedJuly2022.xBIM", false),
                Tuple.Create("win10-unconstrained.xbim", false),
                Tuple.Create("win11-formatVersion9060.xbim", false),
                // do not run this file on Windows 10 installations
                Tuple.Create("win11-unconstrained.xbim", true)
            };

            // Detect Windows 10 (build < 22000). Windows 11 has build numbers >= 22000.
            var isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
            var osVersion = Environment.OSVersion.Version;
            var isWindows10 = isWindows && osVersion.Major == 10 && osVersion.Build < 22000;

            foreach (var entry in files)
            {
                var name = entry.Item1;
                var skipOnWin10 = entry.Item2;
                var file = Path.Combine(testDir, name);
                Assert.True(File.Exists(file), $"Test file not found: {file}");

                if (skipOnWin10 && isWindows10)
                {
                    // Skip testing this file on Windows 10 installations.
                    Console.WriteLine($"Skipping {name} on Windows 10");
                    continue;
                }

                using (var model = new IO.Esent.EsentModel(ef2x3))
                {
                    model.Open(file, XbimDBAccess.Read);
                    // Ensure we can enumerate instances without throwing
                    var count = model.Instances.Count;
                    Assert.True(count >= 0, $"Count should be >= 0 for {file}");
                }

                using (var model2 = IfcStore.Open(file))
                {
                    // Ensure we can enumerate instances without throwing
                    var count = model2.Instances.Count;
                    Assert.True(count >= 0, $"Count should be >= 0 for {file}");
                    output.WriteLine($"Successfully opened and counted {count} entities in {name}");
                }
            }
        }
    }
}

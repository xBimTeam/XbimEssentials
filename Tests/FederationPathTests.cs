using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using System.IO;
using System.Diagnostics;

namespace Xbim.Essentials.Tests
{
    /// <summary>
    /// Summary description for FederationPathTests
    /// </summary>
    [TestClass]

    public class FederationPathTests
    {
        const string sourceFolder = @"FederationTestFiles\";
        const string federationFile = @"RelativePathFederation.ifc";

        [TestMethod]
        public void CanFindRelativeFiles()
        {
            if (Directory.Exists("moved"))
                Directory.Delete("moved", true);
            Directory.CreateDirectory("moved");

            File.Copy("FederationTestFiles\\RelativePathFederation.ifc", "moved\\RelativePathFederation.ifc");
            File.Copy("FederationTestFiles\\P1_cm.ifc", "moved\\P1_cm.ifc");
            File.Copy("FederationTestFiles\\P2_cm.ifc", "moved\\P2_cm.ifc");

            var store = IfcStore.Open("moved\\RelativePathFederation.ifc");
            Assert.AreEqual(2, store.ReferencedModels.Count());
        }
    }
}

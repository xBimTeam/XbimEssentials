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
        [DeploymentItem(sourceFolder + federationFile, "moved")]
        [DeploymentItem(sourceFolder + "P1_cm.ifc", "moved")]
        [DeploymentItem(sourceFolder + "P2_cm.ifc", "moved")]
        public void CanFindRelativeFiles()
        {
            FileInfo f = new FileInfo(".");
            Debug.WriteLine(f.FullName);

            var store = IfcStore.Open("moved\\" + federationFile);
            Assert.AreEqual(2, store.ReferencedModels.Count());
        }
    }
}

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Essentials.Tests.Utilities;
using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem(@"TestSourceFiles")]
    public class BasicModelTests
    {
        [TestMethod]
        public void OpenIfcFile()
        {
            using (var models = new ModelFactory("4walls1floorSite.ifc"))
            {
                models.Do(m => 
                    Assert.IsTrue(m.Instances.Count > 0)
                );
            }
        }

        [TestMethod]
        public void OpenIfcZipFile()
        {
            using (var models = new ModelFactory("TestZip.ifczip"))
            {
                models.Do(m =>
                    Assert.IsTrue(m.Instances.Count > 0)
                );
            }
        }

        [TestMethod]
        public void OpenIfcZipXmlFile()
        {
            using (var models = new ModelFactory("HelloWallXml.ifczip"))
            {
                models.Do(m =>
                    Assert.IsTrue(m.Instances.Count > 0)
                );
            }
        }

        [TestMethod]
        public void OpenIfcZipXmlFileWithProgress()
        {
            // Opening Zipped IfcXml was crashing when updating progress, since the DeflateStream does not implement Position
            void progress(int percent, object o) { };

            var model = Ifc.IfcStore.Open("HelloWallXml.ifczip", null, null, progress, XbimDBAccess.Read);

            Assert.IsTrue(model.Instances.Count > 0);
  
        }

        [TestMethod]
        public void OpenIfcXmlFile()
        {
            using (var models = new ModelFactory("4walls1floorSite.ifcxml"))
            {
                models.Do(m =>
                    Assert.IsTrue(m.Instances.Count > 0)
                );
            }
        }
        
    }
}

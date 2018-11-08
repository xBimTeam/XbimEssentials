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

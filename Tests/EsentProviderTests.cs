using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc;
using Xbim.IO.Esent;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class EsentProviderTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles\\4walls1floorSite.ifc")]
        public void PersistedEsentTest()
        {
            var file = "4walls1floorSite.ifc";
            var db = Guid.NewGuid().ToString() + ".xbim";
            var provider = new EsentModelProvider { DatabaseFileName = db };
            var schema = provider.GetXbimSchemaVersion(file);
            using (var model = provider.Open(file, schema))
            {
                provider.Close(model);
            }

            using (var model = IfcStore.Open(db))
            {
                Assert.IsTrue(model.Instances.Any());
            }

        }
    }
}

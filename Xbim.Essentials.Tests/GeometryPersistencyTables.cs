using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.IO;
using Xbim.IO.Esent;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    
    public class GeometryPersistencyTables
    {
        [TestMethod]
        [DeploymentItem(@"GeometryCacheTestFiles\", @"Persistency\")]
        public void CanUpgradeDbStucture()
        {
            using (var m = new Ifc2x3.IO.XbimModel())
            {
                m.Open(@"Persistency\Monolith_v10.xBIM", XbimDBAccess.Exclusive);
                Assert.AreEqual(1, m.GeometrySupportLevel, "GeometrySupportLevel for Monolith_v10 should be 1");

                var updated = m.EnsureGeometryTables();
                Assert.AreEqual(updated, true, "Should complete returning true");

                m.DeleteGeometryCache();
                Assert.AreEqual(0, m.GeometrySupportLevel,
                    "GeometrySupportLevel for Monolith_v10 should be 0 after removing it.");

                m.Close();
            }
        }

        [TestMethod]
        [DeploymentItem(@"GeometryCacheTestFiles\", @"Persistency\")]
        // [DeploymentItem("TestFiles")]
        public void ResourceReleaseOnOldModel()
        {
            using (var model = IfcStore.Open(@"Persistency\Monolith_v10.xBIM"))
            {
                var geomStore = model.GeometryStore;
                var tp = geomStore.GetType().ToString();
                Assert.IsTrue(geomStore.IsEmpty);
                model.Close();
            }
        }
    }
}

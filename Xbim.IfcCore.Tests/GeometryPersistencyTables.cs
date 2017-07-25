using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.IO;
using Xbim.IO.Esent;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem(@"GeometryCacheTestFiles\", @"Persistency\")]
    public class GeometryPersistencyTables
    {
        [TestMethod]
        public void CanUpgradeDbStucture()
        {
            using (var m = new Xbim.Ifc2x3.IO.XbimModel())
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
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.IO;
using Xbim.IO.Esent;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem(@"GeometryCacheTestFiles\")]
    public class GeometryCacheTests
    {
        [TestMethod]
        public void FileVersionIsCorrect()
        {
            var m = new Xbim.Ifc2x3.IO.XbimModel();

            m.Open("Monolith-NoGeomTables.xBIM", XbimDBAccess.ReadWrite);
            Assert.AreEqual(0, m.GeometrySupportLevel, "GeometrySupportLevel should be 0");
            m.Close();

            m.Open("Monolith_Nogeom_Version1Schema.xBIM");
            Assert.AreEqual(0, m.GeometrySupportLevel, "GeometrySupportLevel should be 0");
            m.Close();

            m.Open("Monolith_v10.xBIM");
            Assert.AreEqual(1, m.GeometrySupportLevel, "GeometrySupportLevel for Monolith_v10 should be 1");
            m.Close();

            m.Open("Monolith_v20.xBIM");
            Assert.AreEqual(2, m.GeometrySupportLevel, "GeometrySupportLevel for Monolith_v20 should be 2");
            m.Close();
        }
    }
}

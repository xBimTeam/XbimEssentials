using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc4;
using Xbim.IO;
using Xbim.IO.Esent;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class GeometryCacheTests
    {
        [TestMethod]
        public void FileVersionIsCorrect()
        {
            var m = new EsentModel(new EntityFactoryIfc4());

            m.Open("GeometryCacheTestFiles\\Monolith-NoGeomTables.xBIM", XbimDBAccess.ReadWrite);
            Assert.AreEqual(0, m.GeometrySupportLevel, "GeometrySupportLevel should be 0");
            m.Close();

            m.Open("GeometryCacheTestFiles\\Monolith_Nogeom_Version1Schema.xBIM");
            Assert.AreEqual(0, m.GeometrySupportLevel, "GeometrySupportLevel should be 0");
            m.Close();

            m.Open("GeometryCacheTestFiles\\Monolith_v10.xBIM");
            Assert.AreEqual(1, m.GeometrySupportLevel, "GeometrySupportLevel for Monolith_v10 should be 1");
            m.Close();

            m.Open("GeometryCacheTestFiles\\Monolith_v20.xBIM");
            Assert.AreEqual(2, m.GeometrySupportLevel, "GeometrySupportLevel for Monolith_v20 should be 2");
            m.Close();
        }
    }
}

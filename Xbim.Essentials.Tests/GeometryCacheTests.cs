using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem(@"GeometryCacheTestFiles\")]
    public class GeometryCacheTests
    {
        [TestMethod]
        public void FileVersionIsCorrect()
        {
            var m = new XbimModel();
            m.Open("Monolith_v10.xBIM");
            Assert.AreEqual(1, m.GeometrySupportLevel, "GeometrySupportLevel for Monolith_v10 should be 1");
            m.Close();

            m.Open("Monolith_v20.xBIM");
            Assert.AreEqual(2, m.GeometrySupportLevel, "GeometrySupportLevel for Monolith_v20 should be 2");
            m.Close();
        }
    }
}

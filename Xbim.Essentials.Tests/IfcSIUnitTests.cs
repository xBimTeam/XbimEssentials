using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Xbim.Essentials.Tests
{
    [DeploymentItem(@"TestSourceFiles\")]
    [TestClass]
    public class IfcSIUnitTests : IfcTestBase
    {
        // Initialisation magic is done in IfcTestBase constructor and BeforeTest()

        /// <summary>
        /// Bug GH Issue#4 (Codeplex 
        /// </summary>
        [TestMethod]
        public void ToString_Should_Equal_PrefixPlusName()
        {
 
            var unit = Model.IfcProject.UnitsInContext.Units.First();

            // Bug #4 was that this returned AMPERE unless Activate had been called.
            Assert.AreEqual("MILLIMETRE", unit.ToString());
        }
        
    }
}

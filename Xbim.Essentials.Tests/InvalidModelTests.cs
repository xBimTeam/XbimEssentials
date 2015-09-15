using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xbim.Essentials.Tests
{
    [DeploymentItem(@"TestSourceFiles\")]
    [TestClass]
    public class InvalidModelTests : IfcTestBase
    {
        public InvalidModelTests()
        //: base()                          /* Passes */
        //: base("Invalid.ifc")             /* Fails */
        //: base("InvalidModelTests.ifc")   /* Passes */
        //: base("InvalidModelTests2.ifc")  /* Passes */
        : base("InvalidModelTests3.ifc")    /* Fails */
        /// Note all models are IDENTICAL except we get different results with each! Race condition triggered on name???
        { }
        

        /// <summary>
        /// Issue XbimEssential#8 - when parsing fails catastrophically the file xbim should not be left open
        /// </summary>
        [TestMethod]
        public void Parsing_Invalid_Model_Should_Not_Leave_Xbim_Open()
        {
            // Arranging occurs in constructor and may fail since InvalidModelTest.ifc has invalid content

            //Act
            this.Model.Dispose();
            // We should be able to delete the xbim
            File.Delete(XbimFile);
            //Assert
            Assert.IsFalse(File.Exists(XbimFile));

        }
    }
}

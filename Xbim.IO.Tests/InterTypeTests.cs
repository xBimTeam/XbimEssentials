using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class InterTypeTests
    {
        [TestMethod]
        public void TestInterschemaTypeAssignment()
        {
            Ifc2x3.MeasureResource.IfcIdentifier a = "aaa";
            Ifc4.MeasureResource.IfcIdentifier b = (string)a;

            Assert.IsTrue(b == "aaa");
        }
    }
}

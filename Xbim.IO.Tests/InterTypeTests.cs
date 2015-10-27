using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class InterTypeTests
    {
        [TestMethod]
        public void TestInterschemaTypeAssignment()
        {
            Ifc2x3.MeasureResource.IfcIdentifier a = "aaa";
            IfcIdentifier b = (string)a;

            Assert.IsTrue(b == "aaa");
        }

        IEnumerable<string> Test()
        {
            for (int i = 0; i < 10; i++)
            {
                yield return new IfcIdentifier("Test...");
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class XbimColourTests
    {
        [TestMethod]
        public void CanPersistColour()
        {
            var c = new XbimColour(0.3f, 0.4f, 0.5f, 0.6f);
            var asString = c.ToString();
            var c2 = XbimColour.FromString(asString);
            var isEqual = c2.Equals(c);
            Assert.IsTrue(isEqual, "XbimColour ToString Persistency failed.");

            var custom = "R:0.3 G:0.4 B:0.5 A:0.6";
            c2 = XbimColour.FromString(custom);
            isEqual = c2.Equals(c);
            Assert.IsTrue(isEqual, "XbimColour ToString Persistency failed.");

            custom = "R:0,3 G:0,4 B:0,5 A:0,6";
            c2 = XbimColour.FromString(custom);
            isEqual = c2.Equals(c);
            Assert.IsTrue(isEqual, "XbimColour ToString Persistency failed.");
        }
    }
}

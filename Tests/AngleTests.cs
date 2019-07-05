using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Geometry;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class AngleTests
    {
        [TestMethod]
        public void ColinearityTest()
        {
            var a = new XbimVector3D(1, 0, 0);
            var b = new XbimVector3D(0.5, 0, 0);

            var ident = a.Angle(a);
            Assert.AreEqual(0, ident, 1e-9);

            var angle = a.Angle(b);
            Assert.AreEqual(0, angle, 1e-9);

            b = new XbimVector3D(1, 1, 0);
            angle = a.Angle(b);
            Assert.AreEqual(Math.PI / 4.0, angle, 1e-9);

            b = new XbimVector3D(0, 1, 0);
            angle = a.Angle(b);
            Assert.AreEqual(Math.PI / 2.0, angle, 1e-9);

            b = new XbimVector3D(-1, 1, 0);
            angle = a.Angle(b);
            Assert.AreEqual(Math.PI * 3.0 / 4.0, angle, 1e-9);

            b = new XbimVector3D(-1, 0, 0);
            angle = a.Angle(b);
            Assert.AreEqual(Math.PI, angle, 1e-9);

            b = new XbimVector3D(1, -1, 0);
            angle = a.Angle(b);
            Assert.AreEqual(Math.PI / 4.0, angle, 1e-9);

            b = new XbimVector3D(-1, -1, 0);
            angle = a.Angle(b);
            Assert.AreEqual(Math.PI * 3.0/4.0, angle, 1e-9);
        }
    }
}

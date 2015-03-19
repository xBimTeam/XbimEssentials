using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Geometry;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class PackedNormalTests
    {
        [TestMethod]
        public void RoundTripTest()
        {
            var vec = new XbimVector3D(0.0749087609620539, 0.264167604633194, 0.961563390626687);
            Debug.WriteLine(vec);
            
            var pack = new XbimPackedNormal(vec);
            var roundVec = pack.Normal;
            Debug.WriteLine(roundVec);

            var a = vec.CrossProduct(roundVec);
            var x = Math.Abs(a.Length);
            var y = vec.DotProduct(roundVec);
            var angle = Math.Atan2(x, y);
            if (angle > 0.1)
                Debug.WriteLine("vector: {0}, angle: {1:F3}", vec, angle);
            Assert.IsTrue(angle < 0.13);

        }
    }
}

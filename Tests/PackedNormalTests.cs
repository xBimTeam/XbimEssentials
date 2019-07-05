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
            var tests = new[]
            {
                new XbimVector3D(0.0749087609620539, 0.264167604633194, 0.961563390626687),
                new XbimVector3D(-0.0535755113215756, 0.316639902069201, 0.947031592400295),
                new XbimVector3D(-0.0403690470743153, -0.0845001599207948, 0.995605375142015),
                new XbimVector3D(-0.170389413996118, 0.321003309980549, 0.931624560957681)
            };

            foreach (var vec in tests)
            {
                Debug.WriteLine(vec);

                var pack = new XbimPackedNormal(vec);
                var roundVec = pack.Normal;
                Debug.WriteLine(roundVec);

                var a = vec.CrossProduct(roundVec);
                var x = Math.Abs(a.Length);
                var y = vec.DotProduct(roundVec);
                var angle = Math.Atan2(x, y);
                if (angle > 0.1)
                    Debug.WriteLine("vector: {0}, angle: {1:F3}", vec, angle*180.0/Math.PI);
                Assert.IsTrue(angle < 0.13);    
            }
            

        }
    }
}

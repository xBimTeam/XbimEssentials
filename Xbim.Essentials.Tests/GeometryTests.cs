using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Geometry;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class GeometryTests
    {
        [TestMethod]
        public void PackedNormalTests()
        {
           // var vArray = new[] {new XbimVector3D(0, 0, 1), new XbimVector3D(0, 1, 0), new XbimVector3D(1, 0, 0), new XbimVector3D(1, 0, 0)};
            var vectors = (List<XbimVector3D>)UniformPointsOnSphere(40);

            foreach (var v in vectors)
            {
                var packed = new XbimPackedNormal(v);
                var v2 = packed.Normal;
                var a = v.CrossProduct(v2);
                var x = Math.Abs(a.Length);
                var y = v.DotProduct(v2);
                var angle = Math.Atan2(x, y);
                Assert.IsTrue(angle < 0.1);
            }


        }
        IEnumerable<XbimVector3D> UniformPointsOnSphere(float n)
        {
            var points = new List<XbimVector3D>();
            var i = Math.PI * (3 - Math.Sqrt(5));
            var o = 2 / n;
            for (var k = 0; k < n; k++)
            {
                var y = k * o - 1 + (o / 2);
                var r = Math.Sqrt(1 - y * y);
                var phi = k * i;
                var v = new XbimVector3D(Math.Cos(phi)*r, y, Math.Sin(phi)*r);
                points.Add(v);
            }
            return points;
        }
    }
}

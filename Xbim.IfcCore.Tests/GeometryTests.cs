using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Geometry;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class GeometryTests
    {
        [TestMethod]
        public void QuaternionTests()
        {
            var q = new XbimQuaternion();
            Assert.AreEqual(true, q.IsIdentity(), "Uninitialised quaternion should be identity.");

            q = new XbimQuaternion(0.0f, 0.0f, 0.0f, 1.0f);
            Assert.AreEqual(true, q.IsIdentity(), "Should be identity when initialised with floats.");

            var mat = new XbimMatrix3D();
            q = mat.GetRotationQuaternion();
            Assert.AreEqual(true, q.IsIdentity(), "Quaternion from identity matrix shold be identity.");
        }

        [TestMethod]
        public void PackedNormalTests()
        {
            var vectors = (List<XbimVector3D>)UniformPointsOnSphere(100);

            foreach (var v in vectors)
            {
                var packed = new XbimPackedNormal(v);
                var v2 = packed.Normal;
                var a = v.CrossProduct(v2);
                var x = Math.Abs(a.Length);
                var y = v.DotProduct(v2);
                var angle = Math.Atan2(x, y);
                if (angle > 0.1) 
                    Debug.WriteLine("vector: {0}, angle: {1:F3}", v, angle);
                Assert.IsTrue(angle < 0.13);
            }

            //text axis aligned normals (this should be much more precise)
            var vArray = new[]
            {
                new XbimVector3D(0, 0, 1), 
                new XbimVector3D(0, 0, -1), 
                new XbimVector3D(0, 1, 0), 
                new XbimVector3D(0, -1, 0), 
                new XbimVector3D(1, 0, 0), 
                new XbimVector3D(-1, 0, 0)
            };
            foreach (var v in vArray)
            {
                var packed = new XbimPackedNormal(v);
                var v2 = packed.Normal;
                var a = v.CrossProduct(v2);
                var x = Math.Abs(a.Length);
                var y = v.DotProduct(v2);
                var angle = Math.Atan2(x, y);
                Assert.IsTrue(angle < 1e-10);
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

        [TestMethod]
        public void MatrixArrayConversion()
        {
            var m = XbimMatrix3D.CreateTranslation(10, 20, 30);
            m.RotateAroundXAxis(Math.PI / 4);
            m.Scale(.05);

            var outM = m.ToArray(true);
            var rback = XbimMatrix3D.FromArray(outM);

            Assert.AreEqual(rback.M11, m.M11);
            Assert.AreEqual(rback.M12, m.M12);
            Assert.AreEqual(rback.M13, m.M13);
            Assert.AreEqual(rback.M14, m.M14);

            Assert.AreEqual(rback.M21, m.M21);
            Assert.AreEqual(rback.M22, m.M22);
            Assert.AreEqual(rback.M23, m.M23);
            Assert.AreEqual(rback.M24, m.M24);

            Assert.AreEqual(rback.M31, m.M31);
            Assert.AreEqual(rback.M32, m.M32);
            Assert.AreEqual(rback.M33, m.M33);
            Assert.AreEqual(rback.M34, m.M34);

            Assert.AreEqual(rback.OffsetX, m.OffsetX);
            Assert.AreEqual(rback.OffsetY, m.OffsetY);
            Assert.AreEqual(rback.OffsetZ, m.OffsetZ);
            Assert.AreEqual(rback.M44, m.M44);

        }
    }
}

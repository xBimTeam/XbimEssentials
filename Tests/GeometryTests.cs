using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.IO;

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
        public void MatrixTests()
        {
            var m = new XbimMatrix3D();

            Assert.AreEqual(XbimMatrix3D.Identity.Up, m.Up, "Uninitialised matrix Up vector is equal to identity's.");
            Assert.AreEqual(XbimMatrix3D.Identity.Right, m.Right, "Uninitialised matrix Right vector is equal to identity's.");
            Assert.AreEqual(XbimMatrix3D.Identity.Forward, m.Forward, "Uninitialised matrix Forward vector is equal to identity's.");

            var _ = m.M21; // randomly accessing on of matrix' elements

            // Vectors assertions should pass
            Assert.AreEqual(XbimMatrix3D.Identity.Up, m.Up, "Up vector is equal to identity's after matrix' cell access.");
            Assert.AreEqual(XbimMatrix3D.Identity.Right, m.Right, "Right vector is equal to identity's after matrix' cell access.");
            Assert.AreEqual(XbimMatrix3D.Identity.Forward, m.Forward, "Forward vector is equal to identity's after matrix' cell access.");


            m = new XbimMatrix3D(XbimVector3D.Zero);

            Assert.AreEqual(XbimMatrix3D.Identity.Up, m.Up, "Matrix initialized with Zero vector - Up vector is equal to identity's.");
            Assert.AreEqual(XbimMatrix3D.Identity.Right, m.Right, "Matrix initialized with Zero vector - Right vector is equal to identity's.");
            Assert.AreEqual(XbimMatrix3D.Identity.Forward, m.Forward, "Matrix initialized with Zero vector - Forward vector is equal to identity's.");

            _ = m.M21;

            Assert.AreEqual(XbimMatrix3D.Identity.Up, m.Up, "Up vector is equal to identity's after matrix' cell access.");
            Assert.AreEqual(XbimMatrix3D.Identity.Right, m.Right, "Right vector is equal to identity's after matrix' cell access.");
            Assert.AreEqual(XbimMatrix3D.Identity.Forward, m.Forward, "Forward vector is equal to identity's after matrix' cell access.");


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
                var v = new XbimVector3D(Math.Cos(phi) * r, y, Math.Sin(phi) * r);
                points.Add(v);
            }
            return points;
        }


        [TestMethod]
        public void CameraLookAtWorks()
        {
            var eye = new XbimVector3D(1, 0, 1);
            var target = new XbimVector3D(0, 0, 0);
            var up = new XbimVector3D(-1, 0, 1).Normalized();
            var la = XbimMatrix3D.CreateLookAt(eye, target, up);


            var pnt = new XbimPoint3D(-1, 0, 1);
            var trsf = la.Transform(pnt);

            Assert.AreEqual(0, trsf.X);
            Assert.AreEqual(1.4142135623730949, trsf.Y);
            Assert.AreEqual(-1.4142135623730949, trsf.Z);
        }

        [TestMethod]
        public void DirectionsWithLessThan3DimsCanBeNormalisedToVector()
        {
            using (var model = IfcStore.Create(XbimSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel))
            using (var txn = model.BeginTransaction())
            {
                var dir = model.Instances.New<IfcDirection>();
                dir.SetXY(1, double.NaN);

                var vectorised = dir.Normalise();
                Assert.AreEqual(1, vectorised.X);
                Assert.AreEqual(0, vectorised.Y);
                Assert.AreEqual(0, vectorised.Z);


                var dir2 = model.Instances.New<IfcDirection>();
                dir.SetXY(1, 1);
                vectorised = dir2.Normalise();
                Assert.AreNotEqual(1, vectorised.X);
                Assert.AreEqual(vectorised.Y, vectorised.X);
                Assert.AreEqual(0, vectorised.Z);

                txn.Commit();
            }
        }


        [TestMethod]
        public void TransformationOfRegion()
        {
            XbimRect3D r = new XbimRect3D(
                100, 100, 0,
                200, 200, 0
                );
            XbimMatrix3D m = XbimMatrix3D.CreateRotation( // 90 ccw
                new XbimPoint3D(1, 0, 0),
                new XbimPoint3D(0, 1, 0)
                );
            var rotated = r.Transform(m);

            Assert.AreEqual(-300, rotated.Min.X);
            Assert.AreEqual(100, rotated.Min.Y);
            Assert.AreEqual(0, rotated.Min.Z);

            Assert.AreEqual(-100, rotated.Max.X);
            Assert.AreEqual(300, rotated.Max.Y);
            Assert.AreEqual(0, rotated.Max.Z);

        }

        [TestMethod]
        public void RotationMatrixFromVectorsTests()
        {
            // y to x
            XbimPoint3D from = new XbimPoint3D(0, 1, 0);
            XbimPoint3D to = new XbimPoint3D(1, 0, 0);
            TestRotationCreation(from, to);

            TestRotationCreation(from, from);
            TestRotationCreation(to, to);


            to = new XbimPoint3D(0.4, 0.4, 0);
            TestRotationCreation(from, to);

            to = new XbimPoint3D(-0.4, 0.4, 0);
            TestRotationCreation(from, to);
        }

        private void TestRotationCreation(XbimPoint3D from, XbimPoint3D to)
        {
            var m = XbimMatrix3D.CreateRotation(from, to);
            var toForTest = m.Transform(from);
            var toNormalised = (to - new XbimPoint3D(0, 0, 0)).Normalized();
            var toNormP = new XbimPoint3D(
                toNormalised.X,
                toNormalised.Y,
                toNormalised.Z
                );
            Assert.IsTrue(IsTolerableDifference(toNormP, toForTest));
        }

        private bool IsTolerableDifference(XbimPoint3D expectedValue, XbimPoint3D resultingValue)
        {
            return
                IsTolerableDifference(expectedValue.X, resultingValue.X)
                &&
                IsTolerableDifference(expectedValue.Y, resultingValue.Y)
                &&
                IsTolerableDifference(expectedValue.Z, resultingValue.Z)
                ;
        }

        private bool IsTolerableDifference(double expectedValue, double resultingValue)
        {
            var diff = resultingValue - expectedValue;
            return (diff < 1E-15);
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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Reflection;
using Xbim.Common.Geometry;
using Xbim.Common.Step21;

namespace Xbim.Common.Tests
{
    [TestClass]
    public class PackedNormalTests
    {
        [TestMethod]
        public void PackedNormalRoundTripTest()
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
                Trace.WriteLine(vec);

                var pack = new XbimPackedNormal(vec);
                var roundVec = pack.Normal;
                Trace.WriteLine(roundVec);

                var a = vec.CrossProduct(roundVec);
                var x = Math.Abs(a.Length);
                var y = vec.DotProduct(roundVec);
                var angle = Math.Atan2(x, y);
                if (angle > 0.1)
                    Trace.WriteLine($"vector: {vec}, angle: { angle * 180.0 / Math.PI:F3}");
                Assert.IsTrue(angle < 0.13);
            }


        }
    
    [TestMethod]
    public void StepFileHeaderVersionTest()
    {
            var modelFake = new IModelFake();
            var header = new StepFileHeader(StepFileHeader.HeaderCreationMode.InitWithXbimDefaults, modelFake);
            var t = typeof(IModelFake);
            Assert.IsTrue(header.FileName.OriginatingSystem == t.GetTypeInfo().Assembly.GetName().Name);
            Assert.IsTrue(header.FileName.PreprocessorVersion == string.Format("Processor version {0}", t.GetTypeInfo().Assembly.GetName().Version));
        }
}
}

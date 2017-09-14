using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.IO;
using Xbim.IO.Memory;

namespace Xbim.IfcCore.UnitTests
{
    [TestClass]
    public class PartialFileTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles/SmallModelIfc2x3.ifc")]
        public void CreatingPartialFile()
        {
            using (var w = new StringWriter())
            {
                using (var mm = MemoryModel.OpenRead("TestFiles/SmallModelIfc2x3.ifc"))
                {
                    var extrusion = mm.Instances.FirstOrDefault<IfcExtrudedAreaSolid>();
                    ModelHelper.WritePartialFile(mm, extrusion, w, new HashSet<int>());

                    var part = w.ToString();
                    Assert.IsFalse(string.IsNullOrEmpty(part));

                    using (var insModel = new MemoryModel(new EntityFactory()))
                    {
                        insModel.InsertCopy(extrusion, new Common.XbimInstanceHandleMap(mm, insModel), null, false, true, true);
                        using (var partModel = new MemoryModel(new EntityFactory()))
                        {
                            partModel.LoadStep21Part(part);
                            Assert.IsTrue(insModel.Instances.Count == partModel.Instances.Count);

                            var ext2 = partModel.Instances.FirstOrDefault<IfcExtrudedAreaSolid>();
                            Assert.IsTrue(ext2.Depth == extrusion.Depth);
                            Assert.IsTrue(ext2.ExtrudedDirection.X == extrusion.ExtrudedDirection.X);
                            Assert.IsTrue(ext2.ExtrudedDirection.Y == extrusion.ExtrudedDirection.Y);
                            Assert.IsTrue(ext2.ExtrudedDirection.Z == extrusion.ExtrudedDirection.Z);
                        }
                    }

                }
            }
        }

        [TestMethod]
        public void WeakReferenceTest()
        {
            var count = 10000000;

            var weakCounter = new Counter();
            var weak = new ObjectWithWeakReference() { Counter = weakCounter };
            var strong = new ObjectWithStrongReference();

            var w = Stopwatch.StartNew();
            for (int i = 0; i < count; i++)
            {
                weak.Counter.Count++;
            }
            w.Stop();
            Console.WriteLine($"Weak time: {w.ElapsedMilliseconds}ms");

            w.Restart();
            for (int i = 0; i < count; i++)
            {
                strong.Counter.Count++;
            }
            w.Stop();
            Console.WriteLine($"Strong time: {w.ElapsedMilliseconds}ms");

            Assert.IsTrue(weak.Counter.Count == strong.Counter.Count);
        }

        private class ObjectWithWeakReference
        {
            private WeakReference<Counter> _reference = new WeakReference<Counter>(new Counter());
            public Counter Counter {
                get
                {
                    if (_reference.TryGetTarget(out Counter c))
                        return c;
                    return null;
                }
                set
                {
                    _reference = new WeakReference<Counter>(value);
                }
            }
        }

        private class ObjectWithStrongReference
        {
            public Counter Counter { get; set; } = new Counter();
        }

            private class Counter
        {
            public int Count { get; set; } = 0;
        }
    }
}

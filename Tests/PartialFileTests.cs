using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc4.Interfaces;
using Xbim.IO;
using Xbim.IO.Memory;

namespace Xbim.IfcCore.UnitTests
{
    [TestClass]
    public class PartialFileTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles/SampleHouse4.ifc")]
        public void SkipEntities()
        {
            var file = @"SampleHouse4.ifc";
            var readTypes = new[] {
                typeof(IIfcSite),
                typeof(IIfcPostalAddress)
            };
            var ignoreTypes = typeof(Ifc4.EntityFactoryIfc4)
                .Assembly.GetTypes()
                .Where(t => t.IsClass && t.IsPublic && !t.IsAbstract && !readTypes.Any(rt => rt.IsAssignableFrom(t)))
                .Select(t => t.Name.ToUpperInvariant())
                .ToArray();

            // no ignoring
            var timeA = 0L;
            using (var s = File.OpenRead(file))
            {
                var w = Stopwatch.StartNew();
                var model = MemoryModel.OpenReadStep21(s);
                w.Stop();
                timeA = w.ElapsedMilliseconds;
                Console.WriteLine($"Opening complete model: {timeA}");

                var site = model.Instances.FirstOrDefault<IIfcSite>();
                Assert.IsNotNull(site);
                var address = site.SiteAddress;
            }

            // ignoring almost everything
            var timeB = 0L;
            using (var s = File.OpenRead(file))
            {
                var w = Stopwatch.StartNew();
                var model = MemoryModel.OpenReadStep21(s, null, null, ignoreTypes);
                w.Stop();
                timeB = w.ElapsedMilliseconds;
                Console.WriteLine($"Opening stripped down model: {timeB}");

                var site = model.Instances.FirstOrDefault<IIfcSite>();
                Assert.IsNotNull(site);
                var address = site.SiteAddress;
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles/SmallModelIfc2x3.ifc")]
        public void CreatingPartialFile()
        {
            using (var w = new StringWriter())
            {
                using (var mm = MemoryModel.OpenRead("SmallModelIfc2x3.ifc"))
                {
                    var extrusion = mm.Instances.FirstOrDefault<IfcExtrudedAreaSolid>();
                    ModelHelper.WritePartialFile(mm, extrusion, w, new HashSet<int>());

                    var part = w.ToString();
                    Assert.IsFalse(string.IsNullOrEmpty(part));

                    var ef = new EntityFactoryIfc2x3();
                    using (var insModel = new MemoryModel(ef))
                    {
                        insModel.InsertCopy(extrusion, new Common.XbimInstanceHandleMap(mm, insModel), null, false, true, true);
                        using (var partModel = new MemoryModel(ef))
                        {
                            var errs = partModel.LoadStep21Part(part);
                            Assert.IsTrue(errs == 0);
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
    }
}

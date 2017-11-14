using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc;
using System.Diagnostics;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Esent;
using Xbim.Common.Step21;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class InverseCacheTests
    {
        private const string _file = "SampleHouse4.ifc";

        [TestMethod]
        [DeploymentItem("TestSourceFiles\\" + _file)]
        public void CacheCreation()
        {
            IfcSchemaVersion version;

            //as a MemoryModel
            using (var model = IfcStore.Open(_file, null, -1))
            {
                CacheCreation(model);
                version = model.IfcSchemaVersion;
            }

            //as EsentModel
            using (var model = new EsentModel(version == IfcSchemaVersion.Ifc4 ? (new Ifc4.EntityFactory() as IEntityFactory) : (new Ifc2x3.EntityFactory() as IEntityFactory)))
            {
                model.CreateFrom(_file, null, null, true, true, IO.IfcStorageType.Ifc);
                CacheCreation(model);
            }
        }

        private static void CacheCreation(IModel model)
        {
            using (model.BeginTransaction("Test"))
            {
                //opening transaction while inverse cache is running should throw an exception
                Assert.IsTrue(ThrowsXbimException(() => model.BeginCaching()));
            }

            var cachingTime = 0L;
            var noCachingTime = 0L;

            var w = Stopwatch.StartNew();
            var c = model.Instances.Where<IIfcObject>(o => o.IsDefinedBy.OfType<IIfcTypeObject>().FirstOrDefault()?.Name?.ToString()?.ToLower().Contains("wall") ?? false).Count() ==
                    model.Instances.Where<IIfcObject>(o => o.IsDefinedBy.OfType<IIfcTypeObject>().FirstOrDefault()?.Name?.ToString()?.ToLower().Contains("wall") ?? false).Count() &&
                    model.Instances.Where<IIfcObject>(o => o.IsDefinedBy.OfType<IIfcTypeObject>().FirstOrDefault()?.Name?.ToString()?.ToLower().Contains("wall") ?? false).Count() ==
                    model.Instances.Where<IIfcObject>(o => o.IsDefinedBy.OfType<IIfcTypeObject>().FirstOrDefault()?.Name?.ToString()?.ToLower().Contains("wall") ?? false).Count();
            Assert.IsTrue(c);
            w.Stop();
            noCachingTime = w.ElapsedMilliseconds;

            using (model.BeginCaching())
            {
                Assert.IsTrue(ThrowsXbimException(() => model.BeginTransaction("Exception")));

                w.Restart();
                var a = model.Instances.Where<IIfcObject>(o => o.IsDefinedBy.OfType<IIfcTypeObject>().FirstOrDefault()?.Name?.ToString()?.ToLower().Contains("wall") ?? false).Count() ==
                        model.Instances.Where<IIfcObject>(o => o.IsDefinedBy.OfType<IIfcTypeObject>().FirstOrDefault()?.Name?.ToString()?.ToLower().Contains("wall") ?? false).Count() &&
                        model.Instances.Where<IIfcObject>(o => o.IsDefinedBy.OfType<IIfcTypeObject>().FirstOrDefault()?.Name?.ToString()?.ToLower().Contains("wall") ?? false).Count() ==
                        model.Instances.Where<IIfcObject>(o => o.IsDefinedBy.OfType<IIfcTypeObject>().FirstOrDefault()?.Name?.ToString()?.ToLower().Contains("wall") ?? false).Count();
                Assert.IsTrue(a);
                w.Stop();
                cachingTime = w.ElapsedMilliseconds;
                model.StopCaching();
            }

            Assert.IsTrue(noCachingTime > cachingTime);

        }


        private static bool ThrowsXbimException(Action a)
        {
            try
            {
                a();
            }
            catch (XbimException)
            {
                return true;
            }
            return false;
        }
    }
}

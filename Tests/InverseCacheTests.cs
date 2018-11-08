using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Exceptions;
using System.Diagnostics;
using Xbim.Ifc4.Interfaces;
using Xbim.Essentials.Tests.Utilities;

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
            using (var models = new ModelFactory(_file))
            {
                models.Do(CacheCreation);
            }
        }

        private static void CacheCreation(IModel model)
        {
            using (model.BeginTransaction("Test"))
            {
                //opening transaction while inverse cache is running should throw an exception
                Assert.IsTrue(ThrowsXbimException(() => model.BeginInverseCaching()));
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

            using (var cache = model.BeginInverseCaching())
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
            }
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

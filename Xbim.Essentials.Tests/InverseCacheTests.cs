using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Exceptions;
using Xbim.Ifc;
using Xbim.Ifc2x3.Kernel;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class InverseCacheTests
    {
        [TestMethod]
        [DeploymentItem("TestSourceFiles\\4walls1floorSite.ifc")]
        public void CacheCreation()
        {
            //as a MemoryModel
            using (var model = IfcStore.Open(@"4walls1floorSite.ifc"))
            {
               CacheCreation(model);
            }

            //as EsentModel
            using (var model = IfcStore.Open(@"4walls1floorSite.ifc", null, 0.00001))
            {
                CacheCreation(model);
            }
        }

        private static void CacheCreation(IModel model)
        {
            using (model.BeginTransaction("Test"))
            {
                Assert.IsTrue(ThrowsXbimException(() => model.BeginCaching()));
            }

            using (var c = model.BeginCaching())
            {
                Assert.IsTrue(ThrowsXbimException(() => model.BeginTransaction("Exception")));

                var products = model.Instances.OfType<IfcProduct>();
                foreach (var product in products)
                {
                    IEnumerable<IfcRelDefines> relations;
                    var isDefined = product.IsDefinedBy.ToList();
                    if(isDefined.Any())
                        Assert.IsTrue(c.TryGet("RelatedObjects", product, out relations));
                }
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

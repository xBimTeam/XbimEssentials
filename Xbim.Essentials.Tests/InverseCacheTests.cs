using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            using (var model = IfcStore.Open(@"4walls1floorSite.ifc"))
            {
                using (model.BeginTransaction())
                {
                    Assert.IsTrue(ThrowsXbimException(() => model.BeginCaching()));
                }

                using (var c = model.BeginCaching())
                {
                    Assert.IsTrue(ThrowsXbimException(() => model.BeginTransaction()));

                    var products = model.Instances.OfType<IfcProduct>();
                    foreach (var product in products)
                    {
                        IEnumerable<IfcRelDefines> relations;
                        var isDefined = product.IsDefinedBy.ToList();
                        Assert.IsTrue(c.TryGet("RelatedObjects", product, out relations));
                    }
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

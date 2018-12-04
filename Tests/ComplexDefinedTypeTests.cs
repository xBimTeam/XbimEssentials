using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class ComplexDefinedTypeTests
    {
        [TestMethod]
        public void CreateSerializeAndDeserialize()
        {
            var lat = new List<long> {45, 12, 79};
            var lon = new List<long> {2, 23, 80};

            var model = new MemoryModel(new EntityFactoryIfc2x3());
            using (var txn = model.BeginTransaction("Site creation"))
            {
                var site = model.Instances.New<IfcSite>();
                site.RefLatitude = lat;
                site.RefLongitude = lon;
                txn.Commit();
            }
            using (var fileStream = new StreamWriter("site.ifc"))
            {
                 model.SaveAsStep21(fileStream);
            }
           
            
            model = new MemoryModel(new EntityFactoryIfc2x3());
            model.LoadStep21("site.ifc");
            var site2 = model.Instances.FirstOrDefault<IfcSite>();
            Assert.IsTrue(lat == site2.RefLatitude);
            Assert.AreEqual((IfcCompoundPlaneAngleMeasure)lon, site2.RefLongitude);

        }
    }
}

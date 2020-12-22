using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Model;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4x3.MeasureResource;
using Xbim.Ifc4x3.ProductExtension;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class Ifc4x3Tests
    {
        [TestMethod]
        public void CreateSimpleIfc4x3File()
        {
            using (var model = new StepModel(new Ifc4x3.EntityFactoryIfc4x3Rc2()))
            {
                var i = model.Instances;
                using (var txn = model.BeginTransaction("Sample creation"))
                {
                    var facility = i.New<IfcFacility>();

                    txn.Commit();
                }
                using (var file = File.Create("ifc4x3sample.ifc"))
                {
                    model.SaveAsIfc(file);
                }
            }
        }

        [TestMethod]
        public void ConvertLengthTest()
        {
            var value = new IfcLengthMeasure(20);
            var converted = value.ToIfc4();
            Assert.IsTrue(converted.GetType() == typeof(Ifc4.MeasureResource.IfcLengthMeasure));
            Assert.IsTrue((Ifc4.MeasureResource.IfcLengthMeasure)converted == new Ifc4.MeasureResource.IfcLengthMeasure(20));
        }
    }
}

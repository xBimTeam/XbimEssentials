using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Model;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4x3;
using Xbim.Ifc4x3.MeasureResource;
using Xbim.Ifc4x3.ProductExtension;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class Ifc4x3Tests
    {
        [TestMethod]
        public void Entity_types_should_be_unique()
        {
            var unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var types = typeof(Ifc4x3.EntityFactoryIfc4x3Add1).Assembly.GetTypes().Where(t => typeof(IPersist).IsAssignableFrom(t)).Select(t => t.Name);
            var duplicates = types.Where(t => !unique.Add(t)).ToList();

            Assert.AreEqual(0, duplicates.Count, $"Duplicated types: {string.Join(", ", duplicates)}");
        }

        [TestMethod]
        public void CreateSimpleIfc4x3File()
        {
            using (var model = new StepModel(new Ifc4x3.EntityFactoryIfc4x3Add1()))
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

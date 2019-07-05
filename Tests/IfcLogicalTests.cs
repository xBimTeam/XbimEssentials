using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc.Validation;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PresentationOrganizationResource;
using Xbim.Ifc4.PropertyResource;

namespace Xbim.IO.Tests
{
    [TestClass]
    public class IfcLogicalTests
    {
        private const string logicalUnknown = "Logical .U.";
        private const string logicalTrue = "Logical .T.";
        private const string logicalFalse = "Logical .F.";

        [TestCategory("IfcXml")]
        [TestMethod]
        public void RoundTrip()
        {
            var tempStep = Guid.NewGuid() + ".ifc";
            var tempXml = Path.ChangeExtension(tempStep, ".ifcxml");

            using (var store = IfcStore.Create(XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel))
            {
                using (var txn = store.BeginTransaction("Create test data"))
                {
                    store.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = logicalUnknown;
                        p.NominalValue = new IfcLogical();
                    });
                    store.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = logicalTrue;
                        p.NominalValue = new IfcLogical(true);
                    });
                    store.Instances.New<IfcPropertySingleValue>(p =>
                    {
                        p.Name = logicalFalse;
                        p.NominalValue = new IfcLogical(false);
                    });
                    txn.Commit();
                }
                store.SaveAs(tempStep);
                store.SaveAs(tempXml);
            }

            using (var store = IfcStore.Open(tempStep, null, -1))
            {
                CheckValues(store);
            }

            using (var store = IfcStore.Open(tempXml, null, -1))
            {
                CheckValues(store);
            }

            // delete temp files
            File.Delete(tempStep);
            File.Delete(tempXml);
        }

        private void CheckValues(IModel model)
        {
            var prop = model.Instances.FirstOrDefault<IIfcPropertySingleValue>(p => p.Name == logicalUnknown);
            Assert.IsNotNull(prop);

            var value = (IfcLogical)prop.NominalValue;
            Assert.IsTrue(value.Value == null);

            prop = model.Instances.FirstOrDefault<IIfcPropertySingleValue>(p => p.Name == logicalTrue);
            Assert.IsNotNull(prop);

            value = (IfcLogical)prop.NominalValue;
            Assert.IsTrue((bool?)value.Value == true);

            prop = model.Instances.FirstOrDefault<IIfcPropertySingleValue>(p => p.Name == logicalFalse);
            Assert.IsNotNull(prop);

            value = (IfcLogical)prop.NominalValue;
            Assert.IsTrue((bool?)value.Value == false);

        }

        [TestMethod]
        public void ValidationOfUnknown()
        {
            using (var store = IfcStore.Create(XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel))
            {
                using (var txn = store.BeginTransaction("Create test data"))
                {
                    var layer = store.Instances.New<IfcPresentationLayerWithStyle>(l =>
                    {
                        l.Name = "Name";
                        l.AssignedItems.Add(store.Instances.New<IfcCartesianPoint>(p => p.Coordinates.AddRange(new IfcLengthMeasure[] { 0, 0, 0 })));
                        l.LayerFrozen = new IfcLogical();
                        l.LayerOn = new IfcLogical();
                        l.LayerBlocked = new IfcLogical();
                    });
                    txn.Commit();

                    // test validation
                    var v = new Validator
                    {
                        ValidateLevel = ValidationFlags.Properties,
                        CreateEntityHierarchy = true
                    };
                    var result = v.Validate(store.Instances);
                    Assert.IsFalse(result.Any());
                }
            }
        }
    }
}

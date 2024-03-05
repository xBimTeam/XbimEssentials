using System;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Enumerations;
using Xbim.Common.ExpressValidation;
using Xbim.Ifc;
using Xbim.Ifc.Validation;
using Xbim.Ifc2x3.HVACDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class ValidationTests
    {
        [TestMethod]
        public void ValidatesFile()
        {
            const string file = "TestSourceFiles\\InvalidContentFC4.ifc";
            using (var model = IfcStore.Open(file, null, 0))
            {
                //var v2 = model.Metadata.Types()
                //    .Where(x => x.Properties.Any(pr => pr.Value.EntityAttribute.State == EntityAttributeState.Mandatory
                //    && typeof(IExpressValueType).IsAssignableFrom(pr.Value.PropertyInfo.PropertyType)
                //    ));
                var v = new Validator
                {
                    ValidateLevel = ValidationFlags.All,
                    CreateEntityHierarchy = true
                };
                var errors = v.Validate(model);
                foreach (var validationResult in new IfcValidationReporter(errors))
                {
                    Debug.WriteLine(validationResult);
                }
            }
        }

        [TestMethod]
        public void ContextDependentUnitValidationTest()
        {
            using (var model = IfcStore.Create(Common.Step21.XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel))
            {
                using (var txn = model.BeginTransaction())
                {
                    var c = new Create(model);
                    var unit = c.ContextDependentUnit(u =>
                    {
                        u.UnitType = IfcUnitEnum.USERDEFINED;
                        u.Name = "Context Dependent Unit";
                        u.Dimensions = c.DimensionalExponents(e =>
                        {
                            e.LengthExponent = 0;
                            e.MassExponent = 0;
                            e.TimeExponent = 0;
                            e.ElectricCurrentExponent = 0;
                            e.ThermodynamicTemperatureExponent = 0;
                            e.AmountOfSubstanceExponent = 0;
                            e.LuminousIntensityExponent = 0;
                        });
                    });
                    txn.Commit();
                }

                var v = new Validator
                {
                    ValidateLevel = ValidationFlags.All,
                    CreateEntityHierarchy = true
                };
                var errors = v.Validate(model);
                Assert.IsTrue(!errors.Any());

            }
        }

        [TestMethod]
        public void TableValueCheck()
        {
            var path = @"TestSourceFiles\\properties.ifc";
            using (var store = Xbim.Ifc.IfcStore.Open(path))
            {
                var v = new Validator
                {
                    ValidateLevel = ValidationFlags.All,
                    CreateEntityHierarchy = true
                };
                var errs = v.Validate(store);
                Assert.IsTrue(!errs.Any());
            }
        }

        [TestMethod]
        public void ValidatesEntity()
        {
            using (var model = IfcStore.Open("TestSourceFiles\\InvalidContentFC4.ifc", null, 0))
            {
                //var v2 = model.Metadata.Types()
                //    .Where(x => x.Properties.Any(pr => pr.Value.EntityAttribute.State == EntityAttributeState.Mandatory
                //    && typeof(IExpressValueType).IsAssignableFrom(pr.Value.PropertyInfo.PropertyType)
                //    ));
                var v = new Validator
                {
                    ValidateLevel = ValidationFlags.All,
                    CreateEntityHierarchy = true
                };
                var e2 = v.Validate(model.Instances[202]);
                foreach (var validationResult in new IfcValidationReporter(e2))
                {
                    Debug.WriteLine(validationResult);
                }
            }
        }

        [TestMethod]
        public void ValidatesBlenderBIM()
        {
            using (var model = IfcStore.Open("TestSourceFiles\\IfcOpenShell-Issue2499.ifc", null, 0))
            {

                var v = new Validator
                {
                    ValidateLevel = ValidationFlags.All,
                    CreateEntityHierarchy = true
                };
                var result = v.Validate(model.Instances);

                result.Should().HaveCountGreaterThan(0);
                foreach (var validationResult in new IfcValidationReporter(result))
                {
                    Debug.WriteLine(validationResult);
                }
            }
        }


        [TestMethod]
        public void ValidatesSpecificElements2X4()
        {
            using (var model = IfcStore.Open("TestSourceFiles\\AlmostEmptyIFC4.ifc", null, 0))
            {
                var v = new Validator
                {
                    ValidateLevel = ValidationFlags.All,
                    CreateEntityHierarchy = true
                };

                var iCount = 0;
                var e2 = v.Validate(model.Instances[9]);
                foreach (var validationResult in new IfcValidationReporter(e2))
                {
                    Debug.WriteLine(validationResult);
                    iCount++;
                }

                if (iCount != 0)
                    throw new Exception("Unexpected validation error.");
            }
        }

        [TestMethod]
        public void ValidatesSpecificElements2X3()
        {
            using (var model = IfcStore.Open("TestSourceFiles\\ValidationTests2x3.ifc", null, 0))
            {
                var v = new Validator
                {
                    ValidateLevel = ValidationFlags.All,
                    CreateEntityHierarchy = true
                };
                var item = model.Instances[100] as IfcCoilType;
                if (item == null)
                    throw new Exception();

                var e2 = v.Validate(item);
                int iCount = 0;
                foreach (var validationResult in new IfcValidationReporter(e2))
                {
                    Debug.WriteLine(validationResult);
                    iCount++;
                }

                e2 = v.Validate(model.Instances[9]);
                foreach (var validationResult in new IfcValidationReporter(e2))
                {
                    Debug.WriteLine(validationResult);
                    iCount++;
                }

                if (iCount != 0)
                    throw new Exception("Unexpected validation error.");

                //bool res;
                //Debug.WriteLine("fcCoilTypeClause.WR1" + item.ValidateClause(IfcCoilType.IfcCoilTypeClause.WR1));
                //Debug.WriteLine("fcCoilTypeClause.WR1" + item.ValidateClause(IfcTypeObject.IfcTypeObjectClause.WR1));
                //Debug.WriteLine("fcCoilTypeClause.WR1" + item.ValidateClause(IfcTypeProduct.IfcTypeProductClause.WR41));
            }
        }
    }
}
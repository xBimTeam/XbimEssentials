using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class UnitNameTest
    {
        [TestMethod]
        public void NamedUnitTest2X3()
        {
            using (var model = new MemoryModel(new Ifc2x3.EntityFactoryIfc2x3()))
            {
                Ifc2x3.MeasureResource.IfcSIUnit unit;
                using (var txn = model.BeginTransaction(""))
                {
                    unit = model.Instances.New<Ifc2x3.MeasureResource.IfcSIUnit>(u =>
                    {
                        u.Name = Ifc2x3.MeasureResource.IfcSIUnitName.METRE;
                        u.Prefix = Ifc2x3.MeasureResource.IfcSIPrefix.MILLI;
                    });
                    txn.Commit();
                }
                Assert.IsNotNull(unit);
                Assert.IsNotNull(unit.Dimensions);
                Assert.IsTrue(unit.Dimensions.LengthExponent == 1);
                Assert.IsTrue(unit.Dimensions.AmountOfSubstanceExponent == 0);
                Assert.IsTrue(unit.Dimensions.ElectricCurrentExponent == 0);
                Assert.IsTrue(unit.Dimensions.LuminousIntensityExponent == 0);
                Assert.IsTrue(unit.Dimensions.MassExponent == 0);
                Assert.IsTrue(unit.Dimensions.TimeExponent == 0);
                Assert.IsTrue(unit.Dimensions.ThermodynamicTemperatureExponent == 0);

                using (model.BeginTransaction(""))
                {
                    try
                    {
                        //this should throw an exception because dimensions are derived attribute and it is read-only.
                        unit.Dimensions.LengthExponent = 2;
                        Assert.IsTrue(false);
                    }
                    catch (Exception)
                    {
                        Assert.IsTrue(true);
                    }    
                }
            }
        }

        [TestMethod]
        public void NamedUnitTest4()
        {
            using (var model = new MemoryModel(new Ifc4.EntityFactoryIfc4()))
            {
                Ifc4.MeasureResource.IfcSIUnit unit;
                using (var txn = model.BeginTransaction(""))
                {
                    unit = model.Instances.New<Ifc4.MeasureResource.IfcSIUnit>(u =>
                    {
                        u.Name = Ifc4.Interfaces.IfcSIUnitName.METRE;
                        u.Prefix = Ifc4.Interfaces.IfcSIPrefix.MILLI;
                    });
                    txn.Commit();
                }
                Assert.IsNotNull(unit);
                Assert.IsNotNull(unit.Dimensions);
                Assert.IsTrue(unit.Dimensions.LengthExponent == 1);
                Assert.IsTrue(unit.Dimensions.AmountOfSubstanceExponent == 0);
                Assert.IsTrue(unit.Dimensions.ElectricCurrentExponent == 0);
                Assert.IsTrue(unit.Dimensions.LuminousIntensityExponent == 0);
                Assert.IsTrue(unit.Dimensions.MassExponent == 0);
                Assert.IsTrue(unit.Dimensions.TimeExponent == 0);
                Assert.IsTrue(unit.Dimensions.ThermodynamicTemperatureExponent == 0);

                using (model.BeginTransaction(""))
                {
                    try
                    {
                        //this should throw an exception because dimensions are derived attribute and it is read-only.
                        unit.Dimensions.LengthExponent = 2;
                        Assert.IsTrue(false);
                    }
                    catch (Exception)
                    {
                        Assert.IsTrue(true);
                    }
                }
            }
        }
    }
}

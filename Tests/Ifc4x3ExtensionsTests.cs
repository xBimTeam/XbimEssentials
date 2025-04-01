using FluentAssertions;
using System;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.IO.Memory;
using Xunit;
using XbimCommonSchema = Xbim.Ifc4;


namespace Xbim.Essentials.Tests
{
    public class Ifc4x3ExtensionsTests : IDisposable
    {
        protected IModel Model { get; set; }
        protected ITransaction Txn { get; set; }

        public Ifc4x3ExtensionsTests()
        {
            Model = new MemoryModel(new Ifc4x3.EntityFactoryIfc4x3Add2());
            Txn = Model.BeginTransaction("Test");
        }

        [Fact]
        public void CanAddDefiningType()
        {
            var pile = Model.Instances.New<Ifc4x3.StructuralElementsDomain.IfcPile>();
            var type = Model.Instances.New<Ifc4x3.StructuralElementsDomain.IfcPileType>();
            pile.AddDefiningType(type);

            var relatingType = pile.IsTypedBy.First().RelatingType;
            relatingType.Should().NotBeNull();
            relatingType.Should().Be(type);
        }

        [Fact]
        public void CanAddPropertySet()
        {
            var pile = Model.Instances.New<Ifc4x3.StructuralElementsDomain.IfcPile>();
            var pset = Model.Instances.New<Ifc4x3.Kernel.IfcPropertySet>(p=>
            {
                p.Name = "Pset_Test";
            });
            pile.AddPropertySet(pset);

            var pset2 = pile.PropertySets.First();
            pset2.Should().NotBeNull();
            pset2.Should().Be(pset);
        }

        [Fact]
        public void CanGetPropertySet()
        {
            var pile = Model.Instances.New<Ifc4x3.StructuralElementsDomain.IfcPile>();
            var pset = Model.Instances.New<Ifc4x3.Kernel.IfcPropertySet>(p => p.Name = "Pset_Test");
            pile.AddPropertySet(pset);

            var pset2 = pile.GetPropertySet("Pset_Test");
            pset2.Should().NotBeNull();
            pset2.Should().Be(pset);
        }

        [Fact]
        public void GetPropertySetIsCaseSensitive()
        {
            var pile = Model.Instances.New<Xbim.Ifc4x3.StructuralElementsDomain.IfcPile>();
            var pset = Model.Instances.New<Ifc4x3.Kernel.IfcPropertySet>(p => p.Name = "Pset_Test");
            pile.AddPropertySet(pset);

            var pset2 = pile.GetPropertySet("pset_test");
            pset2.Should().BeNull();
        }

        [Fact]
        public void GetPropertySetCanBeCaseInsensitive()
        {
            var pile = Model.Instances.New<Ifc4x3.StructuralElementsDomain.IfcPile>();
            var pset = Model.Instances.New<Ifc4x3.Kernel.IfcPropertySet>(p => p.Name = "Pset_Test");
            pile.AddPropertySet(pset);

            var pset2 = pile.GetPropertySet("pset_test", false);

            pset2.Should().NotBeNull();
            pset2.Should().Be(pset);
        }

        [Fact]
        public void CanGetPropertySingle()
        {
            var pile = Model.Instances.New<Ifc4x3.StructuralElementsDomain.IfcPile>();
            var prop = Model.Instances.New<Ifc4x3.PropertyResource.IfcPropertySingleValue>(p=>
            {
                p.Name = "SomeValue";
                p.NominalValue = new Ifc4x3.MeasureResource.IfcLabel("Abc");
            });
            var pset = Model.Instances.New<Ifc4x3.Kernel.IfcPropertySet>(p =>
            {
                p.Name = "Pset_Test";
                p.HasProperties.Add(prop);
            });
            pile.AddPropertySet(pset);

            var value = pile.GetPropertySingleValue("Pset_Test", "SomeValue");
            value.Should().NotBeNull();
            value.NominalValue.Value.Should().Be("Abc");
        }

        [Fact]
        public void CanGetPropertySingleValue()
        {
            var pile = Model.Instances.New<Ifc4x3.StructuralElementsDomain.IfcPile>();
            var prop = Model.Instances.New<Ifc4x3.PropertyResource.IfcPropertySingleValue>(p =>
            {
                p.Name = "SomeNumber";
                p.NominalValue = new Ifc4x3.MeasureResource.IfcReal(100.5d);
            });
            var pset = Model.Instances.New<Ifc4x3.Kernel.IfcPropertySet>(p =>
            {
                p.Name = "Pset_Test";
                p.HasProperties.Add(prop);
            });
            pile.AddPropertySet(pset);

            var value = pile.GetPropertySingleValue<XbimCommonSchema.MeasureResource.IfcReal>("Pset_Test", "SomeNumber");
            value.Should().NotBeNull();
            value.Value.Should().Be(100.5);
        }

        [Fact]
        public void CanSetPropertySingleValue()
        {
            var pile = Model.Instances.New<Ifc4x3.StructuralElementsDomain.IfcPile>();

            pile.SetPropertySingleValue("Pset_Test", "SomeProp", typeof(XbimCommonSchema.MeasureResource.IfcReal));

            var value = pile.GetPropertySingleValue<XbimCommonSchema.MeasureResource.IfcReal>("Pset_Test", "SomeProp");
            value.Should().NotBeNull();
            value.Value.Should().Be(0);
        }


        [Fact]
        public void CanSetPropertySingleValueGeneric()
        {
            var pile = Model.Instances.New<Ifc4x3.StructuralElementsDomain.IfcPile>();

            pile.SetPropertySingleValue<XbimCommonSchema.MeasureResource.IfcReal>("Pset_Test", "SomeProp");

            var value = pile.GetPropertySingleValue<XbimCommonSchema.MeasureResource.IfcReal>("Pset_Test", "SomeProp");
            value.Should().NotBeNull();
            value.Value.Should().Be(0);
        }

        [Fact]
        public void CanSetPropertySingleValueGenericEdgeCase()
        {
            var pile = Model.Instances.New<Ifc4x3.StructuralElementsDomain.IfcPile>();

            pile.SetPropertySingleValue<XbimCommonSchema.MeasureResource.IfcPositiveLengthMeasure>("Pset_Test", "SomeProp");

            var value = pile.GetPropertySingleValue<XbimCommonSchema.MeasureResource.IfcPositiveLengthMeasure>("Pset_Test", "SomeProp");
            value.Should().NotBeNull();
            value.Value.Should().Be(1.0);
        }

        [Fact]
        public void CanAddElementAndReadQuantity()
        {
            var pile = Model.Instances.New<Ifc4x3.StructuralElementsDomain.IfcPile>();
            var quant = Model.Instances.New<Ifc4x3.QuantityResource.IfcQuantityArea>(qa =>
            {
                qa.Name = "GrossArea";
                qa.AreaValue = 123.4d;
            });
            pile.AddQuantity("BaseQuants", quant, "Some measure");

            var quantities = pile.GetElementQuantity("BaseQuants");
            quantities.Should().NotBeNull();
            var val = quantities.Quantities.First(q => q.Name == "GrossArea");

            val.Should().BeOfType<Ifc4x3.QuantityResource.IfcQuantityArea>();
            (val as Ifc4x3.QuantityResource.IfcQuantityArea).AreaValue.Value.Should().Be(123.4d);
            
        }

        [Fact]
        public void CanAddElementAndReadPhysicalQuantity()
        {
            var pile = Model.Instances.New<Ifc4x3.StructuralElementsDomain.IfcPile>();
            var quant = Model.Instances.New<Ifc4x3.QuantityResource.IfcQuantityArea>(qa =>
            {
                qa.Name = "GrossArea";
                qa.AreaValue = 123.4d;
            });
            pile.AddQuantity("BaseQuants", quant, "Some measure");

            var val = pile.GetQuantity<Ifc4x3.QuantityResource.IfcQuantityArea>("GrossArea");
            val.Should().NotBeNull();
            val.AreaValue.Value.Should().Be(123.4d);
        }

        [Fact]
        public void CanAddElementAndReadPhysicalQuantityWithPset()
        {
            var pile = Model.Instances.New<Ifc4x3.StructuralElementsDomain.IfcPile>();
            var quant = Model.Instances.New<Ifc4x3.QuantityResource.IfcQuantityArea>(qa =>
            {
                qa.Name = "GrossArea";
                qa.AreaValue = 123.4d;
            });
            pile.AddQuantity("BaseQuants", quant, "Some measure");

            var val = pile.GetQuantity<Ifc4x3.QuantityResource.IfcQuantityArea>("BaseQuants", "GrossArea");
            val.Should().NotBeNull();
            val.AreaValue.Value.Should().Be(123.4d);
        }

        [Fact]
        public void CanAddElementAndReadPhysicalSimpleQuantityWithPset()
        {
            var pile = Model.Instances.New<Ifc4x3.StructuralElementsDomain.IfcPile>();
            var quant = Model.Instances.New<Ifc4x3.QuantityResource.IfcQuantityArea>(qa =>
            {
                qa.Name = "GrossArea";
                qa.AreaValue = 123.4d;
            });
            pile.AddQuantity("BaseQuants", quant, "Some measure");

            var val = pile.GetElementPhysicalSimpleQuantity("BaseQuants", "GrossArea");
            val.Should().NotBeNull();
            val.Should().BeOfType<Ifc4x3.QuantityResource.IfcQuantityArea>();
        }

        [InlineData("GFA", XbimQuantityTypeEnum.Area, 15.5d, Ifc4x3.MeasureResource.IfcSIUnitName.SQUARE_METRE)]
        [InlineData("Width", XbimQuantityTypeEnum.Length, 1200.4d, Ifc4x3.MeasureResource.IfcSIUnitName.METRE)]
        [InlineData("Volume", XbimQuantityTypeEnum.Volume, 12234d, Ifc4x3.MeasureResource.IfcSIUnitName.CUBIC_METRE)]
        [InlineData("Count", XbimQuantityTypeEnum.Count, 14d, null)]
        [InlineData("Weight", XbimQuantityTypeEnum.Weight, 999, Ifc4x3.MeasureResource.IfcSIUnitName.GRAM, Ifc4x3.MeasureResource.IfcSIPrefix.KILO)]
        [InlineData("Duration", XbimQuantityTypeEnum.Time, 10d, Ifc4x3.MeasureResource.IfcSIUnitName.SECOND)]
        [Theory]
        public void CanSetElementPhysicalSimpleQuantity(string quantName, XbimQuantityTypeEnum measure, double value, 
            Ifc4x3.MeasureResource.IfcSIUnitName? unitType, Ifc4x3.MeasureResource.IfcSIPrefix? unitPrefix = null)
        {
            var space = Model.Instances.New<Ifc4x3.ProductExtension.IfcSpace>();
            Ifc4x3.MeasureResource.IfcNamedUnit unit = null;
            if(unitType != null)
            {
                unit = Model.Instances.New<Ifc4x3.MeasureResource.IfcSIUnit>(u =>
                {
                    u.Name = unitType.Value;
                    u.Prefix = unitPrefix;
                });
            }

            space.SetElementPhysicalSimpleQuantity("BaseQuants", quantName, value, measure, unit);

            Ifc4x3.MeasureResource.IfcMeasureValue val = measure switch
            {
                XbimQuantityTypeEnum.Area => space.GetQuantity<Ifc4x3.QuantityResource.IfcQuantityArea>("BaseQuants", quantName).AreaValue,
                XbimQuantityTypeEnum.Length => space.GetQuantity<Ifc4x3.QuantityResource.IfcQuantityLength>("BaseQuants", quantName).LengthValue,
                XbimQuantityTypeEnum.Volume => space.GetQuantity<Ifc4x3.QuantityResource.IfcQuantityVolume>("BaseQuants", quantName).VolumeValue,
                XbimQuantityTypeEnum.Count => space.GetQuantity<Ifc4x3.QuantityResource.IfcQuantityCount>("BaseQuants", quantName).CountValue,
                XbimQuantityTypeEnum.Weight => space.GetQuantity<Ifc4x3.QuantityResource.IfcQuantityWeight>("BaseQuants", quantName).WeightValue,
                XbimQuantityTypeEnum.Time => space.GetQuantity<Ifc4x3.QuantityResource.IfcQuantityTime>("BaseQuants", quantName).TimeValue,
                _ => throw new NotImplementedException(),
            };

            val.Value.Should().Be(value);
            //area.Unit.FullName.Should().Be("SQUAREMETRE");
            
        }

        [InlineData("USD", "$", "US Dollar")]
        [InlineData("GBP", "£", "British Pound", "Pound Sterling")]
        [InlineData("EUR", "€", "Euro", "euro")]    // 'euro' since French culture picked up by default
        [InlineData("CAD", "$", "Canadian Dollar")]
        [InlineData("AUD", "$", "Australian Dollar")]
        [InlineData("PLN", "zł", "Polish Zloty", "złoty polski")]
        [Theory]
        public void CanReadMonetaryUnit(string tla, string expectedSymbol, string expectedName, string nativeName = null)
        {
            nativeName = nativeName ?? expectedName;
            var currency = Model.Instances.New<Ifc4x3.MeasureResource.IfcMonetaryUnit>(u =>
            {
                u.Currency = tla;
            });

            currency.FullName.Should().Be(expectedName);
            currency.Symbol().Should().Be(expectedSymbol);
            currency.FullEnglishName().Should().Be(expectedName);
            currency.FullNativeName().Should().Be(nativeName);
        }

        [Fact]
        public void CanGetUnitForProperty()
        {
            XbimCommonSchema.Interfaces.IIfcProject project = Model.Instances.New<Ifc4x3.Kernel.IfcProject>();
            project.Initialize(ProjectUnits.SIUnitsUK);

            var prop = Model.Instances.New<Ifc4x3.PropertyResource.IfcPropertySingleValue>(p =>
            {
                p.Name = "Volume";
                p.NominalValue = new Ifc4x3.MeasureResource.IfcVolumeMeasure(123d);
            });

            XbimCommonSchema.Interfaces.IIfcNamedUnit unit = project.UnitsInContext.GetUnitFor(prop);

            unit.Should().NotBeNull();
            unit.FullName.Should().Be("CUBICMETRE");
        }

        [Fact]
        public void CanGetUnitForPropertyWhenOverridden()
        {
            XbimCommonSchema.Interfaces.IIfcProject project = Model.Instances.New<Ifc4x3.Kernel.IfcProject>();
            project.Initialize(ProjectUnits.SIUnitsUK);
            var modelunit = project.UnitsInContext.Units.OfType<Ifc4x3.MeasureResource.IfcSIUnit>().FirstOrDefault(u => u.UnitType == Ifc4x3.MeasureResource.IfcUnitEnum.LENGTHUNIT);

            var prop = Model.Instances.New<Ifc4x3.PropertyResource.IfcPropertySingleValue>(p =>
            {
                p.Name = "Length";
                p.NominalValue = new Ifc4x3.MeasureResource.IfcReal(123d);
                p.Unit = modelunit;
            });

            XbimCommonSchema.Interfaces.IIfcNamedUnit unit = project.UnitsInContext.GetUnitFor(prop);

            unit.Should().NotBeNull();
            unit.FullName.Should().Be("MILLIMETRE");
        }

        [Fact]
        public void CanGetUnitForQuantity()
        {
            XbimCommonSchema.Interfaces.IIfcProject project = Model.Instances.New<Ifc4x3.Kernel.IfcProject>();
            project.Initialize(ProjectUnits.SIUnitsUK);

            var quant = Model.Instances.New<Ifc4x3.QuantityResource.IfcQuantityTime>(qa =>
            {
                qa.Name = "GrossArea";
                qa.TimeValue = 1.5;
            });

            XbimCommonSchema.Interfaces.IIfcNamedUnit unit = project.UnitsInContext.GetUnitFor(quant);
            unit.Should().NotBeNull();
            unit.FullName.Should().Be("SECOND");
        }

        [InlineData(XbimCommonSchema.Interfaces.IfcUnitEnum.LENGTHUNIT, "MILLIMETRE")]
        [InlineData(XbimCommonSchema.Interfaces.IfcUnitEnum.AREAUNIT, "SQUAREMETRE")]
        [InlineData(XbimCommonSchema.Interfaces.IfcUnitEnum.VOLUMEUNIT, "CUBICMETRE")]
        [InlineData(XbimCommonSchema.Interfaces.IfcUnitEnum.MASSUNIT, "GRAM")]  // Inconsistent - is kg in Ifc2x3/4
        [InlineData(XbimCommonSchema.Interfaces.IfcUnitEnum.THERMODYNAMICTEMPERATUREUNIT, "DEGREECELSIUS")]
        [Theory]
        public void CanGetUnits(XbimCommonSchema.Interfaces.IfcUnitEnum unitType, string expectedUnitName)
        {
            XbimCommonSchema.Interfaces.IIfcProject project = Model.Instances.New<Ifc4x3.Kernel.IfcProject>();
            project.Initialize(ProjectUnits.SIUnitsUK);

            XbimCommonSchema.Interfaces.IIfcNamedUnit result = project.UnitsInContext.GetUnitFor(unitType);
            result.Should().NotBeNull();
            result.UnitType.Should().Be(unitType);
            result.Name().Should().Be(expectedUnitName);
        }

        [Fact]
        public void CanChangeUnits()
        {
            XbimCommonSchema.Interfaces.IIfcProject project = Model.Instances.New<Ifc4x3.Kernel.IfcProject>();
            project.Initialize(ProjectUnits.SIUnitsUK);

            project.UnitsInContext.SetOrChangeConversionUnit(XbimCommonSchema.Interfaces.IfcUnitEnum.LENGTHUNIT, ConversionBasedUnit.Inch);

            var unit = project.UnitsInContext.GetUnitFor(XbimCommonSchema.Interfaces.IfcUnitEnum.LENGTHUNIT);

            unit.Should().NotBeNull();
            unit.Name().Should().Be("inch");
            unit.Should().BeOfType<Ifc4x3.MeasureResource.IfcConversionBasedUnit>();

            var cbt = (Ifc4x3.MeasureResource.IfcConversionBasedUnit)unit;
            cbt.ConversionFactor.ValueComponent.Value.Should().Be(25.4d);
            cbt.ConversionFactor.UnitComponent.FullName.Should().Be("MILLIMETRE");

        }

        #region Dispose
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Txn.Dispose();
                    Model?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

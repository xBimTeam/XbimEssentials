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
    public class Ifc4ExtensionsTests : IDisposable
    {
        protected IModel Model { get; set; }
        protected ITransaction Txn { get; set; }

        public Ifc4ExtensionsTests()
        {
            Model = new MemoryModel(new Ifc4.EntityFactoryIfc4());
            Txn = Model.BeginTransaction("Test");
        }

        [Fact]
        public void CanAddDefiningType()
        {
            var pile = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPile>();
            var type = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPileType>();
            pile.AddDefiningType(type);

            var relatingType = pile.IsTypedBy.First().RelatingType;
            relatingType.Should().NotBeNull();
            relatingType.Should().Be(type);
        }

        [Fact]
        public void CanAddPropertySet()
        {
            var pile = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPile>();
            var pset = Model.Instances.New<Ifc4.Kernel.IfcPropertySet>(p=>
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
            var pile = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPile>();
            var pset = Model.Instances.New<Ifc4.Kernel.IfcPropertySet>(p => p.Name = "Pset_Test");
            pile.AddPropertySet(pset);

            var pset2 = pile.GetPropertySet("Pset_Test");
            pset2.Should().NotBeNull();
            pset2.Should().Be(pset);
        }

        [Fact]
        public void GetPropertySetIsCaseSensitive()
        {
            var pile = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPile>();
            var pset = Model.Instances.New<Ifc4.Kernel.IfcPropertySet>(p => p.Name = "Pset_Test");
            pile.AddPropertySet(pset);

            var pset2 = pile.GetPropertySet("pset_test");
            pset2.Should().BeNull();
        }

        [Fact]
        public void GetPropertySetCanBeCaseInsensitive()
        {
            var pile = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPile>();
            var pset = Model.Instances.New<Ifc4.Kernel.IfcPropertySet>(p => p.Name = "Pset_Test");
            pile.AddPropertySet(pset);

            var pset2 = pile.GetPropertySet("pset_test", false);

            pset2.Should().NotBeNull();
            pset2.Should().Be(pset);
        }

        [Fact]
        public void CanGetPropertySingle()
        {
            var pile = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPile>();
            var prop = Model.Instances.New<Ifc4.PropertyResource.IfcPropertySingleValue>(p=>
            {
                p.Name = "SomeValue";
                p.NominalValue = new Ifc4.MeasureResource.IfcLabel("Abc");
            });
            var pset = Model.Instances.New<Ifc4.Kernel.IfcPropertySet>(p =>
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
            var pile = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPile>();
            var prop = Model.Instances.New<Ifc4.PropertyResource.IfcPropertySingleValue>(p =>
            {
                p.Name = "SomeNumber";
                p.NominalValue = new Ifc4.MeasureResource.IfcReal(100.5d);
            });
            var pset = Model.Instances.New<Ifc4.Kernel.IfcPropertySet>(p =>
            {
                p.Name = "Pset_Test";
                p.HasProperties.Add(prop);
            });
            pile.AddPropertySet(pset);
            var x= pile.GetPropertySingleValue<XbimCommonSchema.MeasureResource.IfcReal >("", "");
            var value = pile.GetPropertySingleValue<XbimCommonSchema.MeasureResource.IfcReal>("Pset_Test", "SomeNumber");
            value.Should().NotBeNull();
            value.Value.Should().Be(100.5);
        }

        [Fact]
        public void CanSetPropertySingleValue()
        {
            var pile = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPile>();

            pile.SetPropertySingleValue("Pset_Test", "SomeProp", typeof(XbimCommonSchema.MeasureResource.IfcReal));

            var value = pile.GetPropertySingleValue<XbimCommonSchema.MeasureResource.IfcReal>("Pset_Test", "SomeProp");
            value.Should().NotBeNull();
            value.Value.Should().Be(0);
        }


        [Fact]
        public void CanSetPropertySingleValueGeneric()
        {
            var pile = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPile>();

            pile.SetPropertySingleValue<XbimCommonSchema.MeasureResource.IfcReal>("Pset_Test", "SomeProp");

            var value = pile.GetPropertySingleValue<XbimCommonSchema.MeasureResource.IfcReal>("Pset_Test", "SomeProp");
            value.Should().NotBeNull();
            value.Value.Should().Be(0);
        }

        [Fact]
        public void CanSetPropertySingleValueGenericEdgeCase()
        {
            var pile = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPile>();

            pile.SetPropertySingleValue<XbimCommonSchema.MeasureResource.IfcPositiveLengthMeasure>("Pset_Test", "SomeProp");

            var value = pile.GetPropertySingleValue<XbimCommonSchema.MeasureResource.IfcPositiveLengthMeasure>("Pset_Test", "SomeProp");
            value.Should().NotBeNull();
            value.Value.Should().Be(1.0);
        }

        [Fact]
        public void CanAddElementAndReadQuantity()
        {
            var pile = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPile>();
            var quant = Model.Instances.New<Ifc4.QuantityResource.IfcQuantityArea>(qa =>
            {
                qa.Name = "GrossArea";
                qa.AreaValue = 123.4d;
            });
            pile.AddQuantity("BaseQuants", quant, "Some measure");

            var quantities = pile.GetElementQuantity("BaseQuants");
            quantities.Should().NotBeNull();
            var val = quantities.Quantities.First(q => q.Name == "GrossArea");

            val.Should().BeOfType<Ifc4.QuantityResource.IfcQuantityArea>();
            (val as Ifc4.QuantityResource.IfcQuantityArea).AreaValue.Value.Should().Be(123.4d);
            
        }

        [Fact]
        public void CanAddElementAndReadPhysicalQuantity()
        {
            var pile = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPile>();
            var quant = Model.Instances.New<Ifc4.QuantityResource.IfcQuantityArea>(qa =>
            {
                qa.Name = "GrossArea";
                qa.AreaValue = 123.4d;
            });
            pile.AddQuantity("BaseQuants", quant, "Some measure");

            var val = pile.GetQuantity<Ifc4.QuantityResource.IfcQuantityArea>("GrossArea");
            val.Should().NotBeNull();
            val.AreaValue.Value.Should().Be(123.4d);
        }

        [Fact]
        public void CanAddElementAndReadPhysicalQuantityWithPset()
        {
            var pile = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPile>();
            var quant = Model.Instances.New<Ifc4.QuantityResource.IfcQuantityArea>(qa =>
            {
                qa.Name = "GrossArea";
                qa.AreaValue = 123.4d;
            });
            pile.AddQuantity("BaseQuants", quant, "Some measure");

            var val = pile.GetQuantity<Ifc4.QuantityResource.IfcQuantityArea>("BaseQuants", "GrossArea");
            val.Should().NotBeNull();
            val.AreaValue.Value.Should().Be(123.4d);
        }

        [Fact]
        public void CanAddElementAndReadPhysicalSimpleQuantityWithPset()
        {
            var pile = Model.Instances.New<Ifc4.StructuralElementsDomain.IfcPile>();
            var quant = Model.Instances.New<Ifc4.QuantityResource.IfcQuantityArea>(qa =>
            {
                qa.Name = "GrossArea";
                qa.AreaValue = 123.4d;
            });
            pile.AddQuantity("BaseQuants", quant, "Some measure");

            var val = pile.GetElementPhysicalSimpleQuantity("BaseQuants", "GrossArea");
            val.Should().NotBeNull();
            val.Should().BeOfType<Ifc4.QuantityResource.IfcQuantityArea>();
        }

        [InlineData("GFA", XbimQuantityTypeEnum.Area, 15.5d, Ifc4.Interfaces.IfcSIUnitName.SQUARE_METRE)]
        [InlineData("Width", XbimQuantityTypeEnum.Length, 1200.4d,  Ifc4.Interfaces.IfcSIUnitName.METRE)]
        [InlineData("Volume", XbimQuantityTypeEnum.Volume, 12234d, Ifc4.Interfaces.IfcSIUnitName.CUBIC_METRE)]
        [InlineData("Count", XbimQuantityTypeEnum.Count, 14d, null)]
        [InlineData("Weight", XbimQuantityTypeEnum.Weight, 999, Ifc4.Interfaces.IfcSIUnitName.GRAM, Ifc4.Interfaces.IfcSIPrefix.KILO)]
        [InlineData("Duration", XbimQuantityTypeEnum.Time, 10d, Ifc4.Interfaces.IfcSIUnitName.SECOND)]
        [Theory]
        public void CanSetElementPhysicalSimpleQuantity(string quantName, XbimQuantityTypeEnum measure, double value,
            Ifc4.Interfaces.IfcSIUnitName? unitType, Ifc4.Interfaces.IfcSIPrefix? unitPrefix = null)
        {
            var space = Model.Instances.New<Ifc4.ProductExtension.IfcSpace>();
            Ifc4.MeasureResource.IfcNamedUnit unit = null;
            if(unitType != null)
            {
                unit = Model.Instances.New<Ifc4.MeasureResource.IfcSIUnit>(u =>
                {
                    u.Name = unitType.Value;
                    u.Prefix = unitPrefix;
                });
            }

            space.SetElementPhysicalSimpleQuantity("BaseQuants", quantName, value, measure, unit);

            Ifc4.MeasureResource.IfcMeasureValue val = measure switch
            {
                XbimQuantityTypeEnum.Area => space.GetQuantity<Ifc4.QuantityResource.IfcQuantityArea>("BaseQuants", quantName).AreaValue,
                XbimQuantityTypeEnum.Length => space.GetQuantity<Ifc4.QuantityResource.IfcQuantityLength>("BaseQuants", quantName).LengthValue,
                XbimQuantityTypeEnum.Volume => space.GetQuantity<Ifc4.QuantityResource.IfcQuantityVolume>("BaseQuants", quantName).VolumeValue,
                XbimQuantityTypeEnum.Count => space.GetQuantity<Ifc4.QuantityResource.IfcQuantityCount>("BaseQuants", quantName).CountValue,
                XbimQuantityTypeEnum.Weight => space.GetQuantity<Ifc4.QuantityResource.IfcQuantityWeight>("BaseQuants", quantName).WeightValue,
                XbimQuantityTypeEnum.Time => space.GetQuantity<Ifc4.QuantityResource.IfcQuantityTime>("BaseQuants", quantName).TimeValue,
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
            var currency = Model.Instances.New<Ifc4.MeasureResource.IfcMonetaryUnit>(u =>
            {
                u.Currency = tla;
            });

            currency.Symbol().Should().Be(expectedSymbol);
            currency.FullEnglishName().Should().Be(expectedName);
            currency.FullNativeName().Should().Be(nativeName);
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

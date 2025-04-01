using FluentAssertions;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Memory;
using Xunit;

namespace Xbim.Essentials.Tests
{
    public class MonetaryUnitTests
    {
 
        [InlineData("USD", "$", "US Dollar")]
        [InlineData("GBP", "£", "British Pound", "Pound Sterling")]
        [InlineData("EUR", "€", "Euro", "euro")]    // 'euro' since French culture picked up by default
        [InlineData("CAD", "$", "Canadian Dollar")]
        [InlineData("AUD", "$", "Australian Dollar")]
        [InlineData("PLN", "zł", "Polish Zloty", "złoty polski")]
        [Theory]
        public void CanReadMonetaryUnit4x3(string tla, string expectedSymbol, string expectedName, string nativeName = null)
        {
            using var model = new MemoryModel(new Ifc4x3.EntityFactoryIfc4x3Add2());
            using var txn = model.BeginTransaction("Test");

            nativeName = nativeName ?? expectedName;
            var currency = model.Instances.New<Ifc4x3.MeasureResource.IfcMonetaryUnit>(u =>
            {
                u.Currency = tla;
            });

            currency.FullName.Should().Be(expectedName);
            currency.Symbol().Should().Be(expectedSymbol);
            currency.FullEnglishName().Should().Be(expectedName);
            currency.FullNativeName().Should().Be(nativeName);
        }

        [InlineData(Ifc2x3.MeasureResource.IfcCurrencyEnum.USD, "$", "US Dollar")]
        [InlineData(Ifc2x3.MeasureResource.IfcCurrencyEnum.GBP, "£", "British Pound", "Pound Sterling")]
        [InlineData(Ifc2x3.MeasureResource.IfcCurrencyEnum.EUR, "€", "Euro", "euro")]    // 'euro' since French culture picked up by default
        [InlineData(Ifc2x3.MeasureResource.IfcCurrencyEnum.CAD, "$", "Canadian Dollar")]
        [InlineData(Ifc2x3.MeasureResource.IfcCurrencyEnum.AUD, "$", "Australian Dollar")]
        [InlineData(Ifc2x3.MeasureResource.IfcCurrencyEnum.PLN, "zł", "Polish Zloty", "złoty polski")]
        [Theory]
        public void CanReadMonetaryUnit2x3(Ifc2x3.MeasureResource.IfcCurrencyEnum tla, string expectedSymbol, string expectedName, string nativeName = null)
        {
            using var model = new MemoryModel(new Ifc2x3.EntityFactoryIfc2x3());
            using var txn = model.BeginTransaction("Test");
            nativeName = nativeName ?? expectedName;
            IIfcMonetaryUnit currency = model.Instances.New<Ifc2x3.MeasureResource.IfcMonetaryUnit>(u =>
            {
                u.Currency = tla;
            });

            currency.FullName.Should().Be(expectedName);
            currency.Symbol().Should().Be(expectedSymbol);
            currency.FullEnglishName().Should().Be(expectedName);
            currency.FullNativeName().Should().Be(nativeName);
        }

        [InlineData("US DOLLAR")]
        [InlineData("BAD")]
        [InlineData("BA")]
        [InlineData("")]
        //[InlineData(null)]
        [Theory]
        public void InvalidMonetaryUnitHandled(string invalidUnit)
        {
            using var model = new MemoryModel(new Ifc4.EntityFactoryIfc4());
            using var txn = model.BeginTransaction("Test");
     
            IIfcMonetaryUnit currency = model.Instances.New<Ifc4.MeasureResource.IfcMonetaryUnit>(u =>
            {
                u.Currency = invalidUnit;
            });

            currency.FullName.Should().Be(invalidUnit);
            currency.Symbol().Should().Be(invalidUnit);
            currency.FullEnglishName().Should().Be(invalidUnit);
            currency.FullNativeName().Should().Be(invalidUnit);
        }
    }
}

using FluentAssertions;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Memory;
using Xunit;


namespace Xbim.Essentials.NetCore.Tests
{
    public class CultureTests
    {
        [InlineData(Ifc2x3.MeasureResource.IfcCurrencyEnum.USD, "$", "US Dollar")]
        [InlineData(Ifc2x3.MeasureResource.IfcCurrencyEnum.GBP, "£", "British Pound", "British Pound")] // en-GB Native name is different in net core to netfx (Pounds Sterling)
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

    }

    
}

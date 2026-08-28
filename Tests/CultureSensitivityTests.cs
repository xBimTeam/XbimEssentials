using System.Globalization;
using Xbim.Ifc;
using Xbim.Ifc4;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.IO.Memory;
using Xunit;

namespace Xbim.Essentials.Tests
{
    /// <summary>
    /// The IfcXml readers resolve element names through <c>string.ToUpper()</c>, which is
    /// culture sensitive. Under the Turkish (and Azeri) casing rules 'i' uppercases to 'İ',
    /// so every type name containing an 'i' - i.e. every Ifc type - fails to resolve.
    /// </summary>
    public class CultureSensitivityTests
    {
        [Theory]
        [InlineData("en-US")]
        [InlineData("tr-TR")]
        public void Can_read_ifcxml_regardless_of_current_culture(string culture)
        {
            var previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);

                using var model = new MemoryModel(new EntityFactoryIfc4());
                model.LoadXml(@"TestFiles\Dimensions.ifcxml");

                Assert.True(model.Instances.Count > 0);
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("tr-TR")]
        public void Can_query_OfType_by_type_name_regardless_of_current_culture(string culture)
        {
            var previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);

                // a threshold of 0 forces the Esent database store rather than the memory model
                using var store = IfcStore.Open(@"TestFiles\SampleHouse4.ifc", null, 0);

                // the type name has to contain a lower case 'i' for the Turkish casing to bite:
                // "IfcWall" upper-cases identically in every culture, "IfcBuilding" does not
                Assert.NotEmpty(store.Instances.OfType("IfcBuilding", false));
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("tr-TR")]
        public void Imperial_unit_symbol_is_detected_regardless_of_current_culture(string culture)
        {
            var previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);

                using var model = new MemoryModel(new EntityFactoryIfc4());
                using var txn = model.BeginTransaction("unit");
                var unit = model.Instances.New<IfcConversionBasedUnit>(u =>
                {
                    u.Name = "inch";
                    u.UnitType = IfcUnitEnum.LENGTHUNIT;
                    u.Dimensions = model.Instances.New<IfcDimensionalExponents>(d =>
                    {
                        d.LengthExponent = 1;
                        d.MassExponent = 0;
                        d.TimeExponent = 0;
                        d.ElectricCurrentExponent = 0;
                        d.ThermodynamicTemperatureExponent = 0;
                        d.AmountOfSubstanceExponent = 0;
                        d.LuminousIntensityExponent = 0;
                    });
                });

                // "inch".ToUpper() is "İNCH" under tr-TR and does not contain "INCH"
                Assert.Equal("in", unit.Symbol);
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        }

        [Theory]
        [InlineData("en-US")]
        [InlineData("tr-TR")]
        public void Case_insensitive_property_set_lookup_works_regardless_of_current_culture(string culture)
        {
            var previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);

                using var model = new MemoryModel(new EntityFactoryIfc4());
                using var txn = model.BeginTransaction("psets");
                var pile = model.Instances.New<Xbim.Ifc4.StructuralElementsDomain.IfcPile>();
                var pset = model.Instances.New<Xbim.Ifc4.Kernel.IfcPropertySet>(p => p.Name = "Pset_WindowCommon");
                pile.AddPropertySet(pset);

                // 'i' and 'I' are different letters under the Turkish casing rules, so a
                // culture-sensitive case-insensitive comparison does not match here
                Assert.NotNull(pile.GetPropertySet("PSET_WINDOWCOMMON", caseSensitive: false));
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        }
    }
}

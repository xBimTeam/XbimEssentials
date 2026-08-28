using System.Globalization;
using Xbim.Ifc;
using Xbim.Ifc4;
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
    }
}

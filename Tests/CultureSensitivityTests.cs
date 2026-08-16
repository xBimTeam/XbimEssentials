using System.Globalization;
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
    }
}

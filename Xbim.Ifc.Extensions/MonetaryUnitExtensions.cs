using System.Linq;
using System.Globalization;
using Xbim.Ifc2x3.MeasureResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class MonetaryUnitExtensions
    {
        /// <summary>
        /// Get Symbol string for money unit
        /// </summary>
        /// <returns>String holding symbol</returns>
        public static string GetSymbol(this IfcMonetaryUnit moneyUnit)
        {
            string value = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
               .Where(c => new RegionInfo(c.LCID).ISOCurrencySymbol == moneyUnit.Currency.ToString())
               .Select(c => new RegionInfo(c.LCID).CurrencySymbol)
               .FirstOrDefault();
            return string.IsNullOrEmpty(value) ? moneyUnit.Currency.ToString() : value;
        }

        /// <summary>
        ///Get full English name of the currency
        /// </summary>
        /// <returns>String as full name</returns>
        public static string GetFullEnglishName(this IfcMonetaryUnit moneyUnit)
        {
            string value = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
               .Where(c => new RegionInfo(c.LCID).ISOCurrencySymbol == moneyUnit.Currency.ToString())
               .Select(c => new RegionInfo(c.LCID).CurrencyEnglishName)
               .FirstOrDefault();
            return string.IsNullOrEmpty(value) ? moneyUnit.Currency.ToString() : value;
        }

        /// <summary>
        ///Get full Native name of the currency
        /// </summary>
        /// <returns>String holding full name</returns>
        public static string GetFullNativeName(this IfcMonetaryUnit moneyUnit)
        {
            string value = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
               .Where(c => new RegionInfo(c.LCID).ISOCurrencySymbol == moneyUnit.Currency.ToString())
               .Select(c => new RegionInfo(c.LCID).CurrencyNativeName)
               .FirstOrDefault();
            return string.IsNullOrEmpty(value) ? moneyUnit.Currency.ToString() : value;
        }


    }
}

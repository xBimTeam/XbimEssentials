using System.Globalization;
using System.Linq;

namespace Xbim.Ifc4.MeasureResource
{
    public partial class IfcMonetaryUnit
    {
        /// <summary>
        /// Get Symbol string for money unit
        /// </summary>
        /// <returns>String holding symbol</returns>
        public string Symbol
        {
            get
            {
                //TODO resolve this, net standard and core do not support culture iteration and it is apparently very slow
                //string value = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                //    .Where(c => new RegionInfo(c.LCID).ISOCurrencySymbol == Currency.ToString())
                //    .Select(c => new RegionInfo(c.LCID).CurrencySymbol)
                //    .FirstOrDefault();
                //return string.IsNullOrEmpty(value) ? Currency.ToString() : value;
                return Currency.ToString();
            }
        }

        /// <summary>
        ///Get full English name of the currency
        /// </summary>
        /// <returns>String as full name</returns>
        public  string FullEnglishName
        {
            get
            {
                //TODO resolve this
                //string value = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                //    .Where(c => new RegionInfo(c.LCID).ISOCurrencySymbol == Currency.ToString())
                //    .Select(c => new RegionInfo(c.LCID).CurrencyEnglishName)
                //    .FirstOrDefault();
                //return string.IsNullOrEmpty(value) ? Currency.ToString() : value;
                return Currency.ToString();
            }
        }

        /// <summary>
        ///Get full Native name of the currency
        /// </summary>
        /// <returns>String holding full name</returns>
        public  string FullNativeName
        {
            get
            {
                //TODO resolve this
                //string value = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                //    .Where(c => new RegionInfo(c.LCID).ISOCurrencySymbol == Currency.ToString())
                //    .Select(c => new RegionInfo(c.LCID).CurrencyNativeName)
                //    .FirstOrDefault();
                //return string.IsNullOrEmpty(value) ? Currency.ToString() : value;
                return Currency.ToString();
            }
        }

        public string FullName
        {
            get
            {
                return FullNativeName;
            }
        }

    }
}

using System.Collections.Generic;
using System.Globalization;
using System;
using Xbim.Ifc4.Interfaces;
using System.Linq;

namespace Xbim.Ifc4.MeasureResource
{
    public static class UnitExtensions
    {


        /// <summary>
        /// Get the full name of the IfcUnit
        /// </summary>
        /// <returns>string holding full name</returns>
        public static string Name(this IIfcUnit ifcUnit)
        {
            return ifcUnit switch
            {
                IIfcDerivedUnit unit => unit.FullName,
                IIfcNamedUnit namedUnit => namedUnit.FullName,
                IIfcMonetaryUnit monetaryUnit => monetaryUnit.FullEnglishName(),
                _ => string.Empty
            };
        }
        /// <summary>
        /// Get the symbol of the IfcUnit
        /// </summary>
        /// <returns>string holding symbol</returns>
        public static string Symbol(this IIfcUnit ifcUnit)
        {
            return ifcUnit switch
            {
                IIfcDerivedUnit unit => unit.FullName,
                IIfcNamedUnit namedUnit => namedUnit.Symbol,
                IIfcMonetaryUnit monetaryUnit => monetaryUnit.Symbol(),
                _ => string.Empty
            };
        }


        /// <summary>
        /// Gets the currency symbol (e.g. $, £, €, kr etc) for the <see cref="IIfcMonetaryUnit"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Symbol(this IIfcMonetaryUnit obj)
        {
            return CurrencyMap.ContainsKey(obj.Currency) ? CurrencyMap[obj.Currency].CurrencySymbol : obj.Currency.ToString();
        }

        /// <summary>
        /// Gets the currency name in English for the <see cref="IIfcMonetaryUnit"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string FullEnglishName(this IIfcMonetaryUnit obj)
        {
            return CurrencyMap.ContainsKey(obj.Currency) ? CurrencyMap[obj.Currency].CurrencyEnglishName : obj.Currency.ToString();
        }

        /// <summary>
        /// Gets the currency name in local culture for the <see cref="IIfcMonetaryUnit"/>
        /// </summary>
        /// <remarks>Note: this is imprecise and inherently ambiguous due to different cultures installed, but for example 
        /// Polish PLN might be known natively as 'złoty polski' rather than 'Polish Zloty'</remarks>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string FullNativeName(this IIfcMonetaryUnit obj)
        {
            return CurrencyMap.ContainsKey(obj.Currency) ? CurrencyMap[obj.Currency].CurrencyNativeName : obj.Currency.ToString();
        }
        private static IDictionary<string, RegionInfo> CurrencyMap => LazyCurrencyMap.Value;


        // Lazily constructed dictionary of 'Regioninfo' by ISO Currency codes, acquired from all installed specific cultures.
        // eg. en-GB => {GBP}, en-US => {USD}, fr-FR => {Euro} etc. This mapping is slightly imperfect in terms of
        // Cultural labling but good enough? See below
        private static Lazy<IDictionary<string, RegionInfo>> LazyCurrencyMap = new Lazy<IDictionary<string, RegionInfo>>(() =>
            CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .Where(c => !skippedCountryCodes.Any(i => c.Name == i))
                .Select(c => new RegionInfo(c.LCID))
                .Distinct(new RegionInfoComparer())
                .ToDictionary(r => r.ISOCurrencySymbol, r => r));

        // We're acquiring currency symbols and localised names etc indirectly from all installed cultures on the platform.
        // But the first culture we locate a currency in may not be the most natural/prevalent. E.g. Locating GBP on Windows,
        // 'cy-GB' (Welsh UK) comes before 'en-GB' (English UK), based on the order of cultures, meaning the GBP
        // CurrencyNativeName appears as 'Punt Prydain' rather than the more natural 'Pound Sterling'.
        // 
        // Note: There are probably many other countries where a currency is shared across cultures and the first culture
        // is not the most natural one. (e.g INR, ZAR), so this list may need updating
        // This only affects currency lookups, not any other Culture/Locale feature.
        private static string[] skippedCountryCodes = new[] { "cy-GB", "gd-GB" };

        private class RegionInfoComparer : IEqualityComparer<RegionInfo>
        {
            public bool Equals(RegionInfo x, RegionInfo y) => x.ISOCurrencySymbol == y.ISOCurrencySymbol;

            public int GetHashCode(RegionInfo obj) => obj.ISOCurrencySymbol.GetHashCode();
        }

    }
}

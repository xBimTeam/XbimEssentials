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

        // .NET doesn't provide a list of global currencies specifically. 
        // We have to acquire currency symbols and localised names etc indirectly from all installed cultures on the platform
        // - which can vary from machine to machine, Framework to framework.
        // But the first culture we locate a currency in may not be the most natural/prevalent. E.g. Locating £ / 'GBP' on Windows net48,
        // 'cy-GB' (Welsh UK) comes before 'en-GB' (English UK), based on the order of cultures, meaning the GBP
        // CurrencyNativeName appears as the Welsh 'Punt Prydain' rather than the more natural 'Pound Sterling' / 'British Pound'. 

        // To address this we prioritse the 'main' culture associated with common currencies so we're showing something
        // sensible most of the time when displaying a native name.
        // Less common currencies may end up with names taken from non-obvious cultures.

        private static string[] commonCurrencyCultures = [
            "en-GB",    // GBP
            "fr-FR",    // Euro
            "en-US",    // USD
            "en-CA",    // CAD$
            "en-AU",    // AUD$
            "en-NZ",    // NZ$
            "en-ZA",    // Rand
            "da-DK",    // Danish Krone
            "nn-NO",    // Norwegian Krone
            "sv-SE",    // Swedish Krona
            "fr-CH",    // Swiss Franc
            "es-MX",    // Mexican Peso
            "pt-BR",    // Brazil 
            "pl-PL",    // Polish Zloty
            "cs-CZ",    // Czech Korona
            "ar-SA",    // Saudi Riyal
            "ar-QA",    // Qatar Riyal
            "ar-AE",    // UAE Diram
            "ja-JP",    // Japanese Yen
            "hi-IN",    // Indian Rupee
            "ur-PK",    // Pakistan
            "ko-KR",    // Korean
            "zh-CN",    // Chinese CHN
            "ru-RU",    // Russian Ruble
        ];

        private static IEnumerable<CultureInfo> prioritisedCultures = 
            CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(c => commonCurrencyCultures.Any(i => c.Name == i))      // Common currency cultures first
                .Union(CultureInfo.GetCultures(CultureTypes.SpecificCultures).Where(c => !commonCurrencyCultures.Any(i => c.Name == i)));    // Then the rest

        // Lazily constructed dictionary of 'Regioninfo' by ISO Currency codes, acquired from all installed specific cultures.
        // eg. en-GB => {GBP}, en-US => {USD}, fr-FR => {Euro} etc. This mapping is slightly imperfect in terms of
        // Cultural labling but good enough? See above
        private static Lazy<IDictionary<string, RegionInfo>> LazyCurrencyMap = new Lazy<IDictionary<string, RegionInfo>>(() =>
                prioritisedCultures
                .Select(c => new RegionInfo(c.Name))
                .Distinct(new RegionInfoComparer())
                .ToDictionary(r => r.ISOCurrencySymbol, r => r));

        private class RegionInfoComparer : IEqualityComparer<RegionInfo>
        {
            public bool Equals(RegionInfo x, RegionInfo y) => x.ISOCurrencySymbol == y.ISOCurrencySymbol;

            public int GetHashCode(RegionInfo obj) => obj.ISOCurrencySymbol.GetHashCode();
        }

    }
}

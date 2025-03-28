using Xbim.Ifc4.Interfaces;
using CrossSchema = Xbim.Ifc4;

namespace Xbim.Ifc
{
    public static class IIfcUnitExtensions
    {
        public static string AsString(this IIfcMeasureWithUnit obj)
        {
            string value = string.Format("{0,0:N2}", obj.ValueComponent.Value);
            var ifcUnit = obj.UnitComponent;
            string unit = ifcUnit.Symbol();
            if (!string.IsNullOrEmpty(unit))
            {
                value += unit;
            }
            return value;
        }

        /// <summary>
        /// Get the full name of the IfcUnit
        /// </summary>
        /// <returns>string holding full name</returns>
        public static string Name(this IIfcUnit ifcUnit)
        {
            // Delegate to Ifc4/Cross schema implementation
            return CrossSchema.MeasureResource.UnitExtensions.Name(ifcUnit);
        }
        /// <summary>
        /// Get the symbol of the IfcUnit
        /// </summary>
        /// <returns>string holding symbol</returns>
        public static string Symbol(this IIfcUnit ifcUnit)
        {
            // Delegate to Ifc4/Cross schema implementation
            return CrossSchema.MeasureResource.UnitExtensions.Symbol(ifcUnit);
        }

        /// <summary>
        /// Gets the currency symbol (e.g. $, £, €, kr etc) for the <see cref="IIfcMonetaryUnit"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Symbol(this IIfcMonetaryUnit obj)
        {
            // Delegate to Ifc4/Cross schema implementation
            return CrossSchema.MeasureResource.UnitExtensions.Symbol(obj);
        }

        /// <summary>
        /// Gets the currency name in English for the <see cref="IIfcMonetaryUnit"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string FullEnglishName(this IIfcMonetaryUnit obj)
        {
            // Delegate to Ifc4/Cross schema implementation
            return CrossSchema.MeasureResource.UnitExtensions.FullEnglishName(obj);
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
            // Delegate to Ifc4/Cross schema implementation
            return CrossSchema.MeasureResource.UnitExtensions.FullNativeName(obj);
        }
    }

    
}

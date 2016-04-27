namespace Xbim.Ifc2x3.MeasureResource
{
    public static class UnitExtensions
    {

        /// <summary>
        /// Get the full name of the IfcUnit
        /// </summary>
        /// <returns>string holding full name</returns>
        public static string Name(this IfcUnit ifcUnit)
        {
            var unit = ifcUnit as IfcDerivedUnit;
            if (unit != null) return unit.FullName;
            var namedUnit = ifcUnit as IfcNamedUnit;
            if (namedUnit != null) return namedUnit.FullName;
            var monetaryUnit = ifcUnit as IfcMonetaryUnit;
            return monetaryUnit != null ? monetaryUnit.FullEnglishName : string.Empty;
        }
        /// <summary>
        /// Get the symbol of the IfcUnit
        /// </summary>
        /// <returns>string holding symbol</returns>
        public static string Symbol(this IfcUnit ifcUnit)
        {
            var unit = ifcUnit as IfcDerivedUnit;
            if (unit != null) return unit.FullName;
            var namedUnit = ifcUnit as IfcNamedUnit;
            if (namedUnit != null) return namedUnit.Symbol;
            var monetaryUnit = ifcUnit as IfcMonetaryUnit;
            return monetaryUnit != null ? monetaryUnit.Symbol : string.Empty;
        }

    }
}

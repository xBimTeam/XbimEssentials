using Xbim.Ifc2x3.MeasureResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class NamedUnitExtensions
    {
        /// <summary>
        /// Get the full name of the IfcNamedUnit
        /// </summary>
        /// <returns>string holding full name</returns>
        public static string GetName(this IfcNamedUnit namedUnit)
        {
            if (namedUnit is IfcSIUnit)
                return ((IfcSIUnit)namedUnit).GetName();
            else if(namedUnit is IfcConversionBasedUnit)
                return ((IfcConversionBasedUnit)namedUnit).Name;
            else if(namedUnit is IfcContextDependentUnit)
                return ((IfcContextDependentUnit)namedUnit).Name;
            else
                return string.Empty;
        }

        /// <summary>
        /// Get the symbol of the IfcNamedUnit
        /// </summary>
        /// <returns>string holding symbol</returns>
        public static string GetSymbol(this IfcNamedUnit namedUnit)
        {
            if (namedUnit is IfcSIUnit)
                return ((IfcSIUnit)namedUnit).GetSymbol();
            else if (namedUnit is IfcConversionBasedUnit)
                return ((IfcConversionBasedUnit)namedUnit).Name;  //elected not to get symbol as a small potential for a infinite loop is same object references itself
            else if (namedUnit is IfcContextDependentUnit)
                return ((IfcContextDependentUnit)namedUnit).Name; //no symbol calc here
            else
                return string.Empty;
        }

        /// <summary>
        /// Get Symbol string for IfcConversionBasedUnit conversion unit
        /// </summary>
        /// <returns>String holding symbol</returns>
        public static string GetSymbol(this IfcConversionBasedUnit ifcConversionBasedUnit)
        {
            string name = ifcConversionBasedUnit.Name;
            if ((ifcConversionBasedUnit.UnitType == IfcUnitEnum.LENGTHUNIT) ||
                (ifcConversionBasedUnit.UnitType == IfcUnitEnum.AREAUNIT) ||
                (ifcConversionBasedUnit.UnitType == IfcUnitEnum.VOLUMEUNIT)
                )
            {
                string pow = string.Empty;
                if (ifcConversionBasedUnit.Dimensions.LengthExponent == 2) //area
                    pow += '\u00B2';//try unicode ((char)0x00B2)add ²
                if (ifcConversionBasedUnit.Dimensions.LengthExponent == 3) //volume
                    pow += '\u00B3';//((char)0x00B3)add ³

                if ((name.Contains("FEET")) || (name.Contains("FOOT")))
                    return "ft" + pow;

                if (name.Contains("INCH"))
                    return "in" + pow;

                return name + pow;
            }
            else
                return name;
        }

        /// <summary>
        /// Get Symbol string for SIUnit unit
        /// </summary>
        /// <returns>String holding symbol</returns>
        public static string GetSymbol(this IfcSIUnit ifcSIUnit)
        {
            IfcSIUnitName ifcSIUnitName = ifcSIUnit.Name;
            IfcSIPrefix ifcSIPrefix;
            string value = string.Empty;
            string prefix = string.Empty;
            if (ifcSIUnit.Prefix != null)
            {
                ifcSIPrefix = (IfcSIPrefix)ifcSIUnit.Prefix;
                switch (ifcSIPrefix)
                {
                    case IfcSIPrefix.CENTI:
                        prefix = "c";
                        break;
                    case IfcSIPrefix.MILLI:
                        prefix = "m";
                        break;
                    case IfcSIPrefix.KILO:
                        prefix = "k";
                        break;
                    default: //TODO: the other values of IfcSIPrefix
                        prefix = ifcSIPrefix.ToString();
                        break;
                }
            }

            switch (ifcSIUnitName)
            {
                case IfcSIUnitName.METRE:
                    value = prefix + "m";
                    break;
                case IfcSIUnitName.SQUARE_METRE:
                    value = prefix + "m" + '\u00B2'; //((char)0x00B2)might need to look at this for other cultures
                    break;
                case IfcSIUnitName.CUBIC_METRE:
                    value = prefix + "m" + '\u00B3';//((char)0x00B3)
                    break;
                case IfcSIUnitName.GRAM:
                    value = prefix + "g";
                    break;
                default://TODO: the other values of IfcSIUnitName
                    value = ifcSIUnit.ToString();
                    break;
            }
            return value;
        }

        /// <summary>
        /// Returns the full name of the unit
        /// </summary>
        /// <returns>string holding name</returns>
        public static string GetName(this IfcSIUnit ifcSIUnit)
        {
            string prefixUnit = (ifcSIUnit.Prefix.HasValue) ? ifcSIUnit.Prefix.ToString() : "";  //see IfcSIPrefix
            string value = ifcSIUnit.Name.ToString();                                   //see IfcSIUnitName
            //Handle the "_" in _name value, should work for lengths, but might have to look at other values later
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains("_"))
                    return value = value.Replace("_", prefixUnit);
                else
                    return value = prefixUnit + value; //combine to give length name
            }
            else
                return string.Format("{0}{1}", ifcSIUnit.Prefix.HasValue ? ifcSIUnit.Prefix.Value.ToString() : "", ifcSIUnit.Name.ToString());
        }
    }
}

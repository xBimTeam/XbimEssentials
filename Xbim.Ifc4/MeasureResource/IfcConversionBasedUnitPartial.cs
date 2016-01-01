using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc4.MeasureResource
{
    public partial class IfcConversionBasedUnit
    {
        public string FullName { get { return Name; } }
        
        /// <summary>
        /// Get Symbol string for IfcConversionBasedUnit conversion unit
        /// </summary>
        /// <returns>String holding symbol</returns>
        public new string Symbol
        {
            get
            {
                string name = Name;
                
                if ((UnitType == IfcUnitEnum.LENGTHUNIT) ||
                    (UnitType == IfcUnitEnum.AREAUNIT) ||
                    (UnitType == IfcUnitEnum.VOLUMEUNIT)
                    )
                {
                    string pow = string.Empty;
                    if (Dimensions.LengthExponent == 2) //area
                        pow += '\u00B2'; //try unicode ((char)0x00B2)add ²
                    if (Dimensions.LengthExponent == 3) //volume
                        pow += '\u00B3'; //((char)0x00B3)add ³

                    if ((name.ToUpper().Contains("FEET")) || (name.ToUpper().Contains("FOOT")))
                        return "ft" + pow;

                    if (name.ToUpper().Contains("INCH"))
                        return "in" + pow;

                    return name + pow;
                }
                return name;
            }
        }
    }
}

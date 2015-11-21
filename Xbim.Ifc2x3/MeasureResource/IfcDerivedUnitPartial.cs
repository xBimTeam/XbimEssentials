using System.Collections.Generic;

namespace Xbim.Ifc2x3.MeasureResource
{
    public partial class IfcDerivedUnit
    {

        /// <summary>
        /// Get the full name of the IfcDerivedUnit
        /// </summary>
        /// <returns>string holding full name</returns>
        public string Name
        {
            get
            {
                if (UserDefinedType.HasValue)
                    return UserDefinedType;
                var values = new List<string>();
                foreach (IfcDerivedUnitElement item in Elements)
                    //loop the units associated to this type
                {
                    var value = item.Unit.Name();
                    //add power
                    if (item.Exponent > 0)
                    {
                        if (item.Exponent == 2)
                        {
                            value += '\u00B2'; //add ² ((char)0x00B2)
                        }
                        else if (item.Exponent == 3)
                        {
                            value += '\u00B3'; //add ³  //((char)0x00B3)
                        }
                        else
                            value += "Pow:" + item.Exponent.ToString();
                    }
                    if(!string.IsNullOrWhiteSpace(value))
                        values.Add(value);
                }
                return values.Count > 1 ? string.Join("/", values) : string.Empty;
            }
        }
    }
}

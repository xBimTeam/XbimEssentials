using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.Extensions
{
    public static class UnitExtensions
    {

        /// <summary>
        /// Get the full name of the IfcUnit
        /// </summary>
        /// <returns>string holding full name</returns>
        public static string GetName(this IfcUnit ifcUnit)
        {

            if (ifcUnit is IfcDerivedUnit)
                return (ifcUnit as IfcDerivedUnit).GetName();
            else if (ifcUnit is IfcNamedUnit)
                return (ifcUnit as IfcNamedUnit).GetName();
            else if (ifcUnit is IfcMonetaryUnit)
                return (ifcUnit as IfcMonetaryUnit).GetFullEnglishName();

            return string.Empty;
        }
        /// <summary>
        /// Get the symbol of the IfcUnit
        /// </summary>
        /// <returns>string holding symbol</returns>
        public static string GetSymbol(this IfcUnit ifcUnit)
        {

            if (ifcUnit is IfcDerivedUnit)
                return (ifcUnit as IfcDerivedUnit).GetName();
            else if (ifcUnit is IfcNamedUnit)
                return (ifcUnit as IfcNamedUnit).GetSymbol();
            else if (ifcUnit is IfcMonetaryUnit)
                return (ifcUnit as IfcMonetaryUnit).GetSymbol();

            return string.Empty;
        }

        /// <summary>
        /// Get the full name of the IfcDerivedUnit
        /// </summary>
        /// <returns>string holding full name</returns>
        public static string GetName(this IfcDerivedUnit ifcDerivedUnit)
        {
            if (ifcDerivedUnit.UserDefinedType.HasValue)
            {
                return ifcDerivedUnit.UserDefinedType;
            }
            else
            {
                List<string> values = new List<string>();
                foreach (IfcDerivedUnitElement item in ifcDerivedUnit.Elements)//loop the units associated to this type
                {
                    string value = string.Empty;
                    value = item.Unit.GetName();
                    //add power
                    if (item.Exponent > 0)
                    {
                        if (item.Exponent == 2)
                        {
                            value += '\u00B2';//add ² ((char)0x00B2)
                        }
                        else if (item.Exponent == 3)
                        {
                            value += '\u00B3';//add ³  //((char)0x00B3)
                        }
                        else
                            value += "Pow:" + item.Exponent.ToString();
                    }
                }
                if (values.Count > 1)
                    return string.Join("/", values);
                else
                    return string.Empty;
                
                
            }
        }
    }
}

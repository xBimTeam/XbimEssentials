#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    Part21Formatter.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.UtilityResource;
using System.Globalization;

#endregion

namespace Xbim.IO
{
    public class Part21Formatter : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        public string Format(string fmt, object arg, IFormatProvider formatProvider)
        {
            // Convert argument to a string           

            if (!String.IsNullOrEmpty(fmt) && fmt.ToUpper() == "R" && arg.GetType() == typeof(double))
            {
                double dArg = (double)arg;
                string result = dArg.ToString("R", CultureInfo.CreateSpecificCulture("en-US"));

                // if compiler flag, only then do the following 3 lines
                string rDoubleStr = dArg.ToString("R", CultureInfo.CreateSpecificCulture("en-US"));
                double fixedDbl = double.Parse(rDoubleStr, CultureInfo.CreateSpecificCulture("en-US"));
                result = fixedDbl.ToString("R", CultureInfo.CreateSpecificCulture("en-US"));

                //decimal decArg = new Decimal(dArg);                                
                // string result = decArg.ToString().ToUpper();
                // string result = string.Format("{0:e22}", arg);
                //string result = dArg.ToString("R", CultureInfo.CreateSpecificCulture("en-US"));
                if (!result.Contains("."))
                {
                    if (result.Contains("E"))
                        result = result.Replace("E", ".E");
                    else
                        result += ".";
                }

                return result;
            }
            else if (!String.IsNullOrEmpty(fmt) && fmt.ToUpper() == "T") //TimeStamp
            {
                string result = arg.ToString().ToUpper();
                DateTime? dt = arg as DateTime?;
                if (dt.HasValue == false)
                    throw new ArgumentException("Only valid DateTime objects can be converted to Part21 Timestamp");
                return IfcTimeStamp.ToTimeStamp(dt.Value).ToPart21;
            }
            else if (!String.IsNullOrEmpty(fmt) && fmt.ToUpper() == "G") //Guid
            {
                string result = arg.ToString().ToUpper();
                Guid guid = (Guid)arg;
                return string.Format(@"'{0}'", IfcGloballyUniqueId.AsPart21(guid));
            }
            // Return string representation of argument for any other formatting code
            else
                return string.Format(@"'{0}'", IfcText.Escape(arg.ToString()));
        }
    }
}
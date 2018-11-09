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
using System.Globalization;

#endregion

namespace Xbim.IO.Step21
{
    public class Part21Formatter : IFormatProvider, ICustomFormatter
    {
        private static readonly CultureInfo _cInfo = new CultureInfo("en-US");

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string fmt, object arg, IFormatProvider formatProvider)
        {
            // Convert argument to a string           

            if (!string.IsNullOrEmpty(fmt) && fmt.ToUpper() == "R" && arg is double)
            {
                var dArg = (double)arg;

                // if compiler flag, only then do the following 3 lines
          
                var rDoubleStr = dArg.ToString("R", StepText.DoubleCulture);
                var fixedDbl = double.Parse(rDoubleStr, StepText.DoubleCulture);
                var result = fixedDbl.ToString("R", StepText.DoubleCulture);

                //decimal decArg = new Decimal(dArg);                                
                // string result = decArg.ToString().ToUpper();
                // string result = string.Format("{0:e22}", arg);
                //string result = dArg.ToString("R", CultureInfo.CreateSpecificCulture("en-US"));
                if (result.Contains(".")) return result;

                if (result.Contains("E"))
                    result = result.Replace("E", ".E");
                else
                    result += ".";

                return result;
            }
            if (!string.IsNullOrEmpty(fmt) && fmt.ToUpper() == "T") //TimeStamp
            {
                if (!(arg is DateTime))
                    throw new ArgumentException("Only valid DateTime objects can be converted to Part21 Timestamp");

                var dateTime = (DateTime)arg;
                var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                //from 1970/1/1 00:00:00 to _lastModifiedDate
                var result = dateTime.Subtract(dt);
                var seconds = Convert.ToInt32(result.TotalSeconds);
                return seconds.ToString();
            }
            if (!string.IsNullOrEmpty(fmt) && fmt.ToUpper() == "G") //Guid
            {
                var guid = (Guid)arg;
                return string.Format(@"'{0}'", guid.ToPart21());
            }
            // Return string representation of argument for any other formatting code
            return string.Format(@"'{0}'", arg.ToString().ToPart21());
        }
    }
}
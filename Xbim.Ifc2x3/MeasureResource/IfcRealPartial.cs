using System;
using System.Globalization;

namespace Xbim.Ifc2x3.MeasureResource
{
    public partial struct IfcReal
    {
        public static string AsPart21(double real)
        {
            double dArg = (double)real;
            var ci = new CultureInfo("en-US");
            string result = dArg.ToString("R", ci);

            // if compiler flag, only then do the following 3 lines
            string rDoubleStr = dArg.ToString("R", ci);
            double fixedDbl = double.Parse(rDoubleStr, ci);
            result = fixedDbl.ToString("R", ci);

            if (!result.Contains("."))
            {
                if (result.Contains("E"))
                    result = result.Replace("E", ".E");
                else
                    result += ".";
            }

            return result;
        }

        public static double ToDouble(string val)
        {
            return Convert.ToDouble(val, new CultureInfo("en-US"));
        }
    }
}

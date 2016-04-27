using System;
using System.Globalization;

namespace Xbim.Ifc2x3.MeasureResource
{
    public partial struct IfcReal
    {
        public static string AsPart21(double real)
        {
            double dArg = (double)real;
            string result = dArg.ToString("R", CultureInfo.CreateSpecificCulture("en-US"));

            // if compiler flag, only then do the following 3 lines
            string rDoubleStr = dArg.ToString("R", CultureInfo.CreateSpecificCulture("en-US"));
            double fixedDbl = double.Parse(rDoubleStr, CultureInfo.CreateSpecificCulture("en-US"));
            result = fixedDbl.ToString("R", CultureInfo.CreateSpecificCulture("en-US"));

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
            return Convert.ToDouble(val, CultureInfo.CreateSpecificCulture("en-US"));
        }
    }
}

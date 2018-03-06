using System.Globalization;

namespace Xbim.IO.Step21
{
    public static class StepDoubleHelper
    {
        public static string AsPart21(this double real)
        {
           
            // if compiler flag, only then do the following 3 lines
            var rDoubleStr = real.ToString("R", StepText.DoubleCulture);
            var fixedDbl = double.Parse(rDoubleStr, StepText.DoubleCulture);
            var result = fixedDbl.ToString("R", StepText.DoubleCulture);

            if (!result.Contains("."))
            {
                if (result.Contains("E"))
                    result = result.Replace("E", ".E");
                else
                    result += ".";
            }

            return result;
        }
    }
}

using System;

namespace Xbim.IO.Step21
{
    public static class StepDateTimeHelper
    {
        public static int ToStep21(this DateTime dateTime)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var result = dateTime.Subtract(dt);
            return Convert.ToInt32(result.TotalSeconds);
        }
    }
}

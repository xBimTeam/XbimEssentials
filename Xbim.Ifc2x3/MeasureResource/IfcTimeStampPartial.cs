using System;

namespace Xbim.Ifc2x3.MeasureResource
{
    public partial  struct IfcTimeStamp
    {
        public static DateTime ToDateTime(IfcTimeStamp timeStamp)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); //from 1970/1/1 00:00:00
            return (dt.AddSeconds(timeStamp));
        }

        public static IfcTimeStamp ToTimeStamp(DateTime dateTime)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            //from 1970/1/1 00:00:00 to _lastModifiedDate
            var result = dateTime.Subtract(dt);
            var seconds = Convert.ToInt32(result.TotalSeconds);
            return new IfcTimeStamp(seconds);
        }
    }
}

using System;
using System.Globalization;

namespace Xbim.Ifc4.DateTimeResource
{

    public partial struct IfcTimeStamp
    {
        public DateTime ToDateTime()
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); //from 1970/1/1 00:00:00
            return dt.AddSeconds(this);
        }

        public static implicit operator DateTime(IfcTimeStamp obj)
        {
            return obj.ToDateTime();

        }

        public static implicit operator IfcTimeStamp(DateTime obj)
        {
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); //from 1970/1/1 00:00:00
            var s = obj - dt;
            return (long)s.TotalSeconds;
        }

        public static implicit operator TimeSpan(IfcTimeStamp obj)
        {
            return TimeSpan.FromSeconds(obj);
        }

        public static implicit operator IfcTimeStamp(TimeSpan obj)
        {
            return (long)obj.TotalSeconds;
        }
       
    }

}

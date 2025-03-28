using System;

namespace Xbim.Ifc4x3.DateTimeResource
{
    public partial struct IfcTime
    {
        public static implicit operator DateTime(IfcTime obj)
        {
            if (DateTime.TryParse(obj, out DateTime d))
                return d;
            return default;
        }

        public static implicit operator IfcTime(DateTime obj)
        {
            var s = obj.ToString("O");
            //2009-06-15T13:45:30.0000000-07:00
            //           ^
            return s.Substring(11);
        }
    }
}

using System;
using System.Globalization;

namespace Xbim.Ifc4x3.DateTimeResource
{
    public partial struct IfcDateTime
    {
        public DateTime ToDateTime()
        {
            return this;
        }

        public static implicit operator DateTime(IfcDateTime obj)
        {
            if(DateTime.TryParse(obj._value, null, DateTimeStyles.RoundtripKind, out DateTime result))
                return result;
            return default;
        }

        public static implicit operator IfcDateTime(DateTime obj)
        {
            obj = DateTime.SpecifyKind(obj, DateTimeKind.Unspecified);
            return obj.ToString("o");
        }
    }
}

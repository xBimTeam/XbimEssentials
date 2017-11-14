using System;
using System.Globalization;

namespace Xbim.Ifc4.DateTimeResource
{
    public partial struct IfcDateTime
    {
        public DateTime ToDateTime()
        {
            return this;
        }

        public static implicit operator DateTime(IfcDateTime obj)
        {
            DateTime result;
            DateTime.TryParse(obj._value, null, DateTimeStyles.RoundtripKind, out result);
            return result;

        }

        public static implicit operator IfcDateTime(DateTime obj)
        {
            obj = DateTime.SpecifyKind(obj, DateTimeKind.Unspecified);
            return obj.ToString("o");
        }
    }
}

using System;
using System.Globalization;

namespace Xbim.Ifc4.DateTimeResource
{
    public partial struct IfcDate
    {
        public DateTime ToDateTime()
        {
            return this;
        }

        public static implicit operator DateTime(IfcDate obj)
        {
            if (DateTime.TryParse(obj._value, null, DateTimeStyles.RoundtripKind, out DateTime result))
                return result;
            return default;
        }

        public static implicit operator IfcDate(DateTime obj)
        {
            obj = DateTime.SpecifyKind(obj, DateTimeKind.Unspecified);
            return obj.ToString("yyyy-MM-dd");
        }
    }
}

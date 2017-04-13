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
            DateTime result;
            DateTime.TryParse(obj._value, null, DateTimeStyles.RoundtripKind, out result);
            return result;

        }

        public static implicit operator IfcDate(DateTime obj)
        {
            obj = DateTime.SpecifyKind(obj, DateTimeKind.Unspecified);
            return obj.ToString("yyyy-MM-dd");
        }
    }
}

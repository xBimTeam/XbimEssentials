using System;

namespace Xbim.Ifc4.MeasureResource
{
    public partial struct IfcCountMeasure
    {
        public static implicit operator IfcCountMeasure(long value)
        {
            return new IfcCountMeasure(value);
        }

        public static implicit operator long(IfcCountMeasure obj)
        {
            return Convert.ToInt64(obj._value);

        }
    }
}

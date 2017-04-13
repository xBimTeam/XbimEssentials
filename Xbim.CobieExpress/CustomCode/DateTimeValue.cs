using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Xbim.CobieExpress
{
    public partial struct DateTimeValue
    {
        public static implicit operator DateTime(DateTimeValue value)
        {
            return DateTime.ParseExact(value._value, "s", CultureInfo.InvariantCulture);
        }

        public static implicit operator DateTimeValue(DateTime value)
        {
            return new DateTimeValue(value.ToString("s"));
        }
        
    }
}

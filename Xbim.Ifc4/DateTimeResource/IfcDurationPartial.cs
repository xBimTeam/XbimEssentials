using System;
using System.Xml;
using Xbim.Common.XbimExtensions;

namespace Xbim.Ifc4.DateTimeResource
{
    public partial struct IfcDuration
    {
        public TimeSpan ToTimeSpan()
        {
            return XmlConvert.ToTimeSpan(Value.ToString());
        }

        public static implicit operator IfcDuration(TimeSpan value)
        {

            //https://www.w3.org/TR/xmlschema-2/#duration
            return new IfcDuration(TimeSpanToString(value));
        }

        public static implicit operator TimeSpan(IfcDuration obj)
        {
            string value = obj;
            return value.Iso8601DurationToTimeSpan();
        }

        /// <summary>
        /// Return the string representation according to xsd:duration rules, xdt:dayTimeDuration rules, or
        /// xdt:yearMonthDuration rules.
        /// </summary>
        private static string TimeSpanToString(TimeSpan span)
        {
            return span.ToIso8601Representation();
        }

    }
}

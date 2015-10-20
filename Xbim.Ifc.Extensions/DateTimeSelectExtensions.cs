using System;
using System.Globalization;
using Xbim.Ifc2x3.DateTimeResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class DateTimeSelectExtensions
    {
        /// <summary>
        /// Get the string value for the IfcDateTimeSelect
        /// </summary>
        /// <param name="ifcDateTimeSelect">IfcDateTimeSelect</param>
        /// <returns>string</returns>
        public static string GetAsString(this IfcDateTimeSelect ifcDateTimeSelect)
        {

            if (ifcDateTimeSelect is IfcDateAndTime)
            {
                var datetime = (ifcDateTimeSelect as IfcDateAndTime);
                var minute = 0;
                if (datetime.TimeComponent.MinuteComponent.HasValue)
                    minute = (int)datetime.TimeComponent.MinuteComponent.Value;
                var second = 0;
                if (datetime.TimeComponent.SecondComponent.HasValue)
                    second = (int)datetime.TimeComponent.SecondComponent.Value;
                return new DateTime((int)datetime.DateComponent.YearComponent, (int)datetime.DateComponent.MonthComponent, (int)datetime.DateComponent.DayComponent, (int)datetime.TimeComponent.HourComponent, minute, second).ToString(CultureInfo.InvariantCulture);
            }
            if (ifcDateTimeSelect is IfcCalendarDate)
            {
                var date = (ifcDateTimeSelect as IfcCalendarDate);
                return new DateTime((int)date.YearComponent, (int)date.MonthComponent, (int)date.DayComponent).ToString("d");
            }
            if (ifcDateTimeSelect is IfcLocalTime)
            {
                var time = (ifcDateTimeSelect as IfcLocalTime);
                var minute = 0;
                if (time.MinuteComponent.HasValue)
                    minute = (int)time.MinuteComponent.Value;
                var second = 0;
                if (time.SecondComponent.HasValue)
                    second = (int)time.SecondComponent.Value;
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, (int)time.HourComponent, minute, second).ToString("HH:mm:ss");
            }
            return string.Empty;
        }
    }
}

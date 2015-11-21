using System;
using System.Globalization;

namespace Xbim.Ifc2x3.DateTimeResource
{
    public static class DateTimeSelectExtensions
    {
        /// <summary>
        /// Get the string value for the IfcDateTimeSelect
        /// </summary>
        /// <param name="ifcDateTimeSelect">IfcDateTimeSelect</param>
        /// <returns>string</returns>
        public static string AsString(this IfcDateTimeSelect ifcDateTimeSelect)
        {
            var datetime = ifcDateTimeSelect as IfcDateAndTime;
            if (datetime != null)
            {            
                var minute = 0;
                if (datetime.TimeComponent.MinuteComponent.HasValue)
                    minute = (int)datetime.TimeComponent.MinuteComponent.Value;
                var second = 0;
                if (datetime.TimeComponent.SecondComponent.HasValue)
                    second = (int)datetime.TimeComponent.SecondComponent.Value;
                return new DateTime((int)datetime.DateComponent.YearComponent, (int)datetime.DateComponent.MonthComponent, (int)datetime.DateComponent.DayComponent, (int)datetime.TimeComponent.HourComponent, minute, second).ToString(CultureInfo.InvariantCulture);
            }
            var calendarDate = ifcDateTimeSelect as IfcCalendarDate;
            if (calendarDate != null)
            {
                var date = calendarDate;
                return new DateTime((int)date.YearComponent, (int)date.MonthComponent, (int)date.DayComponent).ToString("d");
            }
            var localTime = ifcDateTimeSelect as IfcLocalTime;
            if (localTime != null)
            {
                var time = localTime;
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

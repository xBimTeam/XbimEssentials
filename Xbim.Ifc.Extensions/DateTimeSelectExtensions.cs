using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions.SelectTypes;
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
                IfcDateAndTime datetime = (ifcDateTimeSelect as IfcDateAndTime);
                int minute = 0;
                if (datetime.TimeComponent.MinuteComponent.HasValue)
                    minute = (int)datetime.TimeComponent.MinuteComponent.Value;
                int second = 0;
                if (datetime.TimeComponent.SecondComponent.HasValue)
                    second = (int)datetime.TimeComponent.SecondComponent.Value;
                return new DateTime(datetime.DateComponent.YearComponent, datetime.DateComponent.MonthComponent, datetime.DateComponent.DayComponent, datetime.TimeComponent.HourComponent, minute, second).ToString();
            }
            if (ifcDateTimeSelect is IfcCalendarDate)
            {
                IfcCalendarDate date = (ifcDateTimeSelect as IfcCalendarDate);
                return new DateTime(date.YearComponent, date.MonthComponent, date.DayComponent).ToString("d");
            }
            if (ifcDateTimeSelect is IfcLocalTime)
            {
                IfcLocalTime time = (ifcDateTimeSelect as IfcLocalTime);
                int minute = 0;
                if (time.MinuteComponent.HasValue)
                    minute = (int)time.MinuteComponent.Value;
                int second = 0;
                if (time.SecondComponent.HasValue)
                    second = (int)time.SecondComponent.Value;
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, time.HourComponent, minute, second).ToString("HH:mm:ss");
            }
            return string.Empty;
        }
    }
}

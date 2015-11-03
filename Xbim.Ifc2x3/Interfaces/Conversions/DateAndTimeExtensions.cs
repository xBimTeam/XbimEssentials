using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.DateTimeResource;

namespace Xbim.Ifc2x3.Interfaces.Conversions
{
    public static class DateAndTimeExtensions
    {
  
        public static string ToISODateTimeString(this IfcDateTimeSelect ifcDateTimeSelect)
        {

            if (ifcDateTimeSelect is IfcDateAndTime)
            {
                return ((IfcDateAndTime)ifcDateTimeSelect).ToISODateTimeString();
            }
            else if (ifcDateTimeSelect is IfcCalendarDate)
            {
                return ((IfcCalendarDate)ifcDateTimeSelect).ToISODateTimeString();               
            }
            else if (ifcDateTimeSelect is IfcLocalTime)
            {
                return ((IfcLocalTime)ifcDateTimeSelect).ToISODateTimeString();                  
            }
            var dateTime = new DateTime(1, 1, 1, 12, 0, 0);
            return dateTime.ToString("yyyy-MM-ddThh:mm:ss");
        }

        public static string ToISODateTimeString(this IfcDateAndTime dateAndTime)
        {          
            var year = 1;
            var month = 1;
            var day = 1;
            var hour = 0;
            var minute = 0;
            double seconds = 0;
            var milliSeconds = 0;
            if(dateAndTime.DateComponent!=null)
            {
                year = (int)dateAndTime.DateComponent.YearComponent;
                month = (int)dateAndTime.DateComponent.MonthComponent;
                day = (int)dateAndTime.DateComponent.DayComponent;
            }
            if (dateAndTime.TimeComponent != null)
            {
                hour = (int)dateAndTime.TimeComponent.HourComponent;
                if (dateAndTime.TimeComponent.MinuteComponent.HasValue)
                    minute = (int)dateAndTime.TimeComponent.MinuteComponent.Value;
                if (dateAndTime.TimeComponent.SecondComponent.HasValue)
                    seconds = dateAndTime.TimeComponent.SecondComponent.Value;
                var secondInt = Math.Truncate(seconds);
                milliSeconds = (int)((seconds - secondInt) * 1000);
                seconds = secondInt;
            }
            
            var dateTime = new DateTime(year, month, day, hour, minute, (int)seconds, milliSeconds);
            return dateTime.ToString("yyyy-MM-ddThh:mm:ss.fff");
        }

        public static string ToISODateTimeString(this IfcCalendarDate calendarDate)
        {
            var dateTime = new DateTime((int)calendarDate.YearComponent, (int)calendarDate.MonthComponent, (int)calendarDate.DayComponent);
            return dateTime.ToString("yyyy-MM-ddThh:mm:ss"); //cannot be any milliseconds as no time component so omit
        }
        public static string ToISODateTimeString(this IfcLocalTime localTime)
        {
            var minute = 0;
            if (localTime.MinuteComponent.HasValue)
                minute = (int)localTime.MinuteComponent.Value;
            double seconds = 0;
            if (localTime.SecondComponent.HasValue)
                seconds = localTime.SecondComponent.Value;
            var secondInt = Math.Truncate(seconds);
            var milliSeconds = (int)((seconds - secondInt) * 1000);
            seconds = secondInt;
            var dateTime = new DateTime(1,1,1,(int)localTime.HourComponent,minute, (int)seconds, milliSeconds);
            return dateTime.ToString("yyyy-MM-ddThh:mm:ss.fff");
        }
    }
}

    


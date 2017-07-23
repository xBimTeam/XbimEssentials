#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    CalendarDateExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.DateTimeResource;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class CalendarDateExtensions
    {
        public static void SetDate(this IfcCalendarDate cd, int day, int month, int year)
        {
            cd.DayComponent = day;
            cd.MonthComponent = month;
            cd.YearComponent = year;
        }

        /// <summary>
        ///   Initilialises date to the current date on this computer expressed in local time
        /// </summary>
        /// <param name = "cd"></param>
        public static void MakeNow(this IfcCalendarDate cd)
        {
            DateTime now = DateTime.Now;
            cd.DayComponent = now.Day;
            cd.MonthComponent = now.Month;
            cd.YearComponent = now.Year;
        }
    }
}
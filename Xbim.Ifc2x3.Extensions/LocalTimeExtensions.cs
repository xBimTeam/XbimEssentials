#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    LocalTimeExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Globalization;
using Xbim.Ifc2x3.DateTimeResource;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class LocalTimeExtensions
    {

        /// <summary>
        ///   Sets the hours minutes and seconds in the local time and sets the time zone and offsets and daylight saving hours to that of this machine now
        /// </summary>
        /// <param name = "lt"></param>
        /// <param name = "hour"></param>
        /// <param name = "minute"></param>
        /// <param name = "second"></param>
        public static void SetTimeLocal(this IfcLocalTime lt, int hour, int minute, int second)
        {
            lt.HourComponent = hour;
            lt.MinuteComponent = minute;
            lt.SecondComponent = second;
        }

        public static void SetTimeLocal(this IfcLocalTime lt, int hour, int minute)
        {
            lt.HourComponent = hour;
            lt.MinuteComponent = minute;
        }

        public static void SetTimeLocal(this IfcLocalTime lt, int hour)
        {
            lt.HourComponent = hour;
        }

        public static void SetTimeLocal(this IfcLocalTime lt, int hour, int minute, int second,
                                        IfcCoordinatedUniversalTimeOffset uctOffset)
        {
            SetTimeLocal(lt, hour, minute, second);
            lt.Zone = uctOffset;
        }

        public static void SetTimeLocal(this IfcLocalTime lt, int hour, int minute, int second,
                                        IfcCoordinatedUniversalTimeOffset uctOffset, int daylightSavingOffset)
        {
            SetTimeLocal(lt, hour, minute, second, uctOffset);
            lt.DaylightSavingOffset = daylightSavingOffset;
        }
    }
}
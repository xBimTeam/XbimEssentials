#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    CoordinatedUniversalTimeOffsetExtensions.cs
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
    public static class CoordinatedUniversalTimeOffsetExtensions
    {
        public static void MakeNow(this IfcCoordinatedUniversalTimeOffset ucto)
        {
            DateTimeOffset localTime = DateTimeOffset.Now;
            ucto.HourOffset = new IfcHourInDay(localTime.Offset.Hours);
            ucto.MinuteOffset = new IfcMinuteInHour(localTime.Offset.Minutes);
            if (localTime.Offset.Hours < 0 || (localTime.Offset.Hours == 0 && localTime.Offset.Minutes < 0))
                ucto.Sense = IfcAheadOrBehind.BEHIND;
            else
                ucto.Sense = IfcAheadOrBehind.AHEAD;
        }
    }
}
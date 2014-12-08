#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAheadOrBehind.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.DateTimeResource
{
    /// <summary>
    ///   An enumeration type that is used to specify whether a local time is ahead or behind of the coordinated universal time.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-41:1992: An enumeration type that is used to specify whether a local time is ahead or behind of the coordinated universal time. 
    ///   NOTE Corresponding STEP name: ahead_or_behind, please refer to ISO/IS 10303-41:1994 for the final definition of the formal standard. 
    ///   HISTORY New type in IFC Release 2.0.
    /// </remarks>
    public enum IfcAheadOrBehind
    {
        /// <summary>
        ///   The time is ahead of coordinated universal time.
        /// </summary>
        AHEAD,

        /// <summary>
        ///   The time is ahead of coordinated universal time.
        /// </summary>
        BEHIND,
    }
}
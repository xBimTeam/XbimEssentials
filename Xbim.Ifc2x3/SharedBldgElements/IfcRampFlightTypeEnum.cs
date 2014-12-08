#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRampFlightTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the different types of linear elements an IfcRampFlightType object can fulfill:
    /// </summary>
    public enum IfcRampFlightTypeEnum
    {
        /// <summary>
        ///   A ramp flight with a straight walking line.
        /// </summary>
        STRAIGHT,

        /// <summary>
        ///   A ramp flight with a circular or elliptic walking line.
        /// </summary>
        SPIRAL,

        /// <summary>
        ///   User-defined ramp flight.
        /// </summary>
        USERDEFINED,

        /// <summary>
        ///   Undefined ramp flight.
        /// </summary>
        NOTDEFINED
    }
}
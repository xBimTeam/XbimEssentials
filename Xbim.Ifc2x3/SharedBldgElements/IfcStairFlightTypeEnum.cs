#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStairFlightTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the different types of stair flights
    /// </summary>
    public enum IfcStairFlightTypeEnum
    {
        /// <summary>
        ///   A stair flight with a straight walking line.
        /// </summary>
        STRAIGHT,

        /// <summary>
        ///   A stair flight with a straight walking line.
        /// </summary>
        WINDER,

        /// <summary>
        ///   stair flight with a circular or elliptic walking line.
        /// </summary>
        SPIRAL,

        /// <summary>
        ///   A stair flight with a curved walking line.
        /// </summary>
        CURVED,

        /// <summary>
        ///   A stair flight with a free form walking line (and outer boundaries).
        /// </summary>
        FREEFORM,

        /// <summary>
        ///   User-defined stair flight .
        /// </summary>
        USERDEFINED,

        /// <summary>
        ///   Undefined stair flight .
        /// </summary>
        NOTDEFINED
    }
}
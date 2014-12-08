#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcWallTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the different types of walls an IfcWallType object can fulfill:
    /// </summary>
    public enum IfcWallTypeEnum
    {
        /// <summary>
        ///   A standard wall, extruded vertically with a constant thickness along the wall path.
        /// </summary>
        STANDARD,

        /// <summary>
        ///   A polygonal wall, extruded vertically, where the wall thickness changes along the wall path.
        /// </summary>
        POLYGONAL,

        /// <summary>
        ///   A shear wall, having a non-rectangular cross section.
        /// </summary>
        SHEAR,
        ELEMENTEDWALL,
        PLUMBINGWALL,

        /// <summary>
        ///   User-defined wall element
        /// </summary>
        USERDEFINED,

        /// <summary>
        ///   Undefined wall element
        /// </summary>
        NOTDEFINED
    }
}
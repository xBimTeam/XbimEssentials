#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcMemberTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the different types of linear elements an IfcMemberType object can fulfill:
    /// </summary>
    public enum IfcMemberTypeEnum
    {
        /// <summary>
        ///   A linear element (usually sloped) often used for bracing of a girder or truss.
        /// </summary>
        BRACE,

        /// <summary>
        ///   Upper or lower longitudinal member of a truss, used horizontally or sloped.
        /// </summary>
        CHORD,

        /// <summary>
        ///   A linear element (usually used horizontally) within a roof structure to connect rafters and posts.
        /// </summary>
        COLLAR,

        /// <summary>
        ///   A linear element within a girder or truss with no further meaning.
        /// </summary>
        MEMBER,

        /// <summary>
        ///   A linear element within a curtain wall system to connect two (or more) panels.
        /// </summary>
        MULLION,

        /// <summary>
        ///   A linear continuous horizontal element in wall framing, e.g. a head piece or a sole plate.
        /// </summary>
        /// <remarks>
        ///   NOTE  This head piece or sole plate shall not be mixed up with planar elements, such as sheets and panels, that are handled as IfcPlate (and IfcPlateType).
        /// </remarks>
        PLATE,

        /// <summary>
        ///   A linear member (usually used vertically) within a roof structure to support purlins.
        /// </summary>
        POST,

        /// <summary>
        ///   A linear element (usually used horizontally) within a roof structure to support rafters
        /// </summary>
        PURLIN,

        /// <summary>
        ///   A linear elements used to support roof slabs or roof covering, usually used with slope.
        /// </summary>
        RAFTER,

        /// <summary>
        ///   A linear element used to support stair or ramp flights, usually used with slope.
        /// </summary>
        STRINGER,

        /// <summary>
        ///   A linear element often used within a girder or truss.
        /// </summary>
        STRUT,

        /// <summary>
        ///   Vertical element in wall framing.
        /// </summary>
        STUD,

        /// <summary>
        ///   User-defined linear element.
        /// </summary>
        USERDEFINED,

        /// <summary>
        ///   Undefined linear element
        /// </summary>
        NOTDEFINED
    }
}
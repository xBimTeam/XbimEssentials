#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBeamTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the different types of linear elements an IfcBeamType object can fulfill:
    /// </summary>
    public enum IfcBeamTypeEnum
    {
        /// <summary>
        ///   A standard beam usually used horizontally.
        /// </summary>
        BEAM,

        /// <summary>
        ///   A beam used to support a floor or ceiling.
        /// </summary>
        JOIST,

        /// <summary>
        ///   A beam or horizontal piece of material over an opening (e.g. door, window).
        /// </summary>
        LINTEL,

        /// <summary>
        ///   A T-shape beam that forms part of a slab construction.
        /// </summary>
        T_BEAM,

        /// <summary>
        ///   User-defined linear beam element.
        /// </summary>
        USERDEFINED,

        /// <summary>
        ///   Undefined linear beam element
        /// </summary>
        NOTDEFINED
    }
}
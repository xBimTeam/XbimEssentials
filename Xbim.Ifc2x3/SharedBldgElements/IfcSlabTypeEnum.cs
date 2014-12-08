#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSlabTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the available predefined types of a slab. The IfcSlabTypeEnum can be used for slab occurrences, IfcSlab, and slab types, IfcSlabType. A special property set definition may be provided for each predefined type.
    /// </summary>
    public enum IfcSlabTypeEnum
    {
        /// <summary>
        ///   The slab is used to represent a floor slab.
        /// </summary>
        FLOOR,

        /// <summary>
        ///   The slab is used to represent a roof slab (either flat or sloped).
        /// </summary>
        ROOF,

        /// <summary>
        ///   The slab is used to represent a landing within a stair or ramp.
        /// </summary>
        LANDING,

        /// <summary>
        ///   The slab is used to represent a floor slab against the ground (and thereby being a part of the foundation)
        /// </summary>
        BASESLAB,
        USERDEFINED,
        NOTDEFINED
    }
}
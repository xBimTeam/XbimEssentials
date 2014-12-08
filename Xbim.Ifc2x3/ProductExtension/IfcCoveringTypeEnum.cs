#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCoveringTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   This enumeration defines the available generic types for IfcCovering.
    /// </summary>
    public enum IfcCoveringTypeEnum
    {
        /// <summary>
        ///   the covering is used to represent a ceiling
        /// </summary>
        CEILING,

        /// <summary>
        ///   the covering is used to represent a flooring
        /// </summary>
        FLOORING,

        /// <summary>
        ///   the covering is used to represent a cladding
        /// </summary>
        CLADDING,

        /// <summary>
        ///   the covering is used to represent a roof
        /// </summary>
        ROOFING,

        /// <summary>
        ///   the covering is used to insulate an element for thermal or acoustic purposes.
        /// </summary>
        INSULATION,

        /// <summary>
        ///   an impervious layer that could be used for e.g. roof covering (below tiling - that may be known as sarking etc.) or as a damp proof course membrane
        /// </summary>
        MEMBRANE,

        /// <summary>
        ///   the covering is used to isolate a distribution element from a space in which it is contained.
        /// </summary>
        SLEEVING,

        /// <summary>
        ///   the covering is used for wrapping particularly of distribution elements using tape.
        /// </summary>
        WRAPPING,
        USERDEFINED,
        NOTDEFINED
    }
}
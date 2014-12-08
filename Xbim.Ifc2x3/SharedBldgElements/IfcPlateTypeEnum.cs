#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPlateTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    public enum IfcPlateTypeEnum
    {
        /// <summary>
        ///   A planar element within a curtain wall, often consisting of a frame with fixed glazing.
        /// </summary>
        CURTAIN_PANEL,

        /// <summary>
        ///   A planar, flat and thin element, comes usually as metal sheet, and is often used as an additonal part within an assembly.
        /// </summary>
        SHEET,

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
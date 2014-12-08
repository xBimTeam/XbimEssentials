#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcInternalOrExternalEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.ProductExtension
{
    /// <summary>
    ///   Definition from IAI: This enumeration defines the different types of spaces or space boundaries in terms of either being inside the building or outside the building.
    /// </summary>
    public enum IfcInternalOrExternalEnum
    {
        /// <summary>
        ///   Inside the building
        /// </summary>
        /// <remarks>
        ///   IfcSpace The space is an internal space, fully enclosed by physical boundaries (directly or indirectly through adjacent spaces). 
        ///   IfcSpaceBoundary The space boundary faces to the inside of an internal space.
        /// </remarks>
        INTERNAL,

        /// <summary>
        ///   Outside the building
        /// </summary>
        /// <remarks>
        ///   IfcSpace The space is an external space, not (or only partially) enclosed by physical boundaries. 
        ///   IfcSpaceBoundary The space boundary faces to the outer space, or the inside of an external space.
        /// </remarks>
        EXTERNAL,

        /// <summary>
        ///   No information available.
        /// </summary>
        NOTDEFINED
    }
}
#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralSurfaceTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    /// <summary>
    ///   This type definition shall be used to distinguish between different types of structural surface members, 
    ///   such as the typical mechanical function of walls, slabs and shells.
    /// </summary>
    public enum IfcStructuralSurfaceTypeEnum
    {
        BENDING_ELEMENT,
        MEMBRANE_ELEMENT,
        SHELL,
        USERDEFINED,
        NOTDEFINED
    }
}
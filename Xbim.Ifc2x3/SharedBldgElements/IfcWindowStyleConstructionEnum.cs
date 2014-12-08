#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcWindowStyleConstructionEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the basic types of construction of windows. The construction type relates to the main material (or material combination) used for making the window.
    /// </summary>
    public enum IfcWindowStyleConstructionEnum
    {
        ALUMINIUM,
        HIGH_GRADE_STEEL,
        STEEL,
        WOOD,
        ALUMINIUM_WOOD,
        PLASTIC,
        OTHER_CONSTRUCTION,
        NOTDEFINED
    }
}
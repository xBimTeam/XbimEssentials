#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDuctFittingTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.HVACDomain
{
    /// <summary>
    ///   This enumeration is used to identify the primary purpose of a duct fitting. 
    ///   This is a very basic categorization mechanism to generically identify the duct fitting type. 
    ///   Subcategories of duct fittings are not enumerated.
    /// </summary>
    public enum IfcDuctFittingTypeEnum
    {
        BEND,
        CONNECTOR,
        ENTRY,
        EXIT,
        JUNCTION,
        OBSTRUCTION,
        TRANSITION,
        USERDEFINED,
        NOTDEFINED
    }
}
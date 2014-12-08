#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcConnectionTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the different ways how path based elements (here IfcWallStandardCase) can connect..
    /// </summary>
    public enum IfcConnectionTypeEnum
    {
        ATPATH,
        ATSTART,
        ATEND,
        NOTDEFINED
    }
}
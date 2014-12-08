#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcTankTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.HVACDomain
{
    /// <summary>
    ///   Enumeration defining the typical types of tanks.
    /// </summary>
    public enum IfcTankTypeEnum
    {
        PREFORMED,
        SECTIONAL,
        EXPANSION,
        PRESSUREVESSEL,
        USERDEFINED,
        NOTDEFINED
    }
}
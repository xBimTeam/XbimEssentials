#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcColumnTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the different types of linear elements an IfcColumnType object can fulfill:
    /// </summary>
    public enum IfcColumnTypeEnum
    {
        /// <summary>
        ///   A standard column element usually used vertically.
        /// </summary>
        COLUMN,
        USERDEFINED,
        NOTDEFINED
    }
}
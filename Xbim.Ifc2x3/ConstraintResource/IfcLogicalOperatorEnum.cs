#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcLogicalOperatorEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.ConstraintResource
{
    /// <summary>
    ///   IfcLogicalOperatorEnum is an enumeration that defines the logical operators that may be applied 
    ///   for the satisfaction of more than one constraint at a time.
    /// </summary>
    public enum IfcLogicalOperatorEnum
    {
        LOGICALAND,
        LOGICALOR,
        LOGICALNOTAND,
        LOGICALNOTOR
    }
}
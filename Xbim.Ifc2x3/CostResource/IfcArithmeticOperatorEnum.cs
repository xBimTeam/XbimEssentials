#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcArithmeticOperatorEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.CostResource
{
    /// <summary>
    ///   The IfcArithmeticOperatorEnum specifies the form of arithmetical operation implied by the relationship.
    /// </summary>
    /// <remarks>
    ///   Use Definitions
    ///   There can be only one arithmetic operator for each applied value relationship. This is to enforce arithmetic consistency. Given this consistency, the cardinality of the IfcAppliedValueRelationship.Components attribute is a set of one to many applied values that are components of an applied value. 
    ///   EXPRESS specification:
    /// </remarks>
    public enum IfcArithmeticOperatorEnum
    {
        ADD,
        DIVIDE,
        MULTIPLY,
        SUBTRACT
    }
}
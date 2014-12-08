#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcAnalysisTheoryTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    public enum IfcAnalysisTheoryTypeEnum
    {
        FIRST_ORDER_THEORY,
        SECOND_ORDER_THEORY,
        THIRD_ORDER_THEORY,
        FULL_NONLINEAR_THEORY,
        USERDEFINED,
        NOTDEFINED
    }
}
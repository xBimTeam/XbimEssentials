#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStructuralCurveTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    public enum IfcStructuralCurveTypeEnum
    {
        RIGID_JOINED_MEMBER,
        PIN_JOINED_MEMBER,
        CABLE,
        TENSION_MEMBER,
        COMPRESSION_MEMBER,
        USERDEFINED,
        NOTDEFINED
    }
}
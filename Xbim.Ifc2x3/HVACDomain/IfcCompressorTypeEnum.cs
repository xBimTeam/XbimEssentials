#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcCompressorTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.HVACDomain
{
    /// <summary>
    ///   Types of compressors.
    /// </summary>
    public enum IfcCompressorTypeEnum
    {
        DYNAMIC,
        RECIPROCATING,
        ROTARY,
        SCROLL,
        TROCHOIDAL,
        SINGLESTAGE,
        BOOSTER,
        OPENTYPE,
        HERMETIC,
        SEMIHERMETIC,
        WELDEDSHELLHERMETIC,
        ROLLINGPISTON,
        ROTARYVANE,
        SINGLESCREW,
        TWINSCREW,
        USERDEFINED,
        NOTDEFINED
    }
}
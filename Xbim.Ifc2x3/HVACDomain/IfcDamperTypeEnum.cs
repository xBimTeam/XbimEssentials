#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcDamperTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.HVACDomain
{
    public enum IfcDamperTypeEnum
    {
        CONTROLDAMPER,
        FIREDAMPER,
        SMOKEDAMPER,
        FIRESMOKEDAMPER,
        BACKDRAFTDAMPER,
        RELIEFDAMPER,
        BLASTDAMPER,
        GRAVITYDAMPER,
        GRAVITYRELIEFDAMPER,
        BALANCINGDAMPER,
        FUMEHOODEXHAUST,
        USERDEFINED,
        NOTDEFINED
    }
}
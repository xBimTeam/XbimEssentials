#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcHumidifierTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.HVACDomain
{
    public enum IfcHumidifierTypeEnum
    {
        STEAMINJECTION,
        ADIABATICAIRWASHER,
        ADIABATICPAN,
        ADIABATICWETTEDELEMENT,
        ADIABATICATOMIZING,
        ADIABATICULTRASONIC,
        ADIABATICRIGIDMEDIA,
        ADIABATICCOMPRESSEDAIRNOZZLE,
        ASSISTEDELECTRIC,
        ASSISTEDNATURALGAS,
        ASSISTEDPROPANE,
        ASSISTEDBUTANE,
        ASSISTEDSTEAM,
        USERDEFINED,
        NOTDEFINED
    }
}
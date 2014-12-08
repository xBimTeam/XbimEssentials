#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcEvaporativeCoolerTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.HVACDomain
{
    public enum IfcEvaporativeCoolerTypeEnum
    {
        DIRECTEVAPORATIVERANDOMMEDIAAIRCOOLER,
        DIRECTEVAPORATIVERIGIDMEDIAAIRCOOLER,
        DIRECTEVAPORATIVESLINGERSPACKAGEDAIRCOOLER,
        DIRECTEVAPORATIVEPACKAGEDROTARYAIRCOOLER,
        DIRECTEVAPORATIVEAIRWASHER,
        INDIRECTEVAPORATIVEPACKAGEAIRCOOLER,
        INDIRECTEVAPORATIVEWETCOIL,
        INDIRECTEVAPORATIVECOOLINGTOWERORCOILCOOLER,
        INDIRECTDIRECTCOMBINATION,
        USERDEFINED,
        NOTDEFINED
    }
}
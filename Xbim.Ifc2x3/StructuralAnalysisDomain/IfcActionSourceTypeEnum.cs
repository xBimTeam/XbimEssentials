#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcActionSourceTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.StructuralAnalysisDomain
{
    public enum IfcActionSourceTypeEnum
    {
        DEAD_LOAD_G,
        COMPLETION_G1,
        LIVE_LOAD_Q,
        SNOW_S,
        WIND_W,
        PRESTRESSING_P,
        SETTLEMENT_U,
        TEMPERATURE_T,
        EARTHQUAKE_E,
        FIRE,
        IMPULSE,
        IMPACT,
        TRANSPORT,
        ERECTION,
        PROPPING,
        SYSTEM_IMPERFECTION,
        SHRINKAGE,
        CREEP,
        LACK_OF_FIT,
        BUOYANCY,
        ICE,
        CURRENT,
        WAVE,
        RAIN,
        BRAKES,
        USERDEFINED,
        NOTDEFINED
    }
}
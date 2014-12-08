#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcProtectiveDeviceTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.ElectricalDomain
{
    /// <summary>
    ///   Defines the range of different types of protective device available.
    /// </summary>
    public enum IfcProtectiveDeviceTypeEnum
    {
        FUSEDISCONNECTOR,
        CIRCUITBREAKER,
        EARTHFAILUREDEVICE,
        RESIDUALCURRENTCIRCUITBREAKER,
        RESIDUALCURRENTSWITCH,
        VARISTOR,
        USERDEFINED,
        NOTDEFINED
    }
}
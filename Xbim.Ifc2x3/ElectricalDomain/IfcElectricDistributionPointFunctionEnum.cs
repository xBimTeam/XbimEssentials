#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcElectricDistributionPointFunctionEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.ElectricalDomain
{
    /// <summary>
    ///   Defines the range of different functions that an electric distribution point can fulfil.
    /// </summary>
    public enum IfcElectricDistributionPointFunctionEnum
    {
        ALARMPANEL,
        CONSUMERUNIT,
        CONTROLPANEL,
        DISTRIBUTIONBOARD,
        GASDETECTORPANEL,
        INDICATORPANEL,
        MIMICPANEL,
        MOTORCONTROLCENTRE,
        SWITCHBOARD,
        USERDEFINED,
        NOTDEFINED
    }
}
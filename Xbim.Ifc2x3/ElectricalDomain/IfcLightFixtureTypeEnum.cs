#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcLightFixtureTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.ElectricalDomain
{
    /// <summary>
    ///   Defines the range of different types of light fixture available.
    /// </summary>
    public enum IfcLightFixtureTypeEnum
    {
        /// <summary>
        ///   A light fixture that is considered to emit light at a point.
        /// </summary>
        POINTSOURCE,

        /// <summary>
        ///   A light fixture that is considered to emit light from a linear or area face and in a direction.
        /// </summary>
        DIRECTIONSOURCE,
        USERDEFINED,
        NOTDEFINED
    }
}
#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcThermalLoadTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgServiceElements
{
    /// <summary>
    ///   This enumeration defines the type of thermal load for spaces or zones, as derived from various use cases:
    /// </summary>
    public enum IfcThermalLoadTypeEnum
    {
        /// <summary>
        ///   Energy added or removed from air that affects its temperature.
        /// </summary>
        SENSIBLE,

        /// <summary>
        ///   Energy added or removed from air that affects its humidity or concentration of water vapor.
        /// </summary>
        LATENT,

        /// <summary>
        ///   Electromagnetic energy added or removed by emmission or absorption.
        /// </summary>
        RADIANT,

        /// <summary>
        ///   Undefined thermal load type.
        /// </summary>
        NOTDEFINED
    }
}
#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcEvaporatorTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.HVACDomain
{
    /// <summary>
    ///   Enumeration defining the typical types of evaporators.
    /// </summary>
    public enum IfcEvaporatorTypeEnum
    {
        /// <summary>
        ///   Direct-expansion evaporator where a refrigerant evaporates inside a series of baffles that channel the fluid throughout the shell side.
        /// </summary>
        DIRECTEXPANSIONSHELLANDTUBE,

        /// <summary>
        ///   Direct-expansion evaporator where a refrigerant evaporates inside one or more pairs of coaxial tubes.
        /// </summary>
        DIRECTEXPANSIONTUBEINTUBE,

        /// <summary>
        ///   Direct-expansion evaporator where a refrigerant evaporates inside plates brazed or welded together to make up an assembly of separate channels.
        /// </summary>
        DIRECTEXPANSIONBRAZEDPLATE,

        /// <summary>
        ///   Evaporator in which refrigerant evaporates outside tubes.
        /// </summary>
        FLOODEDSHELLANDTUBE,

        /// <summary>
        ///   Evaporator in which refrigerant evaporates inside a simple coiled tube immersed in the fluid to be cooled.
        /// </summary>
        SHELLANDCOIL,

        /// <summary>
        ///   User-defined evaporator type.
        /// </summary>
        USERDEFINED,

        /// <summary>
        ///   Undefined evaporator type.
        /// </summary>
        NOTDEFINED
    }
}
#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSpaceHeaterTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.HVACDomain
{
    /// <summary>
    ///   Enumeration defining the functional type of space heater.
    /// </summary>
    public enum IfcSpaceHeaterTypeEnum
    {
        /// <summary>
        ///   Sectional type radiator typically fabricated from welded sheet metal sections and resembling free standing cast-iron radiators.
        /// </summary>
        SECTIONALRADIATOR,

        /// <summary>
        ///   Panel type radiator typically fabricated with flat panels, with or without an exposed extended fin surface attached to the rear for increased output.
        /// </summary>
        PANELRADIATOR,

        /// <summary>
        ///   Tubular type radiator consisting of supply and return headers with interconnecting parallel tubes in a wide variety of lengths and heights.
        /// </summary>
        TUBULARRADIATOR,

        /// <summary>
        ///   A heat-distributing unit that operates with gravity-circulated air.
        /// </summary>
        CONVECTOR,

        /// <summary>
        ///   Baseboard heater designed for installation along the bottom of walls in place of the conventional baseboard.
        /// </summary>
        BASEBOARDHEATER,

        /// <summary>
        ///   Fin-tube heater typically fabricated from metallic tubing, with metallic fins bonded to the tube.
        /// </summary>
        FINNEDTUBEUNIT,

        /// <summary>
        ///   An assembly typically consisting of a fan, a motor, and a heating element.
        /// </summary>
        UNITHEATER,

        /// <summary>
        ///   User-defined space heater type.
        /// </summary>
        USERDEFINED,

        /// <summary>
        ///   Undefined space heater type.
        /// </summary>
        NOTDEFINED
    }
}
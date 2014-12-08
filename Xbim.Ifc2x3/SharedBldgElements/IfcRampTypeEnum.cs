#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRampTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the basic configuration of the ramp type in terms of the number and shape of ramp flights.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This enumeration defines the basic configuration of the ramp type in terms of the number and shape of ramp flights. The type also distinguished turns by landings. In addition the subdivision of the straight and changing direction ramps is included. The ramp configurations are given for ramps without and with one and two landings.
    ///   Ramps which are subdivided into more than two landings have to be defined by the geometry only. Also ramps with non-regular shapes have to be defined by the geometry only. The type of such ramps is USERDEFINED.
    ///   HISTORY New Enumeration in IFC Release 2.0.
    /// </remarks>
    public enum IfcRampTypeEnum
    {
        /// <summary>
        ///   A ramp - which is a sloping floor, walk, or roadway - connecting two levels. The straight ramp consists of one straight flight without turns or winders.
        /// </summary>
        STRAIGHT_RUN_RAMP,

        /// <summary>
        ///   A straight ramp consisting of two straight flights without turns but with one landing.
        /// </summary>
        TWO_STRAIGHT_RUN_RAMP,

        /// <summary>
        ///   A ramp making a 90° turn, consisting of two straight flights connected by a quarterspace landing. The direction of the turn is determined by the walking line.
        /// </summary>
        QUARTER_TURN_RAMP,

        /// <summary>
        ///   A ramp making a 180° turn, consisting of three straight flights connected by two quarterspace landings. The direction of the turn is determined by the walking line.
        /// </summary>
        TWO_QUARTER_TURN_RAMP,

        /// <summary>
        ///   A ramp making a 180° turn, consisting of two straight flights connected by a halfspace landing. The orientation of the turn is determined by the walking line.
        /// </summary>
        HALF_TURN_RAMP,

        /// <summary>
        ///   A ramp constructed around a circular or elliptical well without newels and landings.
        /// </summary>
        SPIRAL_RAMP,

        /// <summary>
        ///   Free form ramp (user defined operation type)
        /// </summary>
        USERDEFINED,
        NOTDEFINED
    }
}
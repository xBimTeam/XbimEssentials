#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcStairTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   This enumeration defines the basic configuration of the stair type in terms of the number of stair flights and the number of landings.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: This enumeration defines the basic configuration of the stair type in terms of the number of stair flights and the number of landings. The type also distinguished turns by windings or by landings. In addition the subdivision of the straight and changing direction stairs is included. The stair configurations are given for stairs without and with one, two or three landings.
    ///   Stairs which are subdivided into more than three landings have to be defined by the geometry only. Also stairs with non-regular shapes have to be defined by the geometry only. The type of such stairs is OTHEROPERATION.
    ///   HISTORY New Enumeration in IFC Release 2.0.
    /// </remarks>
    public enum IfcStairTypeEnum
    {
        /// <summary>
        ///   A stair extending from one level to another without turns or winders. The stair consists of one straight flight.
        /// </summary>
        STRAIGHT_RUN_STAIR,

        /// <summary>
        ///   A straight stair consisting of two straight flights without turns but with one landing.
        /// </summary>
        TWO_STRAIGHT_RUN_STAIR,

        /// <summary>
        ///   A stair consisting of one flight with a quarter winder, which is making a 90° turn. The direction of the turn is determined by the walking line.
        /// </summary>
        QUARTER_WINDING_STAIR,

        /// <summary>
        ///   A stair making a 90° turn, consisting of two straight flights connected by a quarterspace landing. The direction of the turn is determined by the walking line.
        /// </summary>
        QUARTER_TURN_STAIR,

        /// <summary>
        ///   A stair consisting of one flight with one half winder, which makes a 180° turn. The orientation of the turn is determined by the walking line.
        /// </summary>
        HALF_WINDING_STAIR,

        /// <summary>
        ///   A stair making a 180° turn, consisting of two straight flights connected by a halfspace landing. The orientation of the turn is determined by the walking line.
        /// </summary>
        HALF_TURN_STAIR,

        /// <summary>
        ///   A stair making a 180° turn, consisting of three straight flights connected by two quarterspace landings. The direction of the turns is determined by the walking line.
        /// </summary>
        TWO_QUARTER_WINDING_STAIR,

        /// <summary>
        ///   A stair making a 180° turn, consisting of three straight flights connected by two quarterspace landings. The direction of the turns is determined by the walking line.
        /// </summary>
        TWO_QUARTER_TURN_STAIR,

        /// <summary>
        ///   A stair consisting of one flight with three quarter winders, which make a 90° turn. The stair makes a 270° turn. The direction of the turns is determined by the walking line.
        /// </summary>
        THREE_QUARTER_WINDING_STAIR,

        /// <summary>
        ///   A stair making a 270° turn, consisting of four straight flights connected by three quarterspace landings. The direction of the turns is determined by the walking line.
        /// </summary>
        THREE_QUARTER_TURN_STAIR,

        /// <summary>
        ///   A stair constructed with winders around a circular newel often without landings. Depending on outer boundary it can be either a circular, elliptical or rectancular spiral stair. The orientation of the winding stairs is determined by the walking line.
        /// </summary>
        SPIRAL_STAIR,

        /// <summary>
        ///   A stair having one straight flight to a wide quarterspace landing, and two side flights from that landing into opposite directions. The stair is making a 90° turn. The direction of traffic is determined by the walking line.
        /// </summary>
        DOUBLE_RETURN_STAIR,

        /// <summary>
        ///   A stair extending from one level to another without turns or winders. The stair is consisting of one curved flight.
        /// </summary>
        CURVED_RUN_STAIR,

        /// <summary>
        ///   A curved stair consisting of two curved flights without turns but with one landing.
        /// </summary>
        TWO_CURVED_RUN_STAIR,

        /// <summary>
        ///   Free form stair (user defined operation type)
        /// </summary>
        USERDEFINED,
        NOTDEFINED
    }
}
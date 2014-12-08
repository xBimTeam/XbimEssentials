#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRailingTypeEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.SharedBldgElements
{
    /// <summary>
    ///   Enumeration defining the valid types of railings that can be predefined using the enumeration values.
    /// </summary>
    public enum IfcRailingTypeEnum
    {
        /// <summary>
        ///   A type of railing designed to serve as an optional structural support for loads applied by human occupants (at hand height). Generally located adjacent to ramps and stairs. Generally floor or wall mounted.
        /// </summary>
        HANDRAIL,

        /// <summary>
        ///   A type of railing designed to guard human occupants from falling off a stair, ramp or landing where there is a vertical drop at the edge of such floors/landings.
        /// </summary>
        GUARDRAIL,

        /// <summary>
        ///   Similar to the definitions of a guardrail except the location is at the edge of a floor, rather then a stair or ramp. Examples are balustrates at roof-tops or balconies.
        /// </summary>
        BALUSTRADE,

        /// <summary>
        ///   User-defined railing element, a term to identify the user type is given by the attribute IfcRailing.ObjectType.
        /// </summary>
        USERDEFINED,

        /// <summary>
        ///   Undefined railing element, no type information available.
        /// </summary>
        NOTDEFINED
    }
}
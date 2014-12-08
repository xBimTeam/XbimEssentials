#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSIPrefix.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.MeasureResource
{
    /// <summary>
    ///   An SI prefix is the name of a prefix that may be associated with an SI unit. 
    ///   The definitions of SI prefixes are specified in ISO 1000 (clause 3).
    /// </summary>
    public enum IfcSIPrefix
    {
        /// <summary>
        ///   10^18
        /// </summary>
        EXA,

        /// <summary>
        ///   10^15
        /// </summary>
        PETA,

        /// <summary>
        ///   10^12
        /// </summary>
        TERA,

        /// <summary>
        ///   10^9
        /// </summary>
        GIGA,

        /// <summary>
        ///   10^6
        /// </summary>
        MEGA,

        /// <summary>
        ///   10^3
        /// </summary>
        KILO,

        /// <summary>
        ///   10^2
        /// </summary>
        HECTO,

        /// <summary>
        ///   10
        /// </summary>
        DECA,

        /// <summary>
        ///   10^-1
        /// </summary>
        DECI,

        /// <summary>
        ///   10^-2
        /// </summary>
        CENTI,

        /// <summary>
        ///   10^-3
        /// </summary>
        MILLI,

        /// <summary>
        ///   10^-6
        /// </summary>
        MICRO,

        /// <summary>
        ///   10^-9
        /// </summary>
        NANO,

        /// <summary>
        ///   10^-12
        /// </summary>
        PICO,

        /// <summary>
        ///   10^-15
        /// </summary>
        FEMTO,

        /// <summary>
        ///   10^-18
        /// </summary>
        ATTO
    }
}
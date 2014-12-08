using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc2x3.ProcessExtensions
{
    /// <summary>
    /// An IfcWorkControlTypeEnum is an enumeration data type that specifies the types of work control 
    /// from which the relevant control can be selected.
    /// </summary>
    /// <remarks>Definition from IAI: An IfcWorkControlTypeEnum is an enumeration data type that specifies the types of work control 
    /// from which the relevant control can be selected. HISTORY: New type in IFC Release 2.0
    /// </remarks>
    public enum IfcWorkControlTypeEnum
    {
        /// <summary>
        /// A control in which actual items undertaken are indicated.
        /// </summary>
        ACTUAL,
        /// <summary>
        /// A control that is a baseline from which changes that are made later can be recognized.
        /// </summary>
        BASELINE,
        /// <summary>
        /// A control showing planned items.
        /// </summary>
        PLANNED,
        /// <summary>
        ///   A user defined specification is given...
        /// </summary>
        USERDEFINED,
        /// <summary>
        ///   No specification given.
        /// </summary>
        NOTDEFINED
    }
}

﻿#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    ValidationFlags.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;

#endregion

namespace Xbim.Common.Enumerations
{
    /// <summary>
    ///   Set the level of the Validation, multiple flags can be set
    /// </summary>
    [Flags]
    public enum ValidationFlags
    {
        /// <summary>
        ///   Executes no validation checks
        /// </summary>
        None = 0,

        /// <summary>
        ///   Checks all validation levels
        /// </summary>
        All = 1,

        /// <summary>
        ///   Checks that all property values meet their Ifc Schema Constraints
        /// </summary>
        Properties = 2,

        /// <summary>
        ///   Checks all inverse realationships meet their Ifc Schema Constraints
        /// </summary>
        Inverses = 4
    }
}
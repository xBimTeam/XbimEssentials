#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelConnects.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   A connectivity relationship (IfcRelConnects) that connects objects under some criteria.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: A connectivity relationship (IfcRelConnects) that connects objects under some criteria. As a general connectivity it does not imply constraints, however subtypes of the relationship define the applicable object types for the connectivity relationship and the semantics of the particular connectivity.
    ///   HISTORY: New entity in IFC Release 2x.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcRelConnects : IfcRelationship
    {
    }
}
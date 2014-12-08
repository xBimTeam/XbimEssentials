#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelationship.cs
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
    ///   The abstract generalization of all objectified relationships in IFC.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The abstract generalization of all objectified relationships in IFC. Objectified relationships are the preferred way to handle relationships among objects. This allows to keep relationship specific properties directly at the relationship and opens the possibility to later handle relationship specific behavior. 
    ///   There are two different types of relationships, 1-to-1 relationships and 1-to-many relationship. used within the subtypes of IfcRelationship. The following convention applies to all subtypes:
    ///   The two sides of the objectified relationship are named 
    ///   - Relating+name of relating object and 
    ///   - Related+name of related object
    ///   In case of the 1-to-many relationship, the related side of the relationship shall be an aggregate SET 1:N 
    ///   HISTORY: New entitiy in IFC Release 1.0.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcRelationship : IfcRoot
    {
    }
}
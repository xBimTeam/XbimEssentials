#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelNests.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Linq;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.Kernel
{
    /// <summary>
    ///   nesting relationship IfcRelNests is a special type of the general composition/decomposition (or whole/part) relationship IfcRelDecomposes.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The nesting relationship IfcRelNests is a special type of the general composition/decomposition (or whole/part) relationship IfcRelDecomposes. The nesting relationship can be applied to all subtypes of object, however it requires both the whole and the part to be of the same object type.
    ///   EXAMPLE: A nesting of costs is the composition of a complex cost from other costs. A nesting of work tasks is the composition of an overall work task from more specific work tasks. In all cases the whole has the same type as the parts.
    ///   Decompositions imply a dependency, i.e. the definition of the whole depends on the definition of the parts and the parts depend on the existence of the whole. The behaviour that is implied from the dependency has to be established inside the applications.
    ///   HISTORY: New entity in IFC Release 2.0.
    ///   Formal Propositions:
    ///   WR1   :   The type of the RelatingObject shall always be the same as the type of each RelatedObject, i.e. the RelatingObject and all RelatedObject's are of the same type.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelNests : IfcRelDecomposes
    {
        public override string WhereRule()
        {
            if (RelatedObjects.Count(r => r.GetType() == RelatingObject.GetType()) != RelatedObjects.Count)
                return
                    "WR1 RelNests : The type of the RelatingObject shall always be the same as the type of each RelatedObject, i.e. the RelatingObject and all RelatedObject's are of the same type.\n";
            else return "";
        }
    }
}
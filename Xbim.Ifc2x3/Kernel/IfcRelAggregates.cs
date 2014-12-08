#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcRelAggregates.cs
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
    ///   The aggregation relationship IfcRelAggregates is a special type of the general composition/decomposition (or whole/part) relationship IfcRelDecomposes.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The aggregation relationship IfcRelAggregates is a special type of the general composition/decomposition (or whole/part) relationship IfcRelDecomposes. The aggregation relationship can be applied to all subtypes of object.
    ///   Some further specializations of decomposition may imply additional constraints and meanings, such as the requirement of aggregates to represent physical containment. In cases of physical containment the representation (within the same representation context) of the whole can be taken from the sum of the representations of the parts.
    ///   EXAMPLE: A roof is the aggregation of the roof elements, such as roof slabs, rafters, purlins, etc. Within the same representation context, e.g. the detailed geometric representation, the shape representation of the roof is given by the shape representation of its parts
    ///   Decompositions imply a dependency, i.e. the definition of the whole depends on the definition of the parts and the parts depend on the existence of the whole. The behaviour that is implied from the dependency has to be established inside the applications.
    ///   HISTORY: New entity in IFC Release 2x.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcRelAggregates : IfcRelDecomposes
    {
        public override string WhereRule()
        {
            return "";
        }
    }
}
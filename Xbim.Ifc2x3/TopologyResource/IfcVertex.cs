#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcVertex.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.TopologyResource
{
    /// <summary>
    ///   A vertex is the topological construct corresponding to a point. It has dimensionality 0 and extent 0. The domain of a vertex, if present, 
    ///   is a point in m dimensional real space RM; this is represented by the vertex point subtype.
    /// </summary>
    [IfcPersistedEntityAttribute]
    public class IfcVertex : IfcTopologicalRepresentationItem
    {
        public override string WhereRule()
        {
            return "";
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
        }
    }
}
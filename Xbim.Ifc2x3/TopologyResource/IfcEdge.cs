#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcEdge.cs
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
    ///   An edge is the topological construct corresponding to the connection of two vertices.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: An edge is the topological construct corresponding to the connection of two vertices. More abstractly, it may stand for a logical relationship between two vertices. The domain of an edge, if present, is a finite, non-self-intersecting open curve in RM, that is, a connected 1-dimensional manifold. The bounds of an edge are two vertices, which need not be distinct. The edge is oriented by choosing its traversal direction to run from the first to the second vertex. If the two vertices are the same, the edge is a self loop. The domain of the edge does not include its bounds, and 0 ≤ Ξ ≤ ∞. Associated with an edge may be a geometric curve to locate the edge in a coordinate space; this is represented by the edge curve (IfcEdgeCurve) subtype. The curve shall be finite and non-self-intersecting within the domain of the edge. An edge is a graph, so its multiplicity M and graph genus Ge may be determined by the graph traversal algorithm. Since M = E = 1, the Euler equation (1) reduces in the case to 
    ///   where V = 1 or 2, and Ge = 1 or 0. Specifically, the topological edge defining data shall satisfy: 
    ///   - an edge has two vertices 
    ///  
    ///   - the vertices need not be distinct 
    ///  
    ///   - Equation (2) shall hold. 
    ///  
    ///   NOTE Corresponding STEP entity: edge. Please refer to ISO/IS 10303-42:1994, p. 130 for the final definition of the formal standard. 
    ///   HISTORY New Entity in IFC Release 2.0
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcEdge : IfcTopologicalRepresentationItem
    {
        private IfcVertex _edgeStart;
        private IfcVertex _edgeEnd;

        /// <summary>
        ///   Start point (vertex) of the edge.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcVertex EdgeStart
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _edgeStart;
            }
            set { this.SetModelValue(this, ref _edgeStart, value, v => EdgeStart = v, "EdgeStart"); }
        }

        /// <summary>
        ///   End point (vertex) of the edge. The same vertex can be used for both EdgeStart and EdgeEnd.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcVertex EdgeEnd
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _edgeEnd;
            }
            set { this.SetModelValue(this, ref _edgeEnd, value, v => EdgeEnd = v, "EdgeEnd"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _edgeStart = (IfcVertex) value.EntityVal;
                    break;
                case 1:
                    _edgeEnd = (IfcVertex) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            return "";
        }
    }
}
#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcOrientedEdge.cs
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
    ///   An oriented edge is an edge constructed from another edge and contains a BOOLEAN direction flag to indicate whether or not the orientation of the constructed edge agrees with the orientation of the original edge.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: An oriented edge is an edge constructed from another edge and contains a BOOLEAN direction flag to indicate whether or not the orientation of the constructed edge agrees with the orientation of the original edge. Except for perhaps orientation, the oriented edge is equivalent to the original edge. 
    ///   NOTE A common practice is solid modelling systems is to have an entity that represents the "use" or "traversal" of an edge. This "use" entity explicitly represents the requirement in a manifold solid that each edge must be traversed exactly twice, once in each direction. The "use" functionality is provided by the edge subtype oriented edge.
    ///   NOTE: Corresponding STEP entity: oriented_edge. Please refer to ISO/IS 10303-42:1994, p. 133 for the final definition of the formal standard. 
    ///   HISTORY: New Entity in IFC Release 2.0
    ///   Formal Propositions:
    ///   WR1   :   The edge element shall not be an oriented edge.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcOrientedEdge : IfcEdge
    {
        private bool _orientation = true;
        private IfcEdge _edgeElement;

        /// <summary>
        ///   Edge entity used to construct this oriented edge.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcEdge EdgeElement
        {
            get { return _edgeElement; }
            set { _edgeElement = value; }
        }

        /// <summary>
        ///   If TRUE the topological orientation as used coincides with the orientation from start vertex to end vertex of the edge element. If FALSE otherwise.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public bool Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        /// <summary>
        ///   The start vertex of the oriented edge. It derives from the vertices of the edge element after taking account of the orientation.
        /// </summary>
        public new IfcVertex EdgeStart
        {
            get { return _orientation ? _edgeElement.EdgeStart : _edgeElement.EdgeEnd; }
        }

        /// <summary>
        ///   The end vertex of the oriented edge. It derives from the vertices of the edge element after taking account of the orientation.
        /// </summary>
        public new IfcVertex EdgeEnd
        {
            get { return _orientation ? _edgeElement.EdgeEnd : _edgeElement.EdgeStart; }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                case 1:
                    base.IfcParse(propIndex, value);
                    break;
                case 2:
                    _edgeElement = (IfcEdge) value.EntityVal;
                    break;
                case 3:
                    _orientation = (bool) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
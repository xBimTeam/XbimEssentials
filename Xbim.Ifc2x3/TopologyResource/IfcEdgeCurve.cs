#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcEdgeCurve.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.TopologyResource
{
    /// <summary>
    ///   An edge curve is a special subtype of edge which has its geometry fully defined.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: An edge curve is a special subtype of edge which has its geometry fully defined. The geometry is defined by associating the edge with a curve which may be unbounded. As the topological and geometric directions may be opposed, an indicator (same sense) is used to identify whether the edge and curve directions agree or are opposed. The Boolean value indicates whether the curve direction agrees with (TRUE) or is in the opposite direction (FALSE) to the edge direction. Any geometry associated with the vertices of the edge shall be consistent with the edge geometry. 
    ///   NOTE: Corresponding STEP entity: edge_curve. Please refer to ISO/IS 10303-42:1994, p. 132 for the final definition of the formal standard. Due to the general IFC model specification rule not to use multiple inheritance, the subtype relationship to geometric_representation_item is not included.
    ///   HISTORY: New Entity in IFC Release 2.x. 
    ///   Informal propositions:
    ///   The domain of the edge curve is formally defined to be the domain of its edge geometry as trimmed by the vertices. This domain does not include the vertices. 
    ///   An edge curve has non-zero finite extent. 
    ///   An edge curve is a manifold. 
    ///   An edge curve is arcwise connected. 
    ///   The edge start is not a part of the edge domain. 
    ///   The edge end is not a part of the edge domain. 
    ///   Vertex geometry shall be consistent with edge geometry.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcEdgeCurve : IfcEdge, IfcCurveOrEdgeCurve
    {
        private bool _sameSense;
        private IfcCurve _edgeGeometry;

        /// <summary>
        ///   The curve which defines the shape and spatial location of the edge. This curve may be unbounded and is implicitly trimmed by the vertices of the edge; this defines the edge domain. Multiple edges can reference the same curve.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public IfcCurve EdgeGeometry
        {
            get { return _edgeGeometry; }
            set { _edgeGeometry = value; }
        }

        /// <summary>
        ///   This logical flag indicates whether (TRUE), or not (FALSE) the senses of the edge and the curve defining the edge geometry are the same. The sense of an edge is from the edge start vertex to the edge end vertex; the sense of a curve is in the direction of increasing parameter.
        /// </summary>
        [IfcAttribute(4, IfcAttributeState.Mandatory)]
        public bool SameSense
        {
            get { return _sameSense; }
            set { _sameSense = value; }
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
                    _edgeGeometry = (IfcCurve) value.EntityVal;
                    break;
                case 3:
                    _sameSense = (bool) value.EntityVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }
    }
}
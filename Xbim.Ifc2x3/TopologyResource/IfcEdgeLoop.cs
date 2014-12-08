#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcEdgeLoop.cs
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
    ///   An edge_loop is a loop with nonzero extent.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: An edge_loop is a loop with nonzero extent. It is a path in which the start and end vertices are the same. Its domain, if present, is a closed curve. An edge_loop may overlap itself.
    ///   Informal propositions:
    ///   The genus of the IfcEdgeLoop shall be 1 or greater. 
    ///   The Euler formula shall be satisfied:
    ///   (number of vertices) + genus - (number of edges) = 1; 
    ///   No edge may be referenced more than once by the same IfcEdgeLoop with the same sense. For this purpose, an edge which is not an oriented edge is considered to be referenced with the sense TRUE. 
    ///   NOTE: Corresponding STEP entity: edge_loop. Please refer to ISO/IS 10303-42:1994, p. 122 for the final definition of the formal standard. 
    ///   HISTORY: New Entity in Release IFC 2x Edition 2. 
    ///   EXPRESS specification:
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcEdgeLoop : IfcLoop
    {
        public IfcEdgeLoop()
        {
            _edgeList = new XbimList<IfcOrientedEdge>(this, 4);
        }

        private XbimList<IfcOrientedEdge> _edgeList;

        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.List, 1)]
        public XbimList<IfcOrientedEdge> EdgeList
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _edgeList;
            }
            set { this.SetModelValue(this, ref _edgeList, value, v => EdgeList = v, "EdgeList"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _edgeList.Add((IfcOrientedEdge) value.EntityVal);
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }
    }
}
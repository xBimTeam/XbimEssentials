using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

namespace Xbim.Ifc2x3.TopologyResource
{
    /// <summary>
    ///  A path is a topological entity consisting of an ordered collection of oriented edges, such that the edge start vertex of each edge coincides with the edge end of its predecessor. The path is ordered from the edge start of the first oriented edge to the edge end of the last edge. The BOOLEAN value sense in the oriented edge indicates whether the edge direction agrees with the direction of the path (TRUE) or is the opposite direction (FALSE).
    /// 
    /// An individual edge can only be referenced once by an individual path. An edge can be referenced by multiple paths. An edge can exist independently of a path. 
    /// </summary>
    [IfcPersistedEntity]
    public class IfcPath : IfcTopologicalRepresentationItem
    {
        public IfcPath()
        {
            _EdgeList = new XbimSet<IfcOrientedEdge>(this);
        }

        private XbimSet<IfcOrientedEdge> _EdgeList;

        /// <summary>
        /// The list of oriented edges which are concatenated together to form this path. 
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.Set, IfcAttributeType.Class, 1)]
        public XbimSet<IfcOrientedEdge> EdgeList
        {
            get
            {
                ((IPersistIfcEntity)this).Activate(false);
                return _EdgeList;
            }
            set { this.SetModelValue(this, ref _EdgeList, value, v => EdgeList = v, "EdgeList"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    _EdgeList.Add((IfcOrientedEdge)value.EntityVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public override string WhereRule()
        {
            var edges = EdgeList.ToArray();
            for (int i = 0; i < EdgeList.Count-1; i++)
            {
                var ancestor = edges[i];
                var descendant = edges[i + 1];

                if (ancestor.EdgeEnd != descendant.EdgeStart)
                    return "WR1: The end vertex of each edge shall be the same as the start vertex of its successor. \n";
            }

            return "";
        }
    }
}

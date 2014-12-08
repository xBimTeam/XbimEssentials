#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcPolyLoop.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.TopologyResource
{
    /// <summary>
    ///   A poly loop is a loop with straight edges bounding a planar region in space.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A poly loop is a loop with straight edges bounding a planar region in space. A poly loop is a loop of genus 1 where the loop is represented by an ordered coplanar collection of points forming the vertices of the loop. The loop is composed of straight line segments joining a point in the collection to the succeeding point in the collection. The closing segment is from the last to the first point in the collection. 
    ///   The direction of the loop is in the direction of the line segments. 
    ///   NOTE  This entity exists primarily to facilitate the efficient communication of faceted B-rep models. 
    ///   A poly loop shall conform to the following topological constraints:
    ///   - the loop has the genus of one. 
    ///   - the following equation shall be satisfied 
    ///  
    ///   Definition from IAI:  The IfcPolyLoop is always closed and the last segment is from the last IfcCartesianPoint in the list of Polygon's to the first IfcCartesianPoint. Therefore the first point shall not be repeated at the end of the list, neither by referencing the same instance, nor by using an additional instance of IfcCartesianPoint having the coordinates as the first point. 
    ///   NOTE   Corresponding STEP entity: poly_loop. Please refer to ISO/IS 10303-42:1994, p. 138 for the final definition of the formal standard. Due to the general IFC model specification rule not to use multiple inheritance, the subtype relationship to geometric_representation_item is not included. The derived attribute Dim has been added at this level. 
    ///   HISTORY   New class in IFC Release 1.0 
    ///   Informal propositions: 
    ///   All the points in the polygon defining the poly loop shall be coplanar. 
    ///   The first and the last Polygon shall be different by value. 
    ///   Formal Propositions:
    ///   WR21   :   The space dimensionality of all Points shall be the same.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcPolyLoop : IfcLoop, IBoundary
    {
        private XbimListUnique<IfcCartesianPoint> _polygon;

        public IfcPolyLoop()
        {
            _polygon = new XbimListUnique<IfcCartesianPoint>(this);
        }

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   List of points defining the loop. There are no repeated points in the list.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.ListUnique, 3)]
        public XbimListUnique<IfcCartesianPoint> Polygon
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _polygon;
            }
            set { this.SetModelValue(this, ref _polygon, value, v => Polygon = v, "Polygon"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _polygon.Add((IfcCartesianPoint) value.EntityVal);
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (IfcCartesianPoint pt in Polygon)
            {
                sb.AppendFormat("{0} ", pt);
            }
            return sb.ToString();
        }

        #region IBoundary Members

        IEnumerable<IVertex3D> IBoundary.ClockWiseVertices
        {
            get { return Polygon.Cast<IVertex3D>(); }
        }

        #endregion

        #region IBoundary Members

        /// <summary>
        ///   Polyloops do not have holes so are always the outer bound
        /// </summary>
        public bool IsOuterBound
        {
            get { return true; }
        }

        #endregion
    }
}
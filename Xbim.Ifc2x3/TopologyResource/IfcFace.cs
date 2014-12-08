#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFace.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.TopologyResource
{
    /// <summary>
    ///   A face is a topological entity of dimensionality 2 corresponding to the intuitive notion of a piece of surface bounded by loops.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A face is a topological entity of dimensionality 2 corresponding to the intuitive notion of a piece of surface bounded by loops. Its domain, if present, is an oriented, connected, finite 2-manifold in Rm. A face domain shall not have handles but it may have holes, each hole bounded by a loop. The domain of the underlying geometry of the face, if present, does not contain its bounds, and 0 lt Ξ gt ∞.
    ///   A face is represented by its bounding loops, which are defined as face bounds. A face has a topological normal n and the tangent to a loop is t. For a loop bounding a face with defined geometry, the cross product n x t points toward the interior of the face. That is, each loop runs counter-clockwise around the face when viewed from above, if we consider the normal n to point up. With each loop is associated a BOOLEAN flag to signify whether the loop direction is oriented with respect to the face normal (TRUE) or should be reversed (FALSE). 
    ///   A face shall have at least one bound, and the loops shall not intersect. One loop is optionally distinguished as the outer loop of the face. If so, it establishes a preferred way of embedding the face domain in the plane, in which the other bounding loops of the face are inside the outer bound. Because the face domain is arcwise connected, no inner loop will contain any other loop. This is true regardless of which embedding in the plane is chosen. 
    ///   The edges and vertices referenced by the loops of a face form a graph, of which the individual loops are the connected components. The Euler equation (1) for this graph becomes:
    ///   where Gli is the graph genus of the i th loop.
    ///   NOTE Corresponding STEP entity: face. No subtypes of face have been incorporated into this IFC Release. Please refer to ISO/IS 10303-42:1994, p. 140 for the final definition of the formal standard. The WR1 has not been incorporated, since it is always satisfied, due to the fact that only poly loops exist for face bounds. 
    ///   HISTORY New class in IFC Release 1.0 
    ///   Informal propositions:
    ///   No edge shall be referenced by the face more than twice. 
    ///   Distinct face bounds of the face shall have no common vertices. 
    ///   If geometry is present, distinct loops of the same face shall not intersect. 
    ///   The face shall satisfy the Euler Equation: (number of vertices) - (number of edges) - (number of loops) + (sum of genus for loops) = 0.
    ///   Formal Propositions:
    ///   WR1   :   At most one of the bounds shall be of the type IfcFaceOuterBound
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcFace : IfcTopologicalRepresentationItem, IFace
    {
        public IfcFace()
        {
            mBounds = new XbimSet<IfcFaceBound>(this);
        }

        #region Part 21 Step file Parse routines

        protected XbimSet<IfcFaceBound> mBounds;

        /// <summary>
        ///   Boundaries of the face.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory, IfcAttributeType.Set, 1)]
        public XbimSet<IfcFaceBound> Bounds
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return mBounds;
            }
            set { this.SetModelValue(this, ref mBounds, value, v => Bounds = v, "Bounds"); }
        }


        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                mBounds.Add((IfcFaceBound) value.EntityVal);
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        public override string WhereRule()
        {
            if (Bounds.OfType<IfcFaceOuterBound>().Count() > 1)
                return "WR1 Face : At most one of the bounds shall be of the type FaceOuterBound\n";
            else
                return "";
        }

        #region IFace Members

        IEnumerable<IBoundary> IFace.Boundaries
        {
            get { return Bounds.Cast<IBoundary>(); }
        }

        IVector3D IFace.Normal
        {
            get
            {
                foreach (IfcFaceBound item in Bounds)
                {
                    if (item is IfcFaceOuterBound)
                    {
                        return item.Normal();
                    }
                }
                //no outer bounds take anything
                return Bounds.FirstOrDefault().Normal();
            }
        }


        bool IFace.HasHoles
        {
            get { return Bounds.Count > 1; }
        }

        #endregion


        public bool IsPolygonal
        {
            get
            {
                foreach (var bound in Bounds)
                {
                    if (!(bound.Bound is IfcPolyLoop))
                        return false;
                }
                return true;
            }
        }
    }
}
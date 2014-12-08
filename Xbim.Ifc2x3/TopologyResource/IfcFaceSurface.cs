#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFaceSurface.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.TopologyResource
{
    /// <summary>
    ///   A face surface (IfcFaceSurface) is a subtype of face in which the geometry is defined by an associated surface.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A face surface (IfcFaceSurface) is a subtype of face in which the geometry is defined by an associated surface. The portion of the surface used by the face shall be embeddable in the plane as an open disk, possibly with holes. However, the union of the face with the edges and vertices of its bounding loops need not be embeddable in the plane. It may, for example, cover an entire sphere or torus. As both a face and a geometric surface have defined normal directions, a BOOLEAN flag (the orientation attribute) is used to indicate whether the surface normal agrees with (TRUE) or is opposed to (FALSE) the face normal direction. The geometry associated with any component of the loops of the face shall be consistent with the surface geometry, in the sense that the domains of all the vertex points and edge curves are contained in the face geometry surface. A surface may be referenced by more than one face surface.
    ///   NOTE Corresponding STEP entity: face_surface. Please refer to ISO/IS 10303-42:1994, p. 204 for the final definition of the formal standard. Due to the general IFC model specification rule not to use multiple inheritance, the subtype relationship to geometric_representation_item is not included.
    ///   HISTORY New class in IFC Release 2.x 
    ///   Informal propositions: 
    ///   The domain of the face surface is formally defined to be the domain of its face geometry as trimmed by the loops, this domain does not include the bounding loops. 
    ///   A face surface has non zero finite extent. 
    ///   A face surface is a manifold. 
    ///   A face surface is arcwise connected. 
    ///   A face surface has surface genus 0. 
    ///   The loops are not part of the face domain. 
    ///   Loop geometry shall be consistent with face geometry. This implies that any edge - curves or vertex points used in defining the loops bounding the face surface shall lie on the face geometry. 
    ///   The loops of the face shall not intersect.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcFaceSurface : IfcFace, IfcSurfaceOrFaceSurface, IFace
    {
        private IfcSurface _faceSurface;
        private bool _sameSense = true;

        /// <summary>
        ///   The surface which defines the internal shape of the face. This surface may be unbounded. The domain of the face is defined by this surface and the bounding loops in the inherited attribute SELF\FaceBounds.
        /// </summary>
        [IfcAttribute(2, IfcAttributeState.Mandatory)]
        public IfcSurface Surface
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _faceSurface;
            }
            set { this.SetModelValue(this, ref _faceSurface, value, v => Surface = v, "Surface"); }
        }

        /// <summary>
        ///   This flag indicates whether the sense of the surface normal agrees with (TRUE), or opposes (FALSE), the sense of the topological normal to the face.
        /// </summary>
        [IfcAttribute(3, IfcAttributeState.Mandatory)]
        public bool SameSense
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _sameSense;
            }
            set { this.SetModelValue(this, ref _sameSense, value, v => SameSense = v, "SameSense"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            switch (propIndex)
            {
                case 0:
                    base.IfcParse(propIndex, value);
                    break;
                case 1:
                    _faceSurface = (IfcSurface) value.EntityVal;
                    break;
                case 2:
                    _sameSense = value.BooleanVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
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
                IPlacement3D pos = Surface;
                return pos.Position.P[2];
            }
        }

        bool IFace.HasHoles
        {
            get { return Bounds.Count > 1; }
        }

        #endregion
    }
}
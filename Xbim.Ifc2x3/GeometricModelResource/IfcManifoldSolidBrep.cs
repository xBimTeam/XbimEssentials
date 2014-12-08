#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcManifoldSolidBrep.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using Xbim.Ifc2x3.TopologyResource;
using Xbim.XbimExtensions;

using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    /// <summary>
    ///   A manifold solid B-rep is a finite, arcwise connected volume bounded by one or more surfaces, each of which is a connected, oriented, finite, closed 2-manifold.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A manifold solid B-rep is a finite, arcwise connected volume bounded by one or more surfaces, each of which is a connected, oriented, finite, closed 2-manifold. There is no restriction on the genus of the volume, nor on the number of voids within the volume. 
    ///   The Boundary Representation (B-rep) of a manifold solid utilizes a graph of edges and vertices embedded in a connected, oriented, finite, closed two manifold surface. The embedded graph divides the surface into arcwise connected areas known as faces. The edges and vertices, therefore, form the boundaries of the face and the domain of a face does not include its boundaries. The embedded graph may be disconnected and may be a pseudo graph. The graph is labeled; that is, each entity in the graph has a unique identity. The geometric surface definition used to specify the geometry of a face shall be 2-manifold embeddable in the plane within the domain of the face. In other words, it shall be connected, oriented, finite, non-self-intersecting, and of surface genus 0. 
    ///   Faces do not intersect except along their boundaries. Each edge along the boundary of a face is shared by at most one other face in the assemblage. The assemblage of edges in the B-rep do not intersect except at their boundaries (i.e., vertices). The geometry curve definition used to specify the geometry of an edge shall be arcwise connected and shall not self intersect or overlap within the domain of the edge. The geometry of an edge shall be consistent with the geometry of the faces of which it forms a partial bound. The geometry used to define a vertex shall be consistent with the geometry of the faces and edges of which it forms a partial bound. 
    ///   A B-rep is represented by one or more closed shells which shall be disjoint. One shell, the outer, shall completely enclose all the other shells and no other shell may enclose a shell. The facility to define a B-rep with one or more internal voids is provided by a subtype. The following version of the Euler formula shall be satisfied, where V, E, F, Ll and S are the numbers of unique vertices, edges, faces, loop uses and shells in the model and Gs is the sum of the genus of the shells. 
    ///   Definition from IAI: In the current IFC Release all instances of type IfcManifoldSolidBrep shall be of type faceted B-rep, using only IfcPolyLoop for the bounds of IfcFaceBound. 
    ///   NOTE: Corresponding STEP entity: manifold_solid_brep. Please refer to ISO/IS 10303-42:1994, p. 170 for the final definition of the formal standard. Since only faceted B-rep (with and without voids) is in scope of the current IFC Release the IfcManifoldSolidBrep is defined as ABSTRACT supertype to prevent it from direct instantiation. 
    ///   HISTORY: New entity in IFC Release 1.0 
    ///   Informal proposition:
    ///   The dimensionality of a manifold solid brep shall be 3. 
    ///   The extent of the manifold solid brep shall be finite and non-zero. 
    ///   All elements of the manifold solid brep shall have defined associated geometry. 
    ///   The shell normals shall agree with the B-rep normal and point away from the solid represented by the B-rep. 
    ///   Each face shall be referenced only once by the shells of the manifold solid brep. 
    ///   The Euler equation shall be satisfied for the boundary representation, where the genus term "shell term" us the sum of the genus values for the shells of the brep.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcManifoldSolidBrep : IfcSolidModel, IFaceBasedModel
    {
        private IfcClosedShell _outer;

        #region Part 21 Step file Parse routines

        /// <summary>
        ///   A closed shell defining the exterior boundary of the solid. The shell normal shall point away from the interior of the solid.
        /// </summary>
        [IfcAttribute(1, IfcAttributeState.Mandatory)]
        public IfcClosedShell Outer
        {
            get
            {
                ((IPersistIfcEntity) this).Activate(false);
                return _outer;
            }
            set { this.SetModelValue(this, ref _outer, value, v => Outer = v, "Outer"); }
        }

        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            if (propIndex == 0)
                _outer = (IfcClosedShell) value.EntityVal;
            else
                this.HandleUnexpectedAttribute(propIndex, value);
        }

        #endregion

        #region IFaceBasedModel Members

        IEnumerable<IFace> IFaceBasedModel.Faces
        {
            get { return ((IFaceBasedModel) Outer).Faces; }
        }

        #endregion


        public bool IsPolygonal
        {
            get { return _outer.IsPolygonal; }
        }
    }
}
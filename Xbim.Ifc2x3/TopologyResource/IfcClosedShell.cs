#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcClosedShell.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.TopologyResource
{
    /// <summary>
    ///   A closed shell is a shell of the dimensionality 2 which typically serves as a bound for a region in R3.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A closed shell is a shell of the dimensionality 2 which typically serves as a bound for a region in R3. A closed shell has no boundary, and has non-zero finite extent. If the shell has a domain with coordinate space R3, it divides that space into two connected regions, one finite and the other infinite. In this case, the topological normal of the shell is defined as being directed from the finite to the infinite region. 
    ///   The shell is represented by a collection of faces. The domain of the shell, if present, contains all those faces, together with their bounds. Associated with each face in the shell is a logical value which indicates whether the face normal agrees with (TRUE) or is opposed to (FALSE) the shell normal. The logical value can be applied directly as a BOOLEAN attribute of an oriented face, or be defaulted to TRUE if the shell boundary attribute member is a face without the orientation attribute. 
    ///   The combinatorial restrictions on closed shells and geometrical restrictions on their domains are designed to ensure that any domain associated with a closed shell is a closed, orientable manifold. The domain of a closed shell, if present, is a connected, closed, oriented 2-manifold. It is always topologically equivalent to an H-fold torus for some H ³ 0. The number H is referred to as the surface genus of the shell. If a shell of genus H has a domain within coordinate space R3, then the finite region of space inside it is topologically equivalent to a solid ball with H tunnels drilled through it.
    ///   The Euler equation (7) applies with B=0, because in this case there are no holes. As in the case of open shells, the surface genus H may not be known a priori, but shall be an integer ³ 0. Thus a necessary, but not sufficient, condition for a well-formed closed shell is the following:
    ///   Definition from IAI: In the current IFC Release only poly loops (IfcPolyLoop) are defined for bounds of face bound (IfcFaceBound). This will allow for faceted B-rep only. 
    ///   NOTE: Corresponding STEP entity: closed_shell, please refer to ISO/IS 10303-42:1994, p.149 for the final definition of the formal standard. 
    ///   HISTORY: New class in IFC Release 1.0 
    ///   Informal propositions: 
    ///   Every edge shall be referenced exactly twice by the loops of the face. 
    ///   Each oriented edge shall be unique. 
    ///   No edge shall be referenced by more than two faces. 
    ///   Distinct faces of the shell do not intersect, but may share edges or vertices. 
    ///   Distinct edges do not intersect but may share vertices. 
    ///   Each face reference shall be unique. 
    ///   The loops of the shell shall not be a mixture of poly loop and other loop types. Note: this is given, since only poly loop is defined as face bound definition. 
    ///   The closed shell shall be an oriented arcwise connected 2-manifold. 
    ///   The Euler equation shall be satisfied. Note: Please refer to ISO/IS 10303-42:1994, p.149 for the equation.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcClosedShell : IfcConnectedFaceSet, IfcShell
    {
    }
}
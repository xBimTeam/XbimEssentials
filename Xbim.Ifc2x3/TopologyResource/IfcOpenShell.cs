#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcOpenShell.cs
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
    ///   An open shell is a shell of the dimensionality 2. Its domain, if present, is a finite, connected, oriented, 2-manifold with boundary, but is not a closed surface.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: An open shell is a shell of the dimensionality 2. Its domain, if present, is a finite, connected, oriented, 2-manifold with boundary, but is not a closed surface. It can be thought of as a closed shell with one or more holes punched in it. The domain of an open shell satisfies 0 lt Ξ lt 1. An open shell is functionally more general than a face because its domain can have handles.
    ///   The shell is defined by a collection of faces, which may be oriented faces. The sense of each face, after taking account of the orientation, shall agree with the shell normal as defined below. The orientation can be supplied directly as a BOOLEAN attribute of an oriented face, or be defaulted to TRUE if the shell member is a face without the orientation attribute.
    ///   The following combinatorial restrictions on open shells and geometrical restrictions on their domains are designed, together with the informal propositions, to ensure that any domain associated with an open shell is an orientable manifold.
    ///   Each face reference shall be unique. 
    ///   An open shell shall have at least one face. 
    ///   A given face may exist in more than one open shell. 
    ///   The boundary of an open shell consists of the edges that are referenced only once by the face - bounds (loops) of its faces, together with all of their vertices. The domain of an open shell, if present, contains all edges and vertices of its faces.
    ///   NOTE Note that this is slightly different from the definition of a face domain, which includes none of its bounds. For example, a face domain may exclude an isolated point or line segment. An open shell domain may not. (See the algorithm for computing below.)
    ///   Definition from IAI: In the current IFC Release only poly loops (IfcPolyLoop) are defined for bounds of face bound (IfcFaceBound.Bound). This will allow for faceted B-rep only. For further specification, including the Euler formulars to be satisfied, please refer to ISO 10303-42:1994.
    ///   NOTE: Corresponding STEP entity: open_shell, please refer to ISO/IS 10303-42:1994, p.148 for the final definition of the formal standard. 
    ///   HISTORY: New class in IFC Release 2.x 
    ///   Informal propositions: 
    ///   Every edge shall be referenced exactly twice by the face bounds of the face. 
    ///   Each oriented edge shall be unique. 
    ///   No edge shall be referenced by more than two faces. 
    ///   Distinct faces of the shell do not intersect, but may share edges or vertices. 
    ///   Distinct edges do not intersect but may share vertices. 
    ///   Each face reference shall be unique. 
    ///   The loops of the shell shall not be a mixture of poly loop and other loop types. Note: this is given, since only poly loop is defined as face bound definition. 
    ///   The closed shell shall be an oriented arcwise connected 2-manifold. 
    ///   The Euler equation shall be satisfied. Note: Please refer to ISO/IS 10303-42:1994, p.148 for the equation.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcOpenShell : IfcConnectedFaceSet, IfcShell
    {
    }
}
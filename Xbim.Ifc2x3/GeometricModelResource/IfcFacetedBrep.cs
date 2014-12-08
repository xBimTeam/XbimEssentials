#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFacetedBrep.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    /// <summary>
    ///   A faceted brep is a simple form of boundary representation model in which all faces are planar and all edges are straight lines.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A faceted brep is a simple form of boundary representation model in which all faces are planar and all edges are straight lines. Unlike the B-rep model, edges and vertices are not represented explicitly in the model but are implicitly available through the poly loop entity. A faceted B-rep has to meet the same topological constraints as the manifold solid Brep. 
    ///   NOTE: The faceted brep has been introduced in order to support the larger number of systems that allow boundary type solid representations with planar surfaces only.
    ///   NOTE: Corresponding STEP entity: faceted_brep. Please refer to ISO/IS 10303-42:1994, p. 173 for the final definition of the formal standard. In the current IFC Release faceted B-rep with voids is represented by an own subtype and not defined via an implicit ANDOR supertype constraint as in ISO/IS 10303-42:1994. This change has been made due to the fact, that only ONEOF supertype constraint is allowed within the IFC object model. 
    ///   HISTORY New class in IFC Release 1.0
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcFacetedBrep : IfcManifoldSolidBrep
    {
        public override string WhereRule()
        {
            return "";
        }
    }
}
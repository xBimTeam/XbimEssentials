#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSolidModel.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.GeometricModelResource
{
    /// <summary>
    ///   A solid model is a complete representation of the nominal shape of a product such that all points in the interior are connected.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A solid model is a complete representation of the nominal shape of a product such that all points in the interior are connected. Any point can be classified as being inside, outside, or on the boundary of a solid. There are several different types of solid model representations. 
    ///   NOTE: Corresponding STEP entity: solid_model, only three subtypes have been incorporated into the current IFC Release - subset of manifold_solid_brep (IfcManifoldSolidBrep, constraint to faceted B-rep), swept_area_solid (IfcSweptAreaSolid), the swept_disk_solid (IfcSweptDiskSolid) and subset of csg_solid (IfcCsgSolid). The derived attribute Dim has been added at this level and was therefore demoted from the geometric_representation_item. Please refer to ISO/IS 10303-42:1994, p. 170 for the final definition of the formal standard. 
    ///   HISTORY: New entity in IFC Release 1.5
    /// </remarks>
    [IfcPersistedEntityAttribute, IndexedClass]
    public abstract class IfcSolidModel : IfcGeometricRepresentationItem, IfcBooleanOperand
    {
        /// <summary>
        ///   The space dimensionality of this class, it is always 3.
        /// </summary>
        public IfcDimensionCount Dim
        {
            get { return new IfcDimensionCount(3); }
        }

        int IfcBooleanOperand.Dim
        {
            get { return Dim; }
        }

    }
}
#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcSurface.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions.SelectTypes;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   A surface can be envisioned as a set of connected points in 3-dimensional space which is always locally 2-dimensional, but need not be a manifold.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A surface can be envisioned as a set of connected points in 3-dimensional space which is always locally 2-dimensional, but need not be a manifold. 
    ///   NOTE Corresponding STEP entity: surface, the following subtypes have been incorporated into IFC - elementary_surface (as IfcElementarySurface), swept_surface (as IfcSweptSurface) and bounded_surface (as IfcBoundedSurface). Please refer to ISO/IS 10303-42:1994, p. 68 for the final definition of the formal standard. 
    ///   HISTORY New class in IFC Release 1.5 
    ///   Informal proposition:
    ///   A surface has non zero area. 
    ///   A surface is arcwise connected.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcSurface : IfcGeometricRepresentationItem, IPlacement3D, IfcSurfaceOrFaceSurface,
                                       IfcGeometricSetSelect
    {
        public abstract IfcDimensionCount Dim { get; }

        #region IPlacement3D Members

		// TODO: Should be abstract rather than not implemented?
        IfcAxis2Placement3D IPlacement3D.Position
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
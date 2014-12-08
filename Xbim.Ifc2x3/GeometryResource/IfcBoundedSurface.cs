#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBoundedSurface.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   A bounded surface is a surface of finite area with identifiable boundaries.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: A bounded surface is a surface of finite area with identifiable boundaries.
    ///   NOTE Corresponding STEP name: bounded_surface, only the following subtypes have been incorporated into IFC: rectangular_trimmed_surface as IfcRectangularTrimmedSurface. Please refer to ISO/IS 10303-42:1994, p.78 for the final definition of the formal standard. 
    ///   HISTORY New class in IFC Release 2x 
    ///   Informal propositions:
    ///   A bounded surface has finite non-zero surface area. 
    ///   A bounded surface has boundary curves.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public abstract class IfcBoundedSurface : IfcSurface
    {
        /// <summary>
        ///   The space dimensionality of this class, derived from the dimensionality of the Position.
        /// </summary>
        public abstract override IfcDimensionCount Dim { get; }
    }
}
#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcGeometricProjectionEnum.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.RepresentationResource
{
    /// <summary>
    ///   The IfcGeometricProjectionEnum defines the various representation types that can be semantically distinguished.
    /// </summary>
    /// <remarks>
    ///   Definition from IAI: The IfcGeometricProjectionEnum defines the various representation types that can be semantically distinguished. Often different levels of detail of the shape representation are controlled by the representation type.
    /// </remarks>
    public enum IfcGeometricProjectionEnum
    {
        /// <summary>
        ///   Geometric display representation that shows an abstract, often 1D element representation, e.g. representing a wall by its axis line.
        /// </summary>
        GRAPH_VIEW,

        /// <summary>
        ///   Geometric display representation that shows an abstract, often 2D element representation, e.g. representing a wall by its two foot print edges, surpressing any inner layer representation.
        /// </summary>
        SKETCH_VIEW,

        /// <summary>
        ///   Geometric display representation that shows a full 3D element representation, e.g. representing a wall by its volumetric body.
        /// </summary>
        MODEL_VIEW,

        /// <summary>
        ///   Geometric display representation that shows a full 2D element representation, the level of detail often depends on the target scale, e.g. representing a wall by its two foot print edges and the edges of all inner layers. The projection is shown in ground view as seen from above.
        /// </summary>
        PLAN_VIEW,

        /// <summary>
        ///   Geometric display representation that shows a full 2D element representation, the level of detail often depends on the target scale, e.g. representing a wall by its two foot print edges and the edges of all inner layers. The projection is shown in ground view as seen from below.
        /// </summary>
        REFLECTED_PLAN_VIEW,

        /// <summary>
        ///   Geometric display representation that shows a full 2D element representation, the level of detail often depends on the target scale, e.g. representing a wall by its two inner/outer edges and the edges of all inner layers, if the element is cut by the section line.
        /// </summary>
        SECTION_VIEW,

        /// <summary>
        ///   Geometric display representation that shows a full 2D element representation, the level of detail often depends on the target scale, e.g. representing a wall by its bounding edges if the element is within an elevation view.
        /// </summary>
        ELEVATION_VIEW,

        /// <summary>
        ///   A user defined specification is given by the value of the UserDefinedTargetView attribute.
        /// </summary>
        USERDEFINED,

        /// <summary>
        ///   No specification given.
        /// </summary>
        NOTDEFINED
    }
}
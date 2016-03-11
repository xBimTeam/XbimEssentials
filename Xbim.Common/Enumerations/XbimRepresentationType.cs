#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    XbimRepresentationType.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;

#endregion

namespace Xbim.Common.Enumerations
{
    /// <summary>
    /// </summary>
    public enum XbimRepresentationContextType
    {
        Model,
        Plan,
        Undefined
    }

    /// <summary>
    /// </summary>
    public enum XbimRepresentationIdentifierType
    {
        Axis,
        FootPrint,
        Body
    }

    [Flags]
    public enum XbimRepresentationType : short
    {
        /// <summary>
        ///   2 dimensional curves
        /// </summary>
        Curve2D = 1,

        /// <summary>
        ///   Points, curves, surfaces (2 or 3 dimensional)
        /// </summary>
        GeometricSet = 2,

        /// <summary>
        ///   Points, curves (2 or 3 dimensional)
        /// </summary>
        GeometricCurveSet = 4,

        /// <summary>
        ///   Face based and shell based surface model
        /// </summary>
        SurfaceModel = 8,

        /// <summary>
        ///   Swept solid, Boolean results and Brep bodies
        /// </summary>
        SolidModel = 16,

        /// <summary>
        ///   Swept area solids, by extrusion and revolution
        /// </summary>
        SweptSolid = 32,

        /// <summary>
        ///   Faceted Brep's with and without voids
        /// </summary>
        Brep = 64,

        /// <summary>
        ///   Boolean results of operations between solid models, half spaces and Boolean results
        /// </summary>
        CSG = 128,

        /// <summary>
        ///   Boolean differences between swept area solids, half spaces and Boolean results
        /// </summary>
        Clipping = 256,

        /// <summary>
        ///   Swept area solids created by sweeping a profile along a directrix
        /// </summary>
        AdvancedSweptSolid = 512,

        /// <summary>
        ///   Simplistic 3D representation by a bounding box
        /// </summary>
        BoundingBox = 1024,

        /// <summary>
        ///   cross section based representation of a spine curve and planar cross sections.
        ///   It can represent a surface or a solid and the interpolations of the between the cross sections is not defined
        /// </summary>
        SectionedSpine = 2048,

        /// <summary>
        ///   representation based on mapped item(s), referring to a representation map. 
        ///   Note: it can be seen as an inserted block reference. 
        ///   The shape representation of the mapped item has a representation type declaring the type of its representation items.
        /// </summary>
        MappedRepresentation = 4096
    }
}
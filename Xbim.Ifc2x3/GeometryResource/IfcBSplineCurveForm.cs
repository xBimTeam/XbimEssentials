#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBSplineCurveForm.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   This type is used to indicate that the B-spline curve represents a part of a curve of some sppeci c form.
    /// </summary>
    /// <remarks>
    ///   NOTE: Corresponding STEP type: b_spline_curve_form. Please refer to ISO/IS 10303-42:1994, p. 15 for the final definition of the formal standard. 
    ///   HISTORY: New type in Release IFC2x Edition 2.
    /// </remarks>
    public enum IfcBSplineCurveForm
    {
        /// <summary>
        ///   A connected sequence of line segments represented by degree 1 B-spline basis functions.
        /// </summary>
        POLYLINE_FORM,

        /// <summary>
        ///   An arc of a circle, or a complete circle represented by a B-spline curve.
        /// </summary>
        CIRCULAR_ARC,

        /// <summary>
        ///   An arc of an ellipse, or a complete ellipse, represented by a B-spline curve.
        /// </summary>
        ELLIPTIC_ARC,

        /// <summary>
        ///   An arc of finite length of a parabola represented by a B-spline curve.
        /// </summary>
        PARABOLIC_ARC,

        /// <summary>
        ///   An arc of finite length of one branch of a hyperbola represented by a B-spline curve.
        /// </summary>
        HYPERBOLIC_ARC,

        /// <summary>
        ///   A B-spline curve for which no particular form is specified.
        /// </summary>
        UNSPECIFIED
    }
}
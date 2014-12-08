#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcBezierCurve.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using Xbim.XbimExtensions;
using Xbim.XbimExtensions.Interfaces;

#endregion

namespace Xbim.Ifc2x3.GeometryResource
{
    /// <summary>
    ///   This is a special type of curve which can be represented as a type of B-spline curve in which the knots are evenly spaced and have high multiplicities.
    /// </summary>
    /// <remarks>
    ///   Definition from ISO/CD 10303-42:1992: This is a special type of curve which can be represented as a type of B-spline curve in which the knots are evenly spaced and have high multiplicities. Suitable default values for the knots and knot multiplicities are derived in this case.
    ///   A B-spline curve is a piecewise Bezier curve if it is quasi-uniform except that the interior knots have multiplicity degree rather than having multiplicity one. In this subtype the knot spacing is 1.0, starting at 0.0. A piecewise Bezier curve which has only two knots, 0.0 and 1.0, each of multiplicity (degree+1), is a simple Bezier curve.
    ///   NOTE: A simple Bezier curve can be defined as a B-spline curve with knots by the following data: 
    ///   degree (d) 
    ///   upper index on control points (equal to d)  
    ///   control points (d + 1 cartesian points)  
    ///   knot type (equal to quasi-uniform knots) 
    ///   No other data are needed, except for a rational Bezier curve. In this case the weights data ((d + 1) REALs) shall be given. 
    ///   NOTE: It should be noted that every piecewise Bezier curve has an equivalent representation as a B-spline curve but not every B-spline curve can be represented as a Bezier curve. 
    ///   To define a piecewise Bezier curve as a B-spline:
    ///   The first knot is 0.0 with multiplicity (d + 1). 
    ///   The next knot is 1.0 with multiplicity d (we have now defined the knots for one segment, unless it is the last one). 
    ///   The next knot is 2.0 with multiplicity d (we have now defined the knots for one segment, again unless the second is the last one). 
    ///   Continue to the end of the last segment, call it the n-th segment, at the end of which a knot with value n, multiplicity (d + 1) is added. 
    ///   EXAMPLE:
    ///   A one-segment cubic Bezier curve would have knot sequence (0,1) with multiplicity sequence (4,4). 
    ///   A two-segment cubic piecewise Bezier curve would have knot sequence (0,1,2) with multiplicity sequence (4,3,4). 
    ///   For the piecewise Bezier case, if d is the degree, m is the number of knots with multiplicity d, and N is the total number of knots for the spline, then
    ///   (d+2+k)   = N 
    ///   = (d+1)+md+(d+1) 
    ///   thus m   = (k-d)/d  
    ///   So the knot sequence is (0; 1; ...;m; (m+ 1)) with multiplicities (d + 1; d; : : :; d; d+ 1).
    ///   NOTE: Corresponding STEP entity: bezier_curve. Please refer to ISO/IS 10303-42:1994, p. 51 for the final definition of the formal standard. Due to the constraints in the IFC architecture to not include ANDOR subtype constraints, an explicit subtype IfcRationalBezierCurve is added which holds the same information as the complex entity b_spline_curve AND bezier_curve.
    ///   HISTORY: New entity in Release IFC2x Edition 2.
    /// </remarks>
    [IfcPersistedEntityAttribute]
    public class IfcBezierCurve : IfcBSplineCurve
    {
        public override void IfcParse(int propIndex, IPropertyValue value)
        {
            base.IfcParse(propIndex, value);
        }
    }
}
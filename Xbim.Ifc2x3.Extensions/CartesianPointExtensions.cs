#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    CartesianPointExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System.Collections.Generic;
using Xbim.Common.Geometry;
using Xbim.Ifc2x3.GeometryResource;

#endregion

namespace Xbim.Ifc2x3.Extensions
{

    /// <summary>
    /// Used to compare cartesian points for equality within  a specfied tolerance
    /// </summary>
    public class XbimCartesianPointComparer : IEqualityComparer<IfcCartesianPoint>
    {
        double toleranceSq;
        /// <summary>
        /// Creates a comarer
        /// </summary>
        /// <param name="tolerance">The distance within which the points are defined to be the same point</param>
        public XbimCartesianPointComparer(double tolerance)
        {
            toleranceSq = tolerance * tolerance;
        }

        public bool Equals(IfcCartesianPoint x, IfcCartesianPoint y)
        {
            double vx = x.X - y.X;
            double vy = x.Y - y.Y;
            double vz = x.Z - y.Z;
            double lenSq = vx * vx + vy * vy + vz * vz;
            bool same = lenSq <= toleranceSq;
            
            return same;
        }

        public int GetHashCode(IfcCartesianPoint pt)
        {
            return 1;
            //return (int)pt.X ^ (int)pt.Y ^ (int)pt.Z ;
        }
    }

    public static class XbimPointExtensions
    {
        public static XbimPoint3D CrossProduct(this IfcCartesianPoint a, IfcCartesianPoint b)
        {
            return new XbimPoint3D(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }

    }
}
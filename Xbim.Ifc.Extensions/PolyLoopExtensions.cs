#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    PolyLoopExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using Xbim.Common.Geometry;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.TopologyResource;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class PolyLoopExtensions
    {
        public static IfcAreaMeasure Area(this IfcPolyLoop loop, IfcDirection normal)
        {
            var sum = new XbimPoint3D(0, 0, 0);
            var pts = loop.Polygon;
            for (var i = 0; i < pts.Count - 1; i++)
            {
                sum = XbimPoint3D.Add(sum, pts[i].CrossProduct(pts[i + 1]));
            }
            var n = normal.Normalise();
            return n.DotProduct(new XbimVector3D(sum.X, sum.Y, sum.Z))/2;
        }

        /// <summary>
        /// returns the area of the polyloop
        /// </summary>
        /// <param name="loop"></param>
        /// <returns></returns>
        public static double Area(this IfcPolyLoop loop)
        {
            var sum = new XbimVector3D(0, 0, 0);
            var pts = loop.Polygon;
            for (var i = 0; i < pts.Count - 1; i++)
            {
                var a = new XbimVector3D(pts[i].X,pts[i].Y,pts[i].Z);
                var b = new XbimVector3D(pts[i + 1].X, pts[i + 1].Y, pts[i + 1].Z);
                sum = sum + a.CrossProduct(b);
            }
            var n = loop.NewellsNormal();
            return n.DotProduct(sum) / 2;
        }

        /// <summary>
        /// Calculates the Newell's Normal of the polygon of the loop
        /// </summary>
        /// <param name="loop"></param>
        /// <returns></returns>
        public static XbimVector3D NewellsNormal(this IfcPolyLoop loop)
        {
            double x = 0, y = 0, z = 0;
            IfcCartesianPoint previous = null;
            var count = 0;
            var total = loop.Polygon.Count;
            for (var i = 0; i <= total; i++)
            {
                var current = i < total ? loop.Polygon[i] : loop.Polygon[0];
                if (count > 0)
                {
                    var xn = previous.X;
                    var yn = previous.Y;
                    var zn = previous.Z;
                    var xn1 = current.X;
                    var yn1 = current.Y;
                    var zn1 = current.Z;
                    x += (yn - yn1) * (zn + zn1);
                    y += (xn + xn1) * (zn - zn1);
                    z += (xn - xn1) * (yn + yn1);
                }
                previous = current;
                count++;
            }
            var v = new XbimVector3D(x, y, z);
            v.Normalize();
            return v;
        }
    }
}
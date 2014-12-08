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

using System.Collections.Generic;
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
            IfcCartesianPoint sum = new IfcCartesianPoint(0, 0, 0);
            IList<IfcCartesianPoint> pts = loop.Polygon;
            for (int i = 0; i < pts.Count - 1; i++)
            {
                sum.Add(pts[i].CrossProduct(pts[i + 1]));
            }
            IfcDirection n = normal.Normalise();
            return n.DotProduct(sum)/2;
        }

        /// <summary>
        /// returns the area of the polyloop
        /// </summary>
        /// <param name="loop"></param>
        /// <returns></returns>
        public static double Area(this IfcPolyLoop loop)
        {
            XbimVector3D sum = new XbimVector3D(0, 0, 0);
            IList<IfcCartesianPoint> pts = loop.Polygon;
            for (int i = 0; i < pts.Count - 1; i++)
            {
                XbimVector3D a = new XbimVector3D(pts[i].X,pts[i].Y,pts[i].Z);
                XbimVector3D b = new XbimVector3D(pts[i + 1].X, pts[i + 1].Y, pts[i + 1].Z);
                sum = sum + a.CrossProduct(b);
            }
            XbimVector3D n = loop.NewellsNormal();
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
            IfcCartesianPoint current = null, previous = null, first = null;
            int count = 0;
            int total = loop.Polygon.Count;
            for (int i = 0; i <= total; i++)
            {
                if (i < total)
                    current = loop.Polygon[i];
                else
                    current = loop.Polygon[0];
                if (count > 0)
                {
                    double xn = previous.X;
                    double yn = previous.Y;
                    double zn = previous.Z;
                    double xn1 = current.X;
                    double yn1 = current.Y;
                    double zn1 = current.Z;
                    x += (yn - yn1) * (zn + zn1);
                    y += (xn + xn1) * (zn - zn1);
                    z += (xn - xn1) * (yn + yn1);
                }
                else
                    first = current;
                previous = current;
                count++;
            }
            XbimVector3D v = new XbimVector3D(x, y, z);
            v.Normalize();
            return v;
        }
    }
}
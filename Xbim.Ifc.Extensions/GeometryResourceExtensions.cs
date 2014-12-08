using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.XbimExtensions.SelectTypes;

namespace Xbim.Ifc2x3.Extensions
{
    static public class GeometryResourceExtensions
    {
        static public IfcCircle Create(this IfcCircle c, IfcAxis2Placement position, double radius)
        {
            IfcCircle circle = new IfcCircle()
            {
                Position = position,
                Radius = radius
            };
            return circle;
        }

        static public double Area(this IfcCurve curve)
        {
            //- IfcConic  (abstract)
            //  - IfcCircle (x)
            //  - IfcEllipse (x)
            //- IfcLine (x)
            //- IfcOffsetCurve2D (Throw)
            //- IfcOffsetCurve3D (Throw)
            //- IfcBoundedCurve  (abstract)
            //  - IfcBSplineCurve  (abstract) (Throw)
            //    - IfcBezierCurve (Throw)
            //      - IfcRationalBezierCurve (Throw)
            //  - IfcTrimmedCurve (Throw)
            //  - IfcPolyline (x)
            //  - IfcCompositeCurve (Throw)
            //    - Ifc2DCompositeCurve (Throw)

            if (curve is IfcCircle)
            {
                IfcCircle c = curve as IfcCircle;
                return Math.PI * Math.Pow(c.Radius, 2);
            }
            else if (curve is IfcEllipse)
            {
                IfcEllipse c = curve as IfcEllipse;
                return Math.PI * c.SemiAxis1 * c.SemiAxis2; 
            }
            else if (curve is IfcLine)
            {
                return 0;
            }
            else if (curve is IfcOffsetCurve2D)
            {
                throw new NotImplementedException("Area not implemented for IfcOffsetCurve2D");
            }
            else if (curve is IfcOffsetCurve3D)
            {
                throw new NotImplementedException("Area not implemented for IfcOffsetCurve3D");
            }
            else if (curve is IfcBSplineCurve)
            {
                throw new NotImplementedException("Area not implemented for IfcBSplineCurve");
            }
            else if (curve is IfcTrimmedCurve)
            {
                throw new NotImplementedException("Area not implemented for IfcTrimmedCurve");
            }
            else if (curve is IfcPolyline)
            {
                // todo: needs testing
                IfcPolyline p = curve as IfcPolyline;

                if (p.Dim != 2)
                    throw new NotImplementedException("Area not implemented for 3D IfcPolyline");

                // http://stackoverflow.com/questions/2553149/area-of-a-irregular-shape
                // it assumes that the last point is NOT the same of the first one, but it tolerates the case.
                double area = 0.0f;

                int numVertices = p.Points.Count;
                for (int i = 0; i < numVertices - 1; ++i)
                {
                    area += p.Points[i].X * p.Points[i + 1].Y - p.Points[i + 1].X * p.Points[i].Y;
                }
                area += p.Points[numVertices - 1].X * p.Points[0].Y - p.Points[0].X * p.Points[numVertices - 1].Y;
                area /= 2.0;
                return area;
            }
            else if (curve is Ifc2DCompositeCurve)
            {
                // these are complicated and should be solved with opencascade or some other lib
                throw new NotImplementedException("Area not implemented for Ifc2DCompositeCurve");
            }
            else if (curve is IfcCompositeCurve)
            {
                // these are complicated and should be solved with opencascade or some other lib
                throw new NotImplementedException("Area not implemented for IfcCompositeCurve");
            }

            return double.NaN;
        }
    }
}

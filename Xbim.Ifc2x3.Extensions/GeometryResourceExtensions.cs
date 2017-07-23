using System;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    static public class GeometryResourceExtensions
    {
        static public IfcCircle Create(this IfcCircle circle, IfcAxis2Placement position, double radius)
        {
            var model = circle.Model;
            return model.Instances.New<IfcCircle>(c =>
            {
                c.Position = position;
                c.Radius = radius;
            });
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
                var c = curve as IfcCircle;
                return Math.PI * Math.Pow(c.Radius, 2);
            }
            if (curve is IfcEllipse)
            {
                var c = curve as IfcEllipse;
                return Math.PI * c.SemiAxis1 * c.SemiAxis2; 
            }
            if (curve is IfcLine)
            {
                return 0;
            }
            if (curve is IfcOffsetCurve2D)
            {
                throw new NotImplementedException("Area not implemented for IfcOffsetCurve2D");
            }
            if (curve is IfcOffsetCurve3D)
            {
                throw new NotImplementedException("Area not implemented for IfcOffsetCurve3D");
            }
            if (curve is IfcBSplineCurve)
            {
                throw new NotImplementedException("Area not implemented for IfcBSplineCurve");
            }
            if (curve is IfcTrimmedCurve)
            {
                throw new NotImplementedException("Area not implemented for IfcTrimmedCurve");
            }
            if (curve is IfcPolyline)
            {
                // todo: needs testing
                var p = curve as IfcPolyline;

                if (p.Dim != 2)
                    throw new NotImplementedException("Area not implemented for 3D IfcPolyline");

                // http://stackoverflow.com/questions/2553149/area-of-a-irregular-shape
                // it assumes that the last point is NOT the same of the first one, but it tolerates the case.
                double area = 0.0f;

                var numVertices = p.Points.Count;
                for (var i = 0; i < numVertices - 1; ++i)
                {
                    area += p.Points[i].X * p.Points[i + 1].Y - p.Points[i + 1].X * p.Points[i].Y;
                }
                area += p.Points[numVertices - 1].X * p.Points[0].Y - p.Points[0].X * p.Points[numVertices - 1].Y;
                area /= 2.0;
                return area;
            }
            if (curve is Ifc2DCompositeCurve)
            {
                // these are complicated and should be solved with opencascade or some other lib
                throw new NotImplementedException("Area not implemented for Ifc2DCompositeCurve");
            }
            if (curve is IfcCompositeCurve)
            {
                // these are complicated and should be solved with opencascade or some other lib
                throw new NotImplementedException("Area not implemented for IfcCompositeCurve");
            }

            return double.NaN;
        }
    }
}

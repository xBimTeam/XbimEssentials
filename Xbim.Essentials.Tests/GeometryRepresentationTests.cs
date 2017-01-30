using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Geometry;
using Xbim.Ifc2x3.Extensions;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.ProfileResource;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem(@"TestSourceFiles\")]
    public class GeometryRepresentationTests
    {
        private const double DegreeToRadiantFactor = Math.PI / 180.0;

        [TestMethod]
        public void TrimmedCurveFromCircleInCompositCurveSegmentList()
        {
            using (var model = new XbimModel())
            {
                /* Transformed points on circle arc should correspond to the next
                 * points of the composite curve segments.
                 */

                model.CreateFrom("271830-A-KA-A-Central v03.ifc", null, null, true);

                var space = model.IfcProducts.First(p => p.EntityLabel == 2624) as IfcSpace;
                Assert.IsNotNull(space);

                var representation = space.Representation.Representations.OfType<IfcShapeRepresentation>().First();
                var extrudedAreaSolid = representation.Items.OfType<IfcExtrudedAreaSolid>().First();
                var arbitraryClosedProfileDef = extrudedAreaSolid.SweptArea as IfcArbitraryClosedProfileDef;
                Assert.IsNotNull(arbitraryClosedProfileDef);
                var compositeCurve = arbitraryClosedProfileDef.OuterCurve as IfcCompositeCurve;
                Assert.IsNotNull(compositeCurve);

                var polylinePredecessor = compositeCurve.Segments[4].ParentCurve as IfcPolyline;
                Assert.IsNotNull(polylinePredecessor);
                var trimmedCurve = compositeCurve.Segments[5].ParentCurve as IfcTrimmedCurve;
                Assert.IsNotNull(trimmedCurve);
                var polylineSuccessor = compositeCurve.Segments[6].ParentCurve as IfcPolyline;
                Assert.IsNotNull(polylineSuccessor);

                var pointPredecessor = polylinePredecessor.Points.Last();
                var pointSuccessor = polylineSuccessor.Points.First();
                var circle = trimmedCurve.BasisCurve as IfcCircle;
                Assert.IsNotNull(circle);
                var axis2Placement2D = circle.Position as IfcAxis2Placement2D;
                XbimMatrix3D matrix3D = axis2Placement2D.ToMatrix3D();
                var t1 = (IfcParameterValue) trimmedCurve.Trim1[0];
                var t2 = (IfcParameterValue) trimmedCurve.Trim2[0];
                var r1 = DegreeToRadiant(t1);
                var r2 = DegreeToRadiant(t2);
                var radius = (double) circle.Radius;

                var pointOnArc1 = GetCircleArcPoint2D(r1, radius, matrix3D);
                var pointOnArc2 = GetCircleArcPoint2D(r2, radius, matrix3D);

                Assert.IsTrue(compositeCurve.Segments[4].SameSense);
                Assert.IsFalse(compositeCurve.Segments[5].SameSense); // circle arc in counter direction
                Assert.IsTrue(compositeCurve.Segments[6].SameSense);

                Assert.IsTrue(pointPredecessor.IsEqual(pointOnArc2, 0.001));
                Assert.IsTrue(pointSuccessor.IsEqual(pointOnArc1, 0.001));

                model.Close();
            }
        }

        private static double DegreeToRadiant(double degree)
        {
            return degree * DegreeToRadiantFactor;
        }

        public static IfcCartesianPoint GetCircleArcPoint2D(double rad, double radius, XbimMatrix3D m)
        {
            double x = Math.Cos(rad) * radius;
            double y = Math.Sin(rad) * radius;

            var p = m.Transform(new XbimPoint3D(x, y, 0));
            return new IfcCartesianPoint(p.X, p.Y, double.NaN);
        }
    }
}
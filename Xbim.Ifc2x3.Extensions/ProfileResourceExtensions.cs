using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc2x3.ProfileResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class ProfileResourceExtensions
    {
        public static double GetAreaInSquareMeters(this IfcProfileDef Profile)
        {
            var factorSquareMetre = Math.Pow(Profile.Model.ModelFactors.OneMetre, 2);
            return Profile.GetArea() / factorSquareMetre;
        }
        public static double GetArea(this IfcProfileDef Profile)
        {
            // Inheritance tree of IfcProfileDef:
            //
            //- IfcArbitraryClosedProfileDef (Throw) 
            //  - IfcArbitraryProfileDefWithVoids (Throw) 
            //- IfcArbitraryOpenProfileDef (x)
            //  - IfcCenterLineProfileDef (x)
            //- IfcParameterizedProfileDef  (abstract)
            //  - IfcIShapeProfileDef (x)
            //    - IfcAsymmetricIShapeProfileDef (x)
            //  - IfcCircleProfileDef (x)
            //    - IfcCircleHollowProfileDef (x)
            //  - IfcCraneRailAShapeProfileDef (Throw) 
            //  - IfcCraneRailFShapeProfileDef (Throw)
            //  - IfcCShapeProfileDef (x)
            //  - IfcEllipseProfileDef (x)
            //  - IfcLShapeProfileDef (Throw)
            //  - IfcRectangleProfileDef (x)
            //    - IfcRectangleHollowProfileDef (x)
            //    - IfcRoundedRectangleProfileDef (x)
            //  - IfcTrapeziumProfileDef (x)
            //  - IfcTShapeProfileDef (Throw)
            //  - IfcUShapeProfileDef (Throw)
            //  - IfcZShapeProfileDef (Throw)
            //- IfcCompositeProfileDef 
            if (Profile is IfcArbitraryProfileDefWithVoids)
            {
                throw new NotImplementedException("IfcArbitraryProfileDefWithVoids Area is not implemented");
            }
            else if (Profile is IfcArbitraryClosedProfileDef)
            {
                throw new NotImplementedException("IfcArbitraryClosedProfileDef Area is not implemented");
            }
            else if (Profile is IfcRoundedRectangleProfileDef)
            {
                IfcRoundedRectangleProfileDef p = Profile as IfcRoundedRectangleProfileDef;
                return (p.XDim * p.YDim) - (4 * ConcaveRightAngleFilletArea(p.RoundingRadius));
            }
            else if (Profile is IfcRectangleHollowProfileDef)
            {
                IfcRectangleHollowProfileDef p = Profile as IfcRectangleHollowProfileDef;

                // outer area
                double outerFillet = 0;
                if (p.OuterFilletRadius.HasValue)
                    outerFillet = p.OuterFilletRadius.Value;
                double outer = (p.XDim * p.YDim) - (4 * ConcaveRightAngleFilletArea(outerFillet));
                // inner area
                double innerFillet = 0;
                if (p.InnerFilletRadius.HasValue)
                    innerFillet = p.InnerFilletRadius.Value;
                double inner = ((p.XDim - 2 * p.WallThickness) * (p.YDim - 2 * p.WallThickness))
                    - (4 * ConcaveRightAngleFilletArea(innerFillet));

                return outer - inner;
            }
            else if (Profile is IfcRectangleProfileDef)
            {
                IfcRectangleProfileDef p = Profile as IfcRectangleProfileDef;
                return p.XDim * p.YDim;
            }
            else if (Profile is IfcCircleHollowProfileDef)
            {
                IfcCircleHollowProfileDef p = Profile as IfcCircleHollowProfileDef;
                double outer = Math.PI * Math.Pow(p.Radius, 2);
                double inner = Math.PI * Math.Pow(p.Radius - p.WallThickness, 2);
                return outer - inner;
            }
            else if (Profile is IfcCircleProfileDef)
            {
                IfcCircleProfileDef p = Profile as IfcCircleProfileDef;
                return Math.PI * Math.Pow(p.Radius, 2);
            }
            else if (Profile is IfcCraneRailAShapeProfileDef)
            {
                // geometry is not clear
                throw new NotImplementedException("IfcCraneRailAShapeProfileDef Area is not implemented");
            }
            else if (Profile is IfcCraneRailFShapeProfileDef)
            {
                // geometry is not clear
                throw new NotImplementedException("IfcCraneRailFShapeProfileDef Area is not implemented");
            }
            else if (Profile is IfcCShapeProfileDef)
            {
                IfcCShapeProfileDef p = Profile as IfcCShapeProfileDef;

                // inner area
                double innerFillet = 0;
                if (p.InternalFilletRadius.HasValue)
                    innerFillet = p.InternalFilletRadius.Value;
                double inner = ((p.Width - 2 * p.WallThickness) * (p.Depth - 2 * p.WallThickness))
                    - (4 * ConcaveRightAngleFilletArea(innerFillet));

                // outer area
                double outerFillet = innerFillet + p.WallThickness;
                double outer = (p.Width * p.Depth) - (4 * ConcaveRightAngleFilletArea(outerFillet));

                double girthSideVoid = (p.Depth - 2 * p.Girth) * p.WallThickness;

                //     closed loop area - missing part between girths
                return (outer - inner) - girthSideVoid;
            }
            else if (Profile is IfcEllipseProfileDef)
            {
                IfcEllipseProfileDef p = Profile as IfcEllipseProfileDef;
                return Math.PI * p.SemiAxis1 * p.SemiAxis2;
            }
            else if (Profile is IfcLShapeProfileDef)
            {
                // geometry is not clear
                throw new NotImplementedException("IfcLShapeProfileDef Area is not implemented");
            }
            else if (Profile is IfcTrapeziumProfileDef)
            {
                IfcTrapeziumProfileDef p = Profile as IfcTrapeziumProfileDef;
                return (p.BottomXDim + p.TopXDim) * p.YDim / 2;
            }
            else if (Profile is IfcTShapeProfileDef)
            {
                // geometry is not clear
                throw new NotImplementedException("IfcTShapeProfileDef Area is not implemented");
            }
            else if (Profile is IfcUShapeProfileDef)
            {
                // geometry is not clear
                throw new NotImplementedException("IfcUShapeProfileDef Area is not implemented");
            }
            else if (Profile is IfcZShapeProfileDef)
            {
                // geometry is not clear
                throw new NotImplementedException("IfcZShapeProfileDef Area is not implemented");
            }
            else if (Profile is IfcAsymmetricIShapeProfileDef) // needs to be tested before IfcIShapeProfileDef
            {
                IfcAsymmetricIShapeProfileDef p = Profile as IfcAsymmetricIShapeProfileDef;
                // bottom flange
                double bottomflange = p.OverallWidth * p.FlangeThickness;
                // bottom flange
                double TopFlangeThickness = p.FlangeThickness; // if not specified differently
                if (p.TopFlangeThickness.HasValue)
                    TopFlangeThickness = p.TopFlangeThickness.Value;
                double topFlange = p.TopFlangeWidth * TopFlangeThickness;
                // core
                double core = (p.OverallDepth - p.FlangeThickness - TopFlangeThickness) * p.WebThickness;

                double bottomfillets = 0;
                double topFillets = 0;
                if (p.FilletRadius.HasValue)
                {
                    bottomfillets = 2 * ConcaveRightAngleFilletArea(p.FilletRadius.Value);
                    topFillets = bottomfillets;
                }
                if (p.TopFlangeFilletRadius.HasValue)
                {
                    topFillets = 2 * ConcaveRightAngleFilletArea(p.TopFlangeFilletRadius.Value);
                }
                return bottomflange + topFlange + core + bottomfillets + topFillets;
            }
            else if (Profile is IfcIShapeProfileDef)
            {
                IfcIShapeProfileDef p = Profile as IfcIShapeProfileDef;
                double flanges = p.OverallWidth * p.FlangeThickness * 2; // top and bottom flanges
                double core = (p.OverallDepth - (p.FlangeThickness * 2)) * p.WebThickness;
                double fillets = 0;
                if (p.FilletRadius.HasValue && p.FilletRadius.Value > 0)
                {
                    fillets = 4 * ConcaveRightAngleFilletArea(p.FilletRadius.Value);
                }
                return flanges + core + fillets;
            }
            else if (Profile is IfcArbitraryOpenProfileDef)
            {
                return 0;
            }
            return double.NaN;
        }

        public static double GetPerimeterInMeters(this IfcProfileDef Profile, bool addPerimetersOfVoids = true)
        {
            return Profile.GetPerimeter(addPerimetersOfVoids) / Profile.Model.ModelFactors.OneMetre;
        }
        public static double GetPerimeter(this IfcProfileDef Profile, bool addPerimetersOfVoids = true)
        {
            // Inheritance tree of IfcProfileDef:
            //
            //- IfcArbitraryClosedProfileDef 
            //  - IfcArbitraryProfileDefWithVoids 
            //- IfcArbitraryOpenProfileDef 
            //  - IfcCenterLineProfileDef 
            //- IfcParameterizedProfileDef  (abstract)
            //  - IfcIShapeProfileDef 
            //    - IfcAsymmetricIShapeProfileDef 
            //  - IfcCircleProfileDef 
            //    - IfcCircleHollowProfileDef 
            //  - IfcCraneRailAShapeProfileDef (Throw) 
            //  - IfcCraneRailFShapeProfileDef (Throw)
            //  - IfcCShapeProfileDef 
            //  - IfcEllipseProfileDef 
            //  - IfcLShapeProfileDef (Throw)
            //  - IfcRectangleProfileDef 
            //    - IfcRectangleHollowProfileDef 
            //    - IfcRoundedRectangleProfileDef 
            //  - IfcTrapeziumProfileDef 
            //  - IfcTShapeProfileDef (Throw)
            //  - IfcUShapeProfileDef (Throw)
            //  - IfcZShapeProfileDef (Throw)
            //- IfcCompositeProfileDef 
            if (Profile is IfcArbitraryProfileDefWithVoids)
            {
                throw new NotImplementedException("IfcArbitraryProfileDefWithVoids Perimeter is not implemented");
            }
            else if (Profile is IfcArbitraryClosedProfileDef)
            {
                throw new NotImplementedException("IfcArbitraryClosedProfileDef Perimeter is not implemented");
            }
            else if (Profile is IfcRoundedRectangleProfileDef)
            {
                IfcRoundedRectangleProfileDef p = Profile as IfcRoundedRectangleProfileDef;
                return RoundedRecPerimeter(p.XDim, p.YDim, p.RoundingRadius);
            }
            else if (Profile is IfcRectangleHollowProfileDef)
            {
                IfcRectangleHollowProfileDef p = Profile as IfcRectangleHollowProfileDef;

                // outer 2P
                double outerFillet = 0;
                if (p.OuterFilletRadius.HasValue)
                    outerFillet = p.OuterFilletRadius.Value;
                double outer = RoundedRecPerimeter(p.XDim, p.YDim, outerFillet);

                if (!addPerimetersOfVoids)
                    return outer;
                // inner 2P
                double innerFillet = 0;
                if (p.InnerFilletRadius.HasValue)
                    innerFillet = p.InnerFilletRadius.Value;
                double inner = RoundedRecPerimeter(p.XDim - 2 * p.WallThickness, p.YDim - 2 * p.WallThickness, innerFillet);

                // value
                return outer + inner;
            }
            else if (Profile is IfcRectangleProfileDef)
            {
                IfcRectangleProfileDef p = Profile as IfcRectangleProfileDef;
                return 2 * (p.XDim + p.YDim);
            }
            else if (Profile is IfcCircleHollowProfileDef)
            {
                IfcCircleHollowProfileDef p = Profile as IfcCircleHollowProfileDef;
                double outer = 2 * Math.PI * p.Radius;
                if (!addPerimetersOfVoids)
                    return outer;

                double inner = 2 * Math.PI * (p.Radius - p.WallThickness);

                return outer - inner;
            }
            else if (Profile is IfcCircleProfileDef)
            {
                IfcCircleProfileDef p = Profile as IfcCircleProfileDef;
                return 2 * Math.PI * p.Radius;
            }
            else if (Profile is IfcCraneRailAShapeProfileDef)
            {
                // geometry is not clear
                throw new NotImplementedException("IfcCraneRailAShapeProfileDef Perimeter is not implemented");
            }
            else if (Profile is IfcCraneRailFShapeProfileDef)
            {
                // geometry is not clear
                throw new NotImplementedException("IfcCraneRailFShapeProfileDef Perimeter is not implemented");
            }
            else if (Profile is IfcCShapeProfileDef)
            {
                IfcCShapeProfileDef p = Profile as IfcCShapeProfileDef;

                double raIn = 0;
                if (p.InternalFilletRadius.HasValue)
                    raIn = p.InternalFilletRadius.Value;
                double raOut = raIn + p.WallThickness;

                return
                    2 * Math.PI * raIn + // internal fillet
                    2 * Math.PI * raOut + // external fillet
                    4 * (p.Girth - raOut) + // girt extension
                    2 * p.WallThickness + // girt closures
                    4 * (p.Width - 2 * raOut) + // top and bottom connections
                    2 * (p.Depth - 2 * raOut); // left connections
            }
            else if (Profile is IfcEllipseProfileDef)
            {
                IfcEllipseProfileDef p = Profile as IfcEllipseProfileDef;
                // http://www.mathsisfun.com/geometry/ellipse-perimeter.html
                // Ramanujan  approx 3
                double h = Math.Pow((p.SemiAxis1 - p.SemiAxis2), 2) / Math.Pow((p.SemiAxis1 + p.SemiAxis2), 2);
                return
                    Math.PI * (p.SemiAxis1 + p.SemiAxis2) * (1 + 3 * h / (10 + Math.Sqrt(4 - 3 * h)));
                    

            }
            else if (Profile is IfcLShapeProfileDef)
            {
                // geometry is not clear
                throw new NotImplementedException("IfcLShapeProfileDef Perimeter is not implemented");
            }
            else if (Profile is IfcTrapeziumProfileDef)
            {
                IfcTrapeziumProfileDef p = Profile as IfcTrapeziumProfileDef;
                double diag1 = pita(p.YDim, p.TopXOffset);
                double diag2 = pita(p.YDim, p.BottomXDim - p.TopXOffset - p.TopXDim);
                return p.BottomXDim + p.TopXDim + diag1 + diag2;
            }
            else if (Profile is IfcTShapeProfileDef)
            {
                // geometry is not clear
                throw new NotImplementedException("IfcTShapeProfileDef Perimeter is not implemented");
            }
            else if (Profile is IfcUShapeProfileDef)
            {
                // geometry is not clear
                throw new NotImplementedException("IfcUShapeProfileDef Perimeter is not implemented");
            }
            else if (Profile is IfcZShapeProfileDef)
            {
                // geometry is not clear
                throw new NotImplementedException("IfcZShapeProfileDef Perimeter is not implemented");
            }
            else if (Profile is IfcAsymmetricIShapeProfileDef) // needs to be tested before IfcIShapeProfileDef
            {
                throw new NotImplementedException("IfcAsymmetricIShapeProfileDef Perimeter is not implemented");
                // IfcAsymmetricIShapeProfileDef p = Profile as IfcAsymmetricIShapeProfileDef;
            }
            else if (Profile is IfcIShapeProfileDef)
            {
                throw new NotImplementedException("IfcAsymmetricIShapeProfileDef Perimeter is not implemented");
                // IfcIShapeProfileDef p = Profile as IfcIShapeProfileDef;
            }
            else if (Profile is IfcArbitraryOpenProfileDef)
            {
                return 0;
            }
            return double.NaN;
        }

        private static double pita(IfcPositiveLengthMeasure cat1, IfcLengthMeasure cat2)
        {
            return Math.Sqrt(Math.Pow(cat1, 2) + Math.Pow(cat2, 2)); 
        }

        private static double RoundedRecPerimeter(IfcPositiveLengthMeasure XDim, IfcPositiveLengthMeasure YDim, IfcPositiveLengthMeasure RoundingRadius)
        {
            return
                    2 * Math.PI * RoundingRadius + // circunference of 4 fillets
                    2 * (XDim - (2 * RoundingRadius)) + // 2 sides, each removed of 2 fillets (x dim)
                    2 * (YDim - (2 * RoundingRadius)); // 2 sides, each removed of 2 fillets (y dim)
        }

        private static double ConcaveRightAngleFilletArea(double val)
        {
            // square - circle area 
            double wholeSquare = Math.Pow((2 * val), 2)  // the square area 
                - Math.PI * Math.Pow(val, 2); // the inner circle to be removed
            return wholeSquare / 4; // square is 4 fillets
        }
    }
}

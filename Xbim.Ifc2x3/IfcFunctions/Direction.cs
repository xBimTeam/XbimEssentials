using System;
using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.MeasureResource;

namespace Xbim.Ifc2x3
{
    internal class Direction : IVectorOrDirection
    {
        public int Dim { get; set; }
        public double[] DirectionRatios { get; set; }

        public double X { get { return DirectionRatios[0]; } }
        public double Y { get { return DirectionRatios[1]; } }
        public double Z { get { return DirectionRatios[2]; } }

        public Direction(double x, double y)
        {
            DirectionRatios = new[] {0.0, 0.0, 0.0};
            Dim = 2;
            DirectionRatios[0] = x;
            DirectionRatios[1] = y;
        }

        public Direction(double x, double y, double z)
        {
            DirectionRatios = new[] {0.0, 0.0, 0.0};
            Dim = 3;
            DirectionRatios[0] = x;
            DirectionRatios[1] = y;
            DirectionRatios[2] = z;
        }

        public Direction(IfcDirection from)
        {
            DirectionRatios = new[] {0.0, 0.0, 0.0};
            DirectionRatios[0] = from.X;
            DirectionRatios[1] = from.Y;
            DirectionRatios[2] = from.Z;
            Dim = (int) @from.Dim;
        }
    }
}
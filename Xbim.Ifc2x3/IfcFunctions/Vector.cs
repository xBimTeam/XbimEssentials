using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc2x3   
{
    internal class Vector : IVectorOrDirection
    {
        public Vector(Direction direction, double v)
        {
            this.Orientation = direction;
            this.Magnitude = v;
        }

        public int Dim
        {
            get { return Orientation.Dim; }
            set { Orientation.Dim = value; }
        }

        public double[] DirectionRatios
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Direction Orientation { get; set; }
        public double Magnitude { get; set; }
    }
}

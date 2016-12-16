using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc4.Functions
{
    internal class Vector
    {
        public Vector(Direction orientation, double mod)
        {
            Orientation = orientation;
            Magnitude = mod;
        }

        internal double Magnitude { get; set; }

        internal Direction Orientation { get; set; }

        public int Dim => Orientation.Dim;
    }
}

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
        private double _mag;
        private Direction _orientation;

        public Vector(Direction orientation, double mod)
        {
            _orientation = orientation;
            _mag = mod;
        }

        internal double Magnitude
        {
            get { return _mag; }
            set { _mag = value; }
        }

        internal Direction Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }
    }
}

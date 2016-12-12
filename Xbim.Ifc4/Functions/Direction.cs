using System;
using Xbim.Common;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Ifc4.Functions
{
    internal class Direction
    {
        private int _dim = 0;
        private double _x;
        private double _y;
        private double _z;

        public Direction(double x, double y)
        {
            _x = x;
            _y = y;
            _dim = 2;
        }

        public Direction(double x, double y, double z)
        {
            _x = x;
            _y = y;
            _z = z;
            _dim = 3;
        }

        internal Direction(IIfcDirection source)
        {
            if (source.Dim == 2)
            {
                _dim = 2;
                _x = source.X;
                _y = source.Y;
            }
            else
            {
                _dim = 3;
                _x = source.X;
                _y = source.Y;
                _z = source.Z;
            }
        }

        // implicit conversion from IfcDirection
        public static implicit operator Direction(IfcDirection d)
        {
            return new Direction(d);
        }
        
        public IItemSet<IfcReal> DirectionRatios
        {
            set
            {
                if (value.Count == 2)
                {
                    _dim = 2;
                    _x = value[0];
                    _y = value[1];
                    return;
                }
                if (value.Count == 3)
                {
                    _dim = 3;
                    _x = value[0];
                    _y = value[1];
                    _z = value[2];
                    return;
                }
                throw new Exception("Invalid number of directionratios.");
            }
        }
    }
}
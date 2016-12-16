using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc4.GeometryResource;

namespace Xbim.Ifc4.Functions
{
    internal class VectorOrDirection
    {
        private readonly Direction _direction;
        private readonly Vector _vector;

        public VectorOrDirection(Direction direction)
        {
            _direction = direction;
            _vector = null;
        }

        public static implicit operator Direction(VectorOrDirection d)
        {
            return d.ToDirection();
        }

        public int Dim => _direction?.Dim ?? _vector.Dim;

        internal Direction ToDirection()
        {
            return 
                _direction 
                ?? _vector.Orientation;
        }


        public static implicit operator VectorOrDirection(IfcDirection d)
        {
            return new VectorOrDirection(d);
        }

        public Vector ToVector()
        {
            if (_direction != null)
            {
                var v = new Vector(_direction, 1);
            }
            return _vector;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.Ifc4.Functions
{
    class VectorOrDirection
    {
        private Direction _direction;
        private Vector _vector;

        public VectorOrDirection(Direction direction)
        {
            _direction = direction;
            _vector = null;
        }
    }
}

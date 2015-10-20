using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Ifc2x3.GeometryResource
{
    public partial class IfcPolyline
    {
        /// <summary>
        ///   The dimensionality of all points must be the same and is equal to the forst point
        /// </summary>

        public override IfcDimensionCount Dim
        {
            get { return _points.Count > 0 ? _points[0].Dim : (IfcDimensionCount)0; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Ifc2x3.GeometryResource
{
    public abstract partial class IfcCurve
    {
        /// <summary>
        ///   Derived. The space dimensionality of this abstract class, defined differently for all subtypes, i.e. for IfcLine, IfcConic and IfcBoundedCurve.
        /// </summary>
        public abstract IfcDimensionCount Dim { get; }
    }
}

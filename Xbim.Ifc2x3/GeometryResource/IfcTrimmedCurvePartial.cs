using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xbim.Ifc2x3.GeometryResource
{
    public partial class IfcTrimmedCurve
    {
        public override IfcDimensionCount Dim
        {
            get { return BasisCurve.Dim; }
        }
    }
}

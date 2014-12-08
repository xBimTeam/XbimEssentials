using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.IO
{
    public enum XbimGeometrySort
    {
        OrderByIfcSurfaceStyleThenIfcType,
        OrderByIfcTypeThenIfcProduct,
        OrderByGeometryID

    }
}

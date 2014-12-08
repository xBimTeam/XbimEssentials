using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xbim.IO
{
    public enum XbimGeometryType : byte
    {
        BoundingBox = 0x01,
        MultipleBoundingBox = 0x02,
        TriangulatedMesh = 0x03
    }
}

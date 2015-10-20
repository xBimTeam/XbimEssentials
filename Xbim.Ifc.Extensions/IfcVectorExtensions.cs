using Xbim.Common.Geometry;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcVectorExtensions
    {
        /// <summary>
        ///   Converts an Ifc 3D vector to an Xbim Vector3D
        /// </summary>
        /// <returns></returns>
        public static XbimVector3D XbimVector3D(this IfcVector ifcVec)
        {
            XbimVector3D vec;
            if (ifcVec.Orientation.Dim > 2)
                vec = new XbimVector3D(ifcVec.Orientation.X, ifcVec.Orientation.Y, ifcVec.Orientation.Z);
            else if (ifcVec.Orientation.Dim == 2)
                vec = new XbimVector3D(ifcVec.Orientation.X, ifcVec.Orientation.Y, 0);
            else
                vec = new XbimVector3D();
            vec.Normalize(); //orientation is not normalized
            vec *= ifcVec.Magnitude;
            return vec;
        }

    }
}

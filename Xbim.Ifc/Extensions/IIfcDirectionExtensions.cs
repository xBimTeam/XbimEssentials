using Xbim.Common.Geometry;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc
{
    public static class IIfcDirectionExtensions
    {
        public static XbimVector3D XbimVector3D(this IIfcDirection obj)
        {
            return new XbimVector3D(obj.X, obj.Y, double.IsNaN(obj.Z) ? 0 : obj.Z);
        }


        /// <summary>
        /// Computes and returns the normalised vector for the direction.
        /// </summary>
        /// <returns>A 1-length vector if the direction is meaningful or a 0-length vector otherwise</returns>
        public static XbimVector3D Normalise(this IIfcDirection obj)
        {
            if (obj.Dim == 3)
            {
                var v3D = new XbimVector3D(obj.X, obj.Y, obj.Z);
                v3D.Normalized();
                return v3D;
            }
            // Since the return value is not stored in any field or property 
            // and the function return variable is intrinsically 3D it's reasonable do 
            // deal with dimensions lower than 3
            //
            var compX = obj.X; // each value is nan if the dimension is not specified
            var compY = obj.Y;
            var compZ = obj.Z;

            // substitite nan for 0
            if (double.IsNaN(compX))
                compX = 0;
            if (double.IsNaN(compY))
                compY = 0;
            if (double.IsNaN(compZ))
                compZ = 0;

            var otherCases = new XbimVector3D(compX, compY, compZ);
            // normalied return a 0-len-vector if no significant direction exists
            otherCases.Normalized();
            return otherCases;
        }


        public static void SetXY(this IIfcDirection obj, double x, double y)
        {
            obj.DirectionRatios.Clear();
            obj.DirectionRatios.Add(x);
            obj.DirectionRatios.Add(y);
        }

        public static void SetXYZ(this IIfcDirection obj,double x, double y, double z)
        {
            obj.DirectionRatios.Clear();
            obj.DirectionRatios.Add(x);
            obj.DirectionRatios.Add(y);
            obj.DirectionRatios.Add(z);
        }
    }
}

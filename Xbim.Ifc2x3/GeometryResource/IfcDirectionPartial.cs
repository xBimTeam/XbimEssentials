using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Geometry;

namespace Xbim.Ifc2x3.GeometryResource
{

    public partial class IfcDirection
    {

        public double X
        {
            get
            {
                return DirectionRatios.Count == 0 ? double.NaN : DirectionRatios[0];
            }
            set
            {
                if(DirectionRatios.Count == 0)
                    DirectionRatios.Add(value);
                else
                    DirectionRatios[0] = value;
            }
        }

        public double Y
        {
            get
            {
                return DirectionRatios.Count < 2 ? double.NaN : DirectionRatios[1];
            }
            set
            {
                if (DirectionRatios.Count < 2)
                {
                    if (DirectionRatios.Count == 0) DirectionRatios.Add(double.NaN);
                    DirectionRatios.Add(value);
                }
                else
                    DirectionRatios[1] = value;
            }
        }

        public double Z
        {
            get
            {
                return DirectionRatios.Count < 3 ? double.NaN : DirectionRatios[2];
            }
            set
            {
                if (DirectionRatios.Count < 3)
                {
                    if (DirectionRatios.Count == 0) DirectionRatios.Add(double.NaN);
                    if (DirectionRatios.Count == 1) DirectionRatios.Add(double.NaN);
                    DirectionRatios.Add(value);
                }
                else
                    DirectionRatios[2] = value;
            }
        }
        public XbimVector3D XbimVector3D()
        {
            return new XbimVector3D(X, Y, double.IsNaN(Z) ? 0 : Z);
        }


        /// <summary>
        /// Computes and returns the normalised vector for the direction.
        /// </summary>
        /// <returns>A 1-length vector if the direction is meaningful or a 0-length vector otherwise</returns>
        public XbimVector3D Normalise()
        {
            if (Dim == 3)
            {
                var v3D = new XbimVector3D(X, Y, Z);
                v3D.Normalized();
                return v3D;
            }
            // Since the return value is not stored in any field or property 
            // and the function return variable is intrinsically 3D it's reasonable do 
            // deal with dimensions lower than 3
            //
            var compX = X; // each value is nan if the dimension is not specified
            var compY = Y;
            var compZ = Z;

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


        public void SetXY(double x, double y)
        {
            DirectionRatios.Clear();
            DirectionRatios.Add(x);
            DirectionRatios.Add(y);
        }

        public void SetXYZ(double x, double y, double z)
        {
            DirectionRatios.Clear();
            DirectionRatios.Add(x);
            DirectionRatios.Add(y);
            DirectionRatios.Add(z);
        }
    }
}

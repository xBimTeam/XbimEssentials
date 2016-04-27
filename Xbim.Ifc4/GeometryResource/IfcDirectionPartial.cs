﻿using System;
using Xbim.Common.Geometry;

namespace Xbim.Ifc4.Interfaces
{
    /// <summary>
    /// Readonly interface for IfcDirection
    /// </summary>
    // ReSharper disable once PartialTypeWithSinglePart
    public partial interface @IIfcDirection
    {
        double X { get; }
        double Y { get; }
        double Z { get; }

    }
}
namespace Xbim.Ifc4.GeometryResource
{
    public partial class IfcDirection
    {
        public double X
        {
            get
            {
                return DirectionRatios.Count == 0 ? double.NaN : (double)DirectionRatios[0];
            }
            set
            {
                if (DirectionRatios.Count == 0)
                    DirectionRatios.Add(value);
                else
                    DirectionRatios[0] = value;
            }
        }

        public double Y
        {
            get
            {
                return DirectionRatios.Count < 2 ? double.NaN : (double)DirectionRatios[1];
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
                return DirectionRatios.Count < 3 ? double.NaN : (double)DirectionRatios[2];
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


        public XbimVector3D Normalise()
        {
            if (Dim == 3)
            {
                var v3D = new XbimVector3D(X, Y, Z);
                v3D.Normalized();
                return v3D;
            }
            throw new ArgumentException("Only 3D Directions are supported for normalised at present");
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

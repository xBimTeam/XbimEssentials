using System;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Ifc.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IIfcCartesianTransformExtensions
    {

        public static XbimMatrix3D ToMatrix3D(this IIfcCartesianTransformationOperator ct)
        {
            if (ct is IIfcCartesianTransformationOperator3DnonUniform)
                return ((IIfcCartesianTransformationOperator3DnonUniform)ct).ToMatrix3D();
            else if (ct is IIfcCartesianTransformationOperator3D)
                return ((IIfcCartesianTransformationOperator3D)ct).ToMatrix3D();
            else throw new ArgumentException("ToMatrix3D", "ct");

        }

        /// <summary>
        ///   Builds a windows XbimMatrix3D from a CartesianTransformationOperator3D
        /// </summary>
        /// <param name = "ct3D"></param>
        /// <returns></returns>
        public static XbimMatrix3D ToMatrix3D(this IIfcCartesianTransformationOperator3D ct3D)
        {
            return ConvertCartesianTranformOperator3D(ct3D);         
        }

        private static XbimMatrix3D ConvertCartesianTranformOperator3D(IIfcCartesianTransformationOperator3D ct3D)
        {
            var m3D = ConvertCartesianTransform3D(ct3D);

            m3D.Scale(ct3D.Scl);
            return m3D;
        }

        /// <summary>
        ///   Builds a windows XbimMatrix3D from a CartesianTransformationOperator3DnonUniform
        /// </summary>
        /// <param name = "ct3D"></param>
        /// <returns></returns>
        public static XbimMatrix3D ToMatrix3D(this IIfcCartesianTransformationOperator3DnonUniform ct3D)
        {
            return ConvertCartesianTransformationOperator3DnonUniform(ct3D);   
        }

        private static XbimMatrix3D ConvertCartesianTransformationOperator3DnonUniform(IIfcCartesianTransformationOperator3DnonUniform ct3D)
        {
            XbimVector3D u3; //Z Axis Direction
            XbimVector3D u2; //X Axis Direction
            XbimVector3D u1; //Y axis direction
            if (ct3D.Axis3 != null)
            {
                var dir = ct3D.Axis3;
                u3 = new XbimVector3D(dir.X, dir.Y, dir.Z);
                u3 = u3.Normalized();
            }
            else
                u3 = new XbimVector3D(0, 0, 1);
            if (ct3D.Axis1 != null)
            {
                var dir = ct3D.Axis1;
                u1 = new XbimVector3D(dir.X, dir.Y, dir.Z);
                u1 = u1.Normalized();
            }
            else
            {
                var defXDir = new XbimVector3D(1, 0, 0);
                u1 = u3 != defXDir ? defXDir : new XbimVector3D(0, 1, 0);
            }
            var xVec = XbimVector3D.Multiply(XbimVector3D.DotProduct(u1, u3), u3);
            var xAxis = XbimVector3D.Subtract(u1, xVec);
            xAxis = xAxis.Normalized();

            if (ct3D.Axis2 != null)
            {
                var dir = ct3D.Axis2;
                u2 = new XbimVector3D(dir.X, dir.Y, dir.Z);
                u2 = u2.Normalized();
            }
            else
                u2 = new XbimVector3D(0, 1, 0);

            var tmp = XbimVector3D.Multiply(XbimVector3D.DotProduct(u2, u3), u3);
            var yAxis = XbimVector3D.Subtract(u2, tmp);
            tmp = XbimVector3D.Multiply(XbimVector3D.DotProduct(u2, xAxis), xAxis);
            yAxis = XbimVector3D.Subtract(yAxis, tmp);
            yAxis = yAxis.Normalized();
            u2 = yAxis;
            u1 = xAxis;

            var lo = new XbimPoint3D(ct3D.LocalOrigin.X, ct3D.LocalOrigin.Y, ct3D.LocalOrigin.Z); //local origin

            var matrix = new XbimMatrix3D(u1.X, u1.Y, u1.Z, 0,
                                           u2.X, u2.Y, u2.Z, 0,
                                           u3.X, u3.Y, u3.Z, 0,
                                           lo.X, lo.Y, lo.Z, 1);
            matrix.Scale(new XbimVector3D(ct3D.Scl, ct3D.Scl2, ct3D.Scl3));

            return matrix;
        }

        private static XbimMatrix3D ConvertCartesianTransform3D(IIfcCartesianTransformationOperator3D ct3D)
        {
            XbimVector3D u3; //Z Axis Direction
            XbimVector3D u2; //X Axis Direction
            XbimVector3D u1; //Y axis direction
            if (ct3D.Axis3 != null)
            {
                var dir = ct3D.Axis3;
                u3 = new XbimVector3D(dir.X, dir.Y, dir.Z);
                u3 = u3.Normalized();
            }
            else
                u3 = new XbimVector3D(0, 0, 1);
            if (ct3D.Axis1 != null)
            {
                var dir = ct3D.Axis1;
                u1 = new XbimVector3D(dir.X, dir.Y, dir.Z);
                u1 = u1.Normalized();
            }
            else
            {
                var defXDir = new XbimVector3D(1, 0, 0);
                u1 = u3 != defXDir ? defXDir : new XbimVector3D(0, 1, 0);
            }
            var xVec = XbimVector3D.Multiply(XbimVector3D.DotProduct(u1, u3), u3);
            var xAxis = XbimVector3D.Subtract(u1, xVec);
            xAxis = xAxis.Normalized();

            if (ct3D.Axis2 != null)
            {
                var dir = ct3D.Axis2;
                u2 = new XbimVector3D(dir.X, dir.Y, dir.Z);
                u2 = u2.Normalized();
            }
            else
                u2 = new XbimVector3D(0, 1, 0);

            var tmp = XbimVector3D.Multiply(XbimVector3D.DotProduct(u2, u3), u3);
            var yAxis = XbimVector3D.Subtract(u2, tmp);
            tmp = XbimVector3D.Multiply(XbimVector3D.DotProduct(u2, xAxis), xAxis);
            yAxis = XbimVector3D.Subtract(yAxis, tmp);
            yAxis = yAxis.Normalized();
            u2 = yAxis;
            u1 = xAxis;

            var lo = new XbimPoint3D(ct3D.LocalOrigin.X, ct3D.LocalOrigin.Y, ct3D.LocalOrigin.Z); //local origin

            return new XbimMatrix3D(u1.X, u1.Y, u1.Z, 0,
                                           u2.X, u2.Y, u2.Z, 0,
                                           u3.X, u3.Y, u3.Z, 0,
                                           lo.X, lo.Y, lo.Z, 1);

        }
    }
}

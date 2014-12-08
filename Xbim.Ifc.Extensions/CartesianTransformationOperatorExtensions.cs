#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    CartesianTransformationOperatorExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Xbim.Common.Geometry;
using Xbim.Ifc2x3.GeometryResource;


#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class CartesianTransformationOperatorExtensions
    {
        public static XbimMatrix3D ToMatrix3D(this IfcCartesianTransformationOperator ct, ConcurrentDictionary<int, Object> maps = null)
        {
            if (ct is IfcCartesianTransformationOperator3DnonUniform)
               return ((IfcCartesianTransformationOperator3DnonUniform) ct).ToMatrix3D(maps);
            else if (ct is IfcCartesianTransformationOperator3D)
                return ((IfcCartesianTransformationOperator3D) ct).ToMatrix3D(maps);
            else throw new ArgumentException("ToMatrix3D", "Unsupported CartesianTransformationOperator3D");
            
        }

        /// <summary>
        ///   Builds a windows XbimMatrix3D from a CartesianTransformationOperator3D
        /// </summary>
        /// <param name = "ct3D"></param>
        /// <returns></returns>
        public static XbimMatrix3D ToMatrix3D(this IfcCartesianTransformationOperator3D ct3D, ConcurrentDictionary<int, Object> maps = null)
        {
            if (maps == null)
                return ConvertCartesianTranformOperator3D(ct3D);
            else
            {

                object transform;
                if (maps.TryGetValue(ct3D.EntityLabel, out transform)) //already converted it just return cached
                    return (XbimMatrix3D)transform;
                XbimMatrix3D matrix = ConvertCartesianTranformOperator3D(ct3D);
                maps.TryAdd(ct3D.EntityLabel, matrix);
                return matrix;
            }
        }

        private static XbimMatrix3D ConvertCartesianTranformOperator3D(IfcCartesianTransformationOperator3D ct3D)
        {
            XbimMatrix3D m3d = ConvertCartesianTransform3D(ct3D);

            m3d.Scale(ct3D.Scl);
            return m3d;
        }

        /// <summary>
        ///   Builds a windows XbimMatrix3D from a CartesianTransformationOperator3DnonUniform
        /// </summary>
        /// <param name = "ct3D"></param>
        /// <returns></returns>
        public static XbimMatrix3D ToMatrix3D(this IfcCartesianTransformationOperator3DnonUniform ct3D, ConcurrentDictionary<int, Object> maps = null)
        {
            if (maps == null)
                return ConvertCartesianTransformationOperator3DnonUniform(ct3D);
            else
            {
                object transform;
                if (maps.TryGetValue(ct3D.EntityLabel, out transform)) //already converted it just return cached
                    return (XbimMatrix3D)transform;
                XbimMatrix3D matrix = ConvertCartesianTransformationOperator3DnonUniform(ct3D);
                maps.TryAdd(ct3D.EntityLabel, matrix);
                return matrix;
            }
        }

        private static XbimMatrix3D ConvertCartesianTransformationOperator3DnonUniform(IfcCartesianTransformationOperator3DnonUniform ct3D)
        {
            XbimVector3D u3; //Z Axis Direction
            XbimVector3D u2; //X Axis Direction
            XbimVector3D u1; //Y axis direction
            if (ct3D.Axis3 != null)
            {
                IfcDirection dir = ct3D.Axis3;
                u3 = new XbimVector3D(dir.DirectionRatios[0], dir.DirectionRatios[1], dir.DirectionRatios[2]);
                u3.Normalize();
            }
            else
                u3 = new XbimVector3D(0, 0, 1);
            if (ct3D.Axis1 != null)
            {
                IfcDirection dir = ct3D.Axis1;
                u1 = new XbimVector3D(dir.DirectionRatios[0], dir.DirectionRatios[1], dir.DirectionRatios[2]);
                u1.Normalize();
            }
            else
            {
                XbimVector3D defXDir = new XbimVector3D(1, 0, 0);
                u1 = u3 != defXDir ? defXDir : new XbimVector3D(0, 1, 0);
            }
            XbimVector3D xVec = XbimVector3D.Multiply(XbimVector3D.DotProduct(u1, u3), u3);
            XbimVector3D xAxis = XbimVector3D.Subtract(u1, xVec);
            xAxis.Normalize();

            if (ct3D.Axis2 != null)
            {
                IfcDirection dir = ct3D.Axis2;
                u2 = new XbimVector3D(dir.DirectionRatios[0], dir.DirectionRatios[1], dir.DirectionRatios[2]);
                u2.Normalize();
            }
            else
                u2 = new XbimVector3D(0, 1, 0);

            XbimVector3D tmp = XbimVector3D.Multiply(XbimVector3D.DotProduct(u2, u3), u3);
            XbimVector3D yAxis = XbimVector3D.Subtract(u2, tmp);
            tmp = XbimVector3D.Multiply(XbimVector3D.DotProduct(u2, xAxis), xAxis);
            yAxis = XbimVector3D.Subtract(yAxis, tmp);
            yAxis.Normalize();
            u2 = yAxis;
            u1 = xAxis;

            XbimPoint3D lo = ct3D.LocalOrigin.XbimPoint3D(); //local origin

            XbimMatrix3D matrix = new XbimMatrix3D(u1.X, u1.Y, u1.Z, 0,
                                           u2.X, u2.Y, u2.Z, 0,
                                           u3.X, u3.Y, u3.Z, 0,
                                           lo.X, lo.Y, lo.Z, 1);
            matrix.Scale(new XbimVector3D(ct3D.Scl, ct3D.Scl2, ct3D.Scl3));

            return matrix;
        }

        private static XbimMatrix3D ConvertCartesianTransform3D(IfcCartesianTransformationOperator3D ct3D)
        {
            XbimVector3D u3; //Z Axis Direction
            XbimVector3D u2; //X Axis Direction
            XbimVector3D u1; //Y axis direction
            if (ct3D.Axis3 != null)
            {
                IfcDirection dir = ct3D.Axis3;
                u3 = new XbimVector3D(dir.DirectionRatios[0], dir.DirectionRatios[1], dir.DirectionRatios[2]);
                u3.Normalize();
            }
            else
                u3 = new XbimVector3D(0, 0, 1);
            if (ct3D.Axis1 != null)
            {
                IfcDirection dir = ct3D.Axis1;
                u1 = new XbimVector3D(dir.DirectionRatios[0], dir.DirectionRatios[1], dir.DirectionRatios[2]);
                u1.Normalize();
            }
            else
            {
                XbimVector3D defXDir = new XbimVector3D(1, 0, 0);
                u1 = u3 != defXDir ? defXDir : new XbimVector3D(0, 1, 0);
            }
            XbimVector3D xVec = XbimVector3D.Multiply(XbimVector3D.DotProduct(u1, u3), u3);
            XbimVector3D xAxis = XbimVector3D.Subtract(u1, xVec);
            xAxis.Normalize();

            if (ct3D.Axis2 != null)
            {
                IfcDirection dir = ct3D.Axis2;
                u2 = new XbimVector3D(dir.DirectionRatios[0], dir.DirectionRatios[1], dir.DirectionRatios[2]);
                u2.Normalize();
            }
            else
                u2 = new XbimVector3D(0, 1, 0);

            XbimVector3D tmp = XbimVector3D.Multiply(XbimVector3D.DotProduct(u2, u3), u3);
            XbimVector3D yAxis = XbimVector3D.Subtract(u2, tmp);
            tmp = XbimVector3D.Multiply(XbimVector3D.DotProduct(u2, xAxis), xAxis);
            yAxis = XbimVector3D.Subtract(yAxis, tmp);
            yAxis.Normalize();
            u2 = yAxis;
            u1 = xAxis;

            XbimPoint3D lo = ct3D.LocalOrigin.XbimPoint3D(); //local origin

            return new XbimMatrix3D(u1.X, u1.Y, u1.Z, 0,
                                           u2.X, u2.Y, u2.Z, 0,
                                           u3.X, u3.Y, u3.Z, 0,
                                           lo.X, lo.Y, lo.Z, 1);
           
        }
    }
}
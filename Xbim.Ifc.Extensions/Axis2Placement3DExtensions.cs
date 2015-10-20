#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    Axis2Placement3DExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Concurrent;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc2x3.GeometryResource;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class Axis2Placement3DExtensions
    {
        public static XbimVector3D ZAxisDirection(this IfcAxis2Placement3D ax3)
        {
            if (ax3.RefDirection != null && ax3.Axis != null)
            {
                XbimVector3D za = ax3.Axis.XbimVector3D();
                za.Normalize();
                return za;
            }
            else
                return new XbimVector3D(0, 0, 1);
        }

        public static XbimVector3D XAxisDirection(this IfcAxis2Placement3D ax3)
        {
            if (ax3.RefDirection != null && ax3.Axis != null)
            {
                XbimVector3D xa = ax3.RefDirection.XbimVector3D();
                xa.Normalize();
                return xa;
            }
            else
                return new XbimVector3D(1, 0, 0);
        }

        /// <summary>
        ///   Converts an Axis2Placement3D to a windows XbimMatrix3D
        /// </summary>
        /// <param name = "axis3"></param>
        /// <returns></returns>
        public static XbimMatrix3D ToMatrix3D(this IfcAxis2Placement3D axis3, ConcurrentDictionary<int, Object> maps = null)
        {
            if (maps == null)
                return ConvertAxis3D(axis3);
            else
            {
                    object transform;
                    if (maps != null && maps.TryGetValue(axis3.EntityLabel, out transform)) //already converted it just return cached
                        return (XbimMatrix3D)transform;
                    transform = ConvertAxis3D(axis3);
                    if (maps != null) maps.TryAdd(axis3.EntityLabel, transform);
                    return (XbimMatrix3D)transform;
            }
            
        }

        private static XbimMatrix3D ConvertAxis3D(IfcAxis2Placement3D axis3)
        {

            if (axis3.RefDirection != null && axis3.Axis != null)
            {
                XbimVector3D za = axis3.Axis.XbimVector3D();
                za.Normalize();
                XbimVector3D xa = axis3.RefDirection.XbimVector3D();
                xa.Normalize();
                XbimVector3D ya = XbimVector3D.CrossProduct(za, xa);
                ya.Normalize();
                return new XbimMatrix3D(xa.X, xa.Y, xa.Z, 0, ya.X, ya.Y, ya.Z, 0, za.X, za.Y, za.Z, 0, axis3.Location.X,
                                    axis3.Location.Y, axis3.Location.Z, 1);
            }
            else
                return new XbimMatrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, axis3.Location.X, axis3.Location.Y,
                                    axis3.Location.Z, 1);

        }


        public static void SetNewLocation(this IfcAxis2Placement3D axis3, double x, double y, double z)
        {
            IModel model = axis3.Model;
            IfcCartesianPoint location = model.Instances.New<IfcCartesianPoint>();
            location.X = x;
            location.Y = y;
            location.Z = z;
            axis3.Location = location;
        }


        /// <summary>
        ///   Sets new directions of the axes. Direction vectors are automaticaly normalized.
        /// </summary>
        /// <param name = "axis3"></param>
        /// <param name = "xAxisDirectionX"></param>
        /// <param name = "xAxisDirectionY"></param>
        /// <param name = "xAxisDirectionZ"></param>
        /// <param name = "zAxisDirectionX"></param>
        /// <param name = "zAxisDirectionY"></param>
        /// <param name = "zAxisDirectionZ"></param>
        public static void SetNewDirectionOf_XZ(this IfcAxis2Placement3D axis3, double xAxisDirectionX,
                                                double xAxisDirectionY, double xAxisDirectionZ, double zAxisDirectionX,
                                                double zAxisDirectionY, double zAxisDirectionZ)
        {
            IModel model = axis3.Model;
            IfcDirection zDirection = model.Instances.New<IfcDirection>();
            zDirection.DirectionRatios[0] = zAxisDirectionX;
            zDirection.DirectionRatios[1] = zAxisDirectionY;
            zDirection.DirectionRatios[2] = zAxisDirectionZ;
            zDirection.Normalise();
            axis3.Axis = zDirection;

            IfcDirection xDirection = model.Instances.New<IfcDirection>();
            xDirection.DirectionRatios[0] = xAxisDirectionX;
            xDirection.DirectionRatios[1] = xAxisDirectionY;
            xDirection.DirectionRatios[2] = xAxisDirectionZ;
            xDirection.Normalise();
            axis3.RefDirection = xDirection;
        }
    }
}
#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    Axis2Placement2DExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Concurrent;
using Xbim.Common.Geometry;
using Xbim.Ifc2x3.GeometryResource;


#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class Axis2Placement2DExtensions
    {

        static public IfcAxis2Placement2D Create(this IfcAxis2Placement2D ax, IfcCartesianPoint centre, IfcDirection xAxisDirection)
        {
            return ax.Model.Instances.New<IfcAxis2Placement2D>(a =>
            {
                a.RefDirection = xAxisDirection;
                a.Location = centre;
            });
        }

      

        public static XbimMatrix3D ToMatrix3D(this IfcAxis2Placement2D axis2, ConcurrentDictionary<int, Object> maps = null)
        {
            object transform;
            if (maps != null && maps.TryGetValue(axis2.EntityLabel, out transform)) //already converted it just return cached
                return (XbimMatrix3D)transform;
            if (axis2.RefDirection != null)
            {
                XbimVector3D v = axis2.RefDirection.XbimVector3D();
                v.Normalize();
                transform = new XbimMatrix3D(v.X, v.Y, 0, 0, v.Y, v.X, 0, 0, 0, 0, 1, 0, axis2.Location.X, axis2.Location.Y, 0, 1);
            }
            else
                transform = new XbimMatrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, axis2.Location.X, axis2.Location.Y,
                                    axis2.Location.Z, 1);
            if (maps != null) maps.TryAdd(axis2.EntityLabel, transform);
            return (XbimMatrix3D)transform;
        }
    }
}

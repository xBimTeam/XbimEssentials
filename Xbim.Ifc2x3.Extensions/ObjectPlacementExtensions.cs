#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    ObjectPlacementExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

using System;
using Xbim.Common.Geometry;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.GeometryResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class ObjectPlacementExtensions
    {
        public static XbimMatrix3D ToMatrix3D(this IfcObjectPlacement objPlace)
        {
            IfcLocalPlacement lp = objPlace as IfcLocalPlacement;
            if (lp != null)
            {
                XbimMatrix3D local = lp.RelativePlacement.ToMatrix3D();
                if (lp.PlacementRelTo != null)
                    return local * lp.PlacementRelTo.ToMatrix3D();
                else
                    return local;
            }
            else
                throw new NotImplementedException(String.Format("Placement of type {0} is not implemented",objPlace.GetType().Name));
        }
    }
}
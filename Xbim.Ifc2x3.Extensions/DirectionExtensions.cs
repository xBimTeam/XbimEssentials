#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    DirectionExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using Xbim.Ifc2x3.GeometryResource;


#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class DirectionExtensions
    {

        public static double DotProduct(this IfcDirection dir, IfcCartesianPoint pt)
        {
            return (dir.X*pt.X) + (dir.Y*pt.Y) + (dir.Z*pt.Z);
        }
    }
}
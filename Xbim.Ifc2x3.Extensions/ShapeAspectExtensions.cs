#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc.Extensions
// Filename:    ShapeAspectExtensions.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using Xbim.Ifc2x3.RepresentationResource;

#endregion

namespace Xbim.Ifc2x3.Extensions
{
    public static class ShapeAspectExtensions
    {
        public static IfcShapeModel GetShapeModel(this IfcShapeAspect aspect, string identifier)
        {
            return aspect.ShapeRepresentations.Lookup(identifier);
        }
    }
}
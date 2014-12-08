using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Common.Geometry;
using Xbim.Ifc2x3.GeometricConstraintResource;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using Xbim.Ifc2x3.Extensions;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcProductGeometryExtensions
    {
        /// <summary>
        /// Resolves the objects placement into a global wcs transformation.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static XbimMatrix3D Transform(this IfcProduct product)
        {
            if (product.ObjectPlacement != null)
                return product.ObjectPlacement.ToMatrix3D();
            else
                return XbimMatrix3D.Identity;
            
        }
    }
}

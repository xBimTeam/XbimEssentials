using System.Collections.Generic;
using Xbim.Common.Geometry;
using Xbim.Ifc2x3.Kernel;
using Xbim.IO.Esent;
using XbimGeometry.Interfaces;

namespace Xbim.Ifc2x3.IO
{
    public static class IfcProductExtensions
    {
        public static IEnumerable<XbimGeometryData> GeometryData(this  IfcProduct product, XbimGeometryType geomType)
        {
            var model = product.Model as EsentModel;
            if (model != null)
            {
                foreach (var item in model.GetGeometryData(product.EntityLabel, geomType))
                {
                    yield return item;
                }
            }
        }
    }
}

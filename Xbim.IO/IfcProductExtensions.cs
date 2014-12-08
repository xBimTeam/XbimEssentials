using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.XbimExtensions;
using XbimGeometry.Interfaces;

namespace Xbim.IO
{
    public static class IfcProductExtensions
    {
        public static IEnumerable<XbimGeometryData> GeometryData(this  IfcProduct product, XbimGeometryType geomType)
        {
            XbimModel model = product.ModelOf as XbimModel;
            if (model != null)
            {
                foreach (var item in model.GetGeometryData(product, geomType))
                {
                    yield return item;
                }
            }
        }
    }
}

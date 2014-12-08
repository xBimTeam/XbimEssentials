using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.IO;

namespace Xbim.IO.GroupingAndStyling
{
    public class EntityLabel : IGeomHandlesGrouping
    {
        public Dictionary<string, XbimGeometryHandleCollection> GroupLayers(XbimGeometryHandleCollection InputHandles)
        {
            Dictionary<string, XbimGeometryHandleCollection> result = new Dictionary<string, XbimGeometryHandleCollection>();

            var f = InputHandles.GroupBy(p => p.ProductLabel).Select(g => g.First().ProductLabel).ToList();

            foreach (var label in f)
            {
                XbimGeometryHandleCollection handles = new XbimGeometryHandleCollection(InputHandles.Where(g => g.ProductLabel == label));
                if (handles.Count > 0)
                    result.Add(label.ToString(), handles);
            }
            return result;
        }
    }
}

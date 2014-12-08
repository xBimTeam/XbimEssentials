using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3.Kernel;
using Xbim.IO;

namespace Xbim.IO.GroupingAndStyling
{
    /// <summary>
    /// Organises the inputhandles per type
    /// </summary>
    public class TypeAndStyle : IGeomHandlesGrouping
    {
        public Dictionary<string, XbimGeometryHandleCollection> GroupLayers(XbimGeometryHandleCollection InputHandles)
        {
            // creates a new dictionary and then fills it by type enumerating the known non-abstract subtypes of Product
            Dictionary<string, XbimGeometryHandleCollection> result = new Dictionary<string, XbimGeometryHandleCollection>();
            IfcType baseType = IfcMetaData.IfcType(typeof(IfcProduct));
            foreach (var subType in baseType.NonAbstractSubTypes)
            {
                short ifcTypeId = IfcMetaData.IfcTypeId(subType);
                XbimGeometryHandleCollection handles = new XbimGeometryHandleCollection(InputHandles.Where(g => g.IfcTypeId == ifcTypeId));
                
                // only add the item if there are handles in it
                if (handles.Count > 0) 
                    result.Add(subType.Name, handles);
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.ProductExtension;

namespace Xbim.IO
{
    /// <summary>
    /// An ordered Collection of geometry handles
    /// </summary>
    public class XbimGeometryHandleCollection : List<XbimGeometryHandle>
    {
        public XbimGeometryHandleCollection(IEnumerable<XbimGeometryHandle> enumerable)
            : base(enumerable)
        {
           
        }

        public XbimGeometryHandleCollection()
            : base()
        {
            // TODO: Complete member initialization
        }
        /// <summary>
        /// Returns a list of unique surface style for this collection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XbimSurfaceStyle> GetSurfaceStyles()
        {
            HashSet<XbimSurfaceStyle> uniqueStyles = new HashSet<XbimSurfaceStyle>();
            foreach (var item in this)
            {
                uniqueStyles.Add(item.SurfaceStyle);
            }
            return uniqueStyles;
        }

        [Obsolete("Function relocated to IGeomHandlesGrouping concrete classes for API grouping configuration.", false)]
        public Dictionary<string, XbimGeometryHandleCollection> GroupByBuildingElementTypes()
        {
            GroupingAndStyling.TypeAndStyle t = new GroupingAndStyling.TypeAndStyle();
            return t.GroupLayers(this);
        }

        /// <summary>
        /// Returns all handles that are not of type to exclude
        /// </summary>
        /// <param name="exclude"></param>
        /// <returns></returns>
        public IEnumerable<XbimGeometryHandle> Exclude(params IfcEntityNameEnum[] exclude)
        {
            HashSet<IfcEntityNameEnum> excludeSet = new HashSet<IfcEntityNameEnum>(exclude);
            foreach (var ex in exclude)
            {
                
                IfcType ifcType = IfcMetaData.IfcType((short)ex);
                // bugfix here: loop did not use to include all implementations, but only first level down.
                foreach (var sub in ifcType.NonAbstractSubTypes)
                {
                    var ifcSub = IfcMetaData.IfcType(sub);
                        excludeSet.Add(ifcSub.IfcTypeEnum);
                }
            }

            foreach (var h in this)
                if (!excludeSet.Contains((IfcEntityNameEnum)h.IfcTypeId)) 
                    yield return h;          
        }

        /// <summary>
        /// returns all handles that of of type to include
        /// </summary>
        /// <param name="include"></param>
        /// <returns></returns>
        public IEnumerable<XbimGeometryHandle> Include(params IfcEntityNameEnum[] include)
        {
            HashSet<IfcEntityNameEnum> includeSet = new HashSet<IfcEntityNameEnum>(include);
            foreach (var inc in include)
            {
                IfcType ifcType = IfcMetaData.IfcType((short)inc);
                foreach (var sub in ifcType.IfcSubTypes)
                    includeSet.Add(sub.IfcTypeEnum);
            }
            foreach (var h in this)
                if (includeSet.Contains((IfcEntityNameEnum)h.IfcTypeId)) yield return h;

        }

      
        /// <summary>
        /// Returns all the Geometry Handles for a specified SurfaceStyle
        /// Obtain the SurfaceStyle by calling the GetSurfaceStyles function
        /// </summary>
        /// <param name="forStyle"></param>
        public IEnumerable<XbimGeometryHandle> GetGeometryHandles(XbimSurfaceStyle forStyle)
        {
            foreach (var item in this.Where(gh => gh.SurfaceStyle.Equals(forStyle)))
                yield return item;
        }

        /// <summary>
        /// Returns a map of all the unique surface style and the geometry objects that the style renders
        /// </summary>
        /// <returns></returns>
        public XbimSurfaceStyleMap ToSurfaceStyleMap()
        {
            XbimSurfaceStyleMap result = new XbimSurfaceStyleMap();
            foreach (var style in this.GetSurfaceStyles())
            {
                result.Add(style, new XbimGeometryHandleCollection());
            }
            foreach (var item in this)
            {
                result[item.SurfaceStyle].Add(item);
            }
            return result;
        }
    }
}

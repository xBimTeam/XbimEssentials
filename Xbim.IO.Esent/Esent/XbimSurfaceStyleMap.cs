using System.Collections.Generic;
using System.Linq;

namespace Xbim.IO.Esent
{
    public class XbimSurfaceStyleMap : Dictionary<XbimSurfaceStyle, XbimGeometryHandleCollection>
    {
        /// <summary>
        /// Returns all the  unique style in the map
        /// </summary>
        public IEnumerable<XbimSurfaceStyle> Styles
        {
            get
            {
                return this.Keys;
            }
        }

        /// <summary>
        /// Returns an enumerable of all handles in the map
        /// </summary>
        public IEnumerable<XbimGeometryHandle> GeometryHandles
        {
            get
            {
                foreach (var kvPair in this)
                {
                    foreach (var item in kvPair.Value)
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Returns all the geometry handles for a specified style, use the Styles property for a valid style
        /// </summary>
        public IEnumerable<XbimGeometryHandle> GeometryHandlesForStyle(XbimSurfaceStyle style)
        {
            XbimGeometryHandleCollection coll;
            if(this.TryGetValue(style,out coll))
                return coll;
            else
                return Enumerable.Empty<XbimGeometryHandle>();
        }
    }
}

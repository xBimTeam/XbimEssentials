using System.Collections.Generic;

namespace Xbim.Common.Geometry
{
    /// <summary>
    /// This class is used to organise clusters of elements in case a scene needs to be split up.
    /// </summary>
    public class XbimBBoxClusterElement
    {
        public List<int> GeometryIds;
        public XbimRect3D Bound;
            
         
        public XbimBBoxClusterElement(int geomteryId, XbimRect3D bound)
        {
            GeometryIds = new List<int>(1);
            this.GeometryIds.Add(geomteryId);
            this.Bound = bound;
        }

        public void Add(XbimBBoxClusterElement otherElement)
        {
            GeometryIds = new List<int>(otherElement.GeometryIds.Count);
            GeometryIds.AddRange(otherElement.GeometryIds);
            Bound.Union(otherElement.Bound);
        }
    }
}

using Xbim.Ifc2x3.TopologyResource;

namespace Xbim.Ifc2x3.Extensions
{
    public static class IfcEdgeLoopGeometricExtensions
    {
        /// <summary>
        /// Calculates the maximum number of points in this object, does not remove geometric duplicates
        /// </summary>
        /// <param name="sbsm"></param>
        /// <returns></returns>
        public static int NumberOfPointsMax(this IfcEdgeLoop loop)
        {
            return loop.EdgeList.Count;
            
        }
    }
}

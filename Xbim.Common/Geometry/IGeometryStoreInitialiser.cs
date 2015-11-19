using System;

namespace Xbim.Common.Geometry
{
    public interface IGeometryStoreInitialiser : IDisposable
    {
        /// <summary>
        /// Adds a shape geometry to the store under transaction
        /// </summary>
        /// <param name="shapeGeometry"></param>
        /// <returns>Returns the ID of the shape geometry</returns>
        int AddShapeGeometry(XbimShapeGeometry shapeGeometry);

        /// <summary>
        /// Adds a shape instance to the store under transaction
        /// </summary>
        /// <param name="shapeInstance">The shape instance data</param>
        /// <param name="geometryId">the Id of the geometry shape, must be obtained from AddShapeGeometry</param>
        /// <returns>return the Id of the shape instance</returns>
        int AddShapeInstance(XbimShapeInstance shapeInstance, int geometryId);

        /// <summary>
        /// Adds a collection of geometric regions to the store, this groups geometries into major spatial collection areas 
        /// </summary>
        /// <param name="regions"></param>
        /// <returns>the unique Id of the region collection</returns>
        int AddRegions(XbimRegionCollection regions);
    }
}

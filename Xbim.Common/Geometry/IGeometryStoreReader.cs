using System;
using System.Collections.Generic;

namespace Xbim.Common.Geometry
{
    public interface IGeometryStoreReader : IDisposable
    {
        /// <summary>
        /// Returns an enumerable of all the shape instances in the store
        /// </summary>
        IEnumerable<XbimShapeInstance> ShapeInstances { get; }

        /// <summary>
        /// Returns an enumerable of all the shape instances in the store with the specified context Id
        /// </summary>
        IEnumerable<XbimShapeInstance> ShapeInstancesOfContext(int contextId);

        /// <summary>
        /// Returns an enumerable of all the shape geometries in the store
        /// </summary>
        IEnumerable<XbimShapeGeometry> ShapeGeometries { get; }

        /// <summary>
        /// Returns the shape geometry of the specifed geometry Id
        /// </summary>
        /// <param name="shapeGeometryLabel"></param>
        /// <returns></returns>
        XbimShapeGeometry ShapeGeometry(int shapeGeometryLabel);
        /// <summary>
        /// Returns the geometry of the specified instance
        /// </summary>
        /// <param name="shapeInstance"></param>
        /// <returns></returns>
        XbimShapeGeometry ShapeGeometryOfInstance(XbimShapeInstance shapeInstance);
        /// <summary>
        /// Returns an enumerable of all the shape instances in the store for the specified entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        IEnumerable<XbimShapeInstance> ShapeInstancesOfEntity(IPersistEntity entity);

        IEnumerable<XbimShapeInstance> ShapeInstancesOfEntityType(int entityTypeId);

        /// <summary>
        /// Returns an enumerable of all the shape instances in the store for the specified style
        /// </summary>
        /// <param name="styleLabel">The identifier of the required style</param>
        /// <returns></returns>
        IEnumerable<XbimShapeInstance> ShapeInstancesOfStyle(int styleLabel);
        /// <summary>
        /// Returns an enumerable of all the shape instances in the store with the specified geometry
        /// </summary>
        /// <param name="geometryLabel"></param>
        /// <returns></returns>
        IEnumerable<XbimShapeInstance> ShapeInstancesOfGeometry(int geometryLabel);
        
        /// <summary>
        /// Returns a unique set of all the style IDs of the all the shape instances in the store       
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>        
        ISet<int> StyleIds { get; }
        /// <summary>
        /// Returns the region collection for the store
        /// </summary>
        IEnumerable<XbimRegionCollection> Regions { get; }
        /// <summary>
        /// Returns an eumerable of all the unique context ids in the store
        /// </summary>
        IEnumerable<int> ContextIds { get; } 
    }
}

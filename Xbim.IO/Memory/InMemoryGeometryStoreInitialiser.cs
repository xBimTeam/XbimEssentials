using Xbim.Common.Geometry;

namespace Xbim.IO.Memory
{
    internal class InMemoryGeometryStoreInitialiser : IGeometryStoreInitialiser
    {
        private readonly InMemoryGeometryStore _inMemoryGeometryStore;

        public InMemoryGeometryStoreInitialiser(InMemoryGeometryStore inMemoryGeometryStore)
        {          
            _inMemoryGeometryStore = inMemoryGeometryStore;
        }
        public int AddShapeGeometry(XbimShapeGeometry shapeGeometry)
        {
            int id = _inMemoryGeometryStore.ShapeGeometries.Count + 1;//need 1 based to match database
            _inMemoryGeometryStore.ShapeGeometries.Add(id, shapeGeometry);
            return id;
        }

        public int AddShapeInstance(XbimShapeInstance shapeInstance, int geometryId)
        {
            int id = _inMemoryGeometryStore.ShapeInstances.Count+1; //need 1 based to match database
            shapeInstance.ShapeGeometryLabel = geometryId;
            _inMemoryGeometryStore.ShapeInstances.Add(id, shapeInstance);
            return id;
        }

        public int AddRegions(XbimRegionCollection regions)
        {
            return _inMemoryGeometryStore.AddRegions(regions);        
        }

        public void Dispose()
        {
            
        }
    }
}

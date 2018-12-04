using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Geometry;

namespace Xbim.Common.Model
{
    internal class InMemoryGeometryStoreReader : IGeometryStoreReader
    {
        private readonly InMemoryGeometryStore _inMemoryGeometryStore;

        public InMemoryGeometryStoreReader(InMemoryGeometryStore inMemoryGeometryStore)
        {          
            _inMemoryGeometryStore = inMemoryGeometryStore;
        }
        public IEnumerable<XbimShapeInstance> ShapeInstances
        {
            get 
            {
                return _inMemoryGeometryStore.ShapeInstances.Values;
            }
        }

        public IEnumerable<XbimShapeInstance> ShapeInstancesOfContext(int contextId)
        {
            return _inMemoryGeometryStore.ShapeInstances.Values.Where(s=>s.RepresentationContext==contextId);
        }

        public IEnumerable<XbimShapeGeometry> ShapeGeometries
        {
            get { return _inMemoryGeometryStore.ShapeGeometries.Values.Where(g=>g.Format!=XbimGeometryType.Region); }
        }

        public XbimShapeGeometry ShapeGeometry(int shapeGeometryLabel)
        {
            XbimShapeGeometry shape;
            if (_inMemoryGeometryStore.ShapeGeometries.TryGetValue(shapeGeometryLabel, out shape))
                return shape;
            return null;
        }

        public XbimShapeGeometry ShapeGeometryOfInstance(XbimShapeInstance shapeInstance)
        {
            return ShapeGeometry(shapeInstance.ShapeGeometryLabel);
        }

        public IEnumerable<XbimShapeInstance> ShapeInstancesOfEntity(IPersistEntity entity)
        {
            List<XbimShapeInstance> shapes;
            if (_inMemoryGeometryStore.EntityInstanceLookup.TryGetValue(entity.EntityLabel, out shapes))
                return shapes;
            return Enumerable.Empty<XbimShapeInstance>();
        }
        public IEnumerable<XbimShapeInstance> ShapeInstancesOfEntity(int entityLabel)
        {

            List<XbimShapeInstance> shapes;
            if (_inMemoryGeometryStore.EntityInstanceLookup.TryGetValue(entityLabel, out shapes))
                return shapes;
            return Enumerable.Empty<XbimShapeInstance>();
        }

        public IEnumerable<XbimShapeInstance> ShapeInstancesOfEntityType(int entityTypeId)
        {
            List<XbimShapeInstance> shapes;
            if (_inMemoryGeometryStore.EntityTypeLookup.TryGetValue(entityTypeId, out shapes))
                return shapes;
            return Enumerable.Empty<XbimShapeInstance>();
        }

        public IEnumerable<XbimShapeInstance> ShapeInstancesOfStyle(int styleLabel)
        {
            List<XbimShapeInstance> shapes;
            if (_inMemoryGeometryStore.EntityStyleLookup.TryGetValue(styleLabel, out shapes))
                return shapes;
            return Enumerable.Empty<XbimShapeInstance>();
        }

        public IEnumerable<XbimShapeInstance> ShapeInstancesOfGeometry(int geometryLabel)
        {
            List<XbimShapeInstance> shapes;
            if (_inMemoryGeometryStore.GeometryShapeLookup.TryGetValue(geometryLabel, out shapes))
                return shapes;
            return Enumerable.Empty<XbimShapeInstance>();
        }

        public ISet<int> StyleIds
        {
            get { return _inMemoryGeometryStore.Styles; }
        }
        public XbimContextRegionCollection ContextRegions
        {
            get { return _inMemoryGeometryStore.ContextRegions; }
        }

        public IEnumerable<int> ContextIds
        {
            get { return _inMemoryGeometryStore.ContextIds; }
        }

        public void Dispose()
        {
            
        }
        public XbimRect3D BoundingBox(int entityLabel)
        {
            var bBox = XbimRect3D.Empty;
            foreach (var shape in ShapeInstancesOfEntity(entityLabel))
            {
                bBox.Union(shape.BoundingBox);
            }
            return bBox;
        }
    }
}

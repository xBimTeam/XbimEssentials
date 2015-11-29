using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xbim.Common.Geometry;

namespace Xbim.IO.Memory
{
    internal class InMemoryGeometryStore : IGeometryStore
    {
        private ConcurrentDictionary<int, XbimShapeGeometry> _shapeGeometries;
        private ConcurrentDictionary<int, XbimShapeInstance> _shapeInstances;
        private Dictionary<int, List<XbimShapeInstance>> _entityInstanceLookup;
        private Dictionary<int, List<XbimShapeInstance>> _entityTypeLookup;
        private Dictionary<int, List<XbimShapeInstance>> _entityStyleLookup;
        private Dictionary<int, List<XbimShapeInstance>> _geometryShapeLookup;
        private HashSet<int> _styles;
        private List<XbimRegionCollection> _regions;
        private IEnumerable<int> _contextIds;
        private int _geometryCount;
        private int _instanceCount;
        public IDictionary<int, XbimShapeGeometry> ShapeGeometries
        {
            get { return _shapeGeometries; }
        }

        internal int AddShapeGeometry(XbimShapeGeometry shapeGeometry)
        {
            int id = Interlocked.Increment(ref _geometryCount);
            shapeGeometry.ShapeLabel = id;
            _shapeGeometries.TryAdd(id, shapeGeometry);
            return id;
        }

        public IDictionary<int, XbimShapeInstance> ShapeInstances
        {
            get 
            { 
                return _shapeInstances; 
            }
        }

        internal int AddShapeInstance(XbimShapeInstance shapeInstance, int geometryId)
        {
            int id = Interlocked.Increment(ref _instanceCount);
            shapeInstance.ShapeGeometryLabel = geometryId;
            _shapeInstances.TryAdd(id, shapeInstance);
            return id;
        }

        public IDictionary<int, List<XbimShapeInstance>> EntityInstanceLookup
        {
            get { return _entityInstanceLookup; }
        }

        public IDictionary<int, List<XbimShapeInstance>> EntityTypeLookup
        {
            get { return _entityTypeLookup; }
        }

        public IDictionary<int, List<XbimShapeInstance>> EntityStyleLookup
        {
            get { return _entityStyleLookup; }
        }

        public IDictionary<int, List<XbimShapeInstance>> GeometryShapeLookup
        {
            get { return _geometryShapeLookup; }
        }

        public ISet<int> Styles
        {
            get { return _styles; }
        }

        public List<XbimRegionCollection> Regions
        {
            get { return _regions; }
        }

        public IEnumerable<int> ContextIds
        {
            get { return _contextIds; }
        }

        public IGeometryStoreInitialiser BeginInit()
        {
            _shapeGeometries = new ConcurrentDictionary<int, XbimShapeGeometry>();
            _shapeInstances = new ConcurrentDictionary<int, XbimShapeInstance>();
            _geometryCount = 0;
            _instanceCount = 0;
            return new InMemoryGeometryStoreInitialiser(this);
        }

        internal void EndInit(IGeometryStoreInitialiser transaction)
        {
            _entityInstanceLookup = ShapeInstances.GroupBy(s => s.Value.IfcProductLabel).ToDictionary(s=>s.Key,v=>
            {
                return v.Select(instance => instance.Value).ToList();
            });            

            _entityTypeLookup = ShapeInstances.GroupBy(s => (int)s.Value.IfcTypeId).ToDictionary(s => s.Key, v =>
            {
                return v.Select(instance => instance.Value).ToList();
            });

            _entityStyleLookup = ShapeInstances.GroupBy(s => s.Value.StyleLabel).ToDictionary(s => s.Key, v =>
            {
                return v.Select(instance => instance.Value).ToList();
            });
            _geometryShapeLookup = ShapeInstances.GroupBy(s => s.Value.ShapeGeometryLabel).ToDictionary(s => s.Key, v =>
            {
                return v.Select(instance => instance.Value).ToList();
            });
            _styles = new HashSet<int>(EntityStyleLookup.Select(s => s.Key).Where(k => k > 0));
            _contextIds = new HashSet<int>(ShapeInstances.Select(s => s.Value.RepresentationContext)).Distinct();
        }

        public IGeometryStoreReader BeginRead()
        {
            return new InMemoryGeometryStoreReader(this);
        }

        internal int AddRegions(XbimRegionCollection regions)
        {
            if(_regions==null) _regions=new List<XbimRegionCollection>();
            _regions.Add(regions);
            return _regions.Count - 1;
        }
    }
}

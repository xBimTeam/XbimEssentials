using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Geometry;

namespace Xbim.IO.Esent
{
    internal class EsentGeometryStoreReader : IGeometryStoreReader
    {
        private readonly EsentModel _esentModel;
        private readonly EsentShapeGeometryCursor _shapeGeometryCursor;
        private readonly EsentShapeInstanceCursor _shapeInstanceCursor;
        private EsentReadOnlyTransaction _shapeGeometryTransaction;
        private EsentReadOnlyTransaction _shapeInstanceTransaction;
        private readonly XbimContextRegionCollection _regionsList;
        private readonly HashSet<int> _contextIds;

        public EsentGeometryStoreReader(EsentModel esentModel)
        {
            _esentModel = esentModel;
            _shapeGeometryCursor = _esentModel.GetShapeGeometryTable();
            _shapeInstanceCursor = _esentModel.GetShapeInstanceTable();
            _shapeGeometryTransaction = _shapeGeometryCursor.BeginReadOnlyTransaction();
            _shapeInstanceTransaction = _shapeInstanceCursor.BeginReadOnlyTransaction();       
            _regionsList = new XbimContextRegionCollection();
            IXbimShapeGeometryData regions = new XbimRegionCollection();
            if (_shapeGeometryCursor.TryMoveFirstRegion(ref regions))
            {
                do
                {
                    _regionsList.Add((XbimRegionCollection)regions);
                    regions = new XbimRegionCollection();
                } while (_shapeGeometryCursor.TryMoveNextRegion(ref regions));
            }
            if (!_regionsList.Any()) //we might have an old xbim database regions were stored in the geometry table                
            {
                var legacyCursor = _esentModel.GetGeometryTable();
                using (var txn = legacyCursor.BeginReadOnlyTransaction())
                {
                    foreach (var regionData in legacyCursor.GetGeometryData(Xbim.Common.Geometry.XbimGeometryType.Region))
                    {
                        _regionsList.Add(XbimRegionCollection.FromArray(regionData.ShapeData));
                    }

                }
                _esentModel.FreeTable(legacyCursor);
            }
            _contextIds = new HashSet<int>(ContextIds);
        }
        /// <summary>
        /// Retrieves all shape instances for the given context
        /// </summary>
        /// <param name="contextId"></param>
        /// <returns></returns>
        public IEnumerable<XbimShapeInstance> ShapeInstancesOfContext(int contextId)
        {
            IXbimShapeInstanceData shapeInstance = new XbimShapeInstance();
            if (_shapeInstanceCursor.TrySeekShapeInstance(contextId, ref shapeInstance))
            {
                do
                {
                    yield return (XbimShapeInstance)shapeInstance;
                    shapeInstance = new XbimShapeInstance();
                } while (_shapeInstanceCursor.TryMoveNextShapeInstance(ref shapeInstance));
            }
        }

        /// <summary>
        /// Retrieves all shape instances
        /// </summary>
        /// <returns></returns>
        public IEnumerable<XbimShapeInstance> ShapeInstances
        {
            get
            {
                IXbimShapeInstanceData shapeInstance = new XbimShapeInstance();
                if (_shapeInstanceCursor.TrySeekShapeInstance(ref shapeInstance))
                {
                    do
                    {
                        yield return (XbimShapeInstance) shapeInstance;
                        shapeInstance = new XbimShapeInstance();
                    } while (_shapeInstanceCursor.TryMoveNextShapeInstance(ref shapeInstance));
                }
            }
        }

        public IEnumerable<XbimShapeGeometry> ShapeGeometries
        {
            get
            {
                IXbimShapeGeometryData shapeGeometry = new XbimShapeGeometry();
                if (_shapeGeometryCursor.TryMoveFirstShapeGeometry(ref shapeGeometry))
                {
                    do
                    {
                        if (shapeGeometry.Format != (byte) XbimGeometryType.Region)
                            //ignore regions they are dealt with specially
                            yield return (XbimShapeGeometry) shapeGeometry;
                        shapeGeometry = new XbimShapeGeometry();
                    } while (_shapeGeometryCursor.TryMoveNextShapeGeometry(ref shapeGeometry));
                }
            }
        }

        public XbimShapeGeometry ShapeGeometry(int shapeGeometryLabel)
        {
            IXbimShapeGeometryData shapeGeometry = new XbimShapeGeometry();
            _shapeGeometryCursor.TryGetShapeGeometry(shapeGeometryLabel, ref shapeGeometry);
            return (XbimShapeGeometry)shapeGeometry;
        }

        public XbimShapeGeometry ShapeGeometryOfInstance(XbimShapeInstance shapeInstance)
        {
            return ShapeGeometry(shapeInstance.ShapeGeometryLabel);
        }

        public IEnumerable<XbimShapeInstance> ShapeInstancesOfEntity(IPersistEntity entity)
        {           
           return ShapeInstancesOfEntity(entity.EntityLabel);
        }


        public IEnumerable<XbimShapeInstance> ShapeInstancesOfEntity(int entityLabel)
        {

            IXbimShapeInstanceData shapeInstance = new XbimShapeInstance();
            if (_shapeInstanceCursor.TrySeekShapeInstanceOfProduct(entityLabel, ref shapeInstance))
            {
                do
                {
                    if (_contextIds.Contains(shapeInstance.RepresentationContext))
                    {
                        yield return (XbimShapeInstance)shapeInstance;
                        shapeInstance = new XbimShapeInstance();
                    }

                } while (_shapeInstanceCursor.TryMoveNextShapeInstance(ref shapeInstance));
            }
        }

        public IEnumerable<XbimShapeInstance> ShapeInstancesOfStyle(int styleLabel)
        {
            IXbimShapeInstanceData shapeInstance = new XbimShapeInstance();

            foreach (var context in _contextIds)
            {
                if (_shapeInstanceCursor.TrySeekSurfaceStyle(context, styleLabel, ref shapeInstance))
                {
                    do
                    {
                        yield return (XbimShapeInstance) shapeInstance;
                        shapeInstance = new XbimShapeInstance();
                    } while (_shapeInstanceCursor.TryMoveNextShapeInstance(ref shapeInstance) &&
                             shapeInstance.StyleLabel == styleLabel);
                }
            }
        }

        public IEnumerable<XbimShapeInstance> ShapeInstancesOfGeometry(int geometryLabel)
        {
            IXbimShapeInstanceData shapeInstance = new XbimShapeInstance();
            if (_shapeInstanceCursor.TrySeekShapeInstanceOfGeometry(geometryLabel, ref shapeInstance))
            {
                do
                {
                    if (!_contextIds.Contains(shapeInstance.RepresentationContext))
                        continue;
                    yield return (XbimShapeInstance)shapeInstance;
                    shapeInstance = new XbimShapeInstance();
                } while (_shapeInstanceCursor.TryMoveNextShapeInstance(ref shapeInstance));
            }
        }

        public bool EntityHasShapeInstances(IPersistEntity entity)
        {
            return _shapeInstanceCursor.TrySeekShapeInstanceOfProduct(entity.EntityLabel);
        }


        public ISet<int> StyleIds
        {
            get
            {
                HashSet<int> styleIds = new HashSet<int>();
                foreach (var context in _contextIds)
                {
                    int surfaceStyle;
                    short productType;
                    if (_shapeInstanceCursor.TryMoveFirstSurfaceStyle(context, out surfaceStyle,
                        out productType))
                    {
                        do
                        {
                            if (surfaceStyle > 0) //we have a surface style
                            {
                                styleIds.Add(surfaceStyle);                              
                            }
                            surfaceStyle = _shapeInstanceCursor.SkipSurfaceStyes(surfaceStyle);
                        } while (surfaceStyle != -1);
                        //now get all the undefined styles and use their product type to create the texture
                    }
                }

                return styleIds;
            }
        }
        public XbimContextRegionCollection ContextRegions
        {
            get
            {
                return _regionsList;
            }
        }

        public IEnumerable<int> ContextIds
        {
            get
            {
                foreach (var region in ContextRegions)
                {
                    yield return region.ContextLabel;
                }
            }
        }

        public void Dispose()
        {
            _shapeInstanceTransaction.Dispose();
            _shapeGeometryTransaction.Dispose();
            if (_shapeGeometryCursor != null) _esentModel.FreeTable(_shapeGeometryCursor);
            if (_shapeInstanceCursor != null) _esentModel.FreeTable(_shapeInstanceCursor);
        }


        public IEnumerable<XbimShapeInstance> ShapeInstancesOfEntityType(int entityTypeId)
        {
            IXbimShapeInstanceData shapeInstance = new XbimShapeInstance();
            if (_shapeInstanceCursor.TrySeekProductType((short)entityTypeId, ref shapeInstance))
            {
                do
                {
                    if (_contextIds.Contains(shapeInstance.RepresentationContext))
                    {
                        yield return (XbimShapeInstance)shapeInstance;
                        shapeInstance = new XbimShapeInstance();
                    }
                } while (_shapeInstanceCursor.TryMoveNextShapeInstance(ref shapeInstance) &&
                         shapeInstance.IfcTypeId == entityTypeId);
            }
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

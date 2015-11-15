using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private List<XbimRegionCollection> _regionsList;

        public EsentGeometryStoreReader(EsentModel esentModel)
        {
            _esentModel = esentModel;
            _shapeGeometryCursor = _esentModel.GetShapeGeometryTable();
            _shapeInstanceCursor = _esentModel.GetShapeInstanceTable();
            _shapeGeometryTransaction = _shapeGeometryCursor.BeginReadOnlyTransaction();
            _shapeInstanceTransaction = _shapeInstanceCursor.BeginReadOnlyTransaction();
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

        public IEnumerable<XbimShapeInstance> ShapeInstancesOfEntity(Common.IPersistEntity entity, int contextId)
        {
            IXbimShapeInstanceData shapeInstance = new XbimShapeInstance();
            if (_shapeInstanceCursor.TrySeekShapeInstanceOfProduct(entity.EntityLabel, ref shapeInstance))
            {
                do
                {
                    if (contextId == shapeInstance.RepresentationContext)
                    {
                        yield return (XbimShapeInstance)shapeInstance;
                        shapeInstance = new XbimShapeInstance();
                    }

                } while (_shapeInstanceCursor.TryMoveNextShapeInstance(ref shapeInstance));
            }
        }

        public IEnumerable<XbimShapeInstance> ShapeInstancesOfStyle(int styleLabel, int contextId)
        {
            IXbimShapeInstanceData shapeInstance = new XbimShapeInstance();


            if (_shapeInstanceCursor.TrySeekSurfaceStyle(contextId, styleLabel, ref shapeInstance))
            {
                do
                {
                    yield return (XbimShapeInstance) shapeInstance;
                    shapeInstance = new XbimShapeInstance();
                } while (_shapeInstanceCursor.TryMoveNextShapeInstance(ref shapeInstance) &&
                         shapeInstance.StyleLabel == styleLabel);
            }
        }

        public IEnumerable<XbimShapeInstance> ShapeInstancesOfGeometry(int geometryLabel, int context)
        {
            IXbimShapeInstanceData shapeInstance = new XbimShapeInstance();
            if (_shapeInstanceCursor.TrySeekShapeInstanceOfGeometry(geometryLabel, ref shapeInstance))
            {
                do
                {
                    if (context != shapeInstance.RepresentationContext)
                        continue;
                    yield return (XbimShapeInstance)shapeInstance;
                    shapeInstance = new XbimShapeInstance();
                } while (_shapeInstanceCursor.TryMoveNextShapeInstance(ref shapeInstance));
            }
        }

        public bool EntityHasShapeInstances(Common.IPersistEntity entity)
        {
            return _shapeInstanceCursor.TrySeekShapeInstanceOfProduct(entity.EntityLabel);
        }

        public ISet<int> StyleIds(int context)
        {
            int surfaceStyle;
            short productType;
            HashSet<int> styleIds = new HashSet<int>();
            if (_shapeInstanceCursor.TryMoveFirstSurfaceStyle(context, out surfaceStyle,
                out productType))
            {
                do
                {
                    if (surfaceStyle > 0) //we have a surface style
                    {
                        styleIds.Add(surfaceStyle);
                        surfaceStyle = _shapeInstanceCursor.SkipSurfaceStyes(surfaceStyle);
                    }
                    else //then we use the product type for the surface style
                    {
                        //read all shape instance of style 0 and get their product texture
                        do
                        {
                            styleIds.Add(productType);
                        } 
                        while (
                            _shapeInstanceCursor.TryMoveNextSurfaceStyle(out surfaceStyle, out productType) &&
                            surfaceStyle == 0); //skip over all the zero entries and get their style
                    }
                } while (surfaceStyle != -1);
                //now get all the undefined styles and use their product type to create the texture
            }
            return styleIds;
        }

        public IEnumerable<XbimRegionCollection> Regions
        {
            get
            {
                if (_regionsList == null)
                {
                    _regionsList = new List<XbimRegionCollection>();
                    IXbimShapeGeometryData regions = new XbimRegionCollection();
                    if (_shapeGeometryCursor.TryMoveFirstRegion(ref regions))
                    {
                        do
                        {
                            _regionsList.Add((XbimRegionCollection) regions);
                            regions = new XbimRegionCollection();
                        } while (_shapeGeometryCursor.TryMoveNextRegion(ref regions));
                    }
                }
                return _regionsList;
            }
        }

        public IEnumerable<int> ContextIds
        {
            get
            {
                foreach (var region in Regions)
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
    }
}

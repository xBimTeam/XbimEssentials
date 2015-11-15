using System;
using System.Linq;
using Xbim.Common.Geometry;

namespace Xbim.IO.Esent
{
    internal class EsentGeometryInitialiser : IGeometryStoreInitialiser
    {
        const int TransactionBatchSize = 100;
        private EsentGeometryStore _esentGeometryStore;
        private readonly EsentShapeGeometryCursor _shapeGeometryCursor;
        private readonly EsentShapeInstanceCursor _shapeInstanceCursor;
        private EsentLazyDBTransaction _shapeGeometryTransaction;
        private EsentLazyDBTransaction _shapeInstanceTransaction;
        private int _geometryCount;
        private int _instanceCount;
        
        public EsentGeometryInitialiser(EsentGeometryStore esentGeometryStore, EsentShapeGeometryCursor shapeGeometryCursor, EsentShapeInstanceCursor shapeInstanceCursor)
        {
            try
            {
                _esentGeometryStore = esentGeometryStore;
                _shapeGeometryCursor = shapeGeometryCursor;
                _shapeInstanceCursor = shapeInstanceCursor;
                _shapeGeometryTransaction = _shapeGeometryCursor.BeginLazyTransaction();
                _shapeInstanceTransaction = _shapeInstanceCursor.BeginLazyTransaction();
                _geometryCount = 0;
                _instanceCount = 0;
            }
            catch (Exception e)
            {             
                throw new Exception("Error creating transactions", e);
            }
        }


        public int AddShapeGeometry(IXbimShapeGeometryData shapeGeometry)
        {
            long remainder = _geometryCount % TransactionBatchSize; //pulse transactions
            if (remainder == TransactionBatchSize - 1)
            {
                _shapeGeometryTransaction.Commit();
                _shapeGeometryTransaction.Begin();
            }
            _geometryCount++;
            return _shapeGeometryCursor.AddGeometry(shapeGeometry);
        }

        public int AddShapeInstance(IXbimShapeInstanceData shapeInstance, int geometryId)
        {
            long remainder = _instanceCount % TransactionBatchSize; //pulse transactions
            if (remainder == TransactionBatchSize - 1)
            {
                _shapeInstanceTransaction.Commit();
                _shapeInstanceTransaction.Begin();
            }
            _instanceCount++;
            shapeInstance.ShapeGeometryLabel = geometryId;
            return _shapeInstanceCursor.AddInstance(shapeInstance);
        }

        public int AddRegions(XbimRegionCollection regions)
        {
            _geometryCount++;
            return _shapeGeometryCursor.AddGeometry(regions);
        }

        
        public void Dispose()
        {
            _shapeInstanceTransaction.Dispose();
            _shapeGeometryTransaction.Dispose();

        }

        internal void Commit()
        { 
            _shapeGeometryTransaction.Commit();
            _shapeInstanceTransaction.Commit();            
        }

        /// <summary>
        /// Updates the reference counts for each grometry on completion
        /// </summary>
        internal void UpdateReferenceCounts()
        {
            _shapeGeometryTransaction.Commit();
            _shapeGeometryTransaction.Begin();
            _shapeInstanceTransaction.Commit();
            _shapeInstanceTransaction.Begin();
            using (var reader = _esentGeometryStore.BeginRead())
            {
                //Get the meta data about the instances
                var counts = reader.ShapeInstances.GroupBy(
                    i => i.ShapeGeometryLabel,
                    (label, instances) => new
                    {
                        Label = label,
                        Count = instances.Count()
                    });
                foreach (var item in counts)
                {
                    long remainder = _geometryCount % TransactionBatchSize; //pulse transactions
                    if (remainder == TransactionBatchSize - 1)
                    {
                        _shapeGeometryTransaction.Commit();
                        _shapeGeometryTransaction.Begin();
                    }
                    _geometryCount++;
                    _shapeGeometryCursor.UpdateReferenceCount(item.Label, item.Count);
                }
            }
        }
    }
}

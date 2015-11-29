using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Isam.Esent.Interop;
using Xbim.Common.Geometry;

namespace Xbim.IO.Esent
{
    internal class EsentGeometryInitialiser : IGeometryStoreInitialiser
    {
        const int TransactionBatchSize = 100;
        private readonly EsentGeometryStore _esentGeometryStore;
        private readonly EsentShapeGeometryCursor _shapeGeometryCursor;
        private readonly EsentShapeInstanceCursor _shapeInstanceCursor;
        private EsentLazyDBTransaction _shapeGeometryTransaction;
        private EsentLazyDBTransaction _shapeInstanceTransaction;
        private int _geometryCount;
        private int _instanceCount;
        private static IntPtr _contextId;
        
        public EsentGeometryInitialiser(EsentGeometryStore esentGeometryStore, EsentShapeGeometryCursor shapeGeometryCursor, EsentShapeInstanceCursor shapeInstanceCursor)
        {
            try
            {
                _esentGeometryStore = esentGeometryStore;
                _shapeGeometryCursor = shapeGeometryCursor;
                _shapeInstanceCursor = shapeInstanceCursor;
                _contextId = new IntPtr(_shapeGeometryCursor.GetHashCode());                                  
                SetContext();
                _shapeGeometryTransaction = _shapeGeometryCursor.BeginLazyTransaction();
                _shapeInstanceTransaction = _shapeInstanceCursor.BeginLazyTransaction();
                ResetContext();
               // ResetContext();
                _geometryCount = 0;
                _instanceCount = 0;
            }
            catch (Exception e)
            {             
                throw new Exception("Error creating transactions", e);
            }
        }


        public int AddShapeGeometry(XbimShapeGeometry shapeGeometry)
        {
           
            lock (_shapeGeometryCursor)
            {
                SetContext();
                long remainder = _geometryCount%TransactionBatchSize; //pulse transactions
                if (remainder == TransactionBatchSize - 1)
                {
                    _shapeGeometryTransaction.Commit();
                    _shapeGeometryTransaction.Begin();
                }
                _geometryCount++;
                var ret = _shapeGeometryCursor.AddGeometry(shapeGeometry);
                ResetContext();
                return ret;
            }
          
        }

        public int AddShapeInstance(XbimShapeInstance shapeInstance, int geometryId)
        {
            lock (_shapeInstanceCursor)
            {
                SetContext();
                long remainder = _instanceCount%TransactionBatchSize; //pulse transactions
                if (remainder == TransactionBatchSize - 1)
                {
                    _shapeInstanceTransaction.Commit();
                    _shapeInstanceTransaction.Begin();
                }
                _instanceCount++;
                shapeInstance.ShapeGeometryLabel = geometryId;
                var ret = _shapeInstanceCursor.AddInstance(shapeInstance);
                ResetContext();
                return ret;
            }
        }

        public int AddRegions(XbimRegionCollection regions)
        {
            lock (_shapeGeometryCursor)
            {
                SetContext();
                _geometryCount++;
                var ret = _shapeGeometryCursor.AddGeometry(regions);
                ResetContext();
                return ret;
            }
        }

        
        public void Dispose()
        {
            SetContext();
            _shapeInstanceTransaction.Dispose();
            _shapeGeometryTransaction.Dispose();
            ResetContext();

        }

        internal void Commit()
        { 
            SetContext();
            _shapeGeometryTransaction.Commit();
            _shapeInstanceTransaction.Commit();
            ResetContext();
        }

        /// <summary>
        /// Updates the reference counts for each grometry on completion
        /// </summary>
        internal void UpdateReferenceCounts()
        {
            lock (_shapeGeometryCursor)
            {
                SetContext();
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
                        long remainder = _geometryCount%TransactionBatchSize; //pulse transactions
                        if (remainder == TransactionBatchSize - 1)
                        {
                            _shapeGeometryTransaction.Commit();
                            _shapeGeometryTransaction.Begin();
                        }
                        _geometryCount++;
                        _shapeGeometryCursor.UpdateReferenceCount(item.Label, item.Count);
                    }
                }
                ResetContext();
            }
        }

       
        void IGeometryStoreInitialiser.Commit()
        {       
            _esentGeometryStore.EndInit(this);          
        }

        private void SetContext()
        {
            Api.JetSetSessionContext(_shapeGeometryCursor.Session, _contextId);
            Api.JetSetSessionContext(_shapeInstanceCursor.Session, _contextId);              
        }

        private void ResetContext()
        {
            Api.JetResetSessionContext(_shapeGeometryCursor.Session);
            Api.JetResetSessionContext(_shapeInstanceCursor.Session);
        }
    }
}

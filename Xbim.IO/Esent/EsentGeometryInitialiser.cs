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
        private static IntPtr _geometryContextId;
        private static IntPtr _instanceContextId;
        public EsentGeometryInitialiser(EsentGeometryStore esentGeometryStore, EsentShapeGeometryCursor shapeGeometryCursor, EsentShapeInstanceCursor shapeInstanceCursor)
        {
            try
            {
                _esentGeometryStore = esentGeometryStore;
                _shapeGeometryCursor = shapeGeometryCursor;
                _shapeInstanceCursor = shapeInstanceCursor;
                _geometryContextId = new IntPtr(_shapeGeometryCursor.GetHashCode()); 
                _instanceContextId = new IntPtr(_shapeInstanceCursor.GetHashCode());          
                SetGeometryContext();
                _shapeGeometryTransaction = _shapeGeometryCursor.BeginLazyTransaction();
                ResetGeometryContext();

                SetInstanceContext();
                _shapeInstanceTransaction = _shapeInstanceCursor.BeginLazyTransaction();
                ResetInstanceContext();
              
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
                SetGeometryContext();
                long remainder = _geometryCount%TransactionBatchSize; //pulse transactions
                if (remainder == TransactionBatchSize - 1)
                {
                    _shapeGeometryTransaction.Commit();
                    _shapeGeometryTransaction.Begin();
                }
                _geometryCount++;
                var ret = _shapeGeometryCursor.AddGeometry(shapeGeometry);
                ResetGeometryContext();
                return ret;
            }
          
        }

        public int AddShapeInstance(XbimShapeInstance shapeInstance, int geometryId)
        {
            lock (_shapeInstanceCursor)
            {
                SetInstanceContext();
                long remainder = _instanceCount%TransactionBatchSize; //pulse transactions
                if (remainder == TransactionBatchSize - 1)
                {
                    _shapeInstanceTransaction.Commit();
                    _shapeInstanceTransaction.Begin();
                }
                _instanceCount++;
                shapeInstance.ShapeGeometryLabel = geometryId;
                var ret = _shapeInstanceCursor.AddInstance(shapeInstance);
                ResetInstanceContext();
                return ret;
            }
        }

        public int AddRegions(XbimRegionCollection regions)
        {
            lock (_shapeGeometryCursor)
            {
                SetGeometryContext();
                _geometryCount++;
                var ret = _shapeGeometryCursor.AddGeometry(regions);
                ResetGeometryContext();
                return ret;
            }
        }

        
        public void Dispose()
        {      
            _shapeInstanceTransaction.Dispose();
            _shapeGeometryTransaction.Dispose();

        }

        internal void Commit()
        { 
            SetGeometryContext();
            _shapeGeometryTransaction.Commit();
            ResetGeometryContext();
            SetInstanceContext();
            _shapeInstanceTransaction.Commit();
            ResetInstanceContext();
        }

        /// <summary>
        /// Updates the reference counts for each grometry on completion
        /// </summary>
        internal void UpdateReferenceCounts()
        {
            lock (_shapeGeometryCursor)
            {
                SetGeometryContext();
                SetInstanceContext();
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
                ResetGeometryContext();
                ResetInstanceContext();
            }
        }

       
        void IGeometryStoreInitialiser.Commit()
        {       
            _esentGeometryStore.EndInit(this);          
        }

        private void SetGeometryContext()
        {
            Api.JetSetSessionContext(_shapeGeometryCursor.Session, _geometryContextId);
        }
        private void SetInstanceContext()
        {            
            Api.JetSetSessionContext(_shapeInstanceCursor.Session, _instanceContextId);
        }

        private void ResetGeometryContext()
        {
            Api.JetResetSessionContext(_shapeGeometryCursor.Session);            
        }
        private void ResetInstanceContext()
        {           
            Api.JetResetSessionContext(_shapeInstanceCursor.Session);
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using Xbim.Common.Geometry;
using static Xbim.IO.Esent.EsentModel;

namespace Xbim.IO.Esent
{
    internal class EsentGeometryStore : IGeometryStore
    {
        private ILogger Log => _esentModel.Logger;

        private readonly EsentModel _esentModel;

        private EsentGeometryInitialiser _currentTransaction = null;
        private EsentShapeInstanceCursor _shapeInstanceCursor;
        private EsentShapeGeometryCursor _shapeGeometryCursor;
        private bool _disposed;

        public EsentGeometryStore(EsentModel esentModel )
        {           
            _esentModel = esentModel;
        }

        public EsentModel Model
        {
            get { return _esentModel; }
        }

        private TableStatus _tableStatus = EsentModel.TableStatus.Unknown;

        private TableStatus TableStatus
        {
            get
            {
                if (_tableStatus == TableStatus.Unknown)
                {
                    _tableStatus = _esentModel.Cache.HasTable(EsentShapeGeometryCursor.GeometryTableName)
                        ? TableStatus.Found
                        : TableStatus.Missing;
                }
                return _tableStatus;
            }
        }


        public IGeometryStoreInitialiser BeginInit()
        {            
            try
            {
                if (_currentTransaction == null) //we can start a new one
                {
                    //dispose of any tables because we are going to clear them
                    if (_shapeGeometryCursor != null) 
                    {
                        _shapeGeometryCursor.Dispose();
                        _shapeGeometryCursor = null;
                    }
                    if (_shapeInstanceCursor != null) 
                    {
                        _shapeInstanceCursor.Dispose();
                        _shapeInstanceCursor = null;
                    }
                    //delete any geometries in the database
                    _esentModel.ClearGeometryTables();
                     _shapeGeometryCursor = _esentModel.GetShapeGeometryTable();
                     _shapeInstanceCursor = _esentModel.GetShapeInstanceTable();
                    _currentTransaction = new EsentGeometryInitialiser(this, _shapeGeometryCursor, _shapeInstanceCursor);                            
                    return _currentTransaction;
                } 
                throw new Exception("A transaction is in operation on the geometry store");
            }
            catch (Exception e)
            {
                if (_shapeGeometryCursor != null) _esentModel.FreeTable(_shapeGeometryCursor);
                if (_shapeInstanceCursor != null) _esentModel.FreeTable(_shapeInstanceCursor);
                _currentTransaction = null;
                throw new Exception("Begin initialisation failed on Geometry Store",e);
            }        
        }

        internal void EndInit(IGeometryStoreInitialiser transaction)
        {
            //update the reference counts
            if (transaction == _currentTransaction)
            {
                _currentTransaction.UpdateReferenceCounts();
                _currentTransaction.Commit();              
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
            else
                throw new ArgumentException("The transaction is not the one that is currently running", "transaction");
        }

        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }


        public void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.
                if (disposing)
                {
                    //managed resources                   
                }
                //unmanaged, mostly esent related              
                if (_currentTransaction != null) //we have terminated unexpectedly, the database may be corrupt
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
                if (_shapeGeometryCursor != null) _esentModel.FreeTable(_shapeGeometryCursor);
                if (_shapeInstanceCursor != null) _esentModel.FreeTable(_shapeInstanceCursor);
            }
            _disposed = true;
        }


        public IGeometryStoreReader BeginRead()
        {
             return new EsentGeometryStoreReader(_esentModel); 
        }

        public bool IsEmpty
        {
            get
            {
                if (TableStatus == TableStatus.Missing)
                    return true;
                EsentShapeGeometryCursor shapeGeometryCursor = null;
                try
                {
                    IXbimShapeGeometryData shapeGeometry = new XbimShapeGeometry();
                    shapeGeometryCursor = _esentModel.GetShapeGeometryTable();
                    using (var shapeGeometryTransaction = shapeGeometryCursor.BeginReadOnlyTransaction())
                    {
                        var isEmpty = !shapeGeometryCursor.TryMoveFirstShapeGeometry(ref shapeGeometry);
                        return isEmpty;
                    }
                }
                catch (Exception)
                {
                    Log.LogWarning("Esent model {0} does not contain geometry tables.", _esentModel.DatabaseName);
                    return true;
                }
                finally
                {
                    if(shapeGeometryCursor!=null)
                        _esentModel.FreeTable(shapeGeometryCursor);
                }
            }
        }
    }
}

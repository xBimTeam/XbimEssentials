using System;
using Xbim.Common.Geometry;

namespace Xbim.IO.Esent
{
    internal class EsentGeometryStore : IGeometryStore, IDisposable
    {
        private readonly EsentModel _esentModel;

        private EsentGeometryInitialiser _currentTransaction = null;
        private EsentShapeInstanceCursor _shapeInstanceCursor;
        private EsentShapeGeometryCursor _shapeGeometryCursor;

        public EsentGeometryStore(EsentModel esentModel )
        {           
            _esentModel = esentModel;
           
        }

        public EsentModel Model
        {
            get { return _esentModel; }
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

        public void EndInit(IGeometryStoreInitialiser transaction)
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
            if (_currentTransaction != null) //we have terminated unexpectedly, the database may be corrupt
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
            if (_shapeGeometryCursor != null) _esentModel.FreeTable(_shapeGeometryCursor);
            if (_shapeInstanceCursor != null) _esentModel.FreeTable(_shapeInstanceCursor);
        }


        public IGeometryStoreReader BeginRead()
        {
             return new EsentGeometryStoreReader(_esentModel); 
        }

        
    }
}

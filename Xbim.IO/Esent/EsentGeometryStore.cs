using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XbimGeometry.Interfaces;

namespace Xbim.IO.Esent
{
    internal class EsentGeometryStore : IGeometryStore, IDisposable
    {
        private readonly EsentModel _esentModel;

        private EsentGeometryTransaction _currentTransaction = null;

        public EsentGeometryStore(EsentModel esentModel )
        {           
            _esentModel = esentModel;
           
        }

        public EsentModel Model
        {
            get { return _esentModel; }
        }


        public IGeometryWriteTransaction BeginInit()
        {
            EsentShapeGeometryCursor shapeGeometryCursor = null;
            EsentShapeInstanceCursor shapeInstanceCursor = null;
            try
            {


                if (_currentTransaction == null) //we can start a new one
                {
                    
                     shapeGeometryCursor = _esentModel.GetShapeGeometryTable();
                     shapeInstanceCursor = _esentModel.GetShapeInstanceTable();
                    _currentTransaction = new EsentGeometryTransaction(this, shapeGeometryCursor, shapeInstanceCursor);
                    //delete any geometries in the database
                    return _currentTransaction;
                }
            }
            catch (Exception)
            {
                if (shapeGeometryCursor != null) _esentModel.FreeTable(shapeGeometryCursor);
                if (shapeInstanceCursor != null) _esentModel.FreeTable(shapeInstanceCursor);
                _currentTransaction = null;
            }
            return null;
            
        }

        public void EndInit(IGeometryWriteTransaction transaction)
        {
            if (transaction == _currentTransaction)
            {
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
        }
    }
}

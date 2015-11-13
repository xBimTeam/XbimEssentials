using System.Collections;
using System.Collections.Generic;

namespace XbimGeometry.Interfaces
{
    public interface IGeometryStore
    {
       /// <summary>
        /// Clears the store of any existing geometries and prepares for an update
       /// </summary>
       /// <returns>A Transaction for bulk addition of geometries, prevents update by other processes, if null an Initialisation is in process already or the owning model is not available for write operations </returns>
        IGeometryWriteTransaction BeginInit();
        /// <summary>
        /// Finalises an initialisation and flushes all operations to the store, the transaction must have been obtained from BeginInit
        /// </summary>
        void EndInit(IGeometryWriteTransaction transaction);

        
    }
}

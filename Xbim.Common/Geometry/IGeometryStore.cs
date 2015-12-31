using System;
namespace Xbim.Common.Geometry
{
    public interface IGeometryStore: IDisposable
    {
       /// <summary>
        /// Clears the store of any existing geometries and prepares for an update
       /// </summary>
       /// <returns>A Transaction for bulk addition of geometries, prevents update by other processes, if null an Initialisation is in process already or the owning model is not available for write operations </returns>
        IGeometryStoreInitialiser BeginInit();
       

        /// <summary>
        /// Returns a reader for accessing geometry in the store, nb this is a disposable and should be used in using{} context
        /// </summary>
        IGeometryStoreReader BeginRead();
    }
}

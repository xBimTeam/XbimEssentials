namespace Xbim.Common.Geometry
{
    public interface IGeometryStore
    {
       /// <summary>
        /// Clears the store of any existing geometries and prepares for an update
       /// </summary>
       /// <returns>A Transaction for bulk addition of geometries, prevents update by other processes, if null an Initialisation is in process already or the owning model is not available for write operations </returns>
        IGeometryStoreInitialiser BeginInit();
        /// <summary>
        /// Finalises an initialisation and flushes all operations to the store, the transaction must have been obtained from BeginInit
        /// </summary>
        void EndInit(IGeometryStoreInitialiser transaction);

        /// <summary>
        /// Returns a reader for accessing geometry in the store, nb this is a disposable and should be used in using{} context
        /// </summary>
        IGeometryStoreReader BeginRead();
    }
}

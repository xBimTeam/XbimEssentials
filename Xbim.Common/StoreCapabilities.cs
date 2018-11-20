namespace Xbim.IO
{
    /// <summary>
    /// Describes the capabilities of a Model Store
    /// </summary>
    public class StoreCapabilities
    {
        public StoreCapabilities(bool isTransient, bool supportsTransactions)
        {
            IsTransient = isTransient;
            SupportsTransactions = supportsTransactions;
        }

        /// <summary>
        /// Indicates whether the model store is transient. If true it's an in-memory model, otherwise has a persistant backing store
        /// </summary>
        public bool IsTransient { get; private set; }
        /// <summary>
        /// Indicates whether the model store supports transactions. 
        /// </summary>
        public bool SupportsTransactions { get; private set; }
    }
}

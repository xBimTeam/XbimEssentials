namespace Xbim.IO
{
    /// <summary>
    /// Indicates how a persisted model should be accessed
    /// </summary>
    public enum XbimDBAccess
    {
        /// <summary>
        /// Opens the database for read only transactions
        /// </summary>
        Read,
        /// <summary>
        /// Opens the database for read and write 
        /// </summary>
        ReadWrite,
        /// <summary>
        /// Opens the database exclusively, prevents access from any other processes.
        /// </summary>
        Exclusive
    }
}

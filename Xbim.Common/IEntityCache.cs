using System;

namespace Xbim.Common
{
    /// <summary>
    /// Some implementations of IModel might use optimized data handling where
    /// entity data is loaded on the fly from database, file or other source.
    /// For some scenarios it is usefull to keep entities alive in the cache
    /// so they don't get disposed and every entity only has one instance.
    /// </summary>
    public interface IEntityCache: IDisposable
    {
        /// <summary>
        /// Clears the cache. This might have no 
        /// effect on pure in-memory IModel implementations.
        /// </summary>
        void Clear();

        /// <summary>
        /// Stops the cache but keeps entities already cached. This might have no 
        /// effect on pure in-memory IModel implementations.
        /// </summary>
        void Stop();

        /// <summary>
        /// Starts the cache if it is not active. This might have no 
        /// effect on pure in-memory IModel implementations.
        /// </summary>
        void Start();

        /// <summary>
        /// Number of entities in the cache
        /// </summary>
        int Size { get; }

        /// <summary>
        /// True if cache is active. You can cganhe this by calling Start() and Stop()
        /// </summary>
        bool IsActive { get; }
    }
}

using System;

namespace Xbim.Common
{
    /// <summary>
    /// Inverse relations are one of very important concepts in EXPRESS modeling which allows to mimic 
    /// safe bi-directional relations between two objects. But because EXPRESS model is always flat list of entities
    /// some queries using inverse properties for navigation in model cause exponential cost if the data search.
    /// That is obviously not optimal. Inverse cache should help especially in cases where the IModel implementation
    /// doesn't have any secondary indexing mechanism for inverse relations. It can usually only be used outside
    /// of transaction. Starting transaction when caching is on might raise an exception. You should always keep
    /// inverse cache object inside of using statement. 
    /// </summary>
    public interface IInverseCache : IDisposable
    {
        /// <summary>
        /// Removes all cached inverse relations.
        /// </summary>
        void Clear();

        /// <summary>
        /// Number of cached relations
        /// </summary>
        int Size { get; }
    }
}

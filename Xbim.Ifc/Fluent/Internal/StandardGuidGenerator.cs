using System;
using Xbim.Common;

namespace Xbim.Ifc.Fluent.Internal
{
    /// <summary>
    /// A standard Guid generator
    /// </summary>
    internal class StandardGuidGenerator : IGuidGenerator
    {
        public Guid GenerateForEntity(IPersistEntity entity)
        {
            return Guid.NewGuid();
        }
    }
}

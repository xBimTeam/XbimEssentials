using System.Collections.Generic;

namespace Xbim.Common
{
    public interface IContainsIndexedReferences
    {
        IEnumerable<IPersistEntity> IndexedReferences { get; }
    }
}

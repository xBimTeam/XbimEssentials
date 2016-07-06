using System.Collections.Generic;

namespace Xbim.Common
{
    public interface IContainsIndexedReferences: IPersistEntity
    {
        IEnumerable<IPersistEntity> IndexedReferences { get; }
    }
}

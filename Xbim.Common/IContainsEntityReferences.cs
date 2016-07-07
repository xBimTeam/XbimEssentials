using System.Collections.Generic;

namespace Xbim.Common
{
    public interface IContainsEntityReferences: IPersistEntity
    {
        IEnumerable<IPersistEntity> References { get; } 
    }
}

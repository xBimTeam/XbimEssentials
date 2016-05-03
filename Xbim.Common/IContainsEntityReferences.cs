using System.Collections.Generic;

namespace Xbim.Common
{
    public interface IContainsEntityReferences
    {
        IEnumerable<IPersistEntity> References { get; } 
    }
}

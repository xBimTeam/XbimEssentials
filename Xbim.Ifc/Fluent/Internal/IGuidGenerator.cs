using System;
using Xbim.Common;

namespace Xbim.Ifc.Fluent.Internal
{
    internal interface IGuidGenerator
    {
        Guid GenerateForEntity(IPersistEntity entity);
    }
}

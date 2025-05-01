using System;

namespace Xbim.Ifc.Fluent.Internal
{
    /// <summary>
    /// A standard DateTime generator
    /// </summary>
    internal class StandardDateTimeGenerator : IDateTimeGenerator
    {
        public DateTime Generate()
        {
            return DateTime.UtcNow;
        }
    }
}

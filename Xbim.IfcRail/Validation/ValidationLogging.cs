using Microsoft.Extensions.Logging;
using System;
using Xbim.Common;

namespace Xbim.IfcRail.Validation
{
    internal class ValidationLogging
    {
        internal static ILogger CreateLogger<T>()
        {
            return XbimLogging.CreateLogger<T>();
        }
    }
}
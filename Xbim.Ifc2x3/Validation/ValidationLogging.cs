using Microsoft.Extensions.Logging;
using System;
using Xbim.Common;

namespace Xbim.Ifc2x3.Validation
{
    internal class ValidationLogging
    {
        internal static ILogger CreateLogger<T>()
        {
            return XbimLogging.CreateLogger<T>();
        }
    }
}
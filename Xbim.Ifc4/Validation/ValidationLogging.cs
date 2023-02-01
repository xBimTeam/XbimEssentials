using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xbim.Common.Configuration;

namespace Xbim.Ifc4.Validation
{
    internal class ValidationLogging
    {
        internal static ILogger CreateLogger<T>()
        {
            var factory = XbimServices.Current.GetLoggerFactory();
            return factory.CreateLogger<T>();
        }
    }
}
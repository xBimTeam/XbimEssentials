using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xbim.Common.Configuration
{
    public static class XbimServiceExtensions
    {
        /// <summary>
        /// Convenience method to simplify getting the <see cref="ILoggerFactory"/>
        /// </summary>
        /// <param name="services"></param>
        /// <returns>The <see cref="ILoggerFactory"/></returns>
        public static ILoggerFactory GetLoggerFactory(this XbimServices services)
        {
            return services.ServiceProvider.GetRequiredService<ILoggerFactory>();
        }

        /// <summary>
        /// Convenience method to simplify creating an <see cref="ILogger{TCategoryName}"/> outside of DI
        /// </summary>
        /// <param name="services"></param>
        /// <returns>The <see cref="ILogger{TCategoryName}"/></returns>
        public static ILogger<T> CreateLogger<T>(this XbimServices services)
        {
            return services.ServiceProvider.GetRequiredService<ILogger<T>>();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Xbim.Common
{
    public static class ServiceCollectionExtensions
    {
       

        /// <summary>
        /// Adds xbim default logging services to the specified <see cref="IServiceCollection"/>
        /// </summary>
        /// <remarks>This is automatically applied to the service collection after all other services.
        /// Adding it earlier will prevent real logging systems from being added</remarks>
        /// <param name="services"></param>
        /// <returns>The <see cref="IServiceCollection"/> so additional calls can be chained</returns>
        internal static IServiceCollection AddXbimLogging(this IServiceCollection services)
        {

            // Add fallback loggers in case the consumer doesn't provide any.
            services.TryAddSingleton<ILoggerFactory, NullLoggerFactory>();
            services.TryAddSingleton(NullLogger.Instance as ILogger);
            services.TryAddSingleton(typeof(ILogger<>), typeof(NullLogger<>));

            return services;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;

namespace Xbim.Common.Configuration
{
    public static class MemoryModelServiceCollectionExtensions
    {
        /// <summary>
        /// Adds xbim Toolkit IFC Model services to the specified <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services"></param>
        /// <returns>The <see cref="IServiceCollection"/> so additional calls can be chained</returns>
        public static IServiceCollection AddXbimToolkit(this IServiceCollection services)
        {
            return services.AddXbimToolkit(opt => { });
        }

        /// <summary>
        /// Adds xbim Toolkit IFC Model services to the specified <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns>The <see cref="IServiceCollection"/> so additional calls can be chained</returns>
        public static IServiceCollection AddXbimToolkit(this IServiceCollection services, Action<IXbimConfigurationBuilder> configure)
        {
            var builder = new XbimConfigurationBuilder(services);
            configure(builder);
            // Fall back to MemoryProvider if no IModelProvider specified
            MemoryModelConfigurationBuilderExtensions.AddMemoryModel(builder);
            
            return services;
        }
    }
}

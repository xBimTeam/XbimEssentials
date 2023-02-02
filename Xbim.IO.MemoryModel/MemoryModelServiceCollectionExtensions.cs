using Microsoft.Extensions.DependencyInjection;
using System;
using Xbim.Common.Configuration;

namespace Xbim.Common
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
            return services.AddXbimToolkit(o => o.UseMemoryModel());
        }

        /// <summary>
        /// Adds xbim Toolkit IFC Model services to the specified <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns>The <see cref="IServiceCollection"/> so additional calls can be chained</returns>
        public static IServiceCollection AddXbimToolkit(this IServiceCollection services, Action<IXbimConfigurationBuilder> configure)
        {
                        
            configure(new XbimConfigurationBuilder(services));
            return services;
        }
    }
}

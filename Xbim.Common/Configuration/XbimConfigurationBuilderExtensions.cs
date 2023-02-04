using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xbim.IO;

namespace Xbim.Common.Configuration
{
    /// <summary>
    /// Extension methods for setting up xbim services using an <see cref="IXbimConfigurationBuilder"/>
    /// </summary>
    public static class XbimConfigurationBuilderExtensions
    {

        /// <summary>
        /// Replaces the default <see cref="NullLoggerFactory"/> with a <see cref="ILoggerFactory"/> of your choosing.
        /// </summary>
        /// <remarks>Typically you'd use this where you already have a logging system in DI. You could 
        /// alternatively add a logging implementation directly to via <see cref="XbimServices.ConfigureServices(System.Action{IServiceCollection})"/>.
        /// Using this method means you share a <see cref="ILoggerFactory"/> across your application rather than having multiple ILoggingFactories/></remarks>
        /// <param name="builder"></param>
        /// <param name="loggerFactory"></param>
        /// <returns></returns>
        public static IXbimConfigurationBuilder AddLoggerFactory(this IXbimConfigurationBuilder builder, ILoggerFactory loggerFactory)
        {
            builder.Services.RemoveAll<ILoggerFactory>();
            
            builder.Services.TryAdd(ServiceDescriptor.Singleton<ILoggerFactory>(loggerFactory));
            // Bit of an assumption that the LoggerFactory wil create Logger<> rather than some other ILogger<>
            // but better then leaving NullLoggers resolving. Open Generics make this hard to define any other way
            builder.Services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)));
            return builder;
        }

        /// <summary>
        /// Replace the default <see cref="IModelProvider"/> with your own
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IXbimConfigurationBuilder AddModelProvider<T>(this IXbimConfigurationBuilder builder) where T : IModelProvider
        {

            builder.Services.TryAddSingleton(typeof(IModelProvider), typeof(T));
          
            return builder;
        }
    }
}

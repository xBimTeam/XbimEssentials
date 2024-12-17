using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Diagnostics;

namespace Xbim.Common.Configuration
{
    /// <summary>
    /// An Internal Service provider which serves as a fall back to a minimal services implementation when the consuming application
    /// does not yet explicitly registered the xbim services or <see cref="IServiceProvider"/> on <see cref="XbimServices"/>. 
    /// The fallback services provides a safe baseline but provides no Logging capability.
    /// XBIM INTERNAL USE ONLY. Prefer <see cref="XbimServices"/>
    /// </summary>
    internal class InternalServiceProvider
    {
        /// <summary>
        /// Gets the internal ServiceProvider
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        private readonly static Lazy<InternalServiceProvider> lazySingleton;

        static InternalServiceProvider()
        {
            lazySingleton = new Lazy<InternalServiceProvider>(() => new InternalServiceProvider());
        }

        private InternalServiceProvider()
        {
            // Set up minimal services required to use Geometry.
            var services = new ServiceCollection();
            // Provide a NullLogger implementation so DI dependencies are satsisfied. We don't have a concrete Logger implementation we can use
            services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
            services.TryAdd(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(NullLogger<>)));

            //services.AddXbimToolkit(opt => opt);
            ServiceProvider = services.BuildServiceProvider();

            var warning = @$"NOTE: The xbim InternalServices are being used because Xbim.Common.Configuration.XbimServices has not yet been configured. This fallback service provider has no logging support so you may miss useful output from xbim. To see xbim logs ensure you provide a LoggerFactory to {typeof(XbimServices).FullName} at startup - or provide an existing ServiceProvider to the XbimServices. e.g.

XbimServices.Current.ConfigureServices(s => s.AddXbimToolkit(c => c.AddLoggerFactory(loggerFactory)));
// or
XbimServices.Current.UseExternalServiceProvider(serviceProvider);";

            Debug.WriteLine(warning);
            Console.Error.WriteLine(warning);
        }

        /// <summary>
        /// Gets the Current instance of the <see cref="InternalServiceProvider"/>
        /// </summary>
        public static InternalServiceProvider Current { get => lazySingleton.Value; }


       
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Xbim.Essentials.Tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010029a3c6da60efcb3ebe48c3ce14a169b5fa08ffbf5f276392ffb2006a9a2d596f5929cf0e68568d14ac7cbe334440ca0b182be7fa6896d2a73036f24bca081b2427a8dec5689a97f3d62547acd5d471ee9f379540f338bbb0ae6a165b44b1ae34405624baa4388404bce6d3e30de128cec379147af363ce9c5845f4f92d405ed0")]

namespace Xbim.Common.Configuration
{
    /// <summary>
    /// Class encapsulating the services used in the application managed by Dependency Injection
    /// </summary>
    public class XbimServices
    {

        private XbimServices()
        {
            Rebuild();
        }

        /// <summary>
        /// For testing only
        /// </summary>
        /// <returns></returns>
        internal static XbimServices CreateInstance()
        {
            return new XbimServices();
        }

        private bool isBuilt = false;

        private IServiceCollection services = new ServiceCollection();

        static Lazy<XbimServices> lazySingleton = new Lazy<XbimServices>(() => new XbimServices());
        Lazy<IServiceProvider> serviceProviderBuilder;

        /// <summary>
        /// The shared instance of all Xbim Services
        /// </summary>
        public static XbimServices Current { get; private set; } = lazySingleton.Value;


        /// <summary>
        /// Flag indicating if the DI container has been built
        /// </summary>
        public bool IsBuilt { get => isBuilt; }

        /// <summary>
        /// Configure the internal <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>This cannot be called after <see cref="ServiceProvider"/> has been used since 
        /// the DI  Services will have be built by then</remarks>
        /// <exception cref="InvalidOperationException">When calling after the DI is built</exception>
        public void ConfigureServices(Action<IServiceCollection> configure)
        {
            if(isBuilt)
            {
                throw new InvalidOperationException("The xbim internal ServiceCollection has already been built");
            }
            configure(services);
        }

        /// <summary>
        /// Gets a <see cref="IServiceProvider"/> used for resolving xbim services 
        /// </summary>
        /// <remarks>Used when an external DI service is not employed</remarks>
        public IServiceProvider ServiceProvider => serviceProviderBuilder.Value;

        /// <summary>
        /// For internal and unit test purposes only
        /// </summary>
        internal void Rebuild()
        {
            services.Clear();
            isBuilt = false;
            if (serviceProviderBuilder != null &&
                serviceProviderBuilder.IsValueCreated && 
                serviceProviderBuilder.Value is ServiceProvider sp)
            {
                sp.Dispose();
            }
            //rebuild the Lazy so IServiceProvider is rebuild next time
            serviceProviderBuilder = new Lazy<IServiceProvider>(() =>
            {
                isBuilt = true;
                return services
                    .AddXbimLogging()  // Add after other services have been added to avoid hijacking logging
                    .BuildServiceProvider();
            });
        }

        
    }
}

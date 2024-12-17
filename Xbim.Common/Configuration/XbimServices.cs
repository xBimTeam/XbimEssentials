using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Xbim.Essentials.Tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010029a3c6da60efcb3ebe48c3ce14a169b5fa08ffbf5f276392ffb2006a9a2d596f5929cf0e68568d14ac7cbe334440ca0b182be7fa6896d2a73036f24bca081b2427a8dec5689a97f3d62547acd5d471ee9f379540f338bbb0ae6a165b44b1ae34405624baa4388404bce6d3e30de128cec379147af363ce9c5845f4f92d405ed0")]
[assembly: InternalsVisibleTo("Xbim.Essentials.NetCore.Tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010029a3c6da60efcb3ebe48c3ce14a169b5fa08ffbf5f276392ffb2006a9a2d596f5929cf0e68568d14ac7cbe334440ca0b182be7fa6896d2a73036f24bca081b2427a8dec5689a97f3d62547acd5d471ee9f379540f338bbb0ae6a165b44b1ae34405624baa4388404bce6d3e30de128cec379147af363ce9c5845f4f92d405ed0")]

namespace Xbim.Common.Configuration
{
    /// <summary>
    /// Class encapsulating the services  and <see cref="IServiceProvider"/>s used in the application.
    /// Consumers can provide their own built service provider, or configure the internal xbim <see cref="IServiceProvider"/>.
    /// </summary>
    /// <remarks>the service delays building of the internal service provider when not configured to support 
    /// late configutation of the services.
    /// In this case the service provision is delagated to <see cref="InternalServiceProvider"/></remarks>
    public class XbimServices
    {

        private XbimServices()
        {
            Rebuild();
        }

        /// <summary>
        /// Create a new XbimServiecs instance. For testing only
        /// </summary>
        /// <returns>a private instance of the <see cref="XbimServices"/></returns>
        public static XbimServices CreateInstanceInternal()
        {
            return new XbimServices();
        }

        private bool isBuilt = false;
        

        private IServiceCollection servicesCollection = new ServiceCollection();
        private IServiceProvider externalServiceProvider = null;

        static Lazy<XbimServices> lazySingleton = new Lazy<XbimServices>(() => new XbimServices());
        Lazy<IServiceProvider> serviceProviderBuilder;

        /// <summary>
        /// The shared instance of all Xbim Services
        /// </summary>
        public static XbimServices Current { get; private set; } = lazySingleton.Value;


        /// <summary>
        /// Override xbim's internal <see cref="IServiceCollection"/> with another instance for configuration
        /// </summary>
        /// <remarks>Use this when you have an existing <see cref="IServiceCollection"/> instance you want to share with 
        /// xbim's internal DI. E.g. Useful if you want to share logging configuration. 
        /// <para>Note: XbimServices will automatically generate its own <see cref="IServiceProvider"/> which will 
        /// still resuly in an additional <see cref="IServiceProvider"/> in your application. 
        /// </para>
        /// <para>
        /// Prefer <see cref="UseExternalServiceProvider(IServiceProvider)"/> if you have access to an <see cref="IServiceProvider"/> -
        /// unless you are sure what you're doing. If using this external Service Collection approach you could consider using XbimService's <see cref="ServiceProvider"/> 
        /// instance for resolving your own services.</para>
        /// </remarks>
        /// <param name="collection"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void UseExternalServiceCollection(IServiceCollection collection)
        {
            if (isBuilt)
            {
                throw new InvalidOperationException("The xbim internal ServiceCollection has already been built");
            }
            servicesCollection = collection;
        }

        /// <summary>
        /// Override xbim's internal <see cref="IServiceProvider"/> with a configured instance
        /// </summary>
        /// <remarks>
        /// After calling this any prior or future calls to <see cref="ConfigureServices"/> will be ignored.
        /// Use this when you have access to an existing <see cref="IServiceProvider"/> and need to take full 
        /// control of the DependencyInjection setup for xbim Toolkit in your application.
        /// <para>
        /// You must add a logging implementation satisifing <see cref="Microsoft.Extensions.Logging.ILoggerFactory"/>, 
        /// or call <see cref="XbimServiceCollectionExtensions.AddXbimLogging"/>.
        /// Be sure to call AddXbimToolkit() on your ServiceCollection if using the IfcStore and related 
        /// <see cref="Xbim.IO.IModelProvider"/> services
        /// </para>
        /// </remarks>
        /// <param name="provider"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void UseExternalServiceProvider(IServiceProvider provider)
        {
            if (isBuilt)
            {
                // likely to cause lifecycle issues if we let this happen / e.g. Duplicate Singletons, or
                // unexpected disposal of resources if we GC'd the original provider.
                throw new InvalidOperationException("The xbim internal ServiceProvider has already been built");
            }
            isBuilt = true; // Prevent users Configuring Services as it won't take effect
            externalServiceProvider = provider;
        }

        /// <summary>
        /// Flag indicating if the DI container has been built
        /// </summary>
        public bool IsBuilt { get => isBuilt; }

        /// <summary>
        /// Flag indicating the DI container has been configured. When not configured the ServiceProvider will delay being built, and
        /// service provision will fall back to an internal <see cref="IServiceProvider"/>.
        /// </summary>
        public bool IsConfigured { get => servicesCollection.Any() || externalServiceProvider != null; }

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
            configure(servicesCollection);
        }

        /// <summary>
        /// Gets a <see cref="IServiceProvider"/> used for resolving xbim services 
        /// </summary>
        /// <remarks>To avoid building the service provider too early, when not using an external service provider, 
        /// and the xbim service has not been configured, the system reverts to a basic internal <see cref="IServiceProvider"/></remarks>
        public IServiceProvider ServiceProvider => externalServiceProvider ?? 
            (IsConfigured ? serviceProviderBuilder.Value : InternalServiceProvider.Current.ServiceProvider);

        /// <summary>
        /// Rebuilds the internal xbim DI / Service Provider. For internal and unit test purposes only
        /// </summary>
        internal void Rebuild()
        {
            servicesCollection.Clear();
            isBuilt = false;
            
            if (serviceProviderBuilder != null &&
                serviceProviderBuilder.IsValueCreated && 
                serviceProviderBuilder.Value is ServiceProvider sp)
            {
                sp.Dispose();
            }
            //rebuild the Lazy so IServiceProvider is rebuilt next time
            serviceProviderBuilder = new Lazy<IServiceProvider>(() =>
            {
                isBuilt = true;
                return servicesCollection
                    .AddXbimLogging()  // Add after other services have been added to avoid hijacking logging
                    .BuildServiceProvider();
            });
        }

        
    }
}

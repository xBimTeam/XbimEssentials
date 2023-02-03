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

        private IServiceCollection servicesCollection = new ServiceCollection();
        private IServiceProvider externalServiceProvider = null;

        static Lazy<XbimServices> lazySingleton = new Lazy<XbimServices>(() => new XbimServices());
        Lazy<IServiceProvider> serviceProviderBuilder;

        /// <summary>
        /// The shared instance of all Xbim Services
        /// </summary>
        public static XbimServices Current { get; private set; } = lazySingleton.Value;

        // TODO: Need to consider if this is a good idea/useful. Could be open to abuse/bugs
        ///// <summary>
        ///// Override xbim's internal <see cref="IServiceCollection"/> with another instance for configuration
        ///// </summary>
        ///// <remarks>Use this when you are responsible for creating your application's IServiceProvider, and
        ///// have an existing ServiceCollection you want xbim to register services with. 
        ///// <para>Note: XbimServices will automatically generate its own <see cref="IServiceProvider"/> which may result in
        ///// duplicate providers in your application. 
        ///// </para>
        ///// <para>
        ///// Prefer <see cref="UseExternalServiceProvider(IServiceProvider)"/> 
        ///// unless you are sure what you're doing. If using this approach you would typically use XbimService's <see cref="ServiceProvider"/> 
        ///// instance for resolving your own services.</para>
        ///// </remarks>
        ///// <param name="collection"></param>
        ///// <exception cref="InvalidOperationException"></exception>
        //public void UseExternalServiceCollection(IServiceCollection collection)
        //{
        //    if (isBuilt)
        //    {
        //        throw new InvalidOperationException("The xbim internal ServiceCollection has already been built");
        //    }
        //    servicesCollection = collection;
        //}

        /// <summary>
        /// Override xbim's internal <see cref="IServiceProvider"/> with a configured instance
        /// </summary>
        /// <remarks>
        /// After calling this any prior or future calls to <see cref="ConfigureServices"/> will be ignored.
        /// Use this when you want to take full control of the DependencyInjection setup for xbim Toolkit in your application.
        /// <para>
        /// You must add logging, or call <see cref="XbimServiceCollectionExtensions.AddXbimLogging"/>.
        /// Be sure to call AddXbimToolkit() on your ServiceCollection if using the IfcStore.
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
        /// <remarks>Used when an external DI service is not employed</remarks>
        public IServiceProvider ServiceProvider => externalServiceProvider ?? serviceProviderBuilder.Value;

        /// <summary>
        /// For internal and unit test purposes only
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
            //rebuild the Lazy so IServiceProvider is rebuild next time
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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Xbim.Common.Configuration;
using Xbim.IO;
using Xbim.IO.Memory;

namespace Xbim.Ifc
{

    /// <summary>
    /// A default factory that provides an <see cref="IModelProvider"/>
    /// </summary>
    /// <remarks>The Default ModelProvider is simply a slender wrapped around the DI service.
    /// </remarks>
    public class DefaultModelProviderFactory : IModelProviderFactory
    {
        
        private readonly ILogger logger;

        private readonly ILoggerFactory _loggerFactory;
        private readonly IModelProvider modelProvider;

        public DefaultModelProviderFactory(ILoggerFactory loggerFactory, IModelProvider modelProvider)
        {
            _loggerFactory = loggerFactory ?? XbimServices.Current.GetLoggerFactory();
            this.modelProvider = modelProvider;
            logger = _loggerFactory.CreateLogger<DefaultModelProviderFactory>();
        }

        /// <summary>
        /// Creates a new <see cref="IModelProvider"/>
        /// </summary>
        /// <returns></returns>
        public IModelProvider CreateProvider()
        {
            return  GetDefaultProvider();
        }

        // Historically we used to load different providers depending on what was loaded in the AppDomain.
        // Now we simply rely on the DI config to give us a provider 
        private IModelProvider GetDefaultProvider()
        {
           
            var provider = modelProvider;

            if(provider is MemoryModelProvider)
            {
                        
                logger.LogWarning(
                @"Xbim using the MemoryModelProvider. Working with large models could be sub-optimal and could consume large amounts of RAM. 
To fix this warning, consider calling 'XbimServices.ServiceCollection.AddXbimToolkit(opt=> opt.UseHeuristicModel);' at application startup'");
            }
            return provider;
            
        }

        [Obsolete("Replaced by dependency injection. Use XbimServices.ServiceCollection.AddXbimToolkit(opt=> opt.UseModelProvider<T>() instead")]
        public void Use(Func<IModelProvider> providerFn)
        {
            throw new NotImplementedException();
        }
    }

   
}

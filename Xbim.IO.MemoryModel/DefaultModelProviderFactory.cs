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
    /// <remarks>By default, unless Xbim.Esent.IO is referenced the factory will create a <see cref="MemoryModelProvider"/>
    /// If Esent is loaded, the Heuristic provider is loaded, which provides better scalability.
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

        // We infer the best provider based on what we can find in process. 
        private IModelProvider GetDefaultProvider()
        {
            try
            {
                var provider = modelProvider;

                if (provider != null)
                {
                    if(provider is MemoryModelProvider)
                    {
                        
                        logger.LogWarning(
                        @"Xbim using the MemoryModelProvider. Working with large models could be sub-optimal. 
To fix this warning, consider calling 'XbimServices.ServiceCollection.AddXbimToolkit(opt=> opt.UseHeuristicModel);' at application startup'");
                    }
                    return provider;
                }
                else
                {
                    
                }
            }
            catch(Exception ex)
            {
                logger.LogError(0, ex, @"Failed to resolve Esent.IO's ModelProvider. Defaulting to MemoryModelProvider. 
Consider calling 'XbimServices.ServiceCollection.AddXbimToolkit(opt=> opt.UseHeuristicModel);' at application startup'");
            }
            return new MemoryModelProvider(_loggerFactory);
        }

        [Obsolete("Replaced by dependency injection. Use XbimServices.ServiceCollection.AddXbimToolkit(opt=> opt.UseModelProvider<T>() instead")]
        public void Use(Func<IModelProvider> providerFn)
        {
            throw new NotImplementedException();
        }
    }

   
}

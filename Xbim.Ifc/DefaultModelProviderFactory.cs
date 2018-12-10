using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xbim.Common;
using Xbim.IO;
using Xbim.IO.Memory;

namespace Xbim.Ifc
{

    // Poor-man's DI 

    /// <summary>
    /// A default factory that provides an <see cref="IModelProvider"/>
    /// </summary>
    /// <remarks>By default, unless Xbim.Esent.IO is referenced the factory will create a <see cref="MemoryModelProvider"/>
    /// If Esent is loaded, the Heuristic provider is loaded, which provides better scalability.
    /// </remarks>
    public class DefaultModelProviderFactory : IModelProviderFactory
    {
        private const string EsentAssemblyName = "Xbim.IO.Esent";
        private const string EsentProviderName = "Heuristic";
        private Func<IModelProvider> _modelProvider = null;

        private readonly ILogger logger = XbimLogging.CreateLogger<DefaultModelProviderFactory>();

        /// <summary>
        /// Creates a new <see cref="IModelProvider"/>
        /// </summary>
        /// <returns></returns>
        public IModelProvider CreateProvider()
        {
            return _modelProvider != null ? _modelProvider() :
                GetDefaultProvider();
        }

        // We infer the best provider based on what we can find in process. 
        private IModelProvider GetDefaultProvider()
        {
            try
            {
                // Look for the Heuristic implementation of IModelProvider in the XBim.Esent Assembly if available
                // Xbim.Ifc is not referencing the assembly - so we have do some reflection
                var iModelProvider = typeof(IModelProvider);
                var providerType = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(asm => asm.GetName().Name.StartsWith(EsentAssemblyName, StringComparison.InvariantCultureIgnoreCase))
                    .SelectMany(s => s.GetLoadableTypes())
                    .FirstOrDefault(t => iModelProvider.IsAssignableFrom(t) && t.Name.Contains(EsentProviderName));

                // TODO: we may need to probe for existance of Xbim.IO.Essent assembly on disk, if it 
                // hasn't been loaded

                if (providerType != null)
                {
                    var provider = (IModelProvider)Activator.CreateInstance(providerType);
                    return provider;
                }
                else
                {
                    logger.LogWarning(
                        @"Defaulting to MemoryModelProvider. Working with large models could be sub-optimal. 
To fix this warning, consider calling 'IfcStore.ModelProviderFactory.UseHeuristicModelProvider();' at application startup'");
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex, @"Failed to resolve Esent.IO's ModelProvider. Defaulting to MemoryModelProvider. 
Consider calling 'IfcStore.ModelProviderFactory.UseHeuristicModelProvider();' at application startup'");
            }
            return new MemoryModelProvider();
        }

        /// <summary>
        /// Hook to allow 3rd parties to explicitly configure another <see cref="IModelProvider"/> implementation
        /// to be provided in place of the default provider
        /// </summary>
        /// <param name="providerFn">Delegate to provide a new IModelProvider instance</param>
        public void Use(Func<IModelProvider> providerFn)
        {
            _modelProvider = providerFn ?? throw new ArgumentNullException(nameof(providerFn));
        }
    }

    internal static class TypeLoaderExtensions
    {
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}

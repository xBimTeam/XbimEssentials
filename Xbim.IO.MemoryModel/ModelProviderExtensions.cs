using Xbim.IO;
using Xbim.IO.Memory;

namespace Xbim.Ifc
{
    public static class ModelProviderExtensions
    {
        /// <summary>
        /// Configures the <see cref="IModelProviderFactory"/> to use the <see cref="MemoryModelProvider"/>
        /// </summary>
        /// <remarks>Note: The <see cref="MemoryModelProvider"/> does not support all storage options. In
        /// particular it has no persistance mechanism except from storing back to IFC/IfcXml.
        /// If you require 'random access' without always holding the model in memory, another
        /// ModelProvider may be required. See Xbim.IO.Esent.HeuristicModelProvider.
        /// </remarks>
        /// <param name="providerFactory">The <see cref="IModelProviderFactory"/> to configure</param>
        /// <returns>The <see cref="IModelProviderFactory"/></returns>
        public static IModelProviderFactory UseMemoryModelProvider(this IModelProviderFactory providerFactory)
        {
            providerFactory.Use(() => new MemoryModelProvider());
            return providerFactory;
        }
    }
}

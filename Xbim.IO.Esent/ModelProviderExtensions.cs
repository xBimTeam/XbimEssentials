using Xbim.IO;
using Xbim.IO.Esent;

namespace Xbim.Ifc
{
    public static class ModelProviderExtensions
    {
        /// <summary>
        /// Configures the <see cref="IModelProviderFactory"/> to use the <see cref="HeuristicModelProvider"/>
        /// </summary>
        /// <remarks>This provider gives the best performance and functionality tradeoff by using both the 
        /// <see cref="EsentModel"/> and <see cref="IO.Memory.MemoryModel"/></remarks>
        /// <param name="providerFactory">The <see cref="IModelProviderFactory"/> to configure</param>
        /// <returns>The <see cref="IModelProviderFactory"/></returns>
        public static IModelProviderFactory UseHeuristicModelProvider(this IModelProviderFactory providerFactory)
        {
            providerFactory.Use(() => new HeuristicModelProvider());
            return providerFactory;
        }

        /// <summary>
        /// Configures the <see cref="IModelProviderFactory"/> to use the <see cref="EsentModelProvider"/>
        /// </summary>
        /// <param name="providerFactory">The <see cref="IModelProviderFactory"/> to configure</param>
        /// <returns>The <see cref="IModelProviderFactory"/></returns>
        public static IModelProviderFactory UseEsentModelProvider(this IModelProviderFactory providerFactory)
        {
            providerFactory.Use(() => new EsentModelProvider());
            return providerFactory;
        }
    }
}

using Microsoft.Extensions.DependencyInjection.Extensions;
using Xbim.Ifc;
using Xbim.IO;
using Xbim.IO.Esent;

namespace Xbim.Common.Configuration
{
    public static class EsentModelConfigurationBuilderExtensions
    {
        /// <summary>
        /// Use the <see cref="EsentModel"/> in the IfcStore. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="forceEngineFormatVersion9060">If true, force the engine format version to 9060 when creating databases.</param>
        /// <returns></returns>
        public static IXbimConfigurationBuilder AddEsentModel(this IXbimConfigurationBuilder builder, bool forceEngineFormatVersion9060 = false)
        {
            // Configure the global default used by PersistedEntityInstanceCache
            PersistedEntityInstanceCache.ForceEngineFormatVersion9060 = forceEngineFormatVersion9060;
            builder.Services.TryAddSingleton<IModelProvider, EsentModelProvider>();
            return builder;
        }

        /// <summary>
        /// Uses the best model depending on the model characteristics in the IfcStore. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="forceEngineFormatVersion9060">If true, force the engine format version to 9060 when creating databases.</param>
        /// <returns></returns>
        public static IXbimConfigurationBuilder AddHeuristicModel(this IXbimConfigurationBuilder builder, bool forceEngineFormatVersion9060 = false)
        {
            // Configure the global default used by PersistedEntityInstanceCache
            PersistedEntityInstanceCache.ForceEngineFormatVersion9060 = forceEngineFormatVersion9060;
            builder.Services.TryAddSingleton<IModelProvider, HeuristicModelProvider>();
            return builder;
        }
    }
}

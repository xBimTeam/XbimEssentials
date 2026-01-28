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
        /// <param name="limitEngineFormatVersion">If defined, limits the engine format version to the specified version when creating databases.</param>
        public static IXbimConfigurationBuilder AddEsentModel(this IXbimConfigurationBuilder builder, EngineFormatVersion limitEngineFormatVersion = EngineFormatVersion.Default)
        {
            // Configure the global default used by PersistedEntityInstanceCache
            PersistedEntityInstanceCache.LimitEngineFormatVersion = limitEngineFormatVersion;
            builder.Services.TryAddSingleton<IModelProvider, EsentModelProvider>();
            return builder;
        }

        /// <summary>
        /// Uses the best model depending on the model characteristics in the IfcStore. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="limitEngineFormatVersion">If defined, limits the engine format version to the specified version when creating databases.</param>
        public static IXbimConfigurationBuilder AddHeuristicModel(this IXbimConfigurationBuilder builder, EngineFormatVersion limitEngineFormatVersion = EngineFormatVersion.Default)
        {
            // Configure the global default used by PersistedEntityInstanceCache
            PersistedEntityInstanceCache.LimitEngineFormatVersion = limitEngineFormatVersion;
            builder.Services.TryAddSingleton<IModelProvider, HeuristicModelProvider>();
            return builder;
        }
    }
}

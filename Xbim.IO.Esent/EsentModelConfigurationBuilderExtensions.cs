using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xbim.Common.Configuration;
using Xbim.Ifc;
using Xbim.IO;
using Xbim.IO.Esent;

namespace Xbim.Common
{
    public static class EsentModelConfigurationBuilderExtensions
    {
        /// <summary>
        /// Use the <see cref="EsentModel"/> in the IfcStore. 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IXbimConfigurationBuilder UseEsentModel(this IXbimConfigurationBuilder builder)
        {
            builder.Services.TryAddSingleton<IModelProvider, EsentModelProvider>();
            return builder;
        }

        /// <summary>
        /// Uses the best model depending on the model characteristics in the IfcStore. 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IXbimConfigurationBuilder UseHeuristicModel(this IXbimConfigurationBuilder builder)
        {
            builder.Services.TryAddSingleton<IModelProvider, HeuristicModelProvider>();
            return builder;
        }
    }
}

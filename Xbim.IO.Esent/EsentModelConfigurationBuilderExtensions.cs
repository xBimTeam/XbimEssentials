using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
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
        public static IXbimConfigurationBuilder AddEsentModel(this IXbimConfigurationBuilder builder)
        {
            return AddEsentModel(builder, _ => { });
        }

        /// <summary>
        /// Use the <see cref="EsentModel"/> in the IfcStore. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        public static IXbimConfigurationBuilder AddEsentModel(this IXbimConfigurationBuilder builder, Action<IEsentBuillder> configure)
        {

            builder.Services.TryAddSingleton<IModelProvider, EsentModelProvider>();

            configure(new EsentBuilder(builder.Services));
            return builder;
        }

        /// <summary>
        /// Uses the best model depending on the model characteristics in the IfcStore. 
        /// </summary>
        /// <param name="builder"></param>
        public static IXbimConfigurationBuilder AddHeuristicModel(this IXbimConfigurationBuilder builder)
        {
            return AddHeuristicModel(builder, _ => { });
        }

        /// <summary>
        /// Uses the best model depending on the model characteristics in the IfcStore. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        public static IXbimConfigurationBuilder AddHeuristicModel(this IXbimConfigurationBuilder builder, Action<IEsentBuillder> configure)
        {

            builder.Services.TryAddSingleton<IModelProvider, HeuristicModelProvider>();
            configure(new EsentBuilder(builder.Services));
            return builder;
        }
    }
}

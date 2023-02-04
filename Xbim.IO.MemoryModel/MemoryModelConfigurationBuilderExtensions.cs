using Microsoft.Extensions.DependencyInjection.Extensions;
using Xbim.IO;
using Xbim.IO.Memory;

namespace Xbim.Common.Configuration
{
    public static class MemoryModelConfigurationBuilderExtensions
    {
        /// <summary>
        /// Use the <see cref="MemoryModel"/> in the IfcStore. This is the default.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IXbimConfigurationBuilder AddMemoryModel(this IXbimConfigurationBuilder builder)
        {
            builder.Services.TryAddSingleton<IModelProvider, MemoryModelProvider>();
            return builder;
        }
    }
}

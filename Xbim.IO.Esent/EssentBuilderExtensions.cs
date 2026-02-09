using Microsoft.Extensions.DependencyInjection;

using System;


namespace Xbim.IO.Esent
{
    public static class EssentBuilderExtensions
    {
        /// <summary>
        /// Set the Esent format version for models
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="engineFormatVersion"></param>
        /// <returns></returns>
        public static IEsentBuillder SetFormat(this IEsentBuillder builder, EngineFormatVersion engineFormatVersion) => 
            builder.ConfigureEsent(options => options.FormatVersion = engineFormatVersion);



        private static IEsentBuillder ConfigureEsent(this IEsentBuillder builder, Action<EsentEngineOptions> action)
        {
            builder.Services.Configure(action);
            return builder;
        }
    }
}

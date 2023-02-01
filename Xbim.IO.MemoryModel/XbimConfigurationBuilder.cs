using Microsoft.Extensions.DependencyInjection;

namespace Xbim.Common.Configuration
{
    internal class XbimConfigurationBuilder : IXbimConfigurationBuilder
    {
        public IServiceCollection Services { get; }

        public XbimConfigurationBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}

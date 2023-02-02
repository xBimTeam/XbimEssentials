using Microsoft.Extensions.DependencyInjection;

namespace Xbim.Common.Configuration
{
    /// <summary>
    /// An interface for configuring the xbim system
    /// </summary>
    public interface IXbimConfigurationBuilder
    {
        /// <summary>
        /// Gets the internal <see cref="IServiceCollection"/> where xbim is configured
        /// </summary>
        IServiceCollection Services { get; }
    }
}

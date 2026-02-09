using Microsoft.Extensions.DependencyInjection;

namespace Xbim.IO.Esent
{
    public interface IEsentBuillder
    {
        IServiceCollection Services { get; }
    }

    internal sealed class EsentBuilder : IEsentBuillder
    {
        public EsentBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}

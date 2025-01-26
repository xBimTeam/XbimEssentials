using Microsoft.Extensions.DependencyInjection;
using Xbim.Common.Configuration;
using Xunit;


namespace Xbim.Essentials.NetCore.Tests
{
    
    public class DependencyInjectionTests
    {

        [Fact]
        public void ServiceProviderIsValid()
        {

            var SuT = XbimServices.CreateInstanceInternal();
            SuT.ConfigureServices(s =>
            {
                var services = s.AddXbimToolkit(opt => opt.AddEsentModel())
                    .AddLogging();

                // Manually build a ServiceProvider to sanity check baseline DI is valid

                var provider = services.BuildServiceProvider(new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true,
                });
            });

        }
    }
}

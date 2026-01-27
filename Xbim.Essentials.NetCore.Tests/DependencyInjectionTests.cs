using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xbim.Common.Configuration;
using Xbim.IO.Esent;
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
                PersistedEntityInstanceCache.ForceEngineFormatVersion9060.Should().BeFalse("Esent model should be configured to default engine format for this test.");

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

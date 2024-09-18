using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Configuration;


namespace Xbim.Essentials.NetCore.Tests
{
    [TestClass]
    public class DependencyInjectionTests
    {

        [TestMethod]
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

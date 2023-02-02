using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using Xbim.Common;
using Xbim.Common.Configuration;
using Xbim.Ifc;
using Xbim.IO;
using Xbim.IO.Esent;
using Xunit;
using Xunit.Abstractions;

namespace Xbim.Essentials.Tests
{
    public class DefaultModelProviderFactoryTests
    {
        // The DefaultModelProviderFactory lives in Xbim.Ifc assembly, which now has no reference
        // direct or indirect to IO.Esent. When Esent is loaded in the domain we want the factory 
        // to discover the HeuristicModelProvider.

        // To really test this in an assembly that references Esent (like this test suite),
        // we need to create a fresh appdomain

        // Just to force Esent into memory/AppDomain so we get consistent tests regardless of what test ran prior

        [MethodImpl(MethodImplOptions.NoOptimization)]
        static DefaultModelProviderFactoryTests()
        {
            Type dummyReferenceToLoadEsentAsembly = typeof(Xbim.IO.Esent.EsentModel);
        }



        private readonly ITestOutputHelper testOutputHelper;

        public DefaultModelProviderFactoryTests(ITestOutputHelper  testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            // Create private instance of DI
            xbimServices = XbimServices.CreateInstance();
        }

        XbimServices xbimServices;

        [Fact]
        public void ShouldProvideHeuristicModelProvider_In_IfcStore()
        {

            var modelProvider = IfcStore.ModelProviderFactory.CreateProvider();

            Assert.IsType<HeuristicModelProvider>(modelProvider);
        }

        [Fact]
        public void Can_Override_Default_with_the_Essent_Provider()
        {
            // Arrange
            xbimServices.ConfigureServices(s => s.AddXbimToolkit(opt => opt.UseEsentModel()));
            var serviceProvider = xbimServices.ServiceProvider;

            IModelProviderFactory factory = serviceProvider.GetRequiredService<IModelProviderFactory>();
            // Act

            var modelProvider = factory.CreateProvider();

            // Assert
            Assert.IsType<EsentModelProvider>(modelProvider);
        }

        [Fact]
        public void Can_Override_Default_with_the_Heuristic_Provider()
        {
            // Arrange
            xbimServices.ConfigureServices(s => s.AddXbimToolkit(opt => opt.UseHeuristicModel()));
            var serviceProvider = xbimServices.ServiceProvider;

            IModelProviderFactory factory = serviceProvider.GetRequiredService<IModelProviderFactory>();
            // Act

            var modelProvider = factory.CreateProvider();

            // Assert
            Assert.IsType<HeuristicModelProvider>(modelProvider);
        }

        // Sanity check
        [Fact]
        public void Should_Load_IO_Esent_In_Default_Domain()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(asm => asm.GetName().Name.Equals("Xbim.IO.Esent", StringComparison.InvariantCultureIgnoreCase));
            var asms = AppDomain.CurrentDomain.GetAssemblies().Select(asm => asm.GetName().Name);

            foreach (var m in asms)
            {
                testOutputHelper.WriteLine(m);
            }
            Assert.Single(assemblies);
        }

        //Sanity Check
        [Fact]
        public void In_a_Pristine_Domain_Esent_Should_Not_Be_loaded()
        {
            Proxy proxy = null;
            try
            {
                proxy = CreateNewAppDomain();
                var assemblies = proxy.GetLoadedAssemblies();
                assemblies.Where(asm => asm.GetName().Name == "Xbim.IO.Esent").Should().BeEmpty();
            }
            finally
            {
                // Safely unload the Appdomain
                if(proxy != null)
                {
                    var appDomain = proxy.GetAppDomain();
                    AppDomain.Unload(appDomain);
                }
            }
        }

        [Fact]
        public void In_New_Domain_Default_ModelProvider_loads_MemoryModelProvider()
        {
            Proxy proxy = null;
            try
            {
                // Arrange
                proxy = CreateNewAppDomain();
                // Act (via proxy)
                var providerType = proxy.GetModelProviderType();
                // Assert
                providerType.Name.Should().Be("MemoryModelProvider");
            }
            finally
            {
                // Safely unload the Appdomain
                if (proxy != null)
                {
                    var appDomain = proxy.GetAppDomain();
                    AppDomain.Unload(appDomain);
                }
            }
        }

        



        private static Proxy CreateNewAppDomain()
        {
            AppDomainSetup domaininfo = new AppDomainSetup();
            domaininfo.ApplicationBase = System.Environment.CurrentDirectory;
            Evidence evidence = AppDomain.CurrentDomain.Evidence;
            var domain = AppDomain.CreateDomain("MyDomain", evidence, domaininfo);
            Type proxyType = typeof(Proxy);
            return (Proxy)domain.CreateInstanceAndUnwrap(
                proxyType.Assembly.FullName,
                proxyType.FullName);
        }
    }

    // A proxy class we load in a new domain and can interact with
    public class Proxy : MarshalByRefObject
    {
        public AppDomain GetAppDomain()
        {
            return AppDomain.CurrentDomain;
        }

        public IEnumerable<Assembly> GetLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        
        public Type GetModelProviderType()
        {

            // Arrange
            var xbimServices = XbimServices.CreateInstance();
            xbimServices.ConfigureServices(s => s.AddXbimToolkit());
            var serviceProvider = xbimServices.ServiceProvider;


            IModelProviderFactory modelProviderFactory = serviceProvider.GetRequiredService<IModelProviderFactory>();

           
            return modelProviderFactory.CreateProvider().GetType();
        }
    }
}

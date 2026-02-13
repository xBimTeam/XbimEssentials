using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Xbim.Common;
using Xbim.Common.Configuration;
using Xbim.Ifc;
using Xbim.IO;
using Xunit;

namespace Xbim.Essentials.Tests
{
    // We have two separate test environments going on: vsTest and xUnit. We need to bootstrap each as we've not guarantee
    // on the order tests run in.

    [TestClass]
    public class VsTestInit
    {

        private static IModelProvider _modelProvider;
        [AssemblyInitialize]
        public static void InitializeReferencedAssemblies(TestContext context)
        {
            // Share the implementation
            xUnitInit.Initialize();

            // Initialises the Singleton XbimServices ServiceProvider via IfcStores static ctor.
            _modelProvider = IfcStore.Create(Common.Step21.XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel).ModelProvider;
        }


        [TestMethod]
        public void IsSetup()
        {
            _modelProvider.Should().BeOfType<HeuristicModelProvider>();
        }
    }

    [CollectionDefinition(nameof(xUnitBootstrap))]
    public class xUnitBootstrap : ICollectionFixture<xUnitInit>
    {
        // Does nothing but trigger xUnitUnit construction at beginning of test run
    }


    /// <summary>
    /// Initilialises the Xbim DI Services once as a bootstrap
    /// </summary>
    public class xUnitInit : IDisposable
    {

        public xUnitInit()
        {
            
            Initialize();
            // Trigger initialisation
            _ = IfcStore.Create(Common.Step21.XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);
        }

        public static void Initialize()
        {
            if (!XbimServices.Current.IsBuilt)
            {
                XbimServices.Current.ConfigureServices(s => s.AddXbimToolkit(opt => opt.AddHeuristicModel()));
            }
        }

        public void Dispose()
        {
            
        }
    }

    /// <summary>
    /// Reinstate DI after a xunit fixture may have cleared the shared DI
    /// </summary>
    public class xUnitReinit : IDisposable
    {

        public xUnitReinit()
        {
            // Nothing to do on setup fixture
        }

        public static void Reset()
        {
            if (!XbimServices.Current.IsBuilt)
            {
                XbimServices.Current.ConfigureServices(s => s.AddXbimToolkit(opt => opt.AddHeuristicModel()));
            }
        }

        public void Dispose()
        {
            // Reinitialise the DI
            Reset();
            _ = IfcStore.Create(Common.Step21.XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel);
        }
    }

}

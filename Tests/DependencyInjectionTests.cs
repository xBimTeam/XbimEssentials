﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.IO;
using Xbim.Common;
using Xbim.Common.Configuration;
using Xbim.Common.Model;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.IO;
using Xbim.IO.Esent;
using Xbim.IO.Memory;
using Xunit;

namespace Xbim.Essentials.Tests
{
    [Collection(nameof(xUnitBootstrap))]
    public class DependencyInjectionTests
    {
        public DependencyInjectionTests()
        {
            // Clear the singleton collection each test
            SuT = XbimServices.CreateInstanceInternal();
        }

        private XbimServices SuT;

        [InlineData(typeof(ILoggerFactory))]
        [InlineData(typeof(ILogger))]
        [InlineData(typeof(ILogger<XbimServices>))]
        [InlineData(typeof(IModelProvider))]
        [Theory]
        public void Services_can_be_resolved(Type type)
        {
            SuT.ConfigureServices(s => s.AddXbimToolkit());

            var service = SuT.ServiceProvider.GetRequiredService(type);

            service.Should().NotBeNull();
            
        }

        [Fact]
        public void Services_Start_Unbuilt()
        {
            SuT.IsBuilt.Should().BeFalse();
        }

        [Fact]
        public void MemoryModelProvider_resolved_by_default()
        {
            SuT.ConfigureServices(s => s.AddXbimToolkit());

            var provider = SuT.ServiceProvider.GetRequiredService<IModelProvider>();
            provider.Should().NotBeNull();
            provider.Should().BeOfType<MemoryModelProvider>();
        }

        [Fact]
        public void MemoryModelProvider_resolved_by_default_with_Config()
        {
            SuT.ConfigureServices(s => s.AddXbimToolkit(opt => opt.AddLoggerFactory(new LoggerFactory())));

            var provider = SuT.ServiceProvider.GetRequiredService<IModelProvider>();
            provider.Should().NotBeNull();
            provider.Should().BeOfType<MemoryModelProvider>();
        }

        [Fact]
        public void MemoryModelProvider_can_be_added_explicitly()
        {
            SuT.ConfigureServices(s => s.AddXbimToolkit(opt => opt.AddMemoryModel()));
            
            var provider = SuT.ServiceProvider.GetRequiredService<IModelProvider>();
            provider.Should().NotBeNull();
            provider.Should().BeOfType<MemoryModelProvider>();
        }

        [Fact]
        public void EsentModelProvider_resolves()
        {
            SuT.ConfigureServices(s => s.AddXbimToolkit(opt => opt.AddEsentModel()));

            var provider = SuT.ServiceProvider.GetRequiredService<IModelProvider>();
            provider.Should().NotBeNull();
            provider.Should().BeOfType<EsentModelProvider>();
        }

        [Fact]
        public void HeuristicModelProvider_resolves()
        {
            SuT.ConfigureServices(s => s.AddXbimToolkit(opt => opt.AddHeuristicModel()));

            var provider = SuT.ServiceProvider.GetRequiredService<IModelProvider>();
            provider.Should().NotBeNull();
            provider.Should().BeOfType<HeuristicModelProvider>();
        }

        [Fact]
        public void ThirdParty_modelProviders_resolve()
        {
            SuT.ConfigureServices(s => s.AddXbimToolkit(opt => opt.AddModelProvider<DummyModelProvider>()));

            var provider = SuT.ServiceProvider.GetRequiredService<IModelProvider>();
            provider.Should().NotBeNull();
            provider.Should().BeOfType<DummyModelProvider>();
        }


        [Fact]
        public void Logging_can_be_added()
        {
            SuT.ConfigureServices(s => s.AddLogging()); // MSE Logging as a replacement for default XbimLogging (NullLogger)

            var factory = SuT.ServiceProvider.GetRequiredService<ILoggerFactory>();
            factory.Should().NotBeNull();
            factory.Should().BeOfType<LoggerFactory>();

            var logger = SuT.ServiceProvider.GetRequiredService<ILogger<DependencyInjectionTests>>();
            logger.Should().BeOfType(typeof(Logger<>));
        }

        [Fact]
        public void Logging_can_be_added_after_XbimToolit()
        {
            SuT.ConfigureServices(s => s
                .AddXbimToolkit()
                .AddLogging());    // MSE Logging

            var factory = SuT.ServiceProvider.GetRequiredService<ILoggerFactory>();
            factory.Should().NotBeNull();
            factory.Should().BeOfType<LoggerFactory>();

            var logger = SuT.ServiceProvider.GetRequiredService<ILogger<DependencyInjectionTests>>();
            logger.Should().BeOfType(typeof(Logger<>));
        }

        [Fact]
        public void Logging_can_be_added_before_XbimToolit()
        {
            SuT.ConfigureServices(s => s
                .AddLogging()   // MSE Logging
                .AddXbimToolkit()
                );
            var factory = SuT.ServiceProvider.GetRequiredService<ILoggerFactory>();
            factory.Should().NotBeNull();
            factory.Should().BeOfType<LoggerFactory>();

            var logger = SuT.ServiceProvider.GetRequiredService<ILogger<DependencyInjectionTests>>();
            logger.Should().BeOfType(typeof(Logger<>));
        }

        [Fact]
        public void Logging_can_be_added_before_explicit_XbimLogging()
        {
            SuT.ConfigureServices(s => s
                .AddLogging()   // MSE Logging
                .AddXbimLogging()
                );

            var factory = SuT.ServiceProvider.GetRequiredService<ILoggerFactory>();
            factory.Should().NotBeNull();
            factory.Should().BeOfType<LoggerFactory>();

            var logger = SuT.ServiceProvider.GetRequiredService<ILogger<DependencyInjectionTests>>();
            logger.Should().BeOfType(typeof(Logger<>));
        }

        [Fact]
        public void Adding_Logging_After_NullLogging_Is_Ignored()
        {
            SuT.ConfigureServices(s => s
                .AddXbimLogging()
                .AddLogging()   // MSE Logging
                );

            var factory = SuT.ServiceProvider.GetRequiredService<ILoggerFactory>();
            factory.Should().NotBeNull();
            factory.Should().BeOfType<NullLoggerFactory>();

            var logger = SuT.ServiceProvider.GetRequiredService<ILogger<DependencyInjectionTests>>();
            logger.Should().BeOfType(typeof(NullLogger<>));
        }


        [Fact]
        public void LoggingFactory_can_be_replaced()
        {
            var loggerFactory = new LoggerFactory();

            SuT.ConfigureServices(s => s
                .AddXbimToolkit(opt => opt.AddLoggerFactory(loggerFactory))
                );

            var factory = SuT.ServiceProvider.GetRequiredService<ILoggerFactory>();
            factory.Should().NotBeNull();
            factory.Should().BeOfType<LoggerFactory>();

            var logger = SuT.ServiceProvider.GetRequiredService<ILogger<DependencyInjectionTests>>();
            logger.Should().BeOfType<Logger<DependencyInjectionTests>>();
        }

        [Fact]
        public void Can_Supply_External_ServiceCollection()
        {
            var externalServices = new ServiceCollection();


            SuT.UseExternalServiceCollection(externalServices);

            // XbimServices is will register manadatory services
            var service = SuT.ServiceProvider.GetRequiredService<ILoggerFactory>();

            service.Should().NotBeNull();
        }

        [Fact]
        public void Can_Supply_External_ServiceProvider()
        {
            var externalServices = new ServiceCollection();
            externalServices.AddXbimToolkit()
                .AddLogging();
            var extProvider = externalServices.BuildServiceProvider();
            SuT.UseExternalServiceProvider(extProvider);

            var service = SuT.ServiceProvider.GetRequiredService<IModelProvider>();

            service.Should().NotBeNull();
        }

        [Fact]
        public void Services_Are_Registered_Once_Only()
        {
            var services = new ServiceCollection();
            int count = 0;
            
            SuT.ConfigureServices(s => 
            { 
                s.AddXbimToolkit();
                count = s.Count;
            });

            SuT.ConfigureServices(s =>
            {
                s.AddXbimToolkit();
                s.Count.Should().Be(count);
            });


        }

        [Fact()]
        public void Minimial_Fallback_Services_Are_Always_Available()
        {

            XbimServices.Current.Rebuild();
            using (FileStream fs1 = new FileStream(@"TestFiles/4walls1floorSite.ifc", FileMode.Open))
            {
                var header = StepModel.LoadStep21Header(fs1);

                header.Should().NotBeNull();

            }
            // Add explicitly. Normally set up by IfcStore static ctor etc, but that isundone by XbimServices.Current.Rebuild()
            XbimServices.Current.ConfigureServices(s => s.AddXbimToolkit(t => t.AddHeuristicModel()));
            using FileStream fs2 = new FileStream(@"TestFiles/4walls1floorSite.ifc", FileMode.Open);
            var ifcStore = IfcStore.Open(fs2, StorageType.Ifc, XbimModelType.MemoryModel);
            ifcStore.Instances.Should().NotBeEmpty();
        }


        private class DummyModelProvider : IModelProvider
        {
            public StoreCapabilities Capabilities => throw new NotImplementedException();

            public Func<XbimSchemaVersion, IEntityFactory> EntityFactoryResolver { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public void Close(IModel model)
            {
                throw new NotImplementedException();
            }

            public IModel Create(XbimSchemaVersion ifcVersion, string dbPath)
            {
                throw new NotImplementedException();
            }

            public IModel Create(XbimSchemaVersion ifcVersion, XbimStoreType storageType)
            {
                throw new NotImplementedException();
            }

            public string GetLocation(IModel model)
            {
                throw new NotImplementedException();
            }

            public XbimSchemaVersion GetXbimSchemaVersion(string modelPath)
            {
                throw new NotImplementedException();
            }

            public IModel Open(Stream stream, StorageType dataType, XbimSchemaVersion schemaVersion, XbimModelType modelType, XbimDBAccess accessMode = XbimDBAccess.Read, ReportProgressDelegate progDelegate = null, int codePageOverride = -1)
            {
                throw new NotImplementedException();
            }

            public IModel Open(string path, XbimSchemaVersion schemaVersion, double? ifcDatabaseSizeThreshHold = null, ReportProgressDelegate progDelegate = null, XbimDBAccess accessMode = XbimDBAccess.Read, int codePageOverride = -1)
            {
                throw new NotImplementedException();
            }

            public void Persist(IModel model, string fileName, ReportProgressDelegate progDelegate = null)
            {
                throw new NotImplementedException();
            }
        }
    }
}

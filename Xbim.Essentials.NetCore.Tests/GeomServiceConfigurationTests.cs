#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xbim.Common.Configuration;
using Xbim.Ifc;
using Xbim.IO.Esent;
using Xunit;
using Xunit.Abstractions;

namespace Xbim.Essentials.NetCore.Tests
{
    /// <summary>
    /// Isolated test collection for GeomService configuration tests.
    /// This prevents DI configuration from interfering with other tests.
    /// </summary>
    [CollectionDefinition(nameof(GeomServiceTestCollection), DisableParallelization = true)]
    public class GeomServiceTestCollection
    {
    }

    /// <summary>
    /// Tests that replicate the dependency injection configuration.
    /// These tests use XbimServices.Current (the singleton) to test the production code paths that are not relyiant on external providers.
    /// Each test rebuilds the singleton to ensure isolation.
    /// </summary>
    [Collection(nameof(GeomServiceTestCollection))]
    public class GeomServiceConfigurationTests : IDisposable
    {
        private readonly ITestOutputHelper _output;

        public GeomServiceConfigurationTests(ITestOutputHelper output)
        {
            _output = output;
            // Reset the singleton state before each test to ensure isolation
            XbimServices.Current.Rebuild();
        }

        public void Dispose()
        {
            // Reset after each test to not affect other tests in the solution
            XbimServices.Current.Rebuild();
        }

        /// <summary>
        /// Tests the DI configuration used when logging is enabled in GeomService.
        /// </summary>
        [Fact]
        public void ConfigureServicesWithLogging()
        {
            // Arrange - Configure LoggerFactory using Microsoft.Extensions.Logging 
            // (equivalent to Program.cs Serilog LoggerFactory setup)
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            // Act - Replicate the ConfigureServices call from Program.cs (lines 151-155)
            // This uses XbimServices.Current exactly as production code does
            XbimServices.Current.ConfigureServices(services => services
                .AddXbimToolkit(opt => opt
                    .AddLoggerFactory(loggerFactory)
                    .AddEsentModel(EngineFormatVersion.JET_efvSynchronousLVCleanup)
                ));

            // Assert
            XbimServices.Current.IsBuilt.Should().BeTrue("ConfigureServices should mark the services as built");

            var resolvedLoggerFactory = XbimServices.Current.ServiceProvider.GetRequiredService<ILoggerFactory>();
            resolvedLoggerFactory.Should().NotBeNull();
            resolvedLoggerFactory.Should().Be(loggerFactory, "the provided LoggerFactory should be registered");

            var modelProvider = XbimServices.Current.ServiceProvider.GetRequiredService<Xbim.IO.IModelProvider>();
            modelProvider.Should().NotBeNull();
            modelProvider.Should().BeOfType<EsentModelProvider>("Esent model provider should be configured");

            // Verify Esent engine version is configured correctly
            PersistedEntityInstanceCache.LimitEngineFormatVersion.Should().Be(
                EngineFormatVersion.JET_efvSynchronousLVCleanup,
                "Esent engine format version should match the configured value");
        }

        /// <summary>
        /// Tests the DI configuration used when logging is disabled in GeomService.
        /// </summary>
        [Fact]
        public void ConfigureServicesWithoutLogging()
        {
            // Act - Replicate the ConfigureServices call from Program.cs (lines 160-163)
            // This is the configuration used when doLog is false
            XbimServices.Current.ConfigureServices(services => services
                .AddXbimToolkit(opt => opt
                    .AddEsentModel(EngineFormatVersion.JET_efvSynchronousLVCleanup)
                ));

            // Assert
            XbimServices.Current.IsBuilt.Should().BeTrue("ConfigureServices should mark the services as built");

            var modelProvider = XbimServices.Current.ServiceProvider.GetRequiredService<Xbim.IO.IModelProvider>();
            modelProvider.Should().NotBeNull();
            modelProvider.Should().BeOfType<EsentModelProvider>("Esent model provider should be configured");

            // Verify Esent engine version is configured correctly
            PersistedEntityInstanceCache.LimitEngineFormatVersion.Should().Be(
                EngineFormatVersion.JET_efvSynchronousLVCleanup,
                "Esent engine format version should match the configured value");
        }

        /// <summary>
        /// Tests that the ServiceProvider can be validated using the same validation options
        /// that Program.cs could theoretically use for debugging DI issues.
        /// </summary>
        [Fact]
        public void ServiceProviderValidation()
        {
            // Arrange
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            // Act - Configure services and build a validated ServiceProvider
            XbimServices.Current.ConfigureServices(services =>
            {
                services.AddXbimToolkit(opt => opt
                    .AddLoggerFactory(loggerFactory)
                    .AddEsentModel(EngineFormatVersion.JET_efvSynchronousLVCleanup)
                );

                // Build a separate validated ServiceProvider to ensure DI configuration is valid
                var validationAction = () => services.BuildServiceProvider(new ServiceProviderOptions
                {
                    ValidateOnBuild = true,
                    ValidateScopes = true,
                });

                // Assert - The validation should not throw
                validationAction.Should().NotThrow("DI configuration should be valid");
            });
        }

        /// <summary>
        /// Integration test that replicates the full workflow from Xbim.Sunshine.GeomService/Program.cs Main().
        /// Opens an IFC file and saves it as xbim format, using xUnit output for logging.
        /// </summary>
        [Theory]
        [InlineData(EngineFormatVersion.JET_efvSynchronousLVCleanup, "Format ulVersion: 0x620,60,140")]
        [InlineData(EngineFormatVersion.JET_efvWindows10v2004, "Format ulVersion: 0x620,110,240")] // this may need changing 
        public async Task FullDependencyInjectionWorkflowWithXbimServicesCurrent(EngineFormatVersion version, string expectedFormatString)
        {
            // Arrange - Set up xUnit logging (replaces Serilog in Program.cs)
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(_output, LogLevel.Debug));
            var logger = loggerFactory.CreateLogger<GeomServiceConfigurationTests>();

            logger.LogInformation("============== Starting meshing in GeomService test ==============");

            // Configure services exactly as Program.cs does (lines 151-155)
            XbimServices.Current.ConfigureServices(services => services
                .AddXbimToolkit(opt => opt
                    .AddLoggerFactory(loggerFactory)
                    .AddEsentModel(version)
                ));

            logger.LogDebug("Xbim toolkit configured with logger factory and Esent version");

            // Get the test IFC file (similar to Program.cs command line parsing)
            var ifcFile = new FileInfo(@"TestFiles\SampleHouse4.ifc");
            ifcFile.Exists.Should().BeTrue($"Test file should exist at {ifcFile.FullName}");

            logger.LogInformation($"Ifc: {ifcFile.FullName}");

            // Generate temp file for xbim output (similar to Program.cs lines 166-171)
            string tempFileName;
            do
            {
                tempFileName = Path.ChangeExtension(Path.GetTempFileName(), "xbim");
            } while (File.Exists(tempFileName));

            logger.LogInformation($"temp: {tempFileName}");

            try
            {
                var stopwatch = Stopwatch.StartNew();

                // Act - Open IFC and save as xbim 
                // this approach used to fail in production when using XbimServices.Current, because the 
                // configuration was performed again with default values upon IfcStore.Open
                using (var model = IfcStore.Open(ifcFile.FullName, null, 0, ReportProgress))
                {
                    logger.LogInformation($"Model opened at {stopwatch.ElapsedMilliseconds}ms");
                    logger.LogInformation($"Model schema: {model.SchemaVersion}");
                    logger.LogInformation($"Model instances count: {model.Instances.Count}");

                    model.SaveAs(tempFileName, null, ReportProgress);
                    logger.LogInformation($"Model saved at {stopwatch.ElapsedMilliseconds}ms");

                    model.Close();
                }

                // Assert - Verify the xbim file was created
                var xbimFile = new FileInfo(tempFileName);
                xbimFile.Exists.Should().BeTrue("xbim file should be created");
                xbimFile.Length.Should().BeGreaterThan(0, "xbim file should have content");

                logger.LogInformation($"xbim file size: {xbimFile.Length} bytes");
                logger.LogInformation($"Total elapsed: {stopwatch.ElapsedMilliseconds}ms");

                // Verify Esent format version using esentutl.exe (similar to EsentVersionCompatibilityTests)
                logger.LogInformation("Running esentutl.exe to verify Esent database format version...");

                var esentOutput = await ProcessTaskHelpers.RunProcessAsync("esentutl.exe", $"/mh \"{tempFileName}\"");

                // Log the output for inspection
                _output.WriteLine("=== esentutl.exe output ===");
                _output.WriteLine(esentOutput);
                _output.WriteLine("=== end esentutl.exe output ===");

                // Assert - Verify the format version matches expectations
                esentOutput.Should().Contain(expectedFormatString, $"Esent database should have format version {expectedFormatString}");
                logger.LogInformation($"✓ Esent format version verified: {expectedFormatString}");
                logger.LogInformation("============== GeomService test completed successfully ==============");
            }
            finally
            {
                // Cleanup - Delete temp file and associated .jfm file
                Cleanup(tempFileName, logger);

                // Also clean up the .jfm file if it exists (created by Esent)
                var jfmFile = Path.ChangeExtension(tempFileName, ".jfm");
                Cleanup(jfmFile, logger);
            }
        }

        private static void Cleanup(string tempFileName, ILogger<GeomServiceConfigurationTests>? logger)
        {
            if (File.Exists(tempFileName))
            {
                try
                {
                    File.Delete(tempFileName);
                }
                catch (Exception ex)
                {
                    logger?.LogWarning($"Failed to cleanup temp file: {ex.Message}");
                }
            }
        }

        private void ReportProgress(int percentProgress, object? userState)
        {
            _output.WriteLine($"{percentProgress}% {userState}");
        }
    }
}

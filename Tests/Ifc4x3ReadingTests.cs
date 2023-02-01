using FluentAssertions;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Configuration;
using Xbim.Ifc;
using Xbim.Ifc4x3;
using Xbim.IO.Memory;
using Xunit;

namespace Xbim.Essentials.Tests
{
    [Collection(nameof(xUnitBootstrap))]
    public class Ifc4x3ReadingTests
    {
            
        [Theory]
        [InlineData(@"TestFiles\IFC4x3\DirectrixDerivedReferenceSweptAreaSolid-1.ifc")]
        [InlineData(@"TestFiles\IFC4x3\DirectrixDerivedReferenceSweptAreaSolid-2.ifc")]
        [InlineData(@"TestFiles\IFC4x3\FixedReferenceSweptAreaSolid-1.ifc")]
        [InlineData(@"TestFiles\IFC4x3\Header example.ifc")]
        [InlineData(@"TestFiles\IFC4x3\SectionedSolidHorizontal-1.ifc")]
        [InlineData(@"TestFiles\IFC4x3\SectionedSolidHorizontal-2.ifc")]
        [InlineData(@"TestFiles\IFC4x3\SectionedSolidHorizontal-3.ifc")]
        [InlineData(@"TestFiles\IFC4x3\SectionedSolidHorizontal-4.ifc")]
        [InlineData(@"TestFiles\IFC4x3\SectionedSolidHorizontal-5.ifc")]
        [InlineData(@"TestFiles\IFC4x3\SectionedSolidHorizontal-6.ifc")]
        [InlineData(@"TestFiles\IFC4x3\test0.ifc")]
        [InlineData(@"TestFiles\IFC4x3\test1.ifc")]
        [InlineData(@"TestFiles\IFC4x3\test2.ifc")]
        public void CanParseSampleFiles(string file)
        {
            var config = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.Debug()
                .CreateLogger();
            var logger = (new LoggerFactory()).AddSerilog(config).CreateLogger(typeof(IModel));

            using var stream = File.OpenRead(file);
            var model = new MemoryModel(new EntityFactoryIfc4x3Add1(), logger)
            {
                AllowMissingReferences = false
            };

            var errors = model.LoadStep21(stream, stream.Length);
            errors.Should().Be(0, "There should be no errors.");
        }

        [Theory]
        [InlineData(@"TestFiles\IFC4x3\DirectrixDerivedReferenceSweptAreaSolid-1.ifc")]
        [InlineData(@"TestFiles\IFC4x3\DirectrixDerivedReferenceSweptAreaSolid-2.ifc")]
        [InlineData(@"TestFiles\IFC4x3\FixedReferenceSweptAreaSolid-1.ifc")]
        [InlineData(@"TestFiles\IFC4x3\Header example.ifc")]
        [InlineData(@"TestFiles\IFC4x3\SectionedSolidHorizontal-1.ifc")]
        [InlineData(@"TestFiles\IFC4x3\SectionedSolidHorizontal-2.ifc")]
        [InlineData(@"TestFiles\IFC4x3\SectionedSolidHorizontal-3.ifc")]
        [InlineData(@"TestFiles\IFC4x3\SectionedSolidHorizontal-4.ifc")]
        [InlineData(@"TestFiles\IFC4x3\SectionedSolidHorizontal-5.ifc")]
        [InlineData(@"TestFiles\IFC4x3\SectionedSolidHorizontal-6.ifc")]
        [InlineData(@"TestFiles\IFC4x3\test0.ifc")]
        [InlineData(@"TestFiles\IFC4x3\test1.ifc")]
        [InlineData(@"TestFiles\IFC4x3\test2.ifc")]
        public void IfcStoreCanOpenSampleFiles(string file)
        {
            var config = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.Debug()
                .CreateLogger();
            var logger = (new LoggerFactory()).AddSerilog(config).CreateLogger(typeof(IModel));

       
            using var model = IfcStore.Open(file);

            model.Should().NotBeNull();
        }

        [Theory]
        [InlineData(@"TestFiles\IFC4x3\test2.ifc")]
        public void Ifc4_interfaces_can_be_used_to_read_IFC4x3(string file)
        {
            var config = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.Debug()
                .CreateLogger();
            var logger = (new LoggerFactory()).AddSerilog(config).CreateLogger(typeof(IModel));

            using var model = IfcStore.Open(file);
            var productsIfc4x3 = model.Instances.OfType<Ifc4x3.Kernel.IfcProduct>().Count();
            var productsIfc4 = model.Instances.OfType<Ifc4.Interfaces.IIfcProduct>().Count();
            
            productsIfc4x3.Should().BeGreaterThan(0);
            productsIfc4x3.Should().Be(productsIfc4);

        }
    }
}

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4x3;
using Xbim.IO.Esent;
using Xbim.IO.Memory;
using Xunit;
using Xunit.Abstractions;

namespace Xbim.Essentials.Tests
{
    [Collection(nameof(xUnitBootstrap))]
    public class Ifc4x3ReadingTests : TestBase
    {
            

        [Theory]
        [InlineData(@"TestFiles\IFC4x3_ADD2\basin-advanced-brep.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\basin-faceted-brep.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\basin-tessellation.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\bath-csg-solid.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-curved-i-shape-tessellated.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-extruded-solid.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-parametric-cross-section.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-revolved-solid.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-straight-i-shape-tessellated.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-varying-cardinal-points.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-varying-extrusion-paths.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-varying-profiles.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\brep-model.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\column-extruded-solid.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\column-straight-rectangle-tessellation.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\construction-scheduling-task.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\csg-primitive.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\cube-advanced-brep.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\curve-parameters-in-degrees.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\curve-parameters-in-radians.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\extruded-solid.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\fixed-reference-swept-area-solid.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\geographic-referencing-gk.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\geographic-referencing-rigid-operation.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\geographic-referencing-utm.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\grid-placement.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\linear-placement-of-signal.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\mapped-shape-with-multiple-items.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\mapped-shape-with-transformation.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\mapped-shape-without-transformation.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\polygonal-face-tessellation.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\reinforcing-assembly.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\reinforcing-stirrup.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\sectioned-solid-horizontal.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\slab-extruded-solid.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\slab-openings.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\slab-tessellated-unique-vertices.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\structural-curve-member.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\surface-model.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\tessellation-with-blob-texture.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\tessellation-with-image-texture.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\tessellation-with-individual-colors.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\tessellation-with-pixel-texture.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\triangulated-item.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\wall-extruded-solid.ifc")]
        public void CanParseSampleFiles(string file)
        {
            var loggerFactory = new LoggerFactory();
            var config = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.Debug()
                .CreateLogger();

            loggerFactory.AddSerilog(config);

            using var stream = File.OpenRead(file);
            var model = new MemoryModel(new EntityFactoryIfc4x3Add2(), loggerFactory)
            {
                AllowMissingReferences = false
            };

            var errors = model.LoadStep21(stream, stream.Length);
            errors.Should().Be(0, "There should be no errors.");
        }

        [Theory]
        [InlineData(@"TestFiles\IFC4x3_ADD2\basin-advanced-brep.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\basin-faceted-brep.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\basin-tessellation.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\bath-csg-solid.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-curved-i-shape-tessellated.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-extruded-solid.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-parametric-cross-section.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-revolved-solid.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-straight-i-shape-tessellated.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-varying-cardinal-points.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-varying-extrusion-paths.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\beam-varying-profiles.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\brep-model.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\column-extruded-solid.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\column-straight-rectangle-tessellation.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\construction-scheduling-task.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\csg-primitive.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\cube-advanced-brep.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\curve-parameters-in-degrees.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\curve-parameters-in-radians.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\extruded-solid.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\fixed-reference-swept-area-solid.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\geographic-referencing-gk.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\geographic-referencing-rigid-operation.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\geographic-referencing-utm.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\grid-placement.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\linear-placement-of-signal.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\mapped-shape-with-multiple-items.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\mapped-shape-with-transformation.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\mapped-shape-without-transformation.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\polygonal-face-tessellation.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\reinforcing-assembly.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\reinforcing-stirrup.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\sectioned-solid-horizontal.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\slab-extruded-solid.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\slab-openings.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\slab-tessellated-unique-vertices.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\structural-curve-member.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\surface-model.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\tessellation-with-blob-texture.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\tessellation-with-image-texture.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\tessellation-with-individual-colors.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\tessellation-with-pixel-texture.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\triangulated-item.ifc")]
        [InlineData(@"TestFiles\IFC4x3_ADD2\wall-extruded-solid.ifc")]
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

        [Theory (DisplayName = nameof(Ifc4_interfaces_can_be_used_to_read_IFC4x3)) ]
        [InlineData(@"TestFiles\IFC4x3\test1.ifc")]
        [InlineData(@"TestFiles\IFC4x3\test2.ifc")]
        public void Ifc4_interfaces_can_be_used_to_read_IFC4x3(string file)
        {
            using var model = IfcStore.Open(file);
            var productsIfc4x3 = model.Instances.OfType<Ifc4x3.Kernel.IfcProduct>().Count();
            var productsIfc4 = model.Instances.OfType<Ifc4.Interfaces.IIfcProduct>().Count();
            productsIfc4x3.Should().BeGreaterThan(0);
            productsIfc4x3.Should().Be(productsIfc4);
            var cnt = 0;
            foreach (var item in model.Instances.OfType<IIfcProduct>())
            {
                cnt++;
            }
            cnt.Should().Be(productsIfc4);

            // the following was previosly failing in EsentModelProvider because
            // abstract classes exist in the test files used, which generated a
            // behaviour different from MemoryModelProvider
            //
            // changes added in PersistedEntityInstanceCache should address the issue
            //
            var db = Guid.NewGuid().ToString() + ".xbim";
            var provider = new EsentModelProvider { DatabaseFileName = db };
            var schema = provider.GetXbimSchemaVersion(file);
            using (var eseModel = provider.Open(file, schema))
            {
                var esentProductCount = eseModel.Instances.OfType<Ifc4x3.Kernel.IfcProduct>().Count();
                esentProductCount.Should().Be(productsIfc4);
                var esentInterfaceCount = eseModel.Instances.OfType<Ifc4.Interfaces.IIfcProduct>().Count();
                esentInterfaceCount.Should().Be(productsIfc4);
                provider.Close(model);
            }
            File.Delete(db);
        }

        [Fact]
        public void Can_create_IFC4x3_model_from_scratch()
        {
            using var model = new MemoryModel(new EntityFactoryIfc4x3Add2());
            using var txn = model.BeginTransaction("Creation");

            var i = model.Instances;
            i.New<Ifc4x3.SharedBldgElements.IfcWall>(w => w.Name = "First wall");

            txn.Commit();

            using var stream = File.Create(nameof(Can_create_IFC4x3_model_from_scratch) + ".ifc");
            model.SaveAsIfc(stream);

            using var store = IfcStore.Create(Common.Step21.XbimSchemaVersion.Ifc4x3, IO.XbimStoreType.InMemoryModel);
            using var storeTxn = store.BeginTransaction();
            store.Instances.New<Ifc4x3.SharedBldgElements.IfcWall>(w => w.Name = "Second wall");
            store.SaveAs(nameof(Can_create_IFC4x3_model_from_scratch) + "_store.ifc");
        }
    }
}

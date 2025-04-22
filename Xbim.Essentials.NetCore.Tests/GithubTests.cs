using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xbim.Ifc4;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MaterialResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.IO.Memory;
using Xunit;
using Xunit.Abstractions;

namespace Xbim.Essentials.NetCore.Tests
{
   
    public class GithubTests
    {
        private readonly ITestOutputHelper outputHelper;

        public GithubTests(ITestOutputHelper outputHelper)
        {
            this.outputHelper = outputHelper;
        }

        [Fact]
        public void Issue_603_StyledItem_Ifc2x3Native()
        {
            using var model = new MemoryModel(new Ifc2x3.EntityFactoryIfc2x3());

            Ifc2x3.PresentationAppearanceResource.IfcStyledItem nativeStyledItem = CreateIfc2x3StyledItem(model);

            nativeStyledItem.Styles.Should().HaveCount(1);
            var firstStyle = nativeStyledItem.Styles.First();
            firstStyle.Should().NotBeNull();
            firstStyle.Should().BeOfType<Ifc2x3.PresentationAppearanceResource.IfcPresentationStyleAssignment>();
            // and contents of assignment are the Style
            firstStyle.Styles.Should().HaveCount(1);
            firstStyle.Styles.First().Should().BeOfType<Ifc2x3.PresentationAppearanceResource.IfcSurfaceStyle>();

            IIfcStyledItem si = nativeStyledItem;

            si.Styles.First().Should().BeAssignableTo<IIfcPresentationStyleAssignment>();
        }

        [Fact]
        public void Issue_603_StyledItem_Ifc4x3Native()
        {
            using var model = new MemoryModel(new Ifc4x3.EntityFactoryIfc4x3Add2());

            Ifc4x3.PresentationAppearanceResource.IfcStyledItem nativeStyledItem = CreateIfc4x3StyledItem(model);

            nativeStyledItem.Styles.Should().HaveCount(1);
            var firstStyle = nativeStyledItem.Styles.First();
            firstStyle.Should().NotBeNull();
            firstStyle.Should().BeOfType<Ifc4x3.PresentationAppearanceResource.IfcSurfaceStyle>();

            IIfcStyledItem si = nativeStyledItem;

            si.Styles.First().Should().BeAssignableTo<IIfcSurfaceStyle>();
        }

        [Fact]
        public void Issue_603_StyledItem_Ifc2x3()
        {
            using var model = new MemoryModel(new Ifc2x3.EntityFactoryIfc2x3());

            IIfcStyledItem si = CreateStyledItemViaInterfaces(model);

            var nativeStyledItem = si as Ifc2x3.PresentationAppearanceResource.IfcStyledItem;

            nativeStyledItem.Styles.Should().HaveCount(1);
            var firstStyle = nativeStyledItem.Styles.First();
            firstStyle.Should().NotBeNull();
            firstStyle.Should().BeAssignableTo<IIfcPresentationStyleAssignment>();
            // Styles are accessed indirectly via PresentationStyleAssignment's Styles.
            firstStyle.Styles.Should().HaveCount(1);
            firstStyle.Styles.First().Should().BeAssignableTo<IIfcSurfaceStyle>();
        }


        [Fact]
        public void Issue_603_StyledItem_Ifc4()
        {
            using var model = new MemoryModel(new Ifc4.EntityFactoryIfc4());

            IIfcStyledItem si = CreateStyledItemViaInterfaces(model);

            var nativeStyledItem = si as Ifc4.PresentationAppearanceResource.IfcStyledItem;

            nativeStyledItem.Styles.Should().HaveCount(1);
            var firstStyle = nativeStyledItem.Styles.First();
            firstStyle.Should().NotBeNull();
            firstStyle.Should().BeAssignableTo<IIfcSurfaceStyle>();
        }

        [Fact]
        public void Issue_603_StyledItem_Ifc4x3()
        {
            using var model = new MemoryModel(new Ifc4x3.EntityFactoryIfc4x3Add2());

            IIfcStyledItem si = CreateStyledItemViaInterfaces(model);

            var nativeStyledItem = si as Ifc4x3.PresentationAppearanceResource.IfcStyledItem;

            nativeStyledItem.Styles.Should().HaveCount(1);
            var firstStyle = nativeStyledItem.Styles.First();
            firstStyle.Should().NotBeNull();
            firstStyle.Should().BeAssignableTo<IIfcSurfaceStyle>();
        }

        // Use the IFC2x3 IIfcPresentationStyleAssignment explicitly (vs implicit conversion from IFC4+ IfcPresentationStyles)
        [Fact]
        public void Issue_603_StyledItem_Ifc2x3_BackwardCompatibility()
        {
            using var model = new MemoryModel(new Ifc2x3.EntityFactoryIfc2x3());

            // Explicitly create the IfcPresentationStyleAssignment vs implicit via IFC4 interfaces
            IIfcStyledItem si = CreateStyledItemViaInterfaces(model, true);

            var nativeStyledItem = si as Ifc2x3.PresentationAppearanceResource.IfcStyledItem;

            nativeStyledItem.Styles.Should().HaveCount(1);
            var firstStyle = nativeStyledItem.Styles.First();
            firstStyle.Should().NotBeNull();
            firstStyle.Should().BeAssignableTo<IIfcPresentationStyleAssignment>();
            firstStyle.Styles.Should().HaveCount(1);
            firstStyle.Styles.First().Should().BeAssignableTo<IIfcSurfaceStyle>();
        }

        [Fact]
        public void Issue_620_StoreReleasesResources()
        {
            var filePath = @"TestFiles/Samplehouse4.ifc";
            var baseMemory = Process.GetCurrentProcess().PrivateMemorySize64;

            var warmupBytes = LoadModel(filePath);
            var memoryDelta = warmupBytes - baseMemory;
            outputHelper.WriteLine("Warmup: {0:N0} => {1:N0} bytes = {2:N0} Δ bytes", baseMemory, warmupBytes, memoryDelta);
            
            var sw = new Stopwatch();
            sw.Start();
            var baselineMs = sw.ElapsedMilliseconds;
            baseMemory = Process.GetCurrentProcess().PrivateMemorySize64;
            for (int i =0; i < 25; i++)
            {
                var currentMemory = LoadModel(filePath);
                memoryDelta = currentMemory - baseMemory;
                var timeMs = sw.ElapsedMilliseconds;

                outputHelper.WriteLine("{0}: {1:N0} Δ bytes => {2:N0} in {3:N0}ms", i, memoryDelta, currentMemory, (timeMs - baselineMs));
                baselineMs = sw.ElapsedMilliseconds;
            }
            memoryDelta.Should().BeLessThan(10_000_000);
        }

        private long LoadModel(string filePath)
        {
            GC.Collect();
            var baselineBytes = Process.GetCurrentProcess().PrivateMemorySize64;// Environment.WorkingSet;
            using (var model = MemoryModel.OpenRead(filePath))
            {
                var entities = model.Instances.OfType<IIfcSlab>().ToList();
                // Simulated serialisation
                var serialised = entities.Select(e => e.ToString()).ToList();
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var latestMemory = Process.GetCurrentProcess().PrivateMemorySize64;
            //outputHelper.WriteLine("  {0}: {1}", baselineBytes, latestMemory);
            return latestMemory;
        }

        private IIfcStyledItem CreateStyledItemViaInterfaces(MemoryModel model, bool useStyleAssignment = false)
        {
            using var txn = model.BeginTransaction("Test");
            var factory = new EntityCreator(model);

            var surfaceStyle = factory.SurfaceStyle(style =>
            {
                var defaultStyle = factory.SurfaceStyleShading(l =>
                {
                    l.SurfaceColour = factory.ColourRgb(rgb =>
                    {
                        rgb.Red = 1.0;
                        rgb.Green = 0.0;
                        rgb.Blue = 0.0;
                    });
                });

                style.Side = IfcSurfaceSide.BOTH;
                style.Styles.Add(defaultStyle);
            });
            var si = factory.StyledItem(styleItem =>
            {
                if(useStyleAssignment && model.SchemaVersion == Common.Step21.XbimSchemaVersion.Ifc2X3)
                {
                    var assignment = model.Instances.New<Ifc2x3.PresentationAppearanceResource.IfcPresentationStyleAssignment>(a =>
                    {
                        a.Styles.Add((Ifc2x3.PresentationAppearanceResource.IfcSurfaceStyle)surfaceStyle);
                    });
                    styleItem.Styles.Add(assignment);
                }
                else
                {
                    // IFC 4/4.3 way
                    styleItem.Styles.Add(surfaceStyle);
                }
            });
            txn.Commit();
            return si;
        }

        private Ifc2x3.PresentationAppearanceResource.IfcStyledItem CreateIfc2x3StyledItem(MemoryModel model)
        {
            using var txn = model.BeginTransaction("Test");
            

            var surfaceStyle = model.Instances.New<Ifc2x3.PresentationAppearanceResource.IfcSurfaceStyle>(style =>
            {
                var defaultStyle = model.Instances.New<Ifc2x3.PresentationAppearanceResource.IfcSurfaceStyleShading>(l =>
                {
                    l.SurfaceColour = model.Instances.New<Ifc2x3.PresentationResource.IfcColourRgb>(rgb =>
                    {
                        rgb.Red = 1.0;
                        rgb.Green = 0.0;
                        rgb.Blue = 0.0;
                    });
                });

                style.Styles.Add(defaultStyle);
            });
            var si = model.Instances.New<Ifc2x3.PresentationAppearanceResource.IfcStyledItem>(styleItem =>
            {
                // IfcPresentationStyleAssignment Required in IFC2x3, Optional in IFC4 and deprecated/removed in 4x3
                var presStyleAssignment = model.Instances.New<Ifc2x3.PresentationAppearanceResource.IfcPresentationStyleAssignment>(a =>
                    {
                        a.Styles.Add(surfaceStyle);
                    });
                styleItem.Styles.Add(presStyleAssignment);
            });
            txn.Commit();
            return si;
        }

        private Ifc4x3.PresentationAppearanceResource.IfcStyledItem CreateIfc4x3StyledItem(MemoryModel model)
        {
            using var txn = model.BeginTransaction("Test");


            var surfaceStyle = model.Instances.New<Ifc4x3.PresentationAppearanceResource.IfcSurfaceStyle>(style =>
            {
                var defaultStyle = model.Instances.New<Ifc4x3.PresentationAppearanceResource.IfcSurfaceStyleShading>(l =>
                {
                    l.SurfaceColour = model.Instances.New<Ifc4x3.PresentationAppearanceResource.IfcColourRgb>(rgb =>
                    {
                        rgb.Red = 1.0;
                        rgb.Green = 0.0;
                        rgb.Blue = 0.0;
                    });
                });

                style.Styles.Add(defaultStyle);
            });
            var si = model.Instances.New<Ifc4x3.PresentationAppearanceResource.IfcStyledItem>(styleItem =>
            {
                // IfcPresentationStyleAssignment deprecated/removed in 4x3
                //var presStyleAssignment = model.Instances.New<IfcPresentationStyleAssignment>(a =>
                //{
                //    a.Styles.Add(style);
                //});
                styleItem.Styles.Add(surfaceStyle);
            });
            txn.Commit();
            return si;
        }



        [Fact]
        public void GithubIssue_595_Fails_With_LinqSelectMany()
        {
            // In .net core only, a SelectMany<TSource, TResult> projection could produce a result containing null values when none were null
            // in the TSource set. Only the last projected Items would remain in the TResults collection. When calling ToArray/ToList.
            // This was because when using Ifc4 (cross-schema) interfaces, and projecting and casting (to the IIfcProperty interface) in
            // Netcore SelectMany<T1,T2> this invokes List<T1>.AddRange which in turn called
            // xbim's ProxyItemsSet.CopyTo(T[] targetArray, int offset) which was incorrectly implemented


            using var model = new MemoryModel(new EntityFactoryIfc4());

            InitialiseIfc4Model(model);

            // The issues below disappear when using IfcMaterial directly here
            var imaterial = model.Instances.OfType<IIfcMaterial>().First();


            // ACT

            
            List<IIfcProperty> flattenedProperties = imaterial.HasProperties.SelectMany(a => a.Properties).ToList();

            // Assert

            flattenedProperties.Should().HaveCount(2);

            flattenedProperties.Should().NotContainNulls();
        }

     
        private static void InitialiseIfc4Model(MemoryModel model)
        {
            using (var tx = model.BeginTransaction("memory"))
            {
                var material = model.Instances.New<IfcMaterial>(m => m.Name = "TestMaterial");

                model.Instances.New<IfcMaterialProperties>(ps =>
                {
                    ps.Name = "A";
                    ps.Material = material;
                    ps.Properties.Add(
                        model.Instances.New<IfcPropertySingleValue>(pv =>
                        {
                            pv.Name = "A1";
                            pv.NominalValue = new IfcMolecularWeightMeasure(1);
                        })
                    );
                });

                model.Instances.New<IfcMaterialProperties>(ps =>
                {
                    ps.Name = "B";
                    ps.Material = material;
                    ps.Properties.Add(
                        model.Instances.New<IfcPropertySingleValue>(pv =>
                        {
                            pv.Name = "B1";
                            pv.NominalValue = new IfcThermalConductivityMeasure(2);
                        })
                    );
                });

                tx.Commit();
            }
        }
    }

    public static class Enumerable2
    {
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            foreach (TSource item in source)
            {
                foreach (TResult item2 in selector(item))
                {
                    yield return item2;
                }
            }
        }
    }

}

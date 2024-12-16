using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Model;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4x3;
using Xbim.Ifc4x3.GeometryResource;
using Xbim.Ifc4x3.MeasureResource;
using Xbim.Ifc4x3.ProductExtension;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class Ifc4x3Tests
    {

        [TestMethod]
        public void SpiralsShouldBe2D()
        {
            using (var model = MemoryModel.OpenRead("TestFiles\\IFC4x3_ADD2\\PlacmentOfSignal.ifc"))
            {
                var spiral = model.Instances.FirstOrDefault<IfcSpiral>() as IIfcCurve;

                spiral.Dim.Value.Should().Be(2);
            }
        }
        
        [TestMethod]
        public void SegmentedReferenceCurveShouldBe3D()
        {
            using (var model = MemoryModel.OpenRead("TestFiles\\IFC4x3_ADD2\\PlacmentOfSignal.ifc"))
            {
                var segmentedReferenceCurve = model.Instances.FirstOrDefault<IfcSegmentedReferenceCurve>() as IfcCurve;
                segmentedReferenceCurve.Dim.Value.Should().Be(3);
            }
        }
        
        [TestMethod]
        public void GradientCurveShouldBe3D()
        {
            using (var model = MemoryModel.OpenRead("TestFiles\\IFC4x3_ADD2\\PlacmentOfSignal.ifc"))
            {
                var segmentedReferenceCurve = model.Instances.FirstOrDefault<IfcGradientCurve>() as IfcCurve;
                segmentedReferenceCurve.Dim.Value.Should().Be(3);
            }
        }

        
        [TestMethod]
        public void Entity_types_should_be_unique()
        {
            var unique = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var types = typeof(Ifc4x3.EntityFactoryIfc4x3Add2).Assembly.GetTypes().Where(t => typeof(IPersist).IsAssignableFrom(t)).Select(t => t.Name);
            var duplicates = types.Where(t => !unique.Add(t)).ToList();

            Assert.AreEqual(0, duplicates.Count, $"Duplicated types: {string.Join(", ", duplicates)}");
        }

        [TestMethod]
        public void CreateSimpleIfc4x3File()
        {
            using (var model = new StepModel(new Ifc4x3.EntityFactoryIfc4x3Add2()))
            {
                var i = model.Instances;
                using (var txn = model.BeginTransaction("Sample creation"))
                {
                    var facility = i.New<IfcFacility>();

                    txn.Commit();
                }
                using (var file = File.Create("ifc4x3sample.ifc"))
                {
                    model.SaveAsIfc(file);
                }
            }
        }

        [TestMethod]
        public void ConvertLengthTest()
        {
            var value = new IfcLengthMeasure(20);
            var converted = value.ToIfc4();
            Assert.IsTrue(converted.GetType() == typeof(Ifc4.MeasureResource.IfcLengthMeasure));
            Assert.IsTrue((Ifc4.MeasureResource.IfcLengthMeasure)converted == new Ifc4.MeasureResource.IfcLengthMeasure(20));
        }

        [TestMethod]
        public void PointDimensionsImplemented()
        {
            using (var model = new StepModel(new Ifc4x3.EntityFactoryIfc4x3Add2()))
            {
                var i = model.Instances;
                using (var txn = model.BeginTransaction("Sample creation"))
                {
                    var point = i.New<IfcCartesianPoint>(c =>
                    {
                        c.X = 1;
                        c.Y = 1;
                        c.Z = 1;
                    });


                    point.Dim.Value.Should().Be(3, "There are three coordinates");

                    IIfcCartesianPoint ifc4point = point as IIfcCartesianPoint;
                    ifc4point.Dim.Value.Should().Be(3, "Ifc4 interface returns 3");

                    IfcPoint abstractPoint = point as IfcPoint;
                    abstractPoint.Dim.Value.Should().Be(3, "Ifc4x3 base Point returns 3");
                }
                
            }
        }

        [TestMethod]
        public void SurfaceDimensionsImplemented()
        {
            using (var model = new StepModel(new Ifc4x3.EntityFactoryIfc4x3Add2()))
            {
                var i = model.Instances;
                using (var txn = model.BeginTransaction("Sample creation"))
                {

                    var curve = i.New<IfcLine>(e =>
                    {
                        e.Pnt = i.New<IfcCartesianPoint>(c =>
                        {
                            c.SetXY(0, 0);
                        });

                        e.Dir = i.New<IfcVector>(c =>
                        {
                            c.Orientation = i.New<IfcDirection>(c => c.SetXY(0, 1));
                        });

                    });
                    var segment = i.New<IfcCurveSegment>(c =>
                    {
                        c.ParentCurve = curve;
                    });


                    segment.Dim.Value.Should().Be(2, "There are two coordinates");


                    IfcSegment abstractSegment = segment as IfcSegment;
                    abstractSegment.Dim.Value.Should().Be(2, "Ifc4x3 base Segment returns 3");
                }

            }
        }

        [TestMethod]
        public void CanCreateEntitiesWithFactory()
        {
            using var model = new MemoryModel(new EntityFactoryIfc4x3Add2());
            using var txn = model.BeginTransaction("Creation");

            var c = new EntityCreator(model);
            c.Wall(w => w.Name = "First wall");

            txn.Commit();
        }

        [TestMethod]
        public void IfcLengthMeasureSubClassesCanUsed()
        {
            using (var model = MemoryModel.OpenRead("TestFiles\\IFC4x3\\SectionedSolidHorizontal-1.ifc"))
            {

                
                var curveSegment = model.Instances.FirstOrDefault<IfcCurveSegment>();

                curveSegment.SegmentStart.Should().BeOfType<IfcNonNegativeLengthMeasure>();
                var start = (IIfcLengthMeasure)curveSegment.SegmentStart;
                var length = (IIfcLengthMeasure)curveSegment.SegmentLength;

                start.Value.Should().Be(0);
                length.Value.Should().Be(400);
            }

        }


    }
}

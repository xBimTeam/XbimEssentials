using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Common.Metadata;
using Xbim.Common.Model;
using Xbim.Common.Step21;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class CommonTests
    {
        [TestMethod]
        public void PackedNormalRoundTripTest()
        {
            var tests = new[]
            {
                new XbimVector3D(0.0749087609620539, 0.264167604633194, 0.961563390626687),
                new XbimVector3D(-0.0535755113215756, 0.316639902069201, 0.947031592400295),
                new XbimVector3D(-0.0403690470743153, -0.0845001599207948, 0.995605375142015),
                new XbimVector3D(-0.170389413996118, 0.321003309980549, 0.931624560957681)
            };

            foreach (var vec in tests)
            {
                Trace.WriteLine(vec);

                var pack = new XbimPackedNormal(vec);
                var roundVec = pack.Normal;
                Trace.WriteLine(roundVec);

                var a = vec.CrossProduct(roundVec);
                var x = Math.Abs(a.Length);
                var y = vec.DotProduct(roundVec);
                var angle = Math.Atan2(x, y);
                if (angle > 0.1)
                    Trace.WriteLine($"vector: {vec}, angle: { angle * 180.0 / Math.PI:F3}");
                Assert.IsTrue(angle < 0.13);
            }


        }

        [TestMethod]
        public void StepFileHeaderVersionTest()
        {
            var model = new StepModel(new Ifc4.EntityFactoryIfc4());
            var header = new StepFileHeader(StepFileHeader.HeaderCreationMode.InitWithXbimDefaults, model);
            Assert.IsTrue(header.FileName.OriginatingSystem == model.GetType().GetTypeInfo().Assembly.GetName().Name);
            Assert.IsTrue(header.FileName.PreprocessorVersion == string.Format("Processor version {0}", model.GetType().GetTypeInfo().Assembly.GetName().Version));
        }

        [TestMethod]
        [DeploymentItem("TestFiles\\4walls1floorSite.ifc")]
        public void Can_skip_entities_while_parsing()
        {
            using (var strm = File.OpenRead(@"4walls1floorSite.ifc"))
            {

                var ifc2x3MetaData = ExpressMetaData.GetMetadata((new Xbim.Ifc2x3.EntityFactoryIfc2x3()).GetType().GetTypeInfo().Module);
                var ifc4MetaData = ExpressMetaData.GetMetadata((new Xbim.Ifc4.EntityFactoryIfc4()).GetType().GetTypeInfo().Module);
                var allTypes = new HashSet<string>(
                    ifc2x3MetaData.Types().Where(et => typeof(IPersistEntity).IsAssignableFrom(et.Type) && !et.Type.IsAbstract ).Select(et => et.ExpressNameUpper)
                    .Concat(ifc2x3MetaData.Types().Where(et => typeof(IPersistEntity).IsAssignableFrom(et.Type) && !et.Type.IsAbstract).Select(et => et.ExpressNameUpper)));
                var requiredTypes = new HashSet<string>();
                foreach (var metadata in new[] { ifc2x3MetaData })
                {
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcProject))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcAsset))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcSystem)).Where(t => !typeof(IIfcStructuralAnalysisModel).IsAssignableFrom(t.Type))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcActor))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcElement)).Where(t => !typeof(IIfcFeatureElement).IsAssignableFrom(t.Type)
                    && !typeof(IIfcVirtualElement).IsAssignableFrom(t.Type))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcSpatialElement))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcProxy))) requiredTypes.Add(t.ExpressNameUpper);

                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcTypeProduct))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcPropertySetDefinitionSelect))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcRelDefinesByProperties))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcSimpleProperty))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcElementQuantity))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcPhysicalSimpleQuantity))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcRelDefinesByType))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcUnitAssignment))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcNamedUnit))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcMeasureWithUnit))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcDimensionalExponents))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcRelAssociatesClassification))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcClassificationReference))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcClassification))) requiredTypes.Add(t.ExpressNameUpper);
                    // foreach (var t in metadata.ExpressTypesImplementing(typeof(Xbim.Ifc2x3.DateTimeResource.IfcCalendarDate))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcActorSelect))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcAddress))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcApplication))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcActorRole))) requiredTypes.Add(t.ExpressNameUpper);
                    foreach (var t in metadata.ExpressTypesImplementing(typeof(IIfcDocumentSelect))) requiredTypes.Add(t.ExpressNameUpper);
                }
                var unwantedTypes = allTypes.Except(requiredTypes);
                var unwanted = new HashSet<string>(unwantedTypes);
                using (var mm = MemoryModel.OpenReadStep21(strm, null, null, unwantedTypes.ToList()))
                {
                    foreach(var instance in mm.Instances)
                    {
                        Assert.IsFalse(unwanted.Contains(instance.ExpressType.ExpressNameUpper));
                    }
                }
            }
        }
    }
}

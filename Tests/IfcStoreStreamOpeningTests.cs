using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using System.IO;
using Xbim.IO;
using Xbim.Common.Step21;
using Xbim.Common.Exceptions;
using FluentAssertions;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class IfcStoreStreamOpeningTests
    {
        [TestMethod]
        public void OpenStreamTest()
        {
            const string ifcPath = "TestFiles\\4walls1floorSite.ifc";
            const string xmlPath = "TestFiles\\OpenStreamTest.ifcxml";
            const string zipPath = "TestFiles\\OpenStreamTest.ifczip";
            const string xbimPath = "TestFiles\\OpenStreamTest.xbim";

            var instCount = 0L;
            //create file types
            using (var store = IfcStore.Open(ifcPath))
            {
                instCount = store.Instances.Count;
                store.SaveAs(xmlPath);
                store.SaveAs(zipPath);
                store.SaveAs(xbimPath);
                store.Close();
            }

            //Esent, IFC
            using (var ifc = File.Open(ifcPath, FileMode.Open))
            {
                using (var model = IfcStore.Open(ifc, StorageType.Ifc, XbimSchemaVersion.Ifc2X3, XbimModelType.EsentModel))
                {
                    Assert.AreEqual(instCount, model.Instances.Count);
                    model.Close();
                }
            }
            //Memory, IFC
            using (var ifc = File.Open(ifcPath, FileMode.Open))
            {
                using (var model = IfcStore.Open(ifc, StorageType.Ifc, XbimSchemaVersion.Ifc2X3, XbimModelType.MemoryModel))
                {
                    Assert.AreEqual(instCount, model.Instances.Count);
                    model.Close();
                }
            }
            //Esent, XML
            using (var xml = File.Open(xmlPath, FileMode.Open))
            {
                using (var model = IfcStore.Open(xml, StorageType.IfcXml, XbimSchemaVersion.Ifc2X3, XbimModelType.EsentModel))
                {
                    Assert.AreEqual(instCount, model.Instances.Count);
                    model.Close();
                }
            }
            //Memory, XML
            using (var xml = File.Open(xmlPath, FileMode.Open))
            {
                using (var model = IfcStore.Open(xml, StorageType.IfcXml, XbimSchemaVersion.Ifc2X3, XbimModelType.MemoryModel))
                {
                    Assert.AreEqual(instCount, model.Instances.Count);
                    model.Close();
                }
            }
            //Esent, ZIP
            using (var zip = File.Open(zipPath, FileMode.Open))
            {
                using (var model = IfcStore.Open(zip, StorageType.IfcZip, XbimSchemaVersion.Ifc2X3, XbimModelType.EsentModel))
                {
                    Assert.AreEqual(instCount, model.Instances.Count);
                    model.Close();
                }
            }
            //Memory, ZIP
            using (var zip = File.Open(zipPath, FileMode.Open))
            {
                using (var model = IfcStore.Open(zip, StorageType.IfcZip, XbimSchemaVersion.Ifc2X3, XbimModelType.MemoryModel))
                {
                    Assert.AreEqual(instCount, model.Instances.Count);
                    model.Close();
                }
            }
            //Esent, XBIM
            using (var xbim = File.Open(xbimPath, FileMode.Open))
            {
                using (var model = IfcStore.Open(xbim, StorageType.Xbim, XbimSchemaVersion.Ifc2X3, XbimModelType.EsentModel))
                {
                    Assert.AreEqual(instCount, model.Instances.Count);
                    model.Close();
                }
            }
        }

        [TestMethod]
        public void CanOpenStreamInferringSchemaFromNonSeekableIfc2x3()
        {
            AssertCanOpenFromStream(@"TestFiles/SmallModelIfc2x3.ifc", XbimSchemaVersion.Ifc2X3);
        }

        [TestMethod]
        public void CanOpenStreamInferringSchemaFromNonSeekableIfc4()
        {
            AssertCanOpenFromStream(@"TestFiles/SampleHouse4.ifc", XbimSchemaVersion.Ifc4);
        }

        [TestMethod]
        public void CanOpenStreamInferringSchemaFromNonSeekableIfc4x3()
        {
            AssertCanOpenFromStream(@"TestFiles/IFC4x3/test1.ifc", XbimSchemaVersion.Ifc4x3);
        }

        private void AssertCanOpenFromStream(string ifcPath,  XbimSchemaVersion expectedVersion)
        {
            using var fileStream = File.Open(ifcPath, FileMode.Open);
            using var nonseekableStream = new NonSeekableStream(fileStream);

            using var model = IfcStore.Open(nonseekableStream, StorageType.Ifc, XbimModelType.MemoryModel);
            Assert.AreNotEqual(0, model.Instances.Count);
            Assert.AreEqual(expectedVersion, model.SchemaVersion);
        }


        [TestMethod]
        public void SchemaInferenceThrowsWhenBufferOverflowed()
        {
            string ifcPath = @"TestFiles/SmallModelIfc2x3.ifc";
            using var fileStream = File.Open(ifcPath, FileMode.Open);
            using var nonseekableStream = new NonSeekableStream(fileStream);
            
            var bufferSize = 1; // 1 byte buffer
            var ex = Assert.ThrowsException<XbimException>(() => IfcStore.Open(nonseekableStream, StorageType.Ifc, XbimModelType.MemoryModel, streamBufferSize: bufferSize));
            
            ex.Message.Should().StartWith("Cannot infer Schema for this model since the header size (");
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using System.IO;
using Xbim.IO;
using Xbim.Common.Step21;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class IfcStoreStreamOpeningTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles\\4walls1floorSite.ifc")]
        public void OpenStreamTest()
        {
            const string ifcPath = "4walls1floorSite.ifc";
            const string xmlPath = "OpenStreamTest.ifcxml";
            const string zipPath = "OpenStreamTest.ifczip";
            const string xbimPath = "OpenStreamTest.xbim";

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
                using (var model = IfcStore.Open(ifc, IfcStorageType.Ifc, IfcSchemaVersion.Ifc2X3, XbimModelType.EsentModel))
                {
                    Assert.AreEqual(instCount, model.Instances.Count);
                    model.Close();
                }
            }
            //Memory, IFC
            using (var ifc = File.Open(ifcPath, FileMode.Open))
            {
                using (var model = IfcStore.Open(ifc, IfcStorageType.Ifc, IfcSchemaVersion.Ifc2X3, XbimModelType.MemoryModel))
                {
                    Assert.AreEqual(instCount, model.Instances.Count);
                    model.Close();
                }
            }
            //Esent, XML
            using (var xml = File.Open(xmlPath, FileMode.Open))
            {
                using (var model = IfcStore.Open(xml, IfcStorageType.IfcXml, IfcSchemaVersion.Ifc2X3, XbimModelType.EsentModel))
                {
                    Assert.AreEqual(instCount, model.Instances.Count);
                    model.Close();
                }
            }
            //Memory, XML
            using (var xml = File.Open(xmlPath, FileMode.Open))
            {
                using (var model = IfcStore.Open(xml, IfcStorageType.IfcXml, IfcSchemaVersion.Ifc2X3, XbimModelType.MemoryModel))
                {
                    Assert.AreEqual(instCount, model.Instances.Count);
                    model.Close();
                }
            }
            //Esent, ZIP
            using (var zip = File.Open(zipPath, FileMode.Open))
            {
                using (var model = IfcStore.Open(zip, IfcStorageType.IfcZip, IfcSchemaVersion.Ifc2X3, XbimModelType.EsentModel))
                {
                    Assert.AreEqual(instCount, model.Instances.Count);
                    model.Close();
                }
            }
            //Memory, ZIP
            using (var zip = File.Open(zipPath, FileMode.Open))
            {
                using (var model = IfcStore.Open(zip, IfcStorageType.IfcZip, IfcSchemaVersion.Ifc2X3, XbimModelType.MemoryModel))
                {
                    Assert.AreEqual(instCount, model.Instances.Count);
                    model.Close();
                }
            }
            //Esent, XBIM
            using (var xbim = File.Open(xbimPath, FileMode.Open))
            {
                using (var model = IfcStore.Open(xbim, IfcStorageType.Xbim, IfcSchemaVersion.Ifc2X3, XbimModelType.EsentModel))
                {
                    Assert.AreEqual(instCount, model.Instances.Count);
                    model.Close();
                }
            }
        }
    }
}

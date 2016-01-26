using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.IO.TableStore;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class IFC4TableStorageTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles\\SampleHouse4.ifc")]
        public void Ifc4TableStorageTest()
        {
            var model = IfcStore.Open("SampleHouse4.ifc");
            var mapping = ModelMapping.Load(Properties.Resources.IFC4SampleMapping);
            mapping.Init(model.Metadata);

            var w = new Stopwatch();
            w.Start();
            var storage = new TableStore(model, mapping);
            storage.Store("..\\..\\SampleHouseFromIfc.xlsx");
            w.Stop();
            //Debug.WriteLine(@"{0}ms to store the data as a table.", w.ElapsedMilliseconds);
            Trace.WriteLine(string.Format(@"{0}ms to store the data as a table.", w.ElapsedMilliseconds));
        }
    }
}

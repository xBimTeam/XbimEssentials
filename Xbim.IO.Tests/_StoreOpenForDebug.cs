using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class StoreOpenForDebug
    {
        [TestMethod]
        public void JustOpenOneFile()
        {
            // note: this class can be used to debug suspicious local files. It's not a stable test.
            var filename = "C:\\Users\\Claudio\\OneDrive\\IfcArchive\\Andre\\HaifaBridgeFixed_Classified_WithAxes_WithSystems_WithSpaces.ifc";
            if (!File.Exists(filename))
                return;

            // loading in memory
            using (var store = IfcStore.Open(filename))
            {
                store.Close();
            }

            // loading in esent
            using (var store = IfcStore.Open(filename, null, 0.001))
            {
                store.Close();
            }
        }
    }
}

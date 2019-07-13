using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;

namespace Xbim.MemoryModel.Tests
{
    /// <summary>
    /// this class can be used to debug local files. It's not a stable test.
    /// </summary>
    [TestClass]
    public class StoreOpenForDebug
    {
        /// <summary>
        /// this Method can be used to debug local files. It's not a stable test.
        /// </summary>
        [TestMethod]
        public void NotAnUsefulTestJustDirtyWork()
        {
            string filename = "";
            filename = @"C:\Users\Claudio\Downloads\01_GARDENS_ARCH.ifc";
            //filename = @"C:\Users\Claudio\Downloads\pilastri.ifc";
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

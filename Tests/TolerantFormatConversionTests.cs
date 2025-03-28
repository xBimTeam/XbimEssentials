using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xbim.Ifc;

namespace Xbim.IO.Tests
{
    [TestClass]
    public class TolerantFormatConversionTests
    {
        // this test was known to fail... There's an encoded control code (\b) in the file name inside the header that cannot be persisted to xml
        // without further encoding
        [TestMethod]
        public void ForConsideration_SaveTroublesomeFile()
        {

            var outfile = new FileInfo("out.ifczip");
            if (outfile.Exists)
                outfile.Delete();

            var file = "TestFiles\\BackSpaceInFileName.ifc";

            using var store = IfcStore.Open(file);

            store.SaveAs(outfile.FullName, StorageType.IfcZip | StorageType.IfcXml);
            store.Close();
          
            if (outfile.Exists)
                outfile.Delete();
        }
    }
}

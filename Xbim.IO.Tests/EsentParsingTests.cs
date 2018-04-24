using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Xbim.IO.Tests
{
    [TestClass]
    public class EsentParsingTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles\\Issue107.zip")]
        public void Issue107OnEsentModel()
        {
            // a merged PR on issue 107 makes the memory model more tolerant of bad files.
            // the same does not apply to the database version though.
            //
            using (var model = new Esent.EsentModel(new Ifc2x3.EntityFactory()))
            {
                var temp = Path.GetTempPath() + Guid.NewGuid() + ".zip";
                File.Copy("Issue107.zip", temp);
                try
                {
                    var errCount = model.CreateFrom(temp);
                }
                finally
                {
                    if (File.Exists(temp))
                        File.Delete(temp);
                }

            }
        }
    }
}

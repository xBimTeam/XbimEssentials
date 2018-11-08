using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Xbim.IO.Tests
{
    [TestClass]
    public class EsentParsingTests
    {
        // this test is known to fail at the moment.
        // it's a desired tolerance feature, not yet accomplished
        //
        [TestMethod]
        [Ignore]
        [DeploymentItem("TestFiles\\Issue107.zip")]
        public void ForConsideration_Issue107OnEsentModel()
        {
            // a merged PR on issue 107 makes the memory model more tolerant of bad files.
            // the same does not apply to the database version though.
            //
            using (var model = new Esent.EsentModel(new Ifc2x3.EntityFactoryIfc2x3()))
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

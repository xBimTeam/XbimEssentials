using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                var errCount = model.CreateFrom("Issue107.zip");
            }
        }
    }
}

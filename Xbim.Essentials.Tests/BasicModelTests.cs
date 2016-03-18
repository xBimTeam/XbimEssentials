using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.IO;
using XbimModel = Xbim.Ifc2x3.IO.XbimModel;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem(@"TestSourceFiles\")]
    public class BasicModelTests
    {
        [TestMethod]
        public void OpenIfcFile()
        {
            using (var model = new XbimModel())
            {
                model.CreateFrom("4walls1floorSite.ifc");
                model.Close();
            }

        }

        [TestMethod]
        public void OpenIfcZipFile()
        {
            int percent = 0;
            ReportProgressDelegate progDelegate = delegate(int percentProgress, object userState)
            {
                percent = percentProgress;

            };
            using (var model = new XbimModel())
            {
                model.CreateFrom("4walls1floorSite.ifczip", null, progDelegate);
                model.Close();
                Assert.IsTrue(percent == 100);
            }
        }
        [TestMethod]
        public void OpenIfcXmlFile()
        {
            int percent = 0;
            ReportProgressDelegate progDelegate = delegate(int percentProgress, object userState)
            {
                percent = percentProgress;

            };
            using (var model = new XbimModel())
            {

                model.CreateFrom("4walls1floorSite.ifcxml",null,progDelegate);
                model.Close();
                Assert.IsTrue(percent==100);
            }

        }
        [TestMethod]
        public void OpenIfcFileFromStream()
        {
            using (var fileStream = new FileStream("4walls1floorSite.ifc", FileMode.Open,FileAccess.Read))
            {
                using (var model = new XbimModel())
                {
                    model.CreateFrom(fileStream, fileStream.Length, IfcStorageType.Ifc, "4walls1floorSite.xbim", null, true);                  
                    model.Close();
                }
                fileStream.Close();
            }

        }

       
        [TestMethod]
        public void OpenIfcXmlFileFromStream()
        {
            using (var fileStream = new FileStream("4walls1floorSite.ifcxml", FileMode.Open, FileAccess.Read))
            {
                using (var model = new XbimModel())
                {
                    model.CreateFrom(fileStream, fileStream.Length, IfcStorageType.IfcXml, "4walls1floorSite.xbim");
                    model.Close();
                }
                fileStream.Close();
            }

        }
    }
}

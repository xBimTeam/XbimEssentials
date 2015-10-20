using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc2x3.IO;
using System.IO;
using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem(@"TestSourceFiles\")]
    public class BasicModelTests
    {
        [TestMethod]
        public void OpenIfcFile()
        {
            using (var model = new Xbim.Ifc2x3.IO.XbimModel())
            {
                model.CreateFrom("4walls1floorSite.ifc");
                model.Close();
            }

        }

        [TestMethod]
        public void OpenIfcZipFile()
        {
            using (var model = new Xbim.Ifc2x3.IO.XbimModel())
            {
                model.CreateFrom("4walls1floorSite.ifczip");
                model.Close();
            }

        }
        [TestMethod]
        public void OpenIfcXmlFile()
        {
            using (var model = new Xbim.Ifc2x3.IO.XbimModel())
            {
                model.CreateFrom("4walls1floorSite.ifcxml");
                model.Close();
            }

        }
        [TestMethod]
        public void OpenIfcFileFromStream()
        {
            using (var fileStream = new FileStream("4walls1floorSite.ifc", FileMode.Open,FileAccess.Read))
            {
                using (var model = new Xbim.Ifc2x3.IO.XbimModel())
                {
                    model.CreateFrom(fileStream, XbimStorageType.Step21, "4walls1floorSite.xbim",null,true);                  
                    model.Close();
                }
                fileStream.Close();
            }

        }

        [TestMethod]
        public void OpenIfcZipFileFromStream()
        {
            using (var fileStream = new FileStream("4walls1floorSite.ifczip", FileMode.Open, FileAccess.Read))
            {
                using (var model = new Xbim.Ifc2x3.IO.XbimModel())
                {
                    model.CreateFrom(fileStream, XbimStorageType.Step21, "4walls1floorSite.xbim");
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
                using (var model = new Xbim.Ifc2x3.IO.XbimModel())
                {
                    model.CreateFrom(fileStream, XbimStorageType.Step21, "4walls1floorSite.xbim");
                    model.Close();
                }
                fileStream.Close();
            }

        }
    }
}

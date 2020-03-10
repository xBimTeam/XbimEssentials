using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.IO;
using XbimModel = Xbim.Ifc2x3.IO.XbimModel;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem(@"TestSourceFiles")]
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
        [DeploymentItem(@"TestSourceFiles\4walls1floorSite.ifc", @"IfcTagCoherence\")]
        public void IfcTagCoherence()
        {
            DirectoryInfo d = new DirectoryInfo(".");
            using (var model = IfcStore.Open(@"IfcTagCoherence\4walls1floorSite.ifc")) 
            {
                model.Tag = "Pippo";
                var ent = model.Instances.OfType<Xbim.Ifc4.Interfaces.IIfcProduct>().FirstOrDefault();
                Assert.AreEqual(model.Tag, ent.Model.Tag);
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
                var fileName = Guid.NewGuid() + ".xbim";

                model.CreateFrom("TestZip.ifczip", fileName, progDelegate);
                model.Close();
                Console.WriteLine(percent);
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

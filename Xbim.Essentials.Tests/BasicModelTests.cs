using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.IO;
using System.IO;
using Xbim.XbimExtensions.Interfaces;
using Xbim.Ifc2x3.ActorResource;
using Xbim.Ifc2x3.SharedBldgServiceElements;

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
        [DeploymentItem("TestSourceFiles\fileWithAbstractClass.ifc", "fileWithAbstractClass.ifc")]
        public void ToleratesFileWithAbstractClass()
        {
            // should survive parsing file with abstract class
            // and use null for offending instances.
            // 
            using (var model = new XbimModel())
            {
                model.CreateFrom(@"fileWithAbstractClass.ifc", null, null, true);

                foreach (var item in model.Instances.OfType<IfcFlowSegment>())
                {
                }

                var inst = model.Instances[1240086];
                Assert.IsNotNull(inst, "Instance should exist.");

                var inst2 = model.Instances[1240084];
                Assert.IsNull(inst2, "Instance should not exist.");
                model.Close();
            }
        }

        [TestMethod]
        public void OpenIfcZipFile()
        {
            using (var model = new XbimModel())
            {
                model.CreateFrom("4walls1floorSite.ifczip");
                model.Close();
            }

        }
        [TestMethod]
        public void OpenIfcXmlFile()
        {
            using (var model = new XbimModel())
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
                using (var model = new XbimModel())
                {
                    model.CreateFrom(fileStream, XbimStorageType.IFC, "4walls1floorSite.xbim",null,true);                  
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
                using (var model = new XbimModel())
                {
                    model.CreateFrom(fileStream, XbimStorageType.IFC, "4walls1floorSite.xbim");
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
                    model.CreateFrom(fileStream, XbimStorageType.IFC, "4walls1floorSite.xbim");
                    model.Close();
                }
                fileStream.Close();
            }

        }
    }
}

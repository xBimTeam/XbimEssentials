using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Ifc2x3.IO;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO.Esent;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class ActivationTests
    {
        [DeploymentItem("TestSourceFiles")]
        [TestMethod]
        public void ObjectActivation()
        {
            using (var model = new XbimModel())
            {
                model.CreateFrom("4walls1floorSite.ifc", "activation.xbim");
                model.Close();
            }

            using (var model = new XbimModel())
            {
                model.Open("activation.xbim", XbimDBAccess.ReadWrite);
                using (model.BeginTransaction("Activation tests"))
                {
                    var wall = model.Instances.FirstOrDefault<IfcWall>();
                    Assert.IsTrue(((IPersistEntity)wall).ActivationStatus == ActivationStatus.NotActivated);

                    //property activation
                    var name = wall.Name;
                    Assert.IsNotNull(name);
                    Assert.IsTrue(((IPersistEntity)wall).ActivationStatus == ActivationStatus.ActivatedRead);

                    wall.Name = "New name";
                    Assert.IsTrue(((IPersistEntity)wall).ActivationStatus == ActivationStatus.ActivatedReadWrite);

                    //collection activation
                    var wallType = model.Instances.FirstOrDefault<IfcWallType>();
                    Assert.IsNotNull(wallType);
                    Assert.IsTrue(((IPersistEntity)wallType).ActivationStatus == ActivationStatus.NotActivated);

                    wallType.HasPropertySets.Add(model.Instances.New<IfcPropertySet>(ps => ps.Name = "New property set"));
                    Assert.IsTrue(((IPersistEntity)wallType).ActivationStatus == ActivationStatus.ActivatedReadWrite);
                }
            }
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Ifc2x3.IO;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO.Esent;
using Xbim.Ifc;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class ActivationTests
    {
        [DeploymentItem("TestSourceFiles")]
        [TestMethod]
        public void ObjectActivation()
        {
            using (var model = IfcStore.Open("4walls1floorSite.ifc", null, 0))
            {
                using (model.BeginTransaction("Activation tests"))
                {
                    var wall = model.Instances.FirstOrDefault<IfcWall>();
                    Assert.IsFalse(((IPersistEntity)wall).Activated);

                    //property activation
                    var name = wall.Name;
                    Assert.IsNotNull(name);
                    Assert.IsTrue(((IPersistEntity)wall).Activated);

                    //collection activation
                    var wallType = model.Instances.FirstOrDefault<IfcWallType>();
                    Assert.IsNotNull(wallType);
                    Assert.IsFalse(((IPersistEntity)wallType).Activated);

                    wallType.HasPropertySets.Add(model.Instances.New<IfcPropertySet>(ps => ps.Name = "New property set"));
                    Assert.IsTrue(((IPersistEntity)wallType).Activated);
                }
            }
        }
    }
}

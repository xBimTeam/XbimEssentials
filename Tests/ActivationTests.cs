using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Essentials.Tests.Utilities;
using Xbim.Ifc;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc2x3.SharedBldgElements;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class ActivationTests
    {
        [TestMethod]
        [DeploymentItem("TestSourceFiles")]
        public void ObjectActivation()
        {
            // This test would only make a sense with a not-in-memory model
            using (var model = IfcStore.Open("4walls1floorSite.ifc", null, 0))
            {
                using (var txn = model.BeginTransaction("TXN"))
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

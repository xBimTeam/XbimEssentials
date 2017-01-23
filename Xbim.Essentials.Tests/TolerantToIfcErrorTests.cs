using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc2x3.ActorResource;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class TolerantToIfcErrorTests
    {
        [TestMethod]
        [DeploymentItem(@"TestSourceFiles\InvalidType.ifc")]
        public void ToleratesFileWithInvalidTypeInList()
        {
            using (var model = new XbimModel())
            {
                
                model.CreateFrom("InvalidType.ifc", null, null, true);

                var recursiveElement = model.Instances[582800] as IfcBuildingStorey;
                var items = recursiveElement.ContainsElements.SelectMany(container => container.RelatedElements);

                Assert.AreEqual(items.Count(), 2, "Should find two items");

                model.Close();
            }
        }

        [TestMethod]
        [DeploymentItem(@"TestSourceFiles\InvalidType.ifc")]
        public void ToleratesFileWithInvalidEnumString()
        {
            using (var model = new XbimModel())
            {
                model.CreateFrom("InvalidType.ifc", null, null, true);

               
                var role = model.Instances[2] as IfcActorRole;
                Assert.IsNotNull(role);
                Assert.AreEqual(role.Role, IfcRole.Architect);
                

                model.Close();
            }
        }
    }
}

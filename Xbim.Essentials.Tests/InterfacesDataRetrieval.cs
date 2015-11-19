using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc2x3;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem("TestSourceFiles")]
    public class InterfacesDataRetrieval
    {
        [TestMethod]
        public void Ifc4InterfacesToIfc2X3()
        {
            var model = new MemoryModel<EntityFactory>();
            model.Open("4walls1floorSite.ifc",null);

            var walls = model.Instances.OfType<IIfcWall>().ToList();
            Assert.AreEqual(4, walls.Count);

            foreach (var wall in walls)
            {
                Assert.IsNotNull(wall.Name);

                var properties = wall.IsDefinedBy
                    .Where(r => r.RelatingPropertyDefinition is IIfcPropertySet)
                    .SelectMany(r => ((IIfcPropertySet)r.RelatingPropertyDefinition).HasProperties);

                foreach (var property in properties)
                {
                    Assert.IsNotNull(property.Name);
                    var single = property as IIfcPropertySingleValue;
                    if(single != null)
                        Assert.IsNotNull(single.NominalValue);
                }

                var type = wall.IsTypedBy.Select(r => r.RelatingType).FirstOrDefault();
                Assert.IsNotNull(type);
            }
        }
    }
}

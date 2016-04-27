using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.Kernel;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Esent;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem("TestSourceFiles")]
    public class InterfacesDataRetrieval
    {

        [TestMethod]
        public void EsentDataRetrieval()
        {
            using (var model = new EsentModel(new EntityFactory()))
            {
                model.CreateFrom("4walls1floorSite.ifc", null, null, true);

                var walls = model.Instances.Where<IIfcWall>(w => w.Name != null);
                Assert.AreEqual(4, walls.Count());

                //this is correct now (fixed to search for interface implementations)
                var entities = model.Instances.Where<IPersistEntity>(i => true).ToList();
                
                //this doesn't bring in non-indexed classes
                var entities2 = model.Instances.OfType<IPersistEntity>().ToList();
                
                var totalCount = model.Instances.Count;
                Assert.AreEqual(totalCount, entities.Count);
                Assert.AreEqual(totalCount, entities2.Count);

               

            }
        }

        [TestMethod]
        public void Ifc4InterfacesToIfc2X3()
        {
            var model = new MemoryModel(new EntityFactory());
            model.LoadStep21("4walls1floorSite.ifc",null);

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

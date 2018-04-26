using System.Diagnostics;
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
        [DeploymentItem("TestSourceFiles\\4walls1floorSite.ifc")]
        [DeploymentItem("TestSourceFiles\\AlmostEmptyIFC4.ifc")]
        public void ShouldBeAbleToReadUnitDimensions()
        {
            // this can throw an exception if the model is not inside a transaction
            using (var model = MemoryModel.OpenRead(@"AlmostEmptyIFC4.ifc"))
            {
                var instance = model.Instances[53] as Ifc4.MeasureResource.IfcSIUnit;
                var dimensions = instance.Dimensions;
                Debug.WriteLine(dimensions.ToString());
            }

            // this can throw an exception if the model is not inside a transaction
            using (var model = MemoryModel.OpenRead(@"4walls1floorSite.ifc"))
            {
                var instance = model.Instances[51] as Ifc2x3.MeasureResource.IfcSIUnit;
                var dimensions = instance.Dimensions;
                Debug.WriteLine(dimensions.ToString());
            }
        }


        [TestMethod]
        public void Ifc4InterfacesToIfc2X3()
        {
            var model = new MemoryModel(new EntityFactoryIfc2x3());
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

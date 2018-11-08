using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Ifc2x3.HVACDomain;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc4.Interfaces;
using IfcAirTerminalTypeEnum = Xbim.Ifc4.Interfaces.IfcAirTerminalTypeEnum;
using IfcBeamTypeEnum = Xbim.Ifc4.Interfaces.IfcBeamTypeEnum;
using Xbim.Common.Step21;
using Xbim.IO.Memory;
using Xbim.Essentials.Tests.Utilities;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem("TestSourceFiles/4walls1floorSite.ifc")]
    public class CrossAccessTests
    {
        [TestMethod]
        public void CollectionRemoval()
        {
            // arrange
            using (var model = new MemoryModel(new Ifc2x3.EntityFactoryIfc2x3()))
            using (var txn = model.BeginTransaction(""))
            {
                IfcWall wall1 = model.Instances.New<IfcWall>();
                IfcWall wall2 = model.Instances.New<IfcWall>();

                var propertySet = model.Instances.New<Ifc2x3.Kernel.IfcPropertySet>();
                propertySet.HasProperties.Add(model.Instances.New<Ifc2x3.PropertyResource.IfcPropertySingleValue>());

                // Changing the type of the rel variable from interface to IfcRelDefinesByProperties solves the issue.
                IIfcRelDefinesByProperties rel = model.Instances.New<Ifc2x3.Kernel.IfcRelDefinesByProperties>();

                rel.RelatingPropertyDefinition = propertySet;
                rel.RelatedObjects.Add(wall1);
                rel.RelatedObjects.Add(wall2);

                // act
                rel.RelatedObjects.Remove(wall1);

                // assert failed. Expected: 1, actual: 2 (null, wall2).
                Assert.AreEqual(1, rel.RelatedObjects.Count);

                txn.Commit();
            }
        }

        [TestMethod]
        public void SettingNamesTest()
        {
            using (var model = MemoryModel.OpenRead("4walls1floorSite.ifc"))
            {
                using (var txn = model.BeginTransaction(""))
                {
                    //setting names to new value
                    var walls = model.Instances.OfType<IIfcWall>().ToList();
                    Assert.IsTrue(walls.Any());
                    foreach (var wall in walls)
                        wall.Name = "New name";
                    Assert.IsTrue(walls.All(w => w.Name == "New name"));

                    //setting value to null
                    var relDef = model.Instances.FirstOrDefault<IIfcRelDefinesByProperties>();
                    Assert.IsNotNull(relDef);
                    relDef.RelatingPropertyDefinition = null;
                    Assert.IsNull(relDef.RelatingPropertyDefinition);

                    //LOUVRE doesn't exist in IFC2x3 but should be persisted using IFC2x3 values
                    var airTerminal = model.Instances.New<IfcAirTerminalType>() as IIfcAirTerminalType;
                    airTerminal.PredefinedType = IfcAirTerminalTypeEnum.LOUVRE;
                    Assert.AreEqual(airTerminal.PredefinedType, IfcAirTerminalTypeEnum.LOUVRE);
                    Assert.IsTrue(((IfcAirTerminalType)airTerminal).PredefinedType == Ifc2x3.HVACDomain.IfcAirTerminalTypeEnum.USERDEFINED);
                    Assert.IsTrue(airTerminal.ElementType == "LOUVRE");

                    //beam doesn't have PredefinedType attribute in IFC2x3 but should be stored in extended field
                    var beam = model.Instances.New<IfcBeam>() as IIfcBeam;
                    Assert.IsTrue(beam.PredefinedType == null);
                    beam.PredefinedType = IfcBeamTypeEnum.BEAM;
                    Assert.IsTrue(beam.PredefinedType == IfcBeamTypeEnum.BEAM);


                    txn.RollBack();
                }
            }
        }
    }
}

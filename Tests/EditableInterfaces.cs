using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Ifc2x3.GeometryResource;
using Xbim.Ifc2x3.HVACDomain;
using Xbim.Ifc2x3.MaterialResource;
using Xbim.Ifc2x3.PlumbingFireProtectionDomain;
using Xbim.Ifc2x3.PropertyResource;
using Xbim.Ifc2x3.StructuralAnalysisDomain;
using Xbim.Ifc2x3.UtilityResource;
using Xbim.Ifc4.DateTimeResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.IO.Memory;
using IfcAirTerminalTypeEnum = Xbim.Ifc4.Interfaces.IfcAirTerminalTypeEnum;
using IfcChangeActionEnum = Xbim.Ifc4.Interfaces.IfcChangeActionEnum;
using IfcWasteTerminalTypeEnum = Xbim.Ifc2x3.PlumbingFireProtectionDomain.IfcWasteTerminalTypeEnum;

namespace Xbim.Essentials.Tests
{
    /// <summary>
    /// These tests should test features introduced to be able to edit IFC2x3 as if it was IFC4
    /// </summary>
    [TestClass]
    public class EditableInterfaces
    {
        [TestMethod]
        [DeploymentItem("TestSourceFiles\\4walls1floorSite.ifc")]
        public void SingleFieldTest()
        {
            const string file = "4walls1floorSite.ifc";
            using (var model = MemoryModel.OpenRead(file))
            {
                var extendedChanges = 0;
                model.EntityModified += (entity, property) => { if (property < 0) extendedChanges++; };

                using (var txn = model.BeginTransaction("Test"))
                {
                    var wall = model.Instances.FirstOrDefault<IIfcWall>();
                    Assert.IsNotNull(wall);

                    //defined type
                    wall.Name = "New name";
                    Assert.IsTrue(wall.Name == "New name");

                    //select interface
                    var prop = model.Instances.FirstOrDefault<IIfcPropertySingleValue>();
                    Assert.IsNotNull(prop);
                    Assert.IsNotNull(prop.NominalValue);
                    prop.NominalValue = new IfcLabel("New label");
                    Assert.IsTrue(prop.NominalValue.Equals(new IfcLabel("New label")));
                    Assert.IsTrue(extendedChanges == 0);
                    
                    //non-existing interface implementation
                    var dt = (IfcDate) DateTime.Now;
                    prop.NominalValue = dt;
                    Assert.IsTrue(prop.NominalValue.Equals(dt));
                    Assert.IsTrue(extendedChanges > 0);
                    extendedChanges = 0;

                    //entity
                    var oh = model.Instances.New<IfcOwnerHistory>() as IIfcOwnerHistory;
                    var app = model.Instances.New<IfcApplication>() as IIfcApplication;
                    oh.OwningApplication = app;
                    Assert.AreEqual(oh.OwningApplication, app);

                    //enumeration
                    oh.ChangeAction = IfcChangeActionEnum.MODIFIED;
                    Assert.AreEqual(oh.ChangeAction, IfcChangeActionEnum.MODIFIED);

                    //non-existing defined type
                    var layer = model.Instances.New<IfcMaterialLayer>() as IIfcMaterialLayer;
                    layer.Description = "New description";
                    Assert.IsTrue(layer.Description == "New description");
                    Assert.IsTrue(extendedChanges > 0);
                    extendedChanges = 0;

                    //non-existing entity
                    var member = model.Instances.New<IfcStructuralCurveMember>() as IIfcStructuralCurveMember;
                    member.Axis = model.Instances.New<IfcDirection>(d => d.SetXYZ(0,0,1));
                    Assert.IsTrue(extendedChanges > 0);
                    extendedChanges = 0;

                    //non-existing enumeration
                    wall.PredefinedType = IfcWallTypeEnum.POLYGONAL;
                    Assert.AreEqual(wall.PredefinedType, IfcWallTypeEnum.POLYGONAL);
                    Assert.IsTrue(extendedChanges > 0);
                    extendedChanges = 0;

                    //non-existing enumeration member
                    var att = model.Instances.New<IfcAirTerminalType>();
                    var att4 = (IIfcAirTerminalType)att;
                    att4.PredefinedType = IfcAirTerminalTypeEnum.LOUVRE;
                    Assert.IsTrue(att4.PredefinedType == IfcAirTerminalTypeEnum.LOUVRE);
                    Assert.IsTrue(att.PredefinedType == Ifc2x3.HVACDomain.IfcAirTerminalTypeEnum.USERDEFINED);
                    Assert.IsTrue(att.ElementType == "LOUVRE");
                    

                    //non-existing enumeration member in IFC4
                    var wtt = model.Instances.New<IfcWasteTerminalType>();
                    wtt.PredefinedType = IfcWasteTerminalTypeEnum.GREASEINTERCEPTOR;
                    Assert.IsFalse(wtt.ElementType.HasValue);
                    var wtt4 = (IIfcWasteTerminalType) wtt;
                    Assert.IsTrue(wtt4.PredefinedType == Ifc4.Interfaces.IfcWasteTerminalTypeEnum.USERDEFINED);
                    Assert.IsTrue(wtt4.ElementType == "GREASEINTERCEPTOR");

                    txn.Commit();
                }
            }
        }

        [TestMethod]
        [DeploymentItem("TestSourceFiles\\4walls1floorSite.ifc")]
        public void ItemSetTest()
        {
            const string file = "4walls1floorSite.ifc";
            using (var model = MemoryModel.OpenRead(file))
            {
                var extendedChanges = 0;
                model.EntityModified += (entity, property) => { if (property < 0) extendedChanges++; };

                using (var txn = model.BeginTransaction("Test"))
                {
                    //set of defined types
                    var point = model.Instances.New<IfcCartesianPoint>() as IIfcCartesianPoint;
                    point.Coordinates.Add(1);
                    point.Coordinates.Add(1);
                    point.Coordinates.Add(1);
                    Assert.IsTrue(point.Coordinates.SequenceEqual(new[] { new IfcLengthMeasure(1), new IfcLengthMeasure(1), new IfcLengthMeasure(1) }));
                    
                    //set of entities
                    var rel = model.Instances.FirstOrDefault<IIfcRelDefinesByProperties>();
                    Assert.IsTrue(rel.RelatedObjects.Count > 0);
                    rel.RelatedObjects.Clear();
                    Assert.IsTrue(rel.RelatedObjects.Count == 0);
                    var walls = model.Instances.OfType<IIfcWall>();
                    rel.RelatedObjects.AddRange(walls);
                    Assert.IsTrue(rel.RelatedObjects.Count == 4);

                    //change in hierarchy. IFC4 allows types to be assigned here which should be treated as an extension
                    var wt = model.Instances.FirstOrDefault<IIfcWallType>();
                    Assert.IsNotNull(wt);
                    Assert.IsTrue(extendedChanges == 0);
                    rel.RelatedObjects.Add(wt);
                    Assert.IsTrue(rel.RelatedObjects.Count == 5);
                    Assert.IsTrue(extendedChanges > 0);
                    extendedChanges = 0;


                    //set of selects
                    var en = model.Instances.New<IfcPropertyEnumeratedValue>() as IIfcPropertyEnumeratedValue;
                    en.EnumerationValues.Add(new IfcLabel("Aaa"));
                    Assert.IsTrue(en.EnumerationValues.FirstOrDefault().Equals(new IfcLabel("Aaa")));
                    Assert.IsTrue(extendedChanges == 0);
                    var dt = (IfcDate)DateTime.Now;
                    en.EnumerationValues.Add(dt);
                    Assert.IsTrue(en.EnumerationValues.Contains(dt));
                    Assert.IsTrue(extendedChanges > 0);
                    extendedChanges = 0;


                    txn.Commit();
                }
            }
        }
    }
}

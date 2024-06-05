using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Xbim.Common;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class ReplaceEntityTests
    {
        [TestMethod]
        public void ReplaceEntity_Single()
        {
            using (var model = MemoryModel.OpenRead(@"TestFiles\\Ifc4cube_advanced_brep.ifc"))
            {
                using (var txn = model.BeginTransaction("ReplaceEntity"))
                {
                    var original = model.Instances.Where(x => x.EntityLabel == 113).FirstOrDefault();
                    // #113=IFCCARTESIANPOINT((-0.683012701892219,-0.183012701892219,1.)); Only referenced in #112=IFCAXIS2PLACEMENT3D(#113,#10,#114);
                    var replacement = model.Instances.Where(x => x.EntityLabel == 39).FirstOrDefault();
                    // #39=IFCCARTESIANPOINT((-0.683012701892219,-0.183012701892219,1.)); Only referenced in #38=IFCVERTEXPOINT(#39);

                    // entity not changed, just replaced with another reference have the same value.
                    ModelHelper.Replace<IPersistEntity, IPersistEntity>(model, original, replacement);

                    // exception #112=IFCAXIS2PLACEMENT3D(#39,#10,#114);
                    var inverseEntity = model.Instances.Where(x => x.EntityLabel == 112).FirstOrDefault() as IfcAxis2Placement3D;
                    Assert.IsNotNull(inverseEntity);
                    Assert.IsTrue(inverseEntity.Location.EntityLabel == replacement.EntityLabel);
                    txn.Commit();
                }
                using (var fileStream = new StreamWriter("..\\..\\ReplaceResult_Single.ifc"))
                {
                    model.SaveAsStep21(fileStream);
                }
            }
        }

        [TestMethod]
        public void ReplaceEntity_List()
        {
            using (var model = MemoryModel.OpenRead(@"TestFiles\\Ifc4cube_advanced_brep.ifc"))
            {
                using (var txn = model.BeginTransaction("ReplaceEntity"))
                {
                    var original = model.Instances.Where(x => x.EntityLabel == 47).FirstOrDefault();
                    // #47= IFCCARTESIANPOINT((-0.5,-0.5)); Only referenced in #48= IFCPOLYLINE((#47,#46));
                    var replacement = model.Instances.Where(x => x.EntityLabel == 50).FirstOrDefault();
                    // #50= IFCCARTESIANPOINT((-0.5,-0.5)); Only referenced in #52= IFCPOLYLINE((#51,#50));

                    // entity not changed, just replaced with another reference have the same value.
                    ModelHelper.Replace<IPersistEntity, IPersistEntity>(model, original, replacement);

                    // exception #48=IFCPOLYLINE((#50,#46));
                    var inverseEntity = model.Instances.Where(x => x.EntityLabel == 48).FirstOrDefault() as IfcPolyline;
                    Assert.IsNotNull(inverseEntity);
                    Assert.IsTrue(!inverseEntity.Points.Contains(original));
                    Assert.IsTrue(inverseEntity.Points.Contains(replacement));

                    txn.Commit();
                }
                using (var fileStream = new StreamWriter("..\\..\\ReplaceResult_List.ifc"))
                {
                    model.SaveAsStep21(fileStream);
                }
            }
        }

        [TestMethod]
        public void ReplaceEntity_NestedList()
        {
            using (var model = MemoryModel.OpenRead(@"TestFiles\\Ifc4cube_advanced_brep.ifc"))
            {
                using (var txn = model.BeginTransaction("ReplaceEntity"))
                {
                    var original = model.Instances.Where(x => x.EntityLabel == 171).FirstOrDefault();
                    // #171=IFCCARTESIANPOINT((0.5,0.5,0.));
                    // Only referenced in #170=IFCBSPLINESURFACEWITHKNOTS(3,1,((#171,#172),(#173,#174),(#175,#176),(#177,#178)),.UNSPECIFIED.,.F.,.F.,.U.,(4,4),(2,2),(0.,1224.74487139159),(2.,3.),.UNSPECIFIED.);
                    var replacement = model.Instances.Where(x => x.EntityLabel == 35).FirstOrDefault();
                    // #35=IFCCARTESIANPOINT((0.5,0.5,0.)); Only referenced in #34=IFCVERTEXPOINT(#35);

                    // entity not changed, just replaced with another reference have the same value.
                    ModelHelper.Replace<IPersistEntity, IPersistEntity>(model, original, replacement);

                    // exception #170=IFCBSPLINESURFACEWITHKNOTS(3,1,((#35,#172),(#173,#174),(#175,#176),(#177,#178)),.UNSPECIFIED.,.F.,.F.,.U.,(4,4),(2,2),(0.,1224.74487139159),(2.,3.),.UNSPECIFIED.);
                    var inverseEntity = model.Instances.Where(x => x.EntityLabel == 170).FirstOrDefault() as IfcBSplineSurfaceWithKnots;
                    Assert.IsNotNull(inverseEntity);
                    var flatList = new List<IPersistEntity>();
                    foreach (var cpList in inverseEntity.ControlPointsList)
                        flatList.AddRange(cpList);
                    Assert.IsTrue(!flatList.Contains(original));
                    Assert.IsTrue(flatList.Contains(replacement));
                    txn.Commit();
                }
                using (var fileStream = new StreamWriter("..\\..\\ReplaceResult_NestedList.ifc"))
                {
                    model.SaveAsStep21(fileStream);
                }
            }
        }

        [TestMethod]
        public void ReplaceEntity_ProxyType()
        {
            using var model = new MemoryModel(new Ifc4.EntityFactoryIfc4());
            using (var txn = model.BeginTransaction("Create"))
            {
                var i = model.Instances;
                var type = i.New<IfcBuildingElementProxyType>();
                var instance = i.New<IfcBuildingElementProxy>();
                var rel = i.New<IfcRelDefinesByType>(r => {
                    r.RelatingType = type;
                    r.RelatedObjects.Add(instance);
                });
                var group = i.New<IfcRelAssignsToGroup>();

                var wallType = i.New<IfcWallType>();
                var wall = i.New<IfcWall>();

                ModelHelper.Replace<IPersistEntity, IPersistEntity>(model, instance, wall);
                ModelHelper.Replace<IPersistEntity, IPersistEntity>(model, type, wallType);

                rel.RelatingType.Should().Be(wallType);
                rel.RelatedObjects.Should().NotContain(instance);
                rel.RelatedObjects.Should().Contain(wall);
                group.RelatedObjects.Should().BeEmpty();
            }
        }
    }
}

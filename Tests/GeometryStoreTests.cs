using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Common.Step21;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc;
using Xbim.Ifc4;
using Xbim.Ifc4.SharedBldgElements;
using Xbim.IO;
using Xbim.IO.Esent;

namespace Xbim.IO.Tests
{
    [TestClass]
    public class GeometryStoreTests
    {
        private static readonly IEntityFactory ef4 = new Ifc4.EntityFactoryIfc4();
        private static readonly IEntityFactory ef2x3 = new Ifc2x3.EntityFactoryIfc2x3();

        [TestMethod]
        [DeploymentItem("TestFiles\\4walls1floorSite.ifc")]
        public void EsentGeometryStoreIsEmptyAfterReopen()
        {
            var db = Guid.NewGuid().ToString() + ".xbim";
            var ifc = "4walls1floorSite.ifc";
            var p = new EsentModelProvider { DatabaseFileName = db };
            var s = p.GetXbimSchemaVersion(ifc);
            using (p.Open(ifc, s)) { }

            using (var model = IfcStore.Open(db))
            {
                Assert.IsTrue(model.GeometryStore.IsEmpty);
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles\\OneWall.xBIM")]
        [DeploymentItem("TestFiles\\ShortRebar.xBIM")]
        public void GeometryBinaryRead()
        {
            ParseGeometry("ShortRebar.xBIM");
            // ParseGeometry("OneWall.xBIM");
        }

        private static void ParseGeometry(string name)
        {
            using (var model = IfcStore.Open(name))
            using (var txn = model.GeometryStore.BeginRead())
            {
                foreach (IXbimShapeGeometryData geo in txn.ShapeGeometries)
                {
                    using (var ms = new MemoryStream(geo.ShapeData))
                    using (var br = new BinaryReader(ms))
                    {
                        var v = br.ReadShapeTriangulation();
                        List<float[]> pts;
                        List<int> idxs;
                        v.ToPointsWithNormalsAndIndices(out pts, out idxs);
                    }
                }
            }
        }

        [TestMethod]
        public void IfcStoreRegionWorldCoordTest()
        {
            XbimMatrix3D WorldCoord = new XbimMatrix3D(-0.228171504575237, -0.973620955248947, 0, 0, 0.973620955248947, -0.228171504575237, 0, 0, 0, 0, 1, 0, 543188712, 259041729, 16000, 1);
            var creds = new XbimEditorCredentials();
            using (IfcStore model = IfcStore.Create(@"RegionTest.xbim", creds, XbimSchemaVersion.Ifc2X3))
            {
                var geomStore = model.GeometryStore;
                using (var txn = geomStore.BeginInit())
                {
                    //ADD A REGIONCOLLECTION
                    var regions = new XbimRegionCollection();
                    regions.ContextLabel = 50;
                    var bb = new XbimRect3D(new XbimPoint3D(1, 1, 1), new XbimVector3D(10, 20, 30));
                    regions.Add(new XbimRegion("region1", bb, 100, WorldCoord));
                    txn.AddRegions(regions);

                    txn.Commit();
                }
                model.Close();
            }

            using (var model = IfcStore.Open(@"RegionTest.xbim"))
            {
                var geomStore = model.GeometryStore;
                Assert.IsFalse(geomStore.IsEmpty);
                using (var reader = geomStore.BeginRead())
                {
                    Assert.IsTrue(reader.ContextIds.Any());
                    var regioncoll = reader.ContextRegions.First();
                    var region = regioncoll.FirstOrDefault();
                    Assert.AreEqual(WorldCoord.OffsetX, region.WorldCoordinateSystem.OffsetX, 1.0);
                    Assert.AreEqual(WorldCoord.OffsetY, region.WorldCoordinateSystem.OffsetY, 1.0);
                    Assert.AreEqual(WorldCoord.OffsetZ, region.WorldCoordinateSystem.OffsetZ, 1.0);

                }
                model.Close();
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles\\4walls1floorSite.ifc")]
        public void EsentGeometryStoreReopenAddTest()
        {
            var db = Guid.NewGuid().ToString() + ".xbim";
            var ifc = "4walls1floorSite.ifc";
            var p = new EsentModelProvider { DatabaseFileName = db };
            var s = p.GetXbimSchemaVersion(ifc);
            using (var m = p.Open(ifc, s)) { p.Close(m); }

            using (var model = IfcStore.Open(db, accessMode: XbimDBAccess.ReadWrite))
            {
                var geomStore = model.GeometryStore;
                using (var txn = geomStore.BeginInit())
                {
                    //ADD A GEOMETRY SHAPE
                    var geomData = new XbimShapeGeometry()
                    {
                        IfcShapeLabel = 1,
                        Format = XbimGeometryType.BoundingBox,
                        GeometryHash = 0,
                        LOD = XbimLOD.LOD100,
                        ReferenceCount = 1,
                        ShapeData = "2123",
                        BoundingBox = XbimRect3D.Empty
                    };
                    var shapeGeomLabel = txn.AddShapeGeometry(geomData);

                    //ADD A SHAPE INSTANCE
                    var shapeInstance = new XbimShapeInstance()
                    {
                        ShapeGeometryLabel = shapeGeomLabel,
                        StyleLabel = 5,
                        RepresentationContext = 50

                    };

                    var instanceId = txn.AddShapeInstance(shapeInstance, shapeGeomLabel);
                    Assert.IsTrue(instanceId == 1);

                    //ADD A REGIONCOLLECTION
                    var regions = new XbimRegionCollection
                    {
                        ContextLabel = 50
                    };
                    var bb = new XbimRect3D(new XbimPoint3D(1, 1, 1), new XbimVector3D(10, 20, 30));
                    regions.Add(new XbimRegion("region1", bb, 100, XbimMatrix3D.Identity));
                    txn.AddRegions(regions);

                    txn.Commit();
                }
                model.SaveAs("SampleHouse4.xbim", StorageType.Xbim);
                model.Close();
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void IfcStoreGeometryStoreAddTest()
        {
            using (var model = IfcStore.Open("SampleHouse4.ifc"))
            {
                var geomStore = model.GeometryStore;
                using (var txn = geomStore.BeginInit())
                {
                    //ADD A GEOMETRY SHAPE
                    var geomData = new XbimShapeGeometry()
                    {
                        IfcShapeLabel = 1,
                        Format = XbimGeometryType.BoundingBox,
                        GeometryHash = 0,
                        LOD = XbimLOD.LOD100,
                        ReferenceCount = 1,
                        ShapeData = "2123",
                        BoundingBox = XbimRect3D.Empty
                    };
                    var shapeGeomLabel = txn.AddShapeGeometry(geomData);

                    //ADD A SHAPE INSTANCE
                    var shapeInstance = new XbimShapeInstance()
                    {
                        ShapeGeometryLabel = shapeGeomLabel,
                        StyleLabel = 5,
                        RepresentationContext = 50

                    };

                    var instanceId = txn.AddShapeInstance(shapeInstance, shapeGeomLabel);
                    Assert.IsTrue(instanceId == 1);

                    //ADD A REGIONCOLLECTION
                    var regions = new XbimRegionCollection();
                    regions.ContextLabel = 50;
                    var bb = new XbimRect3D(new XbimPoint3D(1, 1, 1), new XbimVector3D(10, 20, 30));
                    regions.Add(new XbimRegion("region1", bb, 100, XbimMatrix3D.Identity));
                    txn.AddRegions(regions);

                    txn.Commit();
                }
                model.SaveAs("SampleHouse4.xbim", StorageType.Xbim);
                model.Close();
            }
            using (var model = IfcStore.Open(@"SampleHouse4.xbim"))
            {
                var geomStore = model.GeometryStore;
                Assert.IsFalse(geomStore.IsEmpty);
                using (var reader = geomStore.BeginRead())
                {
                    Assert.IsTrue(reader.ContextIds.Any());
                    Assert.IsTrue(reader.ContextRegions.First().ContextLabel == 50);
                    Assert.IsTrue(reader.ShapeGeometries.Count() == 1);
                    Assert.IsTrue(reader.ShapeInstances.Count() == 1);
                    Assert.IsTrue(reader.StyleIds.Count == 1);
                }
                model.Close();
            }
        }




        [TestMethod]
        [DeploymentItem("TestFiles")]
        public void ResourceReleaseTest()
        {
            using (var model = IfcStore.Open("SampleHouse4.ifc", null, 0))
            {
                var geomStore = model.GeometryStore;
                using (var txn = geomStore.BeginInit())
                {
                    //ADD A GEOMETRY SHAPE
                    var geomData = new XbimShapeGeometry()
                    {
                        IfcShapeLabel = 1,
                        Format = XbimGeometryType.BoundingBox,
                        GeometryHash = 0,
                        LOD = XbimLOD.LOD100,
                        ReferenceCount = 1,
                        ShapeData = "2123",
                        BoundingBox = XbimRect3D.Empty
                    };
                    var shapeGeomLabel = txn.AddShapeGeometry(geomData);

                    //ADD A SHAPE INSTANCE
                    var shapeInstance = new XbimShapeInstance()
                    {
                        ShapeGeometryLabel = shapeGeomLabel,
                        StyleLabel = 5,
                        RepresentationContext = 50

                    };

                    var instanceId = txn.AddShapeInstance(shapeInstance, shapeGeomLabel);
                    Assert.IsTrue(instanceId == 1);

                    //ADD A REGIONCOLLECTION
                    var regions = new XbimRegionCollection();
                    regions.ContextLabel = 50;
                    var bb = new XbimRect3D(new XbimPoint3D(1, 1, 1), new XbimVector3D(10, 20, 30));
                    regions.Add(new XbimRegion("region1", bb, 100, XbimMatrix3D.Identity));
                    txn.AddRegions(regions);
                    txn.Commit();
                }
                int c = model.Instances.OfType<IfcDoor>().Count();
                model.Close();
            }
            Thread.SpinWait(200);
            Assert.IsTrue(IO.Esent.EsentModel.ModelOpenCount == 0);

        }

        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void EsentGeometryStoreBatchTest()
        {
            using (var model = new IO.Esent.EsentModel(ef4))
            {
                model.CreateFrom("SampleHouse4.ifc", null, null, true);
                var store = model.GeometryStore;
                using (var txn = store.BeginInit())
                {
                    for (int i = 0; i < 100; i++)
                    {
                        //ADD A GEOMETRY SHAPE
                        var geomData = new XbimShapeGeometry()
                        {
                            IfcShapeLabel = 1,
                            Format = XbimGeometryType.BoundingBox,
                            GeometryHash = 0,
                            LOD = XbimLOD.LOD100,
                            ReferenceCount = 1,
                            ShapeData = "2123",
                            BoundingBox = XbimRect3D.Empty
                        };
                        var shapeGeomLabel = txn.AddShapeGeometry(geomData);
                    }
                    for (int i = 0; i < 100; i++)
                    {
                        //ADD A SHAPE INSTANCE
                        var shapeInstance = new XbimShapeInstance()
                        {
                            ShapeGeometryLabel = i + 1
                        };

                        var instanceId = txn.AddShapeInstance(shapeInstance, i + 1);
                        Assert.IsTrue(instanceId == i + 1);
                    }
                    for (int i = 0; i < 100; i++)
                    {
                        //ADD A SHAPE INSTANCE
                        var shapeInstance = new XbimShapeInstance()
                        {
                            ShapeGeometryLabel = i + 1
                        };

                        var instanceId = txn.AddShapeInstance(shapeInstance, i + 1);
                        Assert.IsTrue(instanceId == i + 101);
                    }
                    for (int i = 0; i < 100; i++)
                    {
                        //ADD A SHAPE INSTANCE
                        var shapeInstance = new XbimShapeInstance()
                        {
                            ShapeGeometryLabel = i + 1
                        };

                        var instanceId = txn.AddShapeInstance(shapeInstance, i + 1);
                        Assert.IsTrue(instanceId == i + 201);
                    }
                    //ADD A REGIONCOLLECTION
                    var regions = new XbimRegionCollection { ContextLabel = 50 };
                    regions.Add(new XbimRegion("region1", XbimRect3D.Empty, 100, XbimMatrix3D.Identity));
                    txn.AddRegions(regions);

                    txn.Commit();
                }
                model.Close();
            }
        }

        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void EsentGeometryStoreMultiThreadTest()
        {
            using (var model = IfcStore.Create(null, XbimSchemaVersion.Ifc4, XbimStoreType.EsentDatabase))
            {

                var store = model.GeometryStore;
                using (var txn = store.BeginInit())
                {

                    Parallel.For(0, 100, i =>
                    {
                        //ADD A GEOMETRY SHAPE
                        var geomData = new XbimShapeGeometry()
                        {
                            IfcShapeLabel = 1,
                            Format = XbimGeometryType.BoundingBox,
                            GeometryHash = 0,
                            LOD = XbimLOD.LOD100,
                            ReferenceCount = 1,
                            ShapeData = "2123",
                            BoundingBox = XbimRect3D.Empty
                        };
                        var shapeInstance = new XbimShapeInstance()
                        {
                            ShapeGeometryLabel = i + 1
                        };
                        var shapeGeomLabel = txn.AddShapeGeometry(geomData);
                        var instanceId = txn.AddShapeInstance(shapeInstance, shapeGeomLabel);
                    });

                    Parallel.For(0, 100, i =>
                    {
                        //ADD A SHAPE INSTANCE
                        var shapeInstance = new XbimShapeInstance()
                        {
                            ShapeGeometryLabel = i + 1
                        };

                        var instanceId = txn.AddShapeInstance(shapeInstance, i + 1);

                    });

                    //ADD A REGIONCOLLECTION
                    var regions = new XbimRegionCollection { ContextLabel = 50 };
                    regions.Add(new XbimRegion("region1", XbimRect3D.Empty, 100, XbimMatrix3D.Identity));
                    txn.AddRegions(regions);

                    txn.Commit();
                }
                model.Close();
            }
        }

        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void InMemoryGeometryStoreMultiThreadTest()
        {
            using (var model = new IO.Memory.MemoryModel(ef4))
            {

                var store = model.GeometryStore;
                using (var txn = store.BeginInit())
                {

                    Parallel.For(0, 100, i =>
                    {
                        //ADD A GEOMETRY SHAPE
                        var geomData = new XbimShapeGeometry()
                        {
                            IfcShapeLabel = 1,
                            Format = XbimGeometryType.BoundingBox,
                            GeometryHash = 0,
                            LOD = XbimLOD.LOD100,
                            ReferenceCount = 1,
                            ShapeData = "2123",
                            BoundingBox = XbimRect3D.Empty
                        };
                        var shapeGeomLabel = txn.AddShapeGeometry(geomData);
                    });

                    Parallel.For(0, 100, i =>
                    {
                        //ADD A SHAPE INSTANCE
                        var shapeInstance = new XbimShapeInstance()
                        {
                            ShapeGeometryLabel = i + 1
                        };

                        var instanceId = txn.AddShapeInstance(shapeInstance, i + 1);

                    });

                    //ADD A REGIONCOLLECTION
                    var regions = new XbimRegionCollection { ContextLabel = 50 };
                    regions.Add(new XbimRegion("region1", XbimRect3D.Empty, 100, XbimMatrix3D.Identity));
                    txn.AddRegions(regions);

                    txn.Commit();
                }

            }
        }


        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void IfcStoreGeometryGeometryClearTest()
        {
            using (var model = new IO.Esent.EsentModel(ef4))
            {
                model.CreateFrom("SampleHouse4.ifc", null, null, true);
                var store = model.GeometryStore;
                using (var txn = store.BeginInit())
                {
                    //ADD A GEOMETRY SHAPE
                    var geomData = new XbimShapeGeometry()
                    {
                        IfcShapeLabel = 1,
                        Format = XbimGeometryType.BoundingBox,
                        GeometryHash = 0,
                        LOD = XbimLOD.LOD100,
                        ReferenceCount = 1,
                        ShapeData = "2123",
                        BoundingBox = XbimRect3D.Empty
                    };
                    var shapeGeomLabel = txn.AddShapeGeometry(geomData);

                    //ADD A SHAPE INSTANCE
                    var shapeInstance = new XbimShapeInstance()
                    {
                        ShapeGeometryLabel = shapeGeomLabel
                    };

                    var instanceId = txn.AddShapeInstance(shapeInstance, shapeGeomLabel);
                    Assert.IsTrue(instanceId == 1);

                    //ADD A REGIONCOLLECTION
                    var regions = new XbimRegionCollection { ContextLabel = 50 };
                    regions.Add(new XbimRegion("region1", XbimRect3D.Empty, 100, XbimMatrix3D.Identity));
                    txn.AddRegions(regions);

                    txn.Commit();
                }
                //now redo which should clear the geoemtry
                using (var txn = store.BeginInit())
                {
                    Assert.IsNotNull(txn);
                    //ADD A GEOMETRY SHAPE
                    var geomData = new XbimShapeGeometry()
                    {
                        IfcShapeLabel = 1,
                        Format = XbimGeometryType.BoundingBox,
                        GeometryHash = 0,
                        LOD = XbimLOD.LOD100,
                        ReferenceCount = 1,
                        ShapeData = "2123",
                        BoundingBox = XbimRect3D.Empty
                    };
                    var shapeGeomLabel = txn.AddShapeGeometry(geomData);

                    //ADD A SHAPE INSTANCE
                    var shapeInstance = new XbimShapeInstance()
                    {
                        ShapeGeometryLabel = shapeGeomLabel
                    };

                    var instanceId = txn.AddShapeInstance(shapeInstance, shapeGeomLabel);
                    Assert.IsTrue(instanceId == 1); //if this is 2 it has failed to clear

                    //ADD A REGIONCOLLECTION
                    var regions = new XbimRegionCollection { ContextLabel = 50 };
                    regions.Add(new XbimRegion("region1", XbimRect3D.Empty, 100, XbimMatrix3D.Identity));
                    txn.AddRegions(regions);

                    txn.Commit();
                }
                model.Close();
            }
        }

        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void EsentGeometryStoreReadTest()
        {
            using (var model = IO.Esent.EsentModel.CreateTemporaryModel(ef4))
            {
                var store = model.GeometryStore;
                using (var txn = store.BeginInit())
                {
                    //ADD A GEOMETRY SHAPE
                    var geomData = new XbimShapeGeometry()
                    {
                        IfcShapeLabel = 1,
                        Format = XbimGeometryType.BoundingBox,
                        GeometryHash = 0,
                        LOD = XbimLOD.LOD300,
                        ReferenceCount = 1,
                        ShapeData = "2123",
                        BoundingBox = XbimRect3D.Empty
                    };
                    var shapeGeomLabel = txn.AddShapeGeometry(geomData);

                    //ADD A SHAPE INSTANCE
                    var shapeInstance = new XbimShapeInstance()
                    {
                        ShapeGeometryLabel = shapeGeomLabel,
                        RepresentationContext = 50
                    };

                    var instanceId = txn.AddShapeInstance(shapeInstance, shapeGeomLabel);
                    Assert.IsTrue(instanceId == 1);

                    //ADD 2 REGIONCOLLECTIONS
                    var regions = new XbimRegionCollection { ContextLabel = 50 };
                    regions.Add(new XbimRegion("region1", XbimRect3D.Empty, 100, XbimMatrix3D.Identity));
                    txn.AddRegions(regions);
                    regions = new XbimRegionCollection { ContextLabel = 51 };
                    regions.Add(new XbimRegion("region2", XbimRect3D.Empty, 100, XbimMatrix3D.Identity));
                    txn.AddRegions(regions);
                    txn.Commit();
                }

                //start to read
                using (var reader = store.BeginRead())
                {
                    Assert.IsTrue(reader.ContextRegions.Count() == 2, "Incorrect number of regions retrieved");
                    var regionsList = reader.ContextRegions.ToList();
                    var contextIds = reader.ContextIds.ToList();
                    for (int i = 0; i < reader.ContextRegions.Count(); i++)
                    {
                        Assert.IsTrue(regionsList[i].ContextLabel == 50 + i);
                        Assert.IsTrue(contextIds[i] == 50 + i);
                    }
                    Assert.IsTrue(reader.ShapeGeometries.Count() == 1, "Should have returned one shape geometry");
                    Assert.IsTrue(reader.ShapeGeometries.First().LOD == XbimLOD.LOD300);
                    Assert.IsTrue(reader.ShapeInstances.Count() == 1, "Should have returned one shape instance");
                    Assert.IsTrue(reader.ShapeInstances.First().RepresentationContext == 50);
                    Assert.IsTrue(reader.ShapeInstancesOfContext(50).Count() == 1);
                }
                model.Close();
            }
        }
    }
}

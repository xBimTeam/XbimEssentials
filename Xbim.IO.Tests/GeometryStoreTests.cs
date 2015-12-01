using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Geometry;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4;

namespace Xbim.EsentModel.Tests
{
    [TestClass]
    public class GeometryStoreTests
    {
        [DeploymentItem("TestFiles")]
        [TestMethod]
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
                        ShapeGeometryLabel = shapeGeomLabel
                    };

                    var instanceId = txn.AddShapeInstance(shapeInstance, shapeGeomLabel);
                    Assert.IsTrue(instanceId==1);

                    //ADD A REGIONCOLLECTION
                    var regions = new XbimRegionCollection();
                    regions.ContextLabel = 50;
                    regions.Add(new XbimRegion("region1",XbimRect3D.Empty,100));
                    txn.AddRegions(regions);

                    txn.Commit();
                }              
                model.Close();
            }
        }

        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void EsentGeometryStoreBatchTest()
        {
            using (var model = new IO.Esent.EsentModel(new EntityFactory()))
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
                            ShapeGeometryLabel = i+1
                        };

                        var instanceId = txn.AddShapeInstance(shapeInstance, i + 1);
                        Assert.IsTrue(instanceId == i+1);
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
                    var regions = new XbimRegionCollection {ContextLabel = 50};
                    regions.Add(new XbimRegion("region1", XbimRect3D.Empty, 100));
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
            using (var model = IfcStore.Create(null,IfcSchemaVersion.Ifc4, XbimStoreType.EsentDatabase))
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
                    regions.Add(new XbimRegion("region1", XbimRect3D.Empty, 100));
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
            using (var model = new IO.Memory.MemoryModel(new EntityFactory()))
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
                    regions.Add(new XbimRegion("region1", XbimRect3D.Empty, 100));
                    txn.AddRegions(regions);

                    txn.Commit();
                }
                
            }
        }


        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void IfcStoreGeometryGeometryClearTest()
        {
            using (var model = new IO.Esent.EsentModel(new EntityFactory()))
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
                    var regions = new XbimRegionCollection {ContextLabel = 50};
                    regions.Add(new XbimRegion("region1", XbimRect3D.Empty, 100));
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
                    var regions = new XbimRegionCollection {ContextLabel = 50};
                    regions.Add(new XbimRegion("region1", XbimRect3D.Empty, 100));
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
            using (var model =  IO.Esent.EsentModel.CreateTemporaryModel(new EntityFactory()))
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
                        ShapeGeometryLabel = shapeGeomLabel,RepresentationContext = 50
                    };

                    var instanceId = txn.AddShapeInstance(shapeInstance, shapeGeomLabel);
                    Assert.IsTrue(instanceId == 1);

                    //ADD 2 REGIONCOLLECTIONS
                    var regions = new XbimRegionCollection {ContextLabel = 50};
                    regions.Add(new XbimRegion("region1", XbimRect3D.Empty, 100));
                    txn.AddRegions(regions);
                    regions = new XbimRegionCollection {ContextLabel = 51};
                    regions.Add(new XbimRegion("region2", XbimRect3D.Empty, 100));
                    txn.AddRegions(regions);
                    txn.Commit();
                }

                //start to read
                using (var reader = store.BeginRead())
                {
                    Assert.IsTrue(reader.Regions.Count() == 2, "Incorrect number of regions retrieved");
                    var regionsList = reader.Regions.ToList();
                    var contextIds = reader.ContextIds.ToList();
                    for (int i = 0; i < reader.Regions.Count(); i++)
                    {
                        Assert.IsTrue(regionsList[i].ContextLabel == 50 + i);
                        Assert.IsTrue(contextIds[i] == 50 + i);
                    }
                    Assert.IsTrue(reader.ShapeGeometries.Count() == 1, "Should have returned one shape geometry");
                    Assert.IsTrue(reader.ShapeGeometries.First().LOD == XbimLOD.LOD300);
                    Assert.IsTrue(reader.ShapeInstances.Count() == 1, "Should have returned one shape instance");
                    Assert.IsTrue(reader.ShapeInstances.First().RepresentationContext==50);
                    Assert.IsTrue(reader.ShapeInstancesOfContext(50).Count() == 1);
                }               
                model.Close();
            }
        }
    }
}

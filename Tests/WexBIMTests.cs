using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Geometry;
using Xbim.Common.XbimExtensions;
using Xbim.Ifc;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.Interfaces;
using Xbim.Tessellator;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class WexBIMTests
    {

        /// <summary>
        /// Reads and writes the geometry of an Ifc file to WexBIM format
        /// </summary>
        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void ReadAndWriteWexBimFile()
        {
            using (var m = IfcStore.Open("SampleHouse4.ifc"))
            {
                var wall = m.Instances[1229] as IIfcWall;
                Assert.IsNotNull(wall, "No IfcWall found in file");
                var brep = m.Instances[1213] as IIfcFacetedBrep;
                Assert.IsNotNull(brep, "No IfcFacetedBrep found in file");
                var styledItem = brep.StyledByItem.FirstOrDefault(); //this is not fast do not do on large models
                Assert.IsNotNull(styledItem, "No StyledItem found in file");



                var repContext = m.Instances[100] as IIfcGeometricRepresentationContext;
                Assert.IsNotNull(repContext, "No context found in file");
                var geomStore = m.GeometryStore;
                using (var txn = geomStore.BeginInit())
                {

                    var tessellator = new XbimTessellator(m, XbimGeometryType.PolyhedronBinary);
                    var geom = tessellator.Mesh(brep);


                    var geomId = txn.AddShapeGeometry(geom);
                    //ADD A SHAPE INSTANCE
                    var shapeInstance = new XbimShapeInstance()
                    {
                        BoundingBox = geom.BoundingBox,
                        IfcProductLabel = wall.EntityLabel,
                        IfcTypeId = m.Metadata.ExpressTypeId("IFCWALL"),
                        StyleLabel = m.Metadata.ExpressTypeId("IFCWALL") * -1,
                        RepresentationType = XbimGeometryRepresentationType.OpeningsAndAdditionsIncluded,
                        ShapeGeometryLabel = geomId,
                        Transformation = XbimMatrix3D.Identity,
                        RepresentationContext = repContext.EntityLabel
                    };
                    txn.AddShapeInstance(shapeInstance, geomId);
                    txn.Commit();
                }
                using (var bw = new BinaryWriter(new FileStream("test.wexBIM", FileMode.Create)))
                {
                    m.SaveAsWexBim(bw);
                    bw.Close();
                }
            }
            using (var fs = new FileStream(@"test.wexBIM", FileMode.Open, FileAccess.Read))
                {
                    using (var br = new BinaryReader(fs))
                    {
                        var magicNumber = br.ReadInt32();
                        Assert.IsTrue(magicNumber == IfcStoreGeometryExtensions.WexBimId);
                        var version = br.ReadByte();
                        var shapeCount = br.ReadInt32();
                        var vertexCount = br.ReadInt32();
                        var triangleCount = br.ReadInt32();
                        var matrixCount = br.ReadInt32();
                        var productCount = br.ReadInt32();
                        var styleCount = br.ReadInt32();
                        var meter = br.ReadSingle();
                        Assert.IsTrue(meter > 0);
                        var regionCount = br.ReadInt16();
                        for (int i = 0; i < regionCount; i++)
                        {
                            var population = br.ReadInt32();
                            var centreX = br.ReadSingle();
                            var centreY = br.ReadSingle();
                            var centreZ = br.ReadSingle();
                            var boundsBytes = br.ReadBytes(6 * sizeof(float));
                            var modelBounds = XbimRect3D.FromArray(boundsBytes);
                        }

                        for (int i = 0; i < styleCount; i++)
                        {
                            var styleId = br.ReadInt32();
                            var red = br.ReadSingle();
                        var green = br.ReadSingle();
                        var blue = br.ReadSingle();
                        var alpha = br.ReadSingle();
                    }
                    for (int i = 0; i < productCount; i++)
                    {
                        var productLabel = br.ReadInt32();
                        var productType = br.ReadInt16();
                        var boxBytes = br.ReadBytes(6 * sizeof(float));
                        XbimRect3D bb = XbimRect3D.FromArray(boxBytes);
                    }
                    for (int i = 0; i < shapeCount; i++)
                    {
                        var shapeRepetition = br.ReadInt32();
                        Assert.IsTrue(shapeRepetition > 0);
                        if (shapeRepetition > 1)
                        {
                            for (int j = 0; j < shapeRepetition; j++)
                            {
                                var ifcProductLabel = br.ReadInt32();
                                var instanceTypeId = br.ReadInt16();
                                var instanceLabel = br.ReadInt32();
                                var styleId = br.ReadInt32();
                                var transform = XbimMatrix3D.FromArray(br.ReadBytes(sizeof(double) * 16));
                            }
                            var triangulation = br.ReadShapeTriangulation();
                            Assert.IsTrue(triangulation.Vertices.Count > 0, "Number of vertices should be greater than zero");

                        }
                        else if (shapeRepetition == 1)
                        {
                            var ifcProductLabel = br.ReadInt32();
                            var instanceTypeId = br.ReadInt16();
                            var instanceLabel = br.ReadInt32();
                            var styleId = br.ReadInt32();
                            var triangulation = br.ReadShapeTriangulation();
                            Assert.IsTrue(triangulation.Vertices.Count>0, "Number of vertices should be greater than zero");
                        }
                    }
                }
            }
        }

    }
}

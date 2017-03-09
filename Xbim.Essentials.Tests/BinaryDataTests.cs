using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Common.Step21;
using Xbim.Ifc4.PresentationAppearanceResource;
using System.Linq;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.Interfaces;
using System.Collections.Generic;
using Xbim.Common;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class BinaryDataTests
    {
        [TestMethod]
        [DeploymentItem("TestSourceFiles\\xbim.png")]
        public void BinaryBlobTextureTest()
        {
            var data = System.IO.File.ReadAllBytes("xbim.png");
            using (var model = IfcStore.Create(IfcSchemaVersion.Ifc4, XbimStoreType.InMemoryModel))
            {
                using (var txn = model.BeginTransaction())
                {
                    var btx = model.Instances.New<IfcBlobTexture>(b => {
                        b.RasterCode = data;
                        b.RasterFormat = "PNG";
                    });
                    txn.Commit();
                }
                model.SaveAs("XbimBlobTexture.ifc");
                model.SaveAs("XbimBlobTexture.ifcxml");
            }

            //open as memory model
            using (var model = IfcStore.Open("XbimBlobTexture.ifc", null, -1))
            {
                var btx = model.Instances.FirstOrDefault<IfcBlobTexture>();
                Assert.IsNotNull(btx);

                var image = btx.RasterCode.Bytes;
                Assert.IsTrue(image.SequenceEqual(data));
            }

            //open as memory model
            using (var model = IfcStore.Open("XbimBlobTexture.ifcxml", null, -1))
            {
                var btx = model.Instances.FirstOrDefault<IfcBlobTexture>();
                Assert.IsNotNull(btx);

                var image = btx.RasterCode.Bytes;
                Assert.IsTrue(image.SequenceEqual(data));
            }

            //open using ESENT
            using (var model = IfcStore.Open("XbimBlobTexture.ifc", null, 0))
            {
                var btx = model.Instances.FirstOrDefault<IfcBlobTexture>();
                Assert.IsNotNull(btx);

                var image = btx.RasterCode.Bytes;
                Assert.IsTrue(image.SequenceEqual(data));

                //save as Esent DB
                model.SaveAs("XbimBlobTexture.xbim");
                model.Close();
            }

            //open EsentDB
            using (var model = IfcStore.Open("XbimBlobTexture.xbim", null, 0))
            {
                var btx = model.Instances.FirstOrDefault<IfcBlobTexture>();
                Assert.IsNotNull(btx);

                var image = btx.RasterCode.Bytes;
                Assert.IsTrue(image.SequenceEqual(data));

                //save from esent to IFC
                model.SaveAs("XbimBlobTexture2.ifc");
                model.Close();
            }

            //open last model as memory model
            using (var model = IfcStore.Open("XbimBlobTexture2.ifc", null, -1))
            {
                var btx = model.Instances.FirstOrDefault<IfcBlobTexture>();
                Assert.IsNotNull(btx);

                var image = btx.RasterCode.Bytes;
                Assert.IsTrue(image.SequenceEqual(data));
            }

        }

        [TestMethod]
        public void PixelTextureTest()
        {
            PixelTextureTestCode(IfcSchemaVersion.Ifc2X3);
            PixelTextureTestCode(IfcSchemaVersion.Ifc4);
        }

        private static void AssertByteArrays(List<byte[]> a, IItemSet<IfcBinary> b)
        {
            Assert.IsNotNull(a);
            Assert.IsNotNull(b);

            Assert.AreEqual(a.Count, b.Count);

            for (int i = 0; i < a.Count; i++)
            {
                Assert.IsTrue(a[i].SequenceEqual(b[i].Bytes));
            }
        }

        private static void PixelTextureTestCode(IfcSchemaVersion version)
        {
            var data = new List<byte[]>() { new byte[] { 0, 0, 255, 255 }, new byte[] { 0, 255, 255, 255 }, new byte[] { 255, 0, 255, 255 }, new byte[] { 255, 0, 0, 255 } };
            using (var model = IfcStore.Create(version, XbimStoreType.InMemoryModel))
            {
                var create = new Create(model);
                using (var txn = model.BeginTransaction())
                {
                    var pt = create.PixelTexture(t => {
                        t.Height = 2;
                        t.Width = 2;
                        t.ColourComponents = 4;
                        t.RepeatS = true;
                        t.RepeatT = true;
                        t.Pixel.AddRange(data.Select(d => new IfcBinary(d)));
                    });
                    txn.Commit();
                }
                model.SaveAs("XbimPixelTexture.ifc");
                model.SaveAs("XbimPixelTexture.ifcxml");
            }

            //open Step21 as memory model
            using (var model = IfcStore.Open("XbimPixelTexture.ifc", null, -1))
            {
                var txt = model.Instances.FirstOrDefault<IIfcPixelTexture>();
                Assert.IsNotNull(txt);

                var pixels = txt.Pixel;
                AssertByteArrays(data, pixels);
            }

            //open XML as memory model
            using (var model = IfcStore.Open("XbimPixelTexture.ifcxml", null, -1))
            {
                var txt = model.Instances.FirstOrDefault<IIfcPixelTexture>();
                Assert.IsNotNull(txt);

                var pixels = txt.Pixel;
                AssertByteArrays(data, pixels);
            }

            //open using ESENT
            using (var model = IfcStore.Open("XbimPixelTexture.ifc", null, 0))
            {
                var txt = model.Instances.FirstOrDefault<IIfcPixelTexture>();
                Assert.IsNotNull(txt);

                var pixels = txt.Pixel;
                AssertByteArrays(data, pixels);

                //save as Esent DB
                model.SaveAs("XbimPixelTexture.xbim");
                model.Close();
            }

            //open EsentDB
            using (var model = IfcStore.Open("XbimPixelTexture.xbim", null, 0))
            {
                var txt = model.Instances.FirstOrDefault<IIfcPixelTexture>();
                Assert.IsNotNull(txt);

                var pixels = txt.Pixel;
                AssertByteArrays(data, pixels);

                //save from esent to IFC
                model.SaveAs("XbimPixelTexture2.ifc");
                model.Close();
            }

            //open last model as memory model
            using (var model = IfcStore.Open("XbimPixelTexture2.ifc", null, -1))
            {
                var txt = model.Instances.FirstOrDefault<IIfcPixelTexture>();
                Assert.IsNotNull(txt);

                var pixels = txt.Pixel;
                AssertByteArrays(data, pixels);
            }

        }
    }
}

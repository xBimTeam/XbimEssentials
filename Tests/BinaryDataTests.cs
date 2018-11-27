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
using Xbim.Ifc4;
using Xbim.IO.Memory;
using System.IO;
using System.Xml;
using Xbim.Essentials.Tests.Utilities;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class BinaryDataTests
    {
        [TestCategory("IfcXml")]
        [TestMethod]
        [DeploymentItem("TestSourceFiles\\xbim.png")]
        public void BinaryBlobTextureTest()
        {
            var data = File.ReadAllBytes("xbim.png");
            using (var model = new MemoryModel(new EntityFactoryIfc4()))
            {
                using (var txn = model.BeginTransaction(""))
                {
                    var btx = model.Instances.New<IfcBlobTexture>(b => {
                        b.RasterCode = data;
                        b.RasterFormat = "PNG";
                    });
                    txn.Commit();
                }
                using (var stepFile = File.Create("XbimBlobTexture.ifc"))
                {
                    model.SaveAsStep21(stepFile);
                }
                using (var xmlFile = File.Create("XbimBlobTexture.ifcxml"))
                {
                    model.SaveAsXml(xmlFile, new XmlWriterSettings { });
                }
            }

            Action<IModel> test = model => {
                var btx = model.Instances.FirstOrDefault<IfcBlobTexture>();
                Assert.IsNotNull(btx);

                var image = btx.RasterCode.Bytes;
                Assert.IsTrue(image.SequenceEqual(data));
            };

            using (var models = new ModelFactory("XbimBlobTexture.ifc"))
            {
                models.Do(test);
            }

            using (var models = new ModelFactory("XbimBlobTexture.ifcxml"))
            {
                models.Do(test);
            }
        }

        [TestCategory("IfcXml")]
        [TestMethod]
        public void PixelTextureTest()
        {
            PixelTextureTestCode(XbimSchemaVersion.Ifc2X3);
            PixelTextureTestCode(XbimSchemaVersion.Ifc4);
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

        private static void PixelTextureTestCode(XbimSchemaVersion version)
        {
            var data = new List<byte[]>() { new byte[] { 0, 0, 255, 255 }, new byte[] { 0, 255, 255, 255 }, new byte[] { 255, 0, 255, 255 }, new byte[] { 255, 0, 0, 255 } };
            using (var model = new MemoryModel(new EntityFactoryIfc4()))
            {
                var create = new Create(model);
                using (var txn = model.BeginTransaction(""))
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

                using (var stepFile = File.Create("XbimPixelTexture.ifc"))
                {
                    model.SaveAsStep21(stepFile);
                }
                using (var xmlFile = File.Create("XbimPixelTexture.ifcxml"))
                {
                    model.SaveAsXml(xmlFile, new XmlWriterSettings { Indent = false });
                }
                
            }

            Action<IModel> test = model => {
                var txt = model.Instances.FirstOrDefault<IIfcPixelTexture>();
                Assert.IsNotNull(txt);

                var pixels = txt.Pixel;
                AssertByteArrays(data, pixels);
            };

            using (var models = new ModelFactory("XbimPixelTexture.ifc"))
            {
                models.Do(test);
            }

            using (var models = new ModelFactory("XbimPixelTexture.ifcxml"))
            {
                models.Do(test);
            }
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Common.Step21;
using Xbim.Ifc4.PresentationAppearanceResource;
using System.Linq;

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
            }
            using (var model = IfcStore.Open("XbimBlobTexture.ifc"))
            {
                var btx = model.Instances.FirstOrDefault<IfcBlobTexture>();
                Assert.IsNotNull(btx);

                var image = btx.RasterCode.Bytes;
                Assert.IsTrue(image.SequenceEqual(data));
            }

        }
    }
}

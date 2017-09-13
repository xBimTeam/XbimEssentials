using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.GeometricModelResource;
using Xbim.IO;
using Xbim.IO.Memory;

namespace Xbim.IfcCore.UnitTests
{
    [TestClass]
    public class PartialFileTests
    {
        [TestMethod]
        [DeploymentItem("TestFiles/SmallModelIfc2x3.ifc")]
        public void CreatingPartialFile()
        {
            using (var w = new StringWriter())
            {
                using (var mm = MemoryModel.OpenRead("TestFiles/SmallModelIfc2x3.ifc"))
                {
                    var extrusion = mm.Instances.FirstOrDefault<IfcExtrudedAreaSolid>();
                    ModelHelper.WritePartialFile(mm, extrusion, w, new HashSet<int>());

                    var part = w.ToString();
                    Assert.IsFalse(string.IsNullOrEmpty(part));

                    using (var insModel = new MemoryModel(new EntityFactory()))
                    {
                        insModel.InsertCopy(extrusion, new Common.XbimInstanceHandleMap(mm, insModel), null, false, true, true);
                        using (var partModel = new MemoryModel(new EntityFactory()))
                        {
                            partModel.Header.FileSchema.Schemas.Add("IFC2X3");
                            partModel.LoadStep21Part(part);
                            Assert.IsTrue(insModel.Instances.Count == partModel.Instances.Count);

                            var ext2 = partModel.Instances.FirstOrDefault<IfcExtrudedAreaSolid>();
                            Assert.IsTrue(ext2.Depth == extrusion.Depth);
                            Assert.IsTrue(ext2.ExtrudedDirection.X == extrusion.ExtrudedDirection.X);
                            Assert.IsTrue(ext2.ExtrudedDirection.Y == extrusion.ExtrudedDirection.Y);
                            Assert.IsTrue(ext2.ExtrudedDirection.Z == extrusion.ExtrudedDirection.Z);
                        }
                    }

                }
            }
        }
    }
}

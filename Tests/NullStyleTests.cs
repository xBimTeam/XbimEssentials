using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.PresentationAppearanceResource;
using Xbim.Ifc2x3.PresentationOrganizationResource;
using Xbim.IO.Memory;
using System.Linq;
using Xbim.Common;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class NullStyleTests
    {
        private static readonly IEntityFactory ef = new EntityFactoryIfc2x3();
        [TestMethod]
        public void SerializationTest()
        {
            var model = new MemoryModel(ef);
            using (var txn = model.BeginTransaction("Null style"))
            {
                model.Instances.New<IfcPresentationLayerWithStyle>(ls => ls.LayerStyles.Add(new IfcNullStyle()));
                txn.Commit();
                using (var fileStream = new StreamWriter("NullStyle.ifc"))
                {
                    model.SaveAsStep21(fileStream);
                }
            }

            model = new MemoryModel(ef);
            model.LoadStep21("NullStyle.ifc");
            var style = model.Instances.FirstOrDefault<IfcPresentationLayerWithStyle>();
            var nStyle = style.LayerStyles.FirstOrDefault();

            Assert.IsTrue(nStyle is IfcNullStyle);
            Assert.IsTrue(((IfcNullStyle)nStyle).Value is IfcNullStyleEnum);
            Assert.IsTrue((IfcNullStyleEnum)(((IfcNullStyle)nStyle).Value) == IfcNullStyleEnum.NULL);
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.PresentationAppearanceResource;
using Xbim.Ifc2x3.PresentationOrganizationResource;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class NullStyleTests
    {
        [TestMethod]
        public void SerializationTest()
        {
            var model = new MemoryModel<EntityFactory>();
            using (var txn = model.BeginTransaction("Null style"))
            {
                model.Instances.New<IfcPresentationLayerWithStyle>(ls => ls.LayerStyles.Add(new IfcNullStyle()));
                model.Save("NullStyle.ifc");
                txn.Commit();
            }

            model = new MemoryModel<EntityFactory>();
            model.Open("NullStyle.ifc");
            var style = model.Instances.FirstOrDefault<IfcPresentationLayerWithStyle>();
            var nStyle = style.LayerStyles.FirstOrDefault();

            Assert.IsTrue(nStyle is IfcNullStyle);
            Assert.IsTrue(((IfcNullStyle)nStyle).Value is IfcNullStyleEnum);
            Assert.IsTrue((IfcNullStyleEnum)(((IfcNullStyle)nStyle).Value) == IfcNullStyleEnum.NULL);
        }
    }
}

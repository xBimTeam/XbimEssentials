using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Logging;
using Xbim.Ifc2x3;
using Xbim.IO.Memory;
using Xbim.IO.Translation;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class SchemaTranslation
    {
        [DeploymentItem("TestFiles")]
        [TestMethod]
        public void TranslateIfc2x3To4()
        {
            using (var source = new MemoryModel<EntityFactory>())
            {
                source.Open("4walls1floorSite.ifc");
                using (var target = new MemoryModel<Ifc4.EntityFactory>())
                {
                    var translator = new ModelTranslator(source, target);
                    using (var txn = target.BeginTransaction("Conversion"))
                    {
                        translator.Translate();
                        txn.Commit();
                    }
                    target.Save("..\\..\\4walls1floorSite_IFC4.ifc");
                }
            }
        }
    }
}

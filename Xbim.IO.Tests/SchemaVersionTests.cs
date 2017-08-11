using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Step21;

namespace Xbim.MemoryModel.Tests
{
    [TestClass]
    public class SchemaVersionTests
    {
        [TestMethod]
        public void CheckSchemaVersions()
        {
            #region Memory Models
            using (var model = new IO.Memory.MemoryModel(new Ifc2x3.EntityFactory()))
            {
                var iModel = model as IModel;
                Assert.IsTrue(model.SchemaVersion == IfcSchemaVersion.Ifc2X3);
            }
            using (var model = new IO.Memory.MemoryModel(new Ifc4.EntityFactory()))
            {
                var iModel = model as IModel;
                Assert.IsTrue(model.SchemaVersion == IfcSchemaVersion.Ifc4);
            }
            using (var model = new IO.Memory.MemoryModel(new CobieExpress.EntityFactory()))
            {
                var iModel = model as IModel;
                Assert.IsTrue(model.SchemaVersion == IfcSchemaVersion.Cobie2X4);
            }
            #endregion
            #region Esent Models
            using (var model = new IO.Esent.EsentModel(new Ifc2x3.EntityFactory()))
            {
                var iModel = model as IModel;
                Assert.IsTrue(model.SchemaVersion == IfcSchemaVersion.Ifc2X3);
            }
            using (var model = new IO.Esent.EsentModel(new Ifc4.EntityFactory()))
            {
                var iModel = model as IModel;
                Assert.IsTrue(model.SchemaVersion == IfcSchemaVersion.Ifc4);
            }
            using (var model = new IO.Esent.EsentModel(new CobieExpress.EntityFactory()))
            {
                var iModel = model as IModel;
                Assert.IsTrue(model.SchemaVersion == IfcSchemaVersion.Cobie2X4);
            }
            #endregion
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Common.Step21;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class SchemaVersionTests
    {
        [TestMethod]
        public void CheckSchemaVersions()
        {
            #region Memory Models
            using (var model = new IO.Memory.MemoryModel(new Ifc2x3.EntityFactoryIfc2x3()))
            {
                var iModel = model as IModel;
                Assert.IsTrue(model.SchemaVersion == XbimSchemaVersion.Ifc2X3);
            }
            using (var model = new IO.Memory.MemoryModel(new Ifc4.EntityFactoryIfc4()))
            {
                var iModel = model as IModel;
                Assert.IsTrue(model.SchemaVersion == XbimSchemaVersion.Ifc4);
            }
            using (var model = new IO.Memory.MemoryModel(new Ifc4.EntityFactoryIfc4x1()))
            {
                var iModel = model as IModel;
                Assert.IsTrue(model.SchemaVersion == XbimSchemaVersion.Ifc4x1);
            }
            
            #endregion
            #region Esent Models
            using (var model = new IO.Esent.EsentModel(new Ifc2x3.EntityFactoryIfc2x3()))
            {
                var iModel = model as IModel;
                Assert.IsTrue(model.SchemaVersion == XbimSchemaVersion.Ifc2X3);
            }
            using (var model = new IO.Esent.EsentModel(new Ifc4.EntityFactoryIfc4()))
            {
                var iModel = model as IModel;
                Assert.IsTrue(model.SchemaVersion == XbimSchemaVersion.Ifc4);
            }
            using (var model = new IO.Esent.EsentModel(new Ifc4.EntityFactoryIfc4x1()))
            {
                var iModel = model as IModel;
                Assert.IsTrue(model.SchemaVersion == XbimSchemaVersion.Ifc4x1);
            }
          
            #endregion
        }
    }
}

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Ifc2x3;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class EsentHeaderPersistenceTests
    {
        private static readonly IEntityFactory ef4 = new Ifc4.EntityFactoryIfc4();
        private static readonly IEntityFactory ef2x3 = new Ifc2x3.EntityFactoryIfc2x3();

        [TestMethod]
        public void TestHeaderPersistance()
        {
            const string db = "headertest.xbim";
            const string name = "Testing model";
            const string schema = "TEST";
            using (var model = IO.Esent.EsentModel.CreateModel(ef2x3, db))
            {
                Assert.IsNotNull(model.Header);
                var called = 0;
                model.Header.PropertyChanged += (sender, args) => called++;

                model.Header.FileName.Name = name;
                model.Header.FileSchema.Schemas.Clear();
                model.Header.FileSchema.Schemas.Add(schema);

                Assert.AreEqual(3, called);
            }

            using (var model = new IO.Esent.EsentModel(ef2x3))
            {
                model.Open(db);
                Assert.AreEqual(name, model.Header.FileName.Name);
                Assert.AreEqual(schema, model.Header.FileSchema.Schemas.First());
            }


        }
    }
}

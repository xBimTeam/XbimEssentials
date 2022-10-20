using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc2x3.Interfaces;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO.Esent;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class IfcStoreTests
    {
        [TestMethod]
        public void UseEsentModelProvider_DifferentCounts_ShouldBeConsistent()
        {
            var file = "TestFiles\\CountTestsModel.ifc";
            EsentModelProvider provider = new EsentModelProvider();
            var model = provider.Open(file, Common.Step21.XbimSchemaVersion.Ifc2X3); ;
            long countInsideTransactionAfterNewWall;

            using (var t = model.BeginTransaction("Hello Test"))
            {

                var countInsideTransactionBeforeNewWall = model.Instances.CountOf<IIfcProduct>();
                var newWall = model.Instances.New<IfcWall>();
                countInsideTransactionAfterNewWall = model.Instances.CountOf<IIfcProduct>();

                Assert.AreEqual(countInsideTransactionBeforeNewWall + 1, countInsideTransactionAfterNewWall, "Count after creation should be count before creation + 1.");

                t.Commit();
            }
            var countOfAfterCommiting = model.Instances.CountOf<IIfcProduct>();
            var ofTypeCountAfterCommiting = model.Instances.OfType<IIfcProduct>().LongCount();

            Assert.AreEqual(countOfAfterCommiting, countInsideTransactionAfterNewWall, "Count after creation should be count after commiting using CountOf.");
            Assert.AreEqual(countOfAfterCommiting, ofTypeCountAfterCommiting, "Count after creation should be count after commiting counting the result of OfType.");
        }
    }
}

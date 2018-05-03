using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Memory;
using Xbim.Essentials.Tests.Utilities;

namespace Xbim.Essentials.Tests
{
    /// <summary>
    /// Summary description for SearchTests
    /// </summary>
    [TestClass]
    public class SearchTests
    {
        private static readonly IEntityFactory ef4 = new Ifc4.EntityFactoryIfc4();
        private static readonly IEntityFactory ef2x3 = new Ifc2x3.EntityFactoryIfc2x3();

        [TestMethod]
        public void SearchTypeHandling()
        {
            const string file = "SearchTypeHandling.ifc";
            ModelFactory.Create(file, Common.Step21.XbimSchemaVersion.Ifc2X3, InitModel);

            using (var models = new ModelFactory(file))
            {
                models.Do(AssertModel);
            }
        }

        private void InitModel(IModel model)
        {
            model.Instances.New<IfcWall>(w => w.Name = "Wall A");
            model.Instances.New<IfcWall>(w => w.Name = "Wall B");
            model.Instances.New<IfcWall>(w => w.Name = "Wall C");
            model.Instances.New<IfcWallStandardCase>(w => w.Name = "Standard Wall A");
            model.Instances.New<IfcWallStandardCase>(w => w.Name = "Standard Wall B");
        }

        private void AssertModel(IModel model)
        {
            Assert.AreEqual(5, model.Instances.OfType<IfcWall>().Count());
            Assert.AreEqual(5, model.Instances.OfType<IIfcWall>().Count());
            Assert.AreEqual(5, model.Instances.Where<IfcWall>(w => true).Count());
            Assert.AreEqual(5, model.Instances.Where<IIfcWall>(w => true).Count());
        }


        [TestMethod]
        [DeploymentItem("TestSourceFiles/4walls1floorSite.ifc")]
        public void CanSearchSite()
        {
            using (var model = MemoryModel.OpenRead("4walls1floorSite.ifc"))
            {
                var sites = model.Instances.OfType<IIfcSite>().ToList();
                Assert.IsTrue(sites.Any());
            }
        }
    }
}

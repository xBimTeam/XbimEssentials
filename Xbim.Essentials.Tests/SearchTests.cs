using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
using Xbim.Ifc;
using Xbim.Ifc2x3;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Esent;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    /// <summary>
    /// Summary description for SearchTests
    /// </summary>
    [TestClass]
    public class SearchTests
    {
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void SearchTypeHandling()
        {
            using (var model = EsentModel.CreateTemporaryModel(new EntityFactory()))
            {
                InitModel(model);
                AssertModel(model);
            }

            using (var model = new MemoryModel(new EntityFactory()))
            {
                InitModel(model);
                AssertModel(model);
            }
        }

        private void InitModel(IModel model)
        {
            using (var txn = model.BeginTransaction("txn"))
            {
                model.Instances.New<IfcWall>(w => w.Name = "Wall A");
                model.Instances.New<IfcWall>(w => w.Name = "Wall B");
                model.Instances.New<IfcWall>(w => w.Name = "Wall C");
                model.Instances.New<IfcWallStandardCase>(w => w.Name = "Standard Wall A");
                model.Instances.New<IfcWallStandardCase>(w => w.Name = "Standard Wall B");

                txn.Commit();
            }
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
            using (var model = IfcStore.Open("4walls1floorSite.ifc"))
            {
                var sites = model.Instances.OfType<IIfcSite>().ToList();
                Assert.IsTrue(sites.Any());
            }
        }
    }
}

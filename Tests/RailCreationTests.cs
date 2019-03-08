using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.IfcRail.RailwayDomain;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class RailCreationTests
    {
        [TestMethod]
        public void CreateBasicFile()
        {
            var file = "railway.ifc";
            using (var model = IfcStore.Create(XbimSchemaVersion.IfcRail, IO.XbimStoreType.InMemoryModel))
            {
                using (var txn = model.BeginTransaction())
                {
                    var i = model.Instances;
                    i.New<IfcRailway>(r => r.Name = "The very first railway");
                    txn.Commit();
                }
                model.SaveAs(file);
            }

            using (var model = IfcStore.Open(file))
            {
                var r = model.Instances.FirstOrDefault<IfcRailway>();
                Assert.IsNotNull(r);
            }
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Common.Step21;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class OwnerHistoryTests
    {
        [TestMethod]
        public void EmptyEditorCredentials()
        {
            using (var model = IfcStore.Create(IfcSchemaVersion.Ifc4, XbimStoreType.InMemoryModel))
            {
                using (var txn = model.BeginTransaction())
                {
                    var create = new Create(model);
                    var wall = create.Wall(w => w.Name = "New wall");

                    var usr = model.DefaultOwningUser;
                }
            }
        }
    }
}

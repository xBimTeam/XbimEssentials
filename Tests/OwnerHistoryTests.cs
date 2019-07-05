using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class OwnerHistoryTests
    {
        [TestMethod]
        public void EmptyEditorCredentials()
        {
            using (var model = IfcStore.Create(XbimSchemaVersion.Ifc4, XbimStoreType.InMemoryModel))
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

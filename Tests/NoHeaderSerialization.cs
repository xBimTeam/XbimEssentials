using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Xbim.Common;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc2x3.UtilityResource;
using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class NoHeaderSerialization
    {
        [TestMethod]
        public void NoHeaderTest()
        {
            var editor = new XbimEditorCredentials
            {
                ApplicationDevelopersName = "You",
                ApplicationFullName = "Your app",
                ApplicationIdentifier = "Your app ID",
                ApplicationVersion = "4.0",
                //your user
                EditorsFamilyName = "Santini Aichel",
                EditorsGivenName = "Johann Blasius",
                EditorsOrganisationName = "Independent Architecture"
            };

            const string file = "header_test.ifc";
            const string xmlfile = "header_test.ifcxml";

            //normal way to create and save with memory model
            using (var model = IfcStore.Create(editor, XbimSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel))
            {
                CreateWall(model);
                model.SaveAs(file);
                model.SaveAs(xmlfile);
                var c = Directory.GetCurrentDirectory();
            }
            using (var model = IfcStore.Open(file))
            {
                CheckWall(model);
            }

            //nullified header schema in memory model
            using (var model = IfcStore.Create(editor, XbimSchemaVersion.Ifc2X3, XbimStoreType.InMemoryModel))
            {
                CreateWall(model);
                model.Header.FileSchema = null;
                Assert.IsNull(model.Header.FileSchema);
                model.SaveAs(file);
            }
            using (var model = IfcStore.Open(file))
            {
                CheckWall(model);
            }
        }

        private const string _wallName = "First wall ever";
        private readonly IfcGloballyUniqueId _wallId = IfcGloballyUniqueId.FromGuid(Guid.NewGuid());

        private void CreateWall(IModel model)
        {
            using (var txn = model.BeginTransaction("Wall creation"))
            {
                var wall = model.Instances.New<IfcWall>();
                wall.Name = _wallName;
                wall.GlobalId = _wallId;
                txn.Commit();
            }
        }

        private void CheckWall(IModel model)
        {
            var wall = model.Instances.FirstOrDefault<IfcWall>();
            Assert.IsTrue(wall.Name == _wallName);
            Assert.IsTrue(wall.GlobalId == _wallId);
        }
            
    }
}

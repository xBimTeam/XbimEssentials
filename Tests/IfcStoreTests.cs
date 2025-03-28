using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Esent;
using Xbim.IO.Step21;

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
                countInsideTransactionAfterNewWall.Should().Be(countInsideTransactionBeforeNewWall + 1, "Adding wall adds one");

                t.Commit();
            }
            var countOfAfterCommiting = model.Instances.CountOf<IIfcProduct>();
            var ofTypeCountAfterCommiting = model.Instances.OfType<IIfcProduct>().LongCount();

            countOfAfterCommiting.Should().Be(countInsideTransactionAfterNewWall);
            countOfAfterCommiting.Should().Be(ofTypeCountAfterCommiting);
        }


        [TestMethod]
        public void AddingUpdatingElementsAddsApplicationOnce()
        {
            var updatedFile = "4walls1floorSiteDoorAmended.ifc";
            long count;
            List<int> origLabels;
            XbimEditorCredentials editor = CreateEditor();
            using (var ifcStore = IfcStore.Open("TestFiles\\4walls1floorSite.ifc", editor))
            {
                count = ifcStore.Instances.Count;
                origLabels = ifcStore.Instances.Select(x => x.EntityLabel).ToList();
                AddProduct("Door1", ifcStore);
                DumpNewItems("first run", ifcStore, origLabels);

                var diffCount = ifcStore.Instances.Count - count;
                diffCount.Should().Be(8);   // Door + Application, OwnerHistory, Org * 2, Person, PersonOrg

                ifcStore.SaveAs(updatedFile);
                ifcStore.Close();
            }
            using (var ifcStore = IfcStore.Open(updatedFile, editor))
            {
                count = ifcStore.Instances.Count;
                origLabels = ifcStore.Instances.Select(x => x.EntityLabel).ToList();
                AddProduct("Door2", ifcStore);
                var toEdit = ifcStore.Instances.OfType<IIfcWall>().First();
                var originator = toEdit.OwnerHistory;
                EditProduct(toEdit, "Wall 3", ifcStore);
                DumpNewItems("added second run", ifcStore, origLabels);

                var diffCount = ifcStore.Instances.Count - count;
                diffCount.Should().Be(3, "Only the door and 2 x OwnerHistory is created. Application, Person etc re-used");
                
                DumpItems("affected items", ifcStore, new[] { toEdit.EntityLabel, originator.EntityLabel });
            }

        }

        

        private XbimEditorCredentials CreateEditor()
        {
            return new XbimEditorCredentials
            {
                ApplicationFullName = "Xbim Essentials Unit tests",
                ApplicationIdentifier = "xbim-tests-1.0",
                ApplicationDevelopersName = "xbim Team",
                ApplicationVersion = "6.0.0",

                EditorsIdentifier = "end.user@acme.com",
                EditorsFamilyName = "End",
                EditorsGivenName = "User",
                EditorsOrganisationIdentifier = "org.acme",
                EditorsOrganisationName = "Acme Inc"
            };
        }

        private void DumpNewItems(string message, IfcStore ifcStore, IEnumerable<int> origLabels)
        {
            var newLabels = ifcStore.Instances.Select(x => x.EntityLabel).ToList();
            var newLabelList = newLabels.Except(origLabels);
            Console.WriteLine(message);
            foreach (var item in newLabelList)
            {
                Console.WriteLine(ifcStore.Instances[item].ToString());
            }
        }

        private void DumpItems(string message, IfcStore ifcStore, IEnumerable<int> items)
        {
            Console.WriteLine(message);
            foreach (var item in items)
            {
                Console.WriteLine(ifcStore.Instances[item].ToString());
            }
        }

        private static void AddProduct(string productName, IfcStore ifcStore)
        {
            using (var txn = ifcStore.BeginTransaction("Adding"))
            {
                var door = ifcStore.Instances.New<IfcDoor>();
                door.Name = productName;
                door.GlobalId = Guid.NewGuid().ToPart21();
                txn.Commit();
            }
        }

        private void EditProduct<T>(T element, string newName, IfcStore ifcStore) where T: IIfcRoot
        {
            using (var txn = ifcStore.BeginTransaction("Adding"))
            {
                element.Name = newName + " Updated";
                element.Description = "Edited";
                txn.Commit();
            }
        }
    }
}

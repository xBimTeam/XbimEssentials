using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Presentation;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class EntitySelectionTests
    {
        [TestMethod]
        [DeploymentItem(@"TestSourceFiles")]
        public void CanAddEntity()
        {
            DirectoryInfo d = new DirectoryInfo(".");
            Debug.WriteLine(d.FullName);
            using (var model = IfcStore.Open(@"P1.xbim"))
            {
                EntitySelection sel = new EntitySelection();
                sel.CollectionChanged += Sel_CollectionChanged;
                

                var adding = model.Instances.OfType<IIfcWall>().FirstOrDefault();
                sel.Add(adding);

                //Assert.IsTrue(restoredOk, "The XbimInstanceHandle could not be found in the dictionary");
                //Assert.AreEqual(1, restored);
            }
        }

        private void Sel_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in e.NewItems)
            {
                Debug.WriteLine(item);
            }
        }
    }
}

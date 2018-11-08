using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xbim.Common;
using Xbim.Ifc4;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    [DeploymentItem("TestSourceFiles\\Alignment")]
    public class IfcAlignmentTests
    {
        private static readonly IEntityFactory ef = new EntityFactoryIfc4x1();
        private static readonly string[] files = new[] {
            "bloss-curve.ifc",
            "horizontal-alignment.ifc",
            "linear-placement.ifc",
            "sectioned-solid.ifc",
            "terrain-and-alignment.ifc",
            "terrain-surface.ifc",
            "vertical-alignment.ifc",
            //"ramp.ifc"
        };

        [TestMethod]
        public void LoadIfcAlignment()
        {
            foreach (var file in files)
            {
                using (var model = new MemoryModel(ef))
                {
                    var errs = model.LoadStep21(file);
                    Assert.AreEqual(0, errs);

                    var posEl = model.Instances.OfType<IfcPositioningElement>();
                    Assert.IsTrue(posEl.Any());
                }

                using (var store = MemoryModel.OpenRead(file))
                {
                    var posEl = store.Instances.OfType<IfcPositioningElement>();
                    Assert.IsTrue(posEl.Any());
                }
            }
        }

        [TestMethod]
        public void CreateIfcAlignment()
        {
            using (var model = new MemoryModel(ef))
            {
                using (var txn = model.BeginTransaction("Init"))
                {
                    var a = model.Instances.New<IfcAlignment>();
                    a.GlobalId = Guid.NewGuid();
                    a.Name = "Testing alignment";
                    a.Axis = model.Instances.New<IfcAlignmentCurve>();
                    txn.Commit();
                }
                using (var w = File.CreateText("xbim_alignment.ifc"))
                {
                    model.SaveAsStep21(w);
                    w.Close();
                }
            }

            using (var model = new MemoryModel(ef))
            {
                model.LoadStep21("xbim_alignment.ifc");

                var a = model.Instances.FirstOrDefault<IfcAlignment>();
                Assert.IsNotNull(a);

                Assert.IsNotNull(a.Axis);
            }
        }
    }
}

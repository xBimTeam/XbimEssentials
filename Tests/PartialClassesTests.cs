using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class PartialClassesTests
    {
        [TestMethod]
        [DeploymentItem("TestSourceFiles\\4walls1floorSite.ifc")]
        [DeploymentItem("TestSourceFiles\\AlmostEmptyIFC4.ifc")]
        public void PartialMultiSchemaOk()
        {
            //// this can throw an exception if the model is not inside a transaction
            //using (var model = IfcStore.Open(@"AlmostEmptyIFC4.ifc"))
            //{
            //    var instance = model.Instances[53] as Ifc4.MeasureResource.IfcSIUnit;
            //    var dimensions = instance.Dimensions;
            //    Debug.WriteLine(dimensions.ToString());
            //}

            // this can throw an exception if the model is not inside a transaction
            using (var model = IfcStore.Open(@"4walls1floorSite.ifc"))
            {
                var instance = model.Instances[142] as IIfcWall;
                var v = instance.IsContainedIn;
                Debug.WriteLine(v.ToString());

                var t2 = instance.Material;
                Debug.WriteLine(t2.ToString());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc2x3.ActorResource;
using Xbim.Ifc2x3.Extensions;
using Xbim.Ifc2x3.ProductExtension;
using Xbim.Ifc2x3.RepresentationResource;
using Xbim.Ifc2x3.SharedBldgElements;
using Xbim.IO;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class TolerantToIfcGeometryErrors
    {
        [TestMethod]
        public void ToleratesOddBodySpecs()
        {
            //// Suspended
            //using (var model = new XbimModel())
            //{
            //    model.CreateFrom("C:\\Users\\Claudio\\OneDrive\\Benghi\\2017 - migliorie e bug\\2017 08 04 - file - consteel\\Hall with podium.ifc", null, null, true);
            //    var rep = model.Instances[64] as IfcShapeRepresentation;
            //    Assert.IsTrue(rep.IsBodyRepresentation(), "Should be a body.");

            //    model.Close();
            //}
        }
    }
}

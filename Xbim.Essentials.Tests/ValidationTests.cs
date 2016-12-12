using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common.Enumerations;
using Xbim.Ifc;
using Xbim.Ifc.Validation;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class ValidationTests
    {

        [TestMethod]
        [DeploymentItem("TestSourceFiles")]
        public void ReadListOfListFromIfcFile()
        {
            TextWriter t = new StringWriter();
            using (var model = IfcStore.Open("AlmostEmptyIFC4.ifc", null, 0))
            {
                var v = new IfcValidator(model);
                v.Validate(t, ValidationFlags.All);

            }
        }
    }
}
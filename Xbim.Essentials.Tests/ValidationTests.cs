using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Common;
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
            using (var model = IfcStore.Open("InvalidContentFC4.ifc", null, 0))
            {
                //var v2 = model.Metadata.Types()
                //    .Where(x => x.Properties.Any(pr => pr.Value.EntityAttribute.State == EntityAttributeState.Mandatory
                //    && typeof(IExpressValueType).IsAssignableFrom(pr.Value.PropertyInfo.PropertyType)
                //    ));
                var v = new IfcValidator(model);
                var cntError = v.Validate(t, ValidationFlags.All);
                Debug.WriteLine($"Validation error count: {cntError}");
                Debug.Write(t.ToString());
            }
        }
    }
}
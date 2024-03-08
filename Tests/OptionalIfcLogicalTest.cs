using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Ifc2x3.MeasureResource;
using Xbim.Ifc4.Interfaces;
using Xbim.IO.Memory;

namespace Xbim.Essentials.Tests
{
    /// <summary>
    /// For failing on IfcMaterialLayer.IsVentilated : OPTIONAL IfcLogical
    /// </summary>
    [TestClass]
    public class OptionalIfcLogicalTest
    {
        [TestMethod]
        public void IfcMaterialLayerTest()
        {
            var ifcPath = "TestFiles/IfcMaterialLayerTestFile.ifc";
            using(MemoryModel model = MemoryModel.OpenReadStep21(ifcPath))
            {
                var entity = model.Instances[347] as IIfcMaterialLayer;
                Assert.IsNotNull(entity);
                Assert.IsNotNull(entity.IsVentilated);
                Assert.Equals(entity.IsVentilated.ToString(), ".U.");
                Assert.Equals(entity.ToString(), 
                    "#347=IFCMATERIALLAYER(#326,417.,.U.,'Component 1',$,$,$);");
            }
        }
    }
}

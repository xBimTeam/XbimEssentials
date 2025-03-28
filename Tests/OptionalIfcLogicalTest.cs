using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private const string ifcFile = "TestFiles/IfcMaterialLayerTestFile.ifc";

        [TestMethod]
        public void UnknownOptionalLogicalShouldNotBeNull()
        {
 
            using(MemoryModel model = MemoryModel.OpenReadStep21(ifcFile))
            {
                var entity = model.Instances[347] as IIfcMaterialLayer;
                entity.Should().NotBeNull();
                // Spot Fix - what the parser should do
                //using var tx = model.BeginTransaction("");
                //entity.IsVentilated = (bool?)null;
                //
                entity.IsVentilated.Should().NotBeNull();   // Fails as we treat .U. as $ rather than as IfcLogical with value unknown.
                entity.IsVentilated.HasValue.Should().BeTrue();
                entity.IsVentilated.Value.Value.Should().BeNull();
                entity.IsVentilated.ToString().Should().Be("unknown");
                entity.IsVentilated.Value.Should().Be(new Ifc4.MeasureResource.IfcLogical());
                entity.ToString().Should().Be("#347=IFCMATERIALLAYER(#326,417.,.U.,'Component 1',$,$,$);");
            }
        }

        [TestMethod]
        public void TrueOptionalLogicalShouldBeTrue()
        {

            using (MemoryModel model = MemoryModel.OpenReadStep21(ifcFile))
            {
                var element = 502;
                var entity = model.Instances[element] as IIfcMaterialLayer;
                entity.Should().NotBeNull();
                entity.IsVentilated.HasValue.Should().BeTrue();
                entity.IsVentilated.Value.Value.Should().BeOfType<bool>().And.Be(true);
                entity.IsVentilated.ToString().Should().Be("true");
                entity.IsVentilated.Value.Should().Be(new Ifc4.MeasureResource.IfcLogical(true));
                entity.ToString().Should().Be($"#{element}=IFCMATERIALLAYER(#326,417.,.T.,'Component 1',$,$,$);");
            }
        }

        [TestMethod]
        public void FalseOptionalLogicalShouldBeFalse()
        {

            using (MemoryModel model = MemoryModel.OpenReadStep21(ifcFile))
            {
                var element = 503;
                var entity = model.Instances[element] as IIfcMaterialLayer;
                entity.Should().NotBeNull();
                entity.IsVentilated.HasValue.Should().BeTrue();
                entity.IsVentilated.Value.Value.Should().BeOfType<bool>().And.Be(false);
                entity.IsVentilated.ToString().Should().Be("false");
                entity.IsVentilated.Value.Should().Be(new Ifc4.MeasureResource.IfcLogical(false));
                entity.ToString().Should().Be($"#{element}=IFCMATERIALLAYER(#326,417.,.F.,'Component 1',$,$,$);");
            }
        }

        [TestMethod]
        public void TrueBooleanShouldBeTrue()
        {

            using (MemoryModel model = MemoryModel.OpenReadStep21(ifcFile))
            {
                var element = 500;
                var entity = model.Instances[element] as IIfcPropertySingleValue;
                entity.Should().NotBeNull();

                entity.NominalValue.Value.Should().BeOfType<bool>().And.Be(true);
                entity.NominalValue.ToString().Should().Be("true");
                entity.NominalValue.Should().Be(new Ifc4.MeasureResource.IfcBoolean(true));
                entity.ToString().Should().Be($"#{element}=IFCPROPERTYSINGLEVALUE('IsExternal',$,IFCBOOLEAN(.T.),$);");
                entity.NominalValue.Equals(true).Should().BeFalse("not implicitly cast");
            }
        }

        [TestMethod]
        public void FalseBooleanShouldBeFalse()
        {

            using (MemoryModel model = MemoryModel.OpenReadStep21(ifcFile))
            {
                var element = 501;
                var entity = model.Instances[element] as IIfcPropertySingleValue;
                entity.Should().NotBeNull();

                entity.NominalValue.Value.Should().BeOfType<bool>().And.Be(false);
                entity.NominalValue.Should().Be(new Ifc4.MeasureResource.IfcBoolean(false));
                entity.ToString().Should().Be($"#{element}=IFCPROPERTYSINGLEVALUE('IsExternal',$,IFCBOOLEAN(.F.),$);");
                entity.NominalValue.Equals(false).Should().BeFalse("not implicitly cast");
            }
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Xbim.Essentials.Tests
{
    [TestClass]
    public class DerivedUnitTests
    {
        [TestMethod]
        public void CheckIfcDerivedUnit()
        {
            var path = @"TestSourceFiles\\IfcDerivedUnit.ifcxml";
            var store = IfcStore.Open(path);
            var derivedUnit = store.Instances.FirstOrDefault<IfcDerivedUnit>(u => u.UnitType == IfcDerivedUnitEnum.USERDEFINED);
            var conversionBasedUnit = store.Instances.FirstOrDefault<IIfcConversionBasedUnit>();
            // dim_cbu is null
            var dim_cbu = conversionBasedUnit.Dimensions;
            // System.NullReferenceException   
            var dim_derivedUnit = derivedUnit.Dimensions;

        }
    }
}

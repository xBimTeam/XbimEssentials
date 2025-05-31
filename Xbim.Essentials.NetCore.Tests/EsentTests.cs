using FluentAssertions;
using System.IO;
using System.Linq;
using Xbim.Ifc;
using Xbim.Ifc4;
using Xbim.IO.Esent;
using Xunit;

namespace Xbim.Essentials.NetCore.Tests
{
    public class EsentTests
    {
        [Fact]
        public void CanCreateEsentModel()
        {
            var esentModel = new Xbim.IO.Esent.EsentModel(new EntityFactoryIfc4());
            esentModel.Should().NotBeNull();
        }

        [Fact]
        public void EsentShouldOptimiseSaveAs()
        {
            var file = @"TestFiles\SampleHouse4.ifc";
            var esentModelFile = Path.ChangeExtension(file, ".esent.xbim");
            File.Delete(esentModelFile); // delete any existing file
            using (var ifcStore = IfcStore.Open(file, null, 0)) // 0 is the database triggering threshold, test esent databases
            {
                var count = ifcStore.Instances.Count;
                count.Should().BeGreaterThan(0, "original model is not empty");
                ifcStore.SaveAs(esentModelFile);
                ifcStore.Close();
            }
            File.Exists(esentModelFile).Should().BeTrue("model should be saved as esent file");
            CoverageProbes.HeuristicModelProvider_EfficientEsentSaveasHit.Should().BeTrue("the efficient save-as path should have been hit");
        }
    }
}

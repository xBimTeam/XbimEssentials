using FluentAssertions;
using Xbim.Ifc4;
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
    }
}

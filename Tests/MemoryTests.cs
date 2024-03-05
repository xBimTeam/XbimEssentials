using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xbim.Common;
using Xbim.Ifc;
using Xunit;

namespace Xbim.Essentials.Tests
{
    [Collection(nameof(xUnitBootstrap))]
    public class MemoryTests
    {
        [Fact]
        public void Shoud_free_memory_after_released()
        {
            var initialMemory = GC.GetTotalMemory(true);
            var modelMemory = 0L;

            using (var model = IfcStore.Open(@"TestFiles\4walls1floorSite.ifc"))
            {
                modelMemory = GC.GetTotalMemory(true);
            }

            GC.Collect();
            var finalMemory = GC.GetTotalMemory(true);
            finalMemory.Should().BeLessThan(modelMemory);
        }
    }
}

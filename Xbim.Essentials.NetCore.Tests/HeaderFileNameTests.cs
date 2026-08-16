using FluentAssertions;
using Xbim.Common.Step21;
using Xbim.Ifc;
using Xbim.IO.Memory;
using Xunit;

namespace Xbim.Essentials.NetCore.Tests
{
    /// <summary>
    /// The STEP <c>HEADER</c>/<c>FILE_NAME</c> entry belongs to the file, not to the path it
    /// happens to be stored under, and it survives opening a model.
    /// </summary>
    public class HeaderFileNameTests
    {
        // CPM.ifc declares FILE_NAME('0001', ...) in its STEP header, which deliberately
        // differs from the name the file has on disk.
        private const string TestFile = @"TestFiles\CPM.ifc";
        private const string HeaderFileName = "0001";

        public HeaderFileNameTests()
        {
            xUnitReinit.Reset();
        }

        [Fact]
        public void Step21_parser_reads_header_file_name()
        {
            using var model = MemoryModel.OpenRead(TestFile);

            model.Header.FileName.Name.Should().Be(HeaderFileName);
        }

        [Fact]
        public void HeuristicModelProvider_preserves_header_file_name()
        {
            var provider = new HeuristicModelProvider();

            using var model = provider.Open(TestFile, XbimSchemaVersion.Ifc2X3);

            model.Header.FileName.Name.Should().Be(HeaderFileName);
        }

        [Fact]
        public void IfcStore_preserves_header_file_name()
        {
            using var store = IfcStore.Open(TestFile);

            store.Header.FileName.Name.Should().Be(HeaderFileName);
        }
    }
}

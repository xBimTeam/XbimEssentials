using System.IO;
using Xbim.Common.Metadata;
using Xbim.IO.Parser;
using Xbim.IO.Step21;

namespace Xbim.IO.MemoryModel
{
    internal class PartialParser : XbimP21Parser
    {
        public PartialParser(string data)
        {
            var scanner = new Scanner();
            scanner.SetSource(data, 0);
            Scanner = scanner;
        }

        public PartialParser(Stream data) : base(data, 0)
        {
        }
    }
}

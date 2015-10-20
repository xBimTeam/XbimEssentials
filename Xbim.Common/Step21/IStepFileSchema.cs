using System.Collections.Generic;
using System.IO;

namespace Xbim.Common.Step21
{
    public interface IStepFileSchema : IPersist, IExpressHeaderType
    {
        List<string> Schemas { get; set; }
        void Write(BinaryWriter binaryWriter);
        void Read(BinaryReader binaryReader);
    }
}

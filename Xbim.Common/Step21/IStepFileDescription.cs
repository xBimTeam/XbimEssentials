using System.Collections.Generic;
using System.IO;

namespace Xbim.Common.Step21
{
    public interface IStepFileDescription : IPersist, IExpressHeaderType
    {
        List<string> Description {get;set;}
        string ImplementationLevel { get; set; }
        int EntityCount { get; set; }
        void Write(BinaryWriter binaryWriter);
        void Read(BinaryReader binaryReader);
    }
}

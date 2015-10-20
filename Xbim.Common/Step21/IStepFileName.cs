using System.Collections.Generic;
using System.IO;

namespace Xbim.Common.Step21
{
    public interface IStepFileName : IPersist, IExpressHeaderType
    {
        string Name{get;set;}
        string TimeStamp { get; set; }
        List<string> AuthorName{ get; set; }
        List<string> Organization { get; set; }
        string PreprocessorVersion { get; set; }
        string OriginatingSystem { get; set; }
        string AuthorizationName { get; set; }
        List<string> AuthorizationMailingAddress { get; set; }
        void Write(BinaryWriter binaryWriter);
        void Read(BinaryReader binaryReader);
    }
}

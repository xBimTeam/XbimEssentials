using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Xbim.Common.Step21
{
    public interface IStepFileName : IPersist, IExpressHeaderType, INotifyPropertyChanged
    {
        string Name{get;set;}
        string TimeStamp { get; set; }
        IList<string> AuthorName{ get; set; }
        IList<string> Organization { get; set; }
        string PreprocessorVersion { get; set; }
        string OriginatingSystem { get; set; }
        string AuthorizationName { get; set; }
        IList<string> AuthorizationMailingAddress { get; set; }
        void Write(BinaryWriter binaryWriter);
        void Read(BinaryReader binaryReader);
    }
}

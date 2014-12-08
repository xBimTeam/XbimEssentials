using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Xbim.XbimExtensions.Interfaces
{
    public interface IIfcFileName : IPersistIfc, ExpressHeaderType
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

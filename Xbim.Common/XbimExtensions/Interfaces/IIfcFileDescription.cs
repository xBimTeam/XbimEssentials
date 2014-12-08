using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Xbim.XbimExtensions.Interfaces
{
    public interface IIfcFileDescription : IPersistIfc, ExpressHeaderType
    {
        List<string> Description {get;set;}
        string ImplementationLevel { get; set; }
        int EntityCount { get; set; }
        void Write(BinaryWriter binaryWriter);
        void Read(BinaryReader binaryReader);
    }
}

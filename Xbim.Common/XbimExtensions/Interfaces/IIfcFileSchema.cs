using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Xbim.XbimExtensions.Interfaces
{
    public interface IIfcFileSchema : IPersistIfc, ExpressHeaderType
    {
        List<string> Schemas { get; set; }
        void Write(BinaryWriter binaryWriter);
        void Read(BinaryReader binaryReader);
    }
}

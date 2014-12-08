using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Xbim.XbimExtensions.Interfaces
{
    public interface IIfcFileHeader
    {
        IIfcFileDescription FileDescription { get; set; }
        IIfcFileName FileName { get; set; }
        IIfcFileSchema FileSchema { get; set; }
        void Write(BinaryWriter binaryWriter);
        void Read(BinaryReader binaryReader);
        string SchemaVersion { get; }
        string CreatingApplication { get; }
        string ModelViewDefinition { get; }
        string Name { get; }
        string TimeStamp { get; }
    }
}

using System.IO;

namespace Xbim.Common.Step21
{
    public interface IStepFileHeader
    {
        IStepFileDescription FileDescription { get; set; }
        IStepFileName FileName { get; set; }
        IStepFileSchema FileSchema { get; set; }
        void Write(BinaryWriter binaryWriter);
        void Read(BinaryReader binaryReader);
        string SchemaVersion { get; }
        string CreatingApplication { get; }
        string ModelViewDefinition { get; }
        string Name { get; }
        string TimeStamp { get; }
    }
}

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Xbim.Common.Step21
{
    public interface IStepFileSchema : IPersist, IExpressHeaderType, INotifyPropertyChanged
    {
        IList<string> Schemas { get; set; }
        void Write(BinaryWriter binaryWriter);
        void Read(BinaryReader binaryReader);
    }
}

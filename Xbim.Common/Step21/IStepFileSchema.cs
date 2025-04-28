using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Xbim.Common.Step21
{
    /// <summary>
    /// Represents the STEP FILE_SCHEMA entity which identifies the EXPRESS schema information for the IFC file
    /// </summary>
    public interface IStepFileSchema : IPersist, IExpressHeaderType, INotifyPropertyChanged
    {
        /// <summary>
        /// The name of the IFC Schema the IFC file adheres to
        /// </summary>
        IList<string> Schemas { get; set; }
        /// <summary>
        /// Writes the FILE_SCHEMA entity to a STEP IFC File
        /// </summary>
        /// <param name="binaryWriter"></param>
        void Write(BinaryWriter binaryWriter);
        /// <summary>
        /// Reads the FILE_SCHEMA entity from a STEP IFC File
        /// </summary>
        /// <param name="binaryReader"></param>
        void Read(BinaryReader binaryReader);
    }
}

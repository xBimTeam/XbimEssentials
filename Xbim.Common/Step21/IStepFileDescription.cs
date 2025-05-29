using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Xbim.Common.Step21
{
    /// <summary>
    /// Represents the STEP FILE_DESCRIPTION entity which provides version information about the IFC file
    /// </summary>
    public interface IStepFileDescription : IPersist, IExpressHeaderType, INotifyPropertyChanged
    {
        /// <summary>
        /// The formal definition of the underlying view definition(s). e.g 'ViewDefinition [CoordinationView]',
        /// or Options/Comments provided by the IFC authoring system.
        /// </summary>
        IList<string> Description {get;set;}
        /// <summary>
        /// The STEP Implementation level. This should always be '2;1' for IFC files
        /// </summary>
        string ImplementationLevel { get; set; }
        /// <summary>
        /// The number of entities in this model
        /// </summary>
        int EntityCount { get; set; }
        /// <summary>
        /// Writes the FILE_DESCRIPTION entity to a STEP IFC File
        /// </summary>
        /// <param name="binaryWriter"></param>
        void Write(BinaryWriter binaryWriter);
        /// <summary>
        /// Reads the FILE_DESCRIPTION entity from a STEP IFC File
        /// </summary>
        /// <param name="binaryReader"></param>
        void Read(BinaryReader binaryReader);
    }
}

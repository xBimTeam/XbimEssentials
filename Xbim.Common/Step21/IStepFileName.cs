using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Xbim.Common.Step21
{
    /// <summary>
    /// Represents the STEP FILE_NAME entity which provides human readable information about the IFC
    /// </summary>
    public interface IStepFileName : IPersist, IExpressHeaderType, INotifyPropertyChanged
    {
        /// <summary>
        /// The local path and file name of the IFC file
        /// </summary>
        string Name {get;set;}
        /// <summary>
        /// The time of creation on the IFC file in ISO 8601 format
        /// </summary>
        string TimeStamp { get; set; }
        /// <summary>
        /// The name and mailing address of the person responsible for creating the IFC.
        /// </summary>
        /// <remarks>E.g. login or email address of the user</remarks>
        IList<string> AuthorName{ get; set; }
        /// <summary>
        /// The group or organization with whom the author is associated.
        /// </summary>
        IList<string> Organization { get; set; }
        /// <summary>
        /// Name and version of the toolbox used to create the IFC file, 
        /// </summary>
        /// <remarks>NOT the name of the application itself.</remarks>
        string PreprocessorVersion { get; set; }
        /// <summary>
        /// Name and version and/or build number of the application which generates the IFC file
        /// </summary>
        /// <remarks>This is the name of the application. Note: The version and/or build number should be as 
        /// specific as possible.
        /// </remarks>
        string OriginatingSystem { get; set; }
        /// <summary>
        /// The name and mailing address of the person who authorized the sending of the IFC File
        /// </summary>
        string AuthorizationName { get; set; }
        /// <summary>
        /// The name and mailing address of the person who authorized the sending of the IFC File
        /// </summary>
        IList<string> AuthorizationMailingAddress { get; set; }
        /// <summary>
        /// Writes the FILE_NAME entity to a STEP IFC File
        /// </summary>
        /// <param name="binaryWriter"></param>
        void Write(BinaryWriter binaryWriter);
        /// <summary>
        /// Reads the FILE_NAME entity from a STEP IFC File
        /// </summary>
        /// <param name="binaryReader"></param>
        void Read(BinaryReader binaryReader);
    }
}

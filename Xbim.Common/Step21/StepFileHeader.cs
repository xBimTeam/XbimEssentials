#region XbimHeader

// The eXtensible Building Information Modelling (xBIM) Toolkit
// Solution:    XbimComplete
// Project:     Xbim.Ifc
// Filename:    IfcFileHeader.cs
// Published:   01, 2012
// Last Edited: 9:04 AM on 20 12 2011
// (See accompanying copyright.rtf)

#endregion

#region Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

#endregion

namespace Xbim.Common.Step21
{
    [Serializable]
    public class StepFileDescription : IStepFileDescription
    {
        private void MakeValid()
        {
            if (Description.Count == 0)
            {
                Description.Add("ViewDefinition [CoordinationView]");
                
            }
            if (string.IsNullOrWhiteSpace(ImplementationLevel))
            {
                ImplementationLevel = "2;1";
            }
            
        }

        public StepFileDescription()
        {
        }

        public StepFileDescription(string implementationLevel)
        {
            ImplementationLevel = implementationLevel;
            Description.Add("ViewDefinition [CoordinationView]");
        }

        public List<string> Description = new List<string>(2);
        private string ImplementationLevel;

        #region ISupportIfcParser Members

        public void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
        {
            switch (propIndex)
            {
                case 0:
                    Description.Add(value.StringVal);
                    break;
                case 1:
                    ImplementationLevel = value.StringVal;
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        #endregion

        #region ISupportIfcParser Members

        public string WhereRule()
        {
            return "";
        }

        #endregion

        internal void Write(BinaryWriter binaryWriter)
        {
            MakeValid();
            binaryWriter.Write(Description.Count);
            foreach (var desc in Description)
                binaryWriter.Write(desc);
            binaryWriter.Write(ImplementationLevel);
        }

        internal void Read(BinaryReader binaryReader)
        {
            var count = binaryReader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                Description.Add(binaryReader.ReadString());
            }
            ImplementationLevel = binaryReader.ReadString();
        }

        List<string> IStepFileDescription.Description
        {
            get
            {
                return Description;
            }
            set
            {
                Description = value;
            }
        }

        string IStepFileDescription.ImplementationLevel
        {
            get
            {
                return ImplementationLevel;
            }
            set
            {
                ImplementationLevel = value;
            }
        }

        int IStepFileDescription.EntityCount { get; set; }


        void IStepFileDescription.Write(BinaryWriter binaryWriter)
        {
            Write(binaryWriter);
        }

        void IStepFileDescription.Read(BinaryReader binaryReader)
        {
            Read(binaryReader);
        }
    }

    [Serializable]
    public class StepFileName : IStepFileName
    {
        public StepFileName(DateTime time)
        {
            TimeStamp = string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", time.Year, time.Month, time.Day,
                                      time.Hour, time.Minute, time.Second);
        }

        public StepFileName()
        {
            SetTimeStampNow();
        }

        public string Name;
        public string TimeStamp;

        public void SetTimeStampNow()
        {
            var now = DateTime.Now;
            TimeStamp = string.Format("{0:0000}-{1:00}-{2:00}T{3:00}:{4:00}:{5:00}", now.Year, now.Month, now.Day,
                                      now.Hour, now.Minute, now.Second);
        }

        public List<string> AuthorName = new List<string>(2);
        public List<string> Organization = new List<string>(6);

        public string PreprocessorVersion;
        public string OriginatingSystem;
        public string AuthorizationName = "";
        public List<string> AuthorizationMailingAddress = new List<string>(6);

        #region ISupportIfcParser Members

        public void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
        {
            switch (propIndex)
            {
                case 0:
                    Name = value.StringVal;
                    break;
                case 1:
                    TimeStamp = value.StringVal;
                    break;
                case 2:
                    AuthorName.Add(value.StringVal);
                    break;
                case 3:
                    Organization.Add(value.StringVal);
                    break;
                case 4:
                    PreprocessorVersion = value.StringVal;
                    break;
                case 5:
                    OriginatingSystem = value.StringVal;
                    break;
                case 6:
                    AuthorizationName = value.StringVal;
                    break;
                case 7:
                    AuthorizationMailingAddress.Add(value.StringVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public string WhereRule()
        {
            return "";
        }

        #endregion

        internal void Write(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Name ?? "");
            binaryWriter.Write(TimeStamp ?? "");
            binaryWriter.Write(AuthorName.Count);
            foreach (var item in AuthorName)
                binaryWriter.Write(item);
            binaryWriter.Write(Organization.Count);
            foreach (var item in Organization)
                binaryWriter.Write(item);
            binaryWriter.Write(PreprocessorVersion ?? "");
            binaryWriter.Write(OriginatingSystem ?? "");
            binaryWriter.Write(AuthorizationName ?? "");
            binaryWriter.Write(AuthorizationMailingAddress.Count);
            foreach (var item in AuthorizationMailingAddress)
                binaryWriter.Write(item);
        }

        internal void Read(BinaryReader binaryReader)
        {
            Name = binaryReader.ReadString();
            TimeStamp = binaryReader.ReadString();
            var count = binaryReader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                AuthorName.Add(binaryReader.ReadString());
            }
            count = binaryReader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                Organization.Add(binaryReader.ReadString());
            }
            PreprocessorVersion = binaryReader.ReadString();
            OriginatingSystem = binaryReader.ReadString();
            AuthorizationName = binaryReader.ReadString();
            count = binaryReader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                AuthorizationMailingAddress.Add(binaryReader.ReadString());
            }
        }

        string IStepFileName.Name
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
            }
        }

        string IStepFileName.TimeStamp
        {
            get
            {
                return TimeStamp;
            }
            set
            {
                TimeStamp = value;
            }
        }

        List<string> IStepFileName.AuthorName
        {
            get
            {
                return AuthorName;
            }
            set
            {
                AuthorName = value;
            }
        }

        List<string> IStepFileName.Organization
        {
            get
            {
                return Organization;
            }
            set
            {
                Organization = value;
            }
        }

        string IStepFileName.PreprocessorVersion
        {
            get
            {
                return PreprocessorVersion;
            }
            set
            {
                PreprocessorVersion = value;
            }
        }

        string IStepFileName.OriginatingSystem
        {
            get
            {
                return OriginatingSystem;
            }
            set
            {
                OriginatingSystem = value;
            }
        }

        string IStepFileName.AuthorizationName
        {
            get
            {
                return AuthorizationName;
            }
            set
            {
                AuthorizationName = value;
            }
        }

        List<string> IStepFileName.AuthorizationMailingAddress
        {
            get
            {
                return AuthorizationMailingAddress;
            }
            set
            {
                AuthorizationMailingAddress = value;
            }
        }


        void IStepFileName.Write(BinaryWriter binaryWriter)
        {
            Write(binaryWriter);
        }

        void IStepFileName.Read(BinaryReader binaryReader)
        {
            Read(binaryReader);
        }
    }

    [Serializable]
    public class StepFileSchema : IStepFileSchema
    {
        public List<string> Schemas = new List<string>();

        public StepFileSchema()
        {
        }

        public StepFileSchema(string version)
        {
            Schemas.Add(version);
        }

        #region ISupportIfcParser Members

        public void Parse(int propIndex, IPropertyValue value, int[] nestedIndex)
        {
            switch (propIndex)
            {
                case 0:
                    if (!Schemas.Contains(value.StringVal)) Schemas.Add(value.StringVal);
                    break;
                default:
                    this.HandleUnexpectedAttribute(propIndex, value); break;
            }
        }

        public string WhereRule()
        {
            return "";
        }

        #endregion

        internal void Write(BinaryWriter binaryWriter)
        {
            if (Schemas.Count == 0) //if no schema is defined the use IFC2x3 for now
                Schemas.Add("IFC2X3");
            binaryWriter.Write(Schemas.Count);
            foreach (var item in Schemas)
                binaryWriter.Write(item);
        }

        internal void Read(BinaryReader binaryReader)
        {
            var count = binaryReader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                Schemas.Add(binaryReader.ReadString());
            }
        }

        List<string> IStepFileSchema.Schemas
        {
            get
            {
                return Schemas;
            }
            set
            {
                Schemas = value;
            }
        }


        void IStepFileSchema.Write(BinaryWriter binaryWriter)
        {
            Write(binaryWriter);
        }

        void IStepFileSchema.Read(BinaryReader binaryReader)
        {
            Read(binaryReader);
        }
    }

    [Serializable]
    public class StepFileHeader : IStepFileHeader
    {
        public enum HeaderCreationMode
        {
            LeaveEmpty,
            InitWithXbimDefaults
        }

        public StepFileHeader(HeaderCreationMode mode)
        {
            if (mode == HeaderCreationMode.InitWithXbimDefaults)
            {
                FileDescription = new StepFileDescription("2;1");
                FileName = new StepFileName(DateTime.Now)
                    {
                        PreprocessorVersion =
                            string.Format("Xbim File Processor version {0}",
                                          Assembly.GetExecutingAssembly().GetName().Version),
                        OriginatingSystem =
                            string.Format("Xbim version {0}",
                                          Assembly.GetExecutingAssembly().GetName().Version),
                    };
                FileSchema = new StepFileSchema();
            }
            else
            {
                // Please note do not put any value initialisation in here
                // Any value initialised here is added to ALL models read from IFC
                // 
                // Any information required before writing a file for schema constraint needs to be checked upon writing
                // e.g. cfr. FileDescription.MakeValid();
                //
                FileDescription = new StepFileDescription();
                FileName = new StepFileName();
                FileSchema = new StepFileSchema();
            }
        }

        public IStepFileDescription FileDescription;
        public IStepFileName FileName;
        public IStepFileSchema FileSchema;

        public void Write(BinaryWriter binaryWriter)
        {
            FileDescription.Write(binaryWriter);
            FileName.Write(binaryWriter);
            FileSchema.Write(binaryWriter);
        }

        public void Read(BinaryReader binaryReader)
        {
            FileDescription.Read(binaryReader);
            FileName.Read(binaryReader);
            FileSchema.Read(binaryReader);
        }

        IStepFileDescription IStepFileHeader.FileDescription
        {
            get
            {
                return FileDescription;
            }
            set
            {
                FileDescription = value;
            }
        }

        IStepFileName IStepFileHeader.FileName
        {
            get
            {
                return FileName;
            }
            set
            {
                FileName = value;
            }

        }

        IStepFileSchema IStepFileHeader.FileSchema
        {
            get
            {
                return FileSchema;
            }
            set
            {
                FileSchema = value;
            }

        }


        public string SchemaVersion
        {
            get
            {
                if (FileSchema != null && FileSchema.Schemas!=null && FileSchema.Schemas.Count > 0)
                    return string.Join(", ", FileSchema.Schemas);
                else
                    return "";
            }
        }

        public string CreatingApplication
        {
            get
            {
                if (FileName != null && FileName.OriginatingSystem != null)
                    return FileName.OriginatingSystem;
                else
                    return "";
            }
        }

        public string ModelViewDefinition
        {
            get
            {
                if (FileDescription != null && FileDescription.Description != null)
                    return string.Join(", ", FileDescription.Description);
                else
                    return "";

            }
        }

        public string Name
        {
            get 
            {
                if (FileName != null && FileName.Name != null)
                    return FileName.Name;
                else
                    return "";
            }
        }

        public String TimeStamp
        {
            get
            {
                if (FileName != null && FileName.TimeStamp != null)
                    return FileName.TimeStamp;
                else
                    return "";
            }
        }
    }
}